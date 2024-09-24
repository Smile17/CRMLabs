using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient crmService = Auth.Authenticate();
            //ProjectGenerator.CreateRecords(crmService);
            //ProjectDayGenerator.CreateRecords(crmService);
            ProjectRecordGenerator.CreateRecords(crmService);

            //var recievedData = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, data);
            //var recievedDataIds = recievedData.Select(x => x.Id).ToList();
            //CrmRequests.ExecuteMultipleDeleteRequests(crmService, recievedDataIds, "cr78f_customaccount");
            Console.ReadLine();
        }
    }
}
