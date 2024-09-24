using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;

namespace DynamicsConnection
{
    internal class AuthWithConfig
    {
        // <add name="Xrm" connectionString="AuthType=Office365;Url=https://orgb5157338.crm.dynamics.com/;UserName=kateryna.mykhailovska@enavate.com;Password=Ecrtb2BBsFaN;" />
        // 
        public static CrmServiceClient Authenticate()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Xrm"].ToString();
            var crmUrl = System.Configuration.ConfigurationManager.AppSettings["crmUrl"];
            var clientId = System.Configuration.ConfigurationManager.AppSettings["clientId"];
            var clientSecret = System.Configuration.ConfigurationManager.AppSettings["clientSecret"];
            var authority = System.Configuration.ConfigurationManager.AppSettings["authority"];
            connectionString = String.Format(connectionString, crmUrl, clientId, clientSecret, authority);

            var crmService = new CrmServiceClient(connectionString);

            
            //CrmServiceClient crmService = new CrmServiceClient(new Uri(crmUrl), clientId, clientSecret, true, "");

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
