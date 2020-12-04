using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Resources
{
    public class CustomerInfo
    {
        public string Region { get; set; }
        public string ResourceGroupName { get; set; }
        public string ACR { get; set; }
    }
}
