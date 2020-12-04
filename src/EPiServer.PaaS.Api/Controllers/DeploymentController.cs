using EPiServer.PaaS.Api.Business;
using EPiServer.PaaS.Api.Models;
using EPiServer.PaaS.Api.ResourceManager;
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
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeploymentController : ControllerBase
    {
        private readonly ILogger<PreparationController> _logger;
        private readonly SecuredHttpRequestMessageFactory _requestFactory;
        private readonly DeploymentRepository _deploymentRepository;
        private readonly IContainerResourceProvider _provider;
        private readonly Subscription _subscription;
        private static readonly HttpClient _client = new HttpClient();

        public DeploymentController(ILogger<PreparationController> logger, 
            Subscription subscription, 
            SecuredHttpRequestMessageFactory requestFactory, 
            DeploymentRepository deploymentRepository, 
            IContainerResourceProvider provider)
        {
            _logger = logger;
            _subscription = subscription;
            _deploymentRepository = deploymentRepository;
            _requestFactory = requestFactory;
            _provider = provider;
        }

        [HttpPut("{packageName}")]
        public async Task<DeploymentResult> Put([FromRoute] string packageName)
        {
            List<string> repositories = _deploymentRepository.GetRepositories(packageName);
            foreach (var repository in repositories)
            {
                await _provider.DeployFromACR(repository, repository);
            }

            return new DeploymentResult()
            {
                Package = packageName,
                Projects = repositories
            };
        }

        [HttpGet("{packageName}")]
        public async Task<List<string>> Get(string packageName)
        {
            List<string> repositories = _deploymentRepository.GetRepositories(packageName);
            await Task.CompletedTask;

            return repositories;
        }

        [HttpPost]
        public async Task<DeploymentResult> Post()
        {
            DeploymentResult result = new DeploymentResult();

            await Task.CompletedTask;

            return result;
        }
    }
}
