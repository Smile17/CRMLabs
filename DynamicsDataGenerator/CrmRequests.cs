using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataGenerator
{
    internal class CrmRequests
    {
        public static string UserTable = "systemuser";
        public static void ExecuteMultipleCreateRequests(IOrganizationService service, List<Entity> entities)
        {
            var multipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (var entity in entities)
            {
                var createRequest = new CreateRequest { Target = entity };
                multipleRequest.Requests.Add(createRequest);
            }

            var multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);
            Console.WriteLine($"Table: {entities[0].LogicalName}");
            foreach (var responseItem in multipleResponse.Responses)
            {
                if (responseItem.Response is CreateResponse createResponse)
                {
                    Console.WriteLine($"Created record with ID: {createResponse.id}");
                }
                else if (responseItem.Fault != null)
                {
                    Console.WriteLine($"Error: {responseItem.Fault.Message}");
                }
            }
        }

        public static DataCollection<Entity> ExecuteMultipleRetrieveRequests(IOrganizationService service, string entityName, int topCount = 5)
        {
            var multipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
            var query = new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet { AllColumns = true },
            };
            if (topCount > 0)
            {
                query.TopCount = topCount;
            }
            var records = service.RetrieveMultiple(query).Entities;
            Console.WriteLine($"Table: {entityName}; Count of found records: {records.Count}");
            foreach (var record in records)
            {
                Console.WriteLine($"Recieved record with ID: {record.Id}");
            }
            return records;
        }

        //-----------------------------------------------

        public static DataCollection<Entity> ExecuteMultipleRetrieveRequests(IOrganizationService service, List<Project> entities)
        {
            var multipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
            var query = new QueryExpression("cr78f_customaccount")
            {
                ColumnSet = new ColumnSet("cr78f_company_name", "cr78f_description"),
                Criteria = new FilterExpression()
            };

            query.Criteria.FilterOperator = LogicalOperator.Or;

            foreach (var entity in entities)
            {
                FilterExpression filter = query.Criteria.AddFilter(LogicalOperator.And);
                filter.Conditions.Add(new ConditionExpression("cr78f_company_name", ConditionOperator.Equal, entity));
                filter.Conditions.Add(new ConditionExpression("cr78f_description", ConditionOperator.Equal, entity.Description));
            }
            var contacts = service.RetrieveMultiple(query).Entities;
            Console.WriteLine($"Count of found records: {contacts.Count}");
            foreach (var contact in contacts)
            {
                Console.WriteLine($"Recieved record with ID: {contact.Id}");
            }
            return contacts;
        }

        public static void ExecuteMultipleRetrieveRequestsByKey(IOrganizationService service, List<Project> entities)
        {
            var multipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (var entity in entities)
            {
                var keyCollection = new KeyAttributeCollection();
                keyCollection.Add("cr78f_company_name", entity.Name);
                keyCollection.Add("cr78f_description", entity.Description);

                RetrieveRequest retrieveRequest = new RetrieveRequest()
                {
                    ColumnSet = new ColumnSet(true),
                    Target = new EntityReference("cr78f_customaccount", keyCollection)
                };

                multipleRequest.Requests.Add(retrieveRequest);
            }

            var multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            foreach (var responseItem in multipleResponse.Responses)
            {
                if (responseItem.Response is RetrieveResponse retrieveResponse)
                {
                    Console.WriteLine($"Created record with ID: {retrieveResponse.Entity.ToString()}");
                }
                else if (responseItem.Fault != null)
                {
                    Console.WriteLine($"Error: {responseItem.Fault.Message}");
                }
            }
        }

        public static void ExecuteMultipleDeleteRequests(IOrganizationService service, List<Guid> ids, string entityLogicalName)
        {
            var multipleRequest = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (var id in ids)
            {
                var deleteRequest = new DeleteRequest
                {
                    Target = new EntityReference(entityLogicalName, id)
                };
                multipleRequest.Requests.Add(deleteRequest);
            }


            var multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            foreach (var responseItem in multipleResponse.Responses)
            {
                if (responseItem.Response is DeleteResponse deleteResponse)
                {
                    Console.WriteLine($"Deleted record: {deleteResponse.Results}");
                }
                else if (responseItem.Fault != null)
                {
                    Console.WriteLine($"Error: {responseItem.Fault.Message}");
                }
            }
        }
    }
}
