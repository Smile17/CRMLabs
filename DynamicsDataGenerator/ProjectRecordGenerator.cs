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
    
    public class Record
    {
        public bool Approved { get; set; }
        public string ApproverComment { get; set; }
        public DateTime Date { get; set; }
        public Entity Day { get; set; }
        public Entity Employee { get; set; }
        public string Description { get; set; }
        public Entity Project { get; set; }
        public decimal ReportedHours { get; set; }
        public string Type { get; set; }
        //public StatusEnum StatusReason { get; set; }
    }

    public class ProjectRecordFaker : Faker<Record>
    {
        public List<string> types = new List<string>() { "968520000", "968520003", "968520001" };
        public ProjectRecordFaker(List<Entity> users, List<Entity> projects, List<Entity> days)
        {
            RuleFor(d => d.Approved, f => f.Random.Bool());
            RuleFor(d => d.ApproverComment, f => f.Random.Words(3));
            RuleFor(d => d.Date, f => f.Date.Recent(1));
            RuleFor(d => d.Day, f => f.PickRandom(days));
            RuleFor(d => d.Employee, f => f.PickRandom(users));
            RuleFor(d => d.Description, f => f.Company.CatchPhrase());
            RuleFor(d => d.Project, f => f.PickRandom(projects));
            RuleFor(d => d.ReportedHours, f => f.Random.Decimal(2, 20));
            RuleFor(d => d.Type, f => f.PickRandom(types));
            //RuleFor(d => d.StatusReason, f => f.PickRandom<StatusEnum>());
        }
    }
    public partial class MapToD365Entity
    {
        public static string RecordTable = "cr78f_customtimetrackingrecord";
        public static Entity MapRecordEntityToD365Entity(Record data)
        {
            Entity entity = new Entity(RecordTable);
            entity["cr78f_reportedhours"] = data.ReportedHours;
            entity["cr78f_project"] = new EntityReference(MapToD365Entity.ProjectTable, data.Project.Id);
            entity["cr78f_day"] = new EntityReference(MapToD365Entity.ProjectDayTable, data.Day.Id); ;
            entity["cr78f_date"] = data.Date;
            entity["cr78f_approved"] = data.Approved;
            entity["cr78f_type"] = new OptionSetValue(Int32.Parse(data.Type));
            entity["cr78f_name"] = data.Description;
            //entity["cr78f_employee"] = new EntityReference(CrmRequests.UserTable, data.Employee.Id);
            entity["cr78f_employee"] = data.Day["cr78f_employee"];
            return entity;
        }
    }
    internal class ProjectRecordGenerator
    {
        public static void CreateRecords(CrmServiceClient crmService)
        {
            var projects = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, MapToD365Entity.ProjectTable);
            var days = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, MapToD365Entity.ProjectDayTable);
            var users = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, CrmRequests.UserTable);
            var records = CrmRequests.ExecuteMultipleRetrieveRequests(crmService, MapToD365Entity.RecordTable);
            //var t = records[3].Attributes["cr78f_type"];
            var data = new ProjectRecordFaker(users.ToList(), projects.ToList(), days.ToList()).Generate(2);
            var crmData = data.Select(MapToD365Entity.MapRecordEntityToD365Entity).ToList();
            CrmRequests.ExecuteMultipleCreateRequests(crmService, crmData);
        }
    }
}
