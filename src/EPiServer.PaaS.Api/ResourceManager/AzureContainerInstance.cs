using EPiServer.PaaS.Api.Resources;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.ResourceManager
{
    public class AzureContainerInstance : IContainerResourceProvider
    {
        private readonly Credential _credential;
        private readonly CustomerInfo _customerInfo;

        public AzureContainerInstance(Credential credential, CustomerInfo customerInfo)
        {
            _credential = credential;
            _customerInfo = customerInfo;
        }

        public async Task DeployFromACR(string name, string image)
        {
            string acrServer = $"{_customerInfo.ACR}.azurecr.io";
            string fullPathImage = $"{acrServer}/{image}:latest";

            IAzure azure = GetAzureContext();
            var containerGroup = azure.ContainerGroups.Define($"{image}_group")
                .WithRegion(_customerInfo.Region)
                .WithExistingResourceGroup(_customerInfo.ResourceGroupName)
                .WithLinux()
                .WithPrivateImageRegistry(acrServer, _credential.ClientId, _credential.ClientSecret)
                .WithoutVolume()
                .DefineContainerInstance(image)
                    .WithImage(fullPathImage)
                    .WithExternalTcpPort(80)
                    .WithCpuCoreCount(0.5)
                    .WithMemorySizeInGB(1)
                    .Attach()
                .WithDnsPrefix(image)
                .Create();

            await Task.CompletedTask;
        }

        private IAzure GetAzureContext()
        {
            IAzure azure;
            ISubscription sub;

            try
            {
                var azureCredential = new AzureCredentialsFactory().FromServicePrincipal(_credential.ClientId, _credential.ClientSecret, _credential.TenantId, AzureEnvironment.AzureGlobalCloud);
                azure = Azure.Authenticate(azureCredential).WithDefaultSubscription();
                sub = azure.GetCurrentSubscription();

                Console.WriteLine($"Authenticated with subscription '{sub.DisplayName}' (ID: {sub.SubscriptionId})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to authenticate:\n{ex.Message}");
                throw;
            }

            return azure;
        }
    }
}
