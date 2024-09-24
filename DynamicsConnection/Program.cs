using Microsoft.Rest;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsConnection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient crmService = Auth.Authenticate();
            CrmServiceClient crmService2 = AuthWithConfig.Authenticate();
            Console.ReadLine();
        }
    }
}
