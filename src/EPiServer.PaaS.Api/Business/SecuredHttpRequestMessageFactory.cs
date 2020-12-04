using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api
{
    public class SecuredHttpRequestMessageFactory
    {
        private readonly Credential _credential;

        public SecuredHttpRequestMessageFactory(Credential credential)
        {
            _credential = credential;
        }

        public HttpRequestMessage Get(string? url, HttpMethod method)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            request.Headers.Add("Authorization", $"Bearer {_credential.Token}");

            return request;
        }
    }
}
