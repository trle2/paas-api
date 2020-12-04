using EPiServer.PaaS.Api.Business;
using EPiServer.PaaS.Api.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreparationController : ControllerBase
    {
        private readonly ILogger<PreparationController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SecuredHttpRequestMessageFactory _requestFactory;
        private readonly DeploymentRepository _deploymentRepository;
        private readonly Subscription _subscription;
        private static readonly HttpClient _client = new HttpClient();

        public PreparationController(ILogger<PreparationController> logger, 
            IWebHostEnvironment webHostEnvironment, 
            Subscription subscription, 
            SecuredHttpRequestMessageFactory requestFactory, 
            DeploymentRepository deploymentRepository)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _subscription = subscription;
            _requestFactory = requestFactory;
            _deploymentRepository = deploymentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateDockerfile([FromQuery] string deploymentPackageName, [FromQuery] string projectName, [FromQuery] string projectLocation, [FromQuery] byte type)
        {
            // Hard-coded!!!
            string dockerfileTemplateName = type == 1 ? "DockerfileTemplateForWebApp" : "DockerfileTemplateForFunctions";
            _deploymentRepository.AddRepository(deploymentPackageName, projectName);

            var dockerTemplateFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Templates");

            using (var fileProvider = new PhysicalFileProvider(dockerTemplateFilePath))
            {
                var stream = fileProvider.GetFileInfo(dockerfileTemplateName).CreateReadStream();
                using (var streamReader = new StreamReader(stream))
                {
                    var dockerTemplateContent = streamReader.ReadToEnd();
                    dockerTemplateContent = type == 1 ? GenerateDockerfileForWebApp(dockerTemplateContent, projectLocation)
                                                      : GenerateDockerfileForFunctions(dockerTemplateContent, projectLocation);

                    using (var outputStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteAsync(dockerTemplateContent);
                            streamWriter.Flush();
                            return File(outputStream.ToArray(), "text/plain");
                        }
                    }
                }
            }
        }

        [HttpGet("{userName}/registry/server")]
        public async Task<string> GetDockerServer(string userName)
        {
            var resourceGroupName = "trle2rs"; //Hard-coded for now. Must associated with userName.

            var url = $"https://management.azure.com/subscriptions/{_subscription.SubscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries?api-version=2019-05-01";
            HttpRequestMessage request = _requestFactory.Get(url, HttpMethod.Get);
            HttpResponseMessage response = await _client.SendAsync(request);

            var registries = await JsonSerializer.DeserializeAsync<RegistryList>(response.Content.ReadAsStream());

            return registries.Registries[0].Properties.LoginServer;
        }

        private string GenerateDockerfileForFunctions(string templateContent, string functionProjectName)
        {
            // Assumed there is only 1 jobs app.
            templateContent = templateContent.Replace("{{PROJECT_FILE_WITH_PATH}}", functionProjectName);
            return templateContent;
        }

        private string GenerateDockerfileForWebApp(string templateContent, string appProjectName)
        {
            Match match = Regex.Match(appProjectName, @"^(\/?.*?\/)?(([\w+\.]+\w+)\.csproj)$");

            if (match.Success)
            {
                var projectNameWithExtension = match.Groups[2].Value;
                var projectName = match.Groups[3].Value;

                templateContent = templateContent.Replace("{{PROJECT_FILE_WITH_PATH}}", appProjectName);
                templateContent = templateContent.Replace("{{PROJECT_NAME_WITH_EXTENSION}}", projectNameWithExtension);
                templateContent = templateContent.Replace("{{PROJECT_NAME}}", projectName);
                return templateContent;
            }

            throw new ArgumentException("Project name");
        }
    }
}
