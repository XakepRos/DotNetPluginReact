using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace HIPPAPlugin
{
    public class DynamicPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new NotImplementedException();

            //obtain the execution context from the service provider
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters.Contains("Target") is Entity)
            {
                // obtain the target entity from the input parameters
                Entity entity = (Entity)context.InputParameters["Target"];

                // If not, this plugin is not registered corectly
                if (entity.LogicalName != "lead")

                    return;

                try
                {
                    //create a task activity to follow up with the account customer in 7 days

                    Entity followup = new Entity("task");

                    followup["subject"] = "send email to the new customer";
                    followup["schedulestarted"] = DateTime.Now.AddDays(7);
                    followup["scheduleEnd"] = DateTime.Now.AddDays(7);
                    followup["category"] = context.PrimaryEntityName;

                    // Refer to the account in the task activity
                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectId = new Guid(context.OutputParameters["id"].ToString());

                        //string regardingobjectid = context.OutputParameters["id"].ToString();

                        string regardingobjectType = "lead";

                        followup["regardingobjectId"] = new EntityReference(regardingobjectType, regardingobjectId);

                    }
                    
                    
                    // obtain the organisarion service reference
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider
                        .GetService(typeof(IOrganizationServiceFactory));

                   IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    service.Create(followup);

                }
                catch(FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occured in the follow up", ex);
                }
            }
            else
            {
                return;
            }

        }
    }
}
