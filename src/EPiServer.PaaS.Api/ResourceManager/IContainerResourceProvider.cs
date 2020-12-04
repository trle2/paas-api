using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.ResourceManager
{
    public interface IContainerResourceProvider
    {
        Task DeployFromACR(string name, string image);
    }
}
