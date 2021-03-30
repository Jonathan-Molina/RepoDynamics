using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvitersPlugins
{
    public class CreateAccount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity Target = (Entity)context.InputParameters["Target"];
                try
                {
                    if (Target.LogicalName.ToLower() == "account")
                    {
                        bool webSite = false;
                        Entity followup = new Entity("task");


                        if (Target.Contains("websiteurl") && (Target.GetAttributeValue<string>("websiteurl") != string.Empty ||
                            Target.GetAttributeValue<string>("websiteurl") != ""))
                            webSite = true;

                        followup["subject"] = webSite ? "Tarea cpn WEB" : "Tarea sin WEB";
                        followup["description"] = webSite ? "El contachto ya posee un sitio web" : "El contacto no posee sitio web";

                        followup["category"] = context.PrimaryEntityName;

                        if (context.OutputParameters.Contains("id"))
                        {
                            Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                            string regardingobjectidType = "account";

                            followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                        }


                        IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        IOrganizationService service = factory.CreateOrganizationService(context.UserId);
                        service.Create(followup);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("Error al ejecutar plugin: ", ex);
                }
            }
        }
    }
}
