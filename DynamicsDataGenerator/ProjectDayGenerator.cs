using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace DynamicsDataGenerator
{
    
    public class ProjectDay
    {
        public DateTime Date { get; set; }
        public Entity Employee { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }



    //public static class ExtensionsForBogus
    //{

    //}
    public class ProjectDayFaker : Faker<ProjectDay>
    {
        public List<string> types = new List<string>() { "968520000", "968520003", "968520001" };
        public ProjectDayFaker(List<Entity> users)
        {
            RuleFor(d => d.Date, f => f.Date.Recent(1));
            RuleFor(d => d.Employee, f => f.PickRandom(users));
            RuleFor(d => d.Name, f => f.Random.Word());
            RuleFor(d => d.Title, f => f.Random.Words(3));

        }
    }
    public partial class MapToD365Entity
    {
        public static string ProjectDayTable = "cr78f_customtimetrackingday";
        public static Entity MapProjectDayEntityToD365Entity(ProjectDay data)
        {
            Entity entity = new Entity(ProjectDayTable);
            entity["cr78f_date"] = data.Date;
            entity["cr78f_employee"] = new EntityReference(CrmRequests.UserTable, data.Employee.Id);
            entity["cr78f_name"] = data.Name;
            entity["cr78f_title"] = data.Title;

            return entity;
        }
    }
    internal class ProjectDayGenerator
    {
        public static void CreateRecords(CrmServiceClient crmService)
        {
            //var projects = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, MapToD365Entity.ProjectTable);
            var users = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, CrmRequests.UserTable);
            var projectDays = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, MapToD365Entity.ProjectDayTable);
            var data = new ProjectDayFaker(users.ToList()).Generate(2);
            var crmData = data.Select(MapToD365Entity.MapProjectDayEntityToD365Entity).ToList();
            CrmRequests.ExecuteMultipleCreateRequests(crmService, crmData);
        }
    }
}
