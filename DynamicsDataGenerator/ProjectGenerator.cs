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
    public class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public byte[] Image { get; set; }
        public Entity Manager { get; set; }

    }
    
    public static class ExtensionsForBogus
    {
        public static List<string> ProjectNames = new List<string>()
        {
                "Project Alpha",
                "Operation Delta",
                "Initiative Gamma",
                "Task Zeta",
                "Mission Epsilon",
                "Green Initiative",
                "Digital Transformation",
                "Innovation Hub",
                "Growth Catalyst",
                "Efficiency Project",
                "Phoenix Rising",
                "Unicorn Hunt",
                "Quantum Leap",
                "Time Traveler",
                "Cosmic Quest"
        };
        public static string CustomProjectNames(this Randomizer r)
        {
            if (ProjectNames.Count == 0) throw new Exception("No more project names without repeating.");
            var i = r.Number(min: 0, max: ProjectNames.Count - 1);
            var val = ProjectNames[i];
            ProjectNames.RemoveAt(i);
            return val;
        }
    }
    public class ProjectFaker : Faker<Project>
    {
        
        public ProjectFaker(List<Entity> users)
        {
            //RuleFor(d => d.Name, f => f.Company.CompanyName());
            RuleFor(d => d.Name, f => f.Random.CustomProjectNames()); // Custom project names
            RuleFor(d => d.Description, f => f.Company.CatchPhrase());
            RuleFor(d => d.StartDate, f => f.Date.Past(1));
            RuleFor(d => d.EndDate, f => f.Date.Future(1));
            RuleFor(d => d.Image, f => f.Random.Bytes(1024 * 1024)); // Simulate 1MB image

            RuleFor(d => d.Manager, f => f.PickRandom(users));
        }
    }

    public partial class MapToD365Entity
    {
        public static string ProjectTable = "cr78f_customtimetrackingproject";
        public static Entity MapProjectEntityToD365Entity(Project data)
        {
            Entity entity = new Entity(ProjectTable);

            entity["cr78f_name"] = data.Name;
            entity["cr78f_description"] = data.Description;
            entity["cr78f_startdate"] = data.StartDate;
            entity["cr78f_enddate"] = data.EndDate;
            //entity["cr78f_image"] = data.Image;
            entity["cr78f_manager"] = new EntityReference(CrmRequests.UserTable, data.Manager.Id);
            return entity;
        }
    }
    internal class ProjectGenerator
    {
        public static void CreateRecords(CrmServiceClient crmService)
        {
            var records = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, MapToD365Entity.ProjectTable);
            var users = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, CrmRequests.UserTable, 0);
            var data = new ProjectFaker(users.ToList()).Generate(5);
            var crmData = data.Select(MapToD365Entity.MapProjectEntityToD365Entity).ToList();
            CrmRequests.ExecuteMultipleCreateRequests(crmService, crmData);
        }
    }
}
