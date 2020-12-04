using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Business
{
    public class DeploymentRepository
    {
        private static readonly ConcurrentDictionary<string, List<string>> DeploymentPackage = new ConcurrentDictionary<string, List<string>>();

        public void AddRepository(string packageName, string repositoryName)
        {
            var package = DeploymentPackage.GetOrAdd(packageName, new List<string>());
            package.Add(repositoryName);
        }

        public List<string> GetRepositories(string packageName)
        {
            return DeploymentPackage[packageName];
        }
    }
}
