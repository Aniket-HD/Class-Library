using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;

namespace Class_Library
{
    public class Taskcreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here.
                    //in memory object of an entity to be created
                    Entity taskRecord = new Entity("task");
                    //setting up attributes
                    taskRecord.Attributes.Add("subject", "Follow up");
                    taskRecord.Attributes.Add("description", "Please follow up with the contact.");
                    //setting up attribues with date
                    taskRecord.Attributes.Add("scheduledend", DateTime.Now.AddDays(2));
                    //setting up attribues with option set
                    taskRecord.Attributes.Add("prioritycode", new OptionSetValue(2));
                    //Setting up parent record or lookup
                    //taskRecord.Attributes.Add("regardingobjectid",new EntityReference("entity", entity.Id));
                    //or easy way would be
                    taskRecord.Attributes.Add("regardingobjectid", entity.ToEntityReference());
                    //in above line Guid is a prerequisite so pipeline will only get Guid after main event i.e., contact creation
                    //therefore this plugin has to be registed on post operation stage
                    //Now push the created object
                    Guid taskGuid = service.Create(taskRecord);
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in Taskcreate.", ex);
                }

                //catch (Exception ex)
                //{
                //    tracingService.Trace("Taskcreate: {0}", ex.ToString());
                //    throw;
                //}
            }
        }
    }
}
