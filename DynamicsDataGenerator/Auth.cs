using Microsoft.Xrm.Tooling.Connector;
using System;

namespace DynamicsDataGenerator
{
    internal class Auth
    {
        public static CrmServiceClient Authenticate()
        {
            string clientId = "value";
            string clientSecret = "value";
            string authority = "https://login.microsoftonline.com/6872ac73-66c4-44ac-b58a-6264181e8029";
            string crmUrl = "value";
            string conString = $"AuthType=ClientSecret; Url={crmUrl}; Clientid={clientId}; ClientSecret={clientSecret}; Authority={authority}; RequirenNewInstance=True;";
            CrmServiceClient crmService = new CrmServiceClient(new Uri(crmUrl), clientId, clientSecret, true, "");

            if (crmService.IsReady)
            {
                Console.WriteLine("CRM Connected Successfully");
            }
            else
            {
                Console.WriteLine($"CRM Connection failed");
            }

            return crmService;
        }
    }
}
