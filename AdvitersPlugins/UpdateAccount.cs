using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvitersPlugins
{
    public class UpdateAccount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity Target = (Entity)context.InputParameters["Target"];
                try
                {
                    if (Target.LogicalName.ToLower() == "account")
                    {
                        bool webSite = false;
                        string accountId = Target.Id.ToString();
                        string tasks = @"  
                                        <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                            <entity name='task'>
                                            <attribute name='activityid'/>
                                            <attribute name='subject'/>
                                                <filter type='and'>
                                                    <condition attribute='regardingobjectid' operator='eq' value='" + accountId + @"' />
                                                </filter>
                                            </entity>
                                        </fetch>";

                        EntityCollection result = service.RetrieveMultiple(new FetchExpression(tasks));
                        foreach (var i in result.Entities)
                        {
                            Entity followup = new Entity("task", i.Id);
                            if (Target.Contains("websiteurl") && (Target.GetAttributeValue<string>("websiteurl") != string.Empty ||
                            Target.GetAttributeValue<string>("websiteurl") != ""))
                                webSite = true;

                            followup["subject"] = webSite ? "Tarea con WEB" : "Tarea sin WEB";
                            followup["description"] = webSite ? "El contachto ya posee un sitio web" : "El contacto no posee sitio web";

                            followup["category"] = context.PrimaryEntityName;

                            if (context.OutputParameters.Contains("id"))
                            {
                                Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                                string regardingobjectidType = "account";

                                followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                            }
                            service.Update(followup);
                        }
                    }
                }
                catch (Exception ex)
                {
                    

                }
            }
        }
    }
}
