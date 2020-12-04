using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Middleware
{
    public class EnsureAccessTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Credential _credential;
        private readonly static HttpClient _client = new HttpClient();

        public EnsureAccessTokenMiddleware(RequestDelegate next, Credential credential)
        {
            _next = next;
            _credential = credential;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(_credential.Token))
            {
                string getTokenUrl = $"https://login.microsoftonline.com/{_credential.TenantId}/oauth2/token";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, getTokenUrl);

                Dictionary<string, string> body = new Dictionary<string, string>()
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _credential.ClientId },
                    { "client_secret", _credential.ClientSecret },
                    { "resource", _credential.Resource }
                };
                request.Content = new FormUrlEncodedContent(body);

                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    var tokenObj = await JsonSerializer.DeserializeAsync<AzureToken>(response.Content.ReadAsStream());

                    _credential.Token = tokenObj.AccessToken;
                }
                else
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }

            await _next(context);
        }
    }
}
