using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Models
{
    public class DeploymentResult
    {
        public bool IsSuccess => true;
        public string Package { get; set; }
        public List<string> Projects { get; set; }
    }
}