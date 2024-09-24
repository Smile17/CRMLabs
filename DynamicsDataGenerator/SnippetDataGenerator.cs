using Bogus;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace DynamicsDataGenerator
{
    internal class SnippetDataGenerator
    {
        public static void Generate()
        {
            var employee = new EmployeeFaker().Generate();
            string jsonString = employee.DumpString();
            //string jsonString = JsonSerializer.Serialize(employee); //using System.Text.Json;
            Console.WriteLine(jsonString);
        }
    }

    #region Class definitions
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployee(int id);
    }

    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public Job Job { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public class Job
    {
        public string Title { get; set; }
        public Department Department { get; set; }
    }

    public class Department
    {
        public string Name { get; set; }
        public string Floor { get; set; }
    }
    #endregion
    #region Faker classes
    public class DepartmentFaker : Faker<Department>
    {
        public DepartmentFaker()
        {
            RuleFor(d => d.Name, f => f.Commerce.Department());
            RuleFor(d => d.Floor, f => f.Random.Number(1, 20).ToString());
        }
    }

    public class JobFaker : Faker<Job>
    {
        public JobFaker()
        {
            RuleFor(j => j.Title, f => f.Name.JobTitle());
            RuleFor(j => j.Department, f => new DepartmentFaker().Generate());
        }
    }

    public class AddressFaker : Faker<Address>
    {
        public AddressFaker()
        {
            RuleFor(a => a.Street, f => f.Address.StreetName());
            RuleFor(a => a.City, f => f.Address.City());
            RuleFor(a => a.ZipCode, f => f.Address.ZipCode());
        }
    }

    public class EmployeeFaker : Faker<Employee>
    {
        public EmployeeFaker()
        {
            RuleFor(e => e.FirstName, f => f.Name.FirstName());
            RuleFor(e => e.LastName, f => f.Name.LastName());
            RuleFor(e => e.Address, f => new AddressFaker().Generate());
            RuleFor(e => e.Job, f => new JobFaker().Generate());
        }
    }
    #endregion

    public static class ExtensionsForTesting
    {
        public static void Dump(this object obj)
        {
            Console.WriteLine(obj.DumpString());
        }

        public static string DumpString(this object obj)
        {
            return JsonSerializer.Serialize(obj);
            //return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}
