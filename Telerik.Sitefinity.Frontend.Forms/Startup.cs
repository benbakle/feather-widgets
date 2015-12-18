using System.Web.UI;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ContentUI;

namespace Telerik.Sitefinity.Frontend.Forms
{
    /// <summary>
    /// Contains the application startup event handles related to the Feather Forms functionality of Sitefinity.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Called before the Asp.Net application is started.
        /// </summary>
        public static void OnPreApplicationStart()
        {
            Bootstrapper.Initialized -= Startup.Bootstrapper_Initialized;
            Bootstrapper.Initialized += Startup.Bootstrapper_Initialized;
        }

        private static void Bootstrapper_Initialized(object sender, Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                ObjectFactory.Container.RegisterInstance<IControlDefinitionExtender>("FormsDefinitionsExtender", new FormsDefinitionsExtender(), new ContainerControlledLifetimeManager());
                
                EventHub.Unsubscribe<IScriptsRegisteringEvent>(Startup.RegisteringFormScriptsHandler);
                EventHub.Subscribe<IScriptsRegisteringEvent>(Startup.RegisteringFormScriptsHandler);
            }
        }

        private static void RegisteringFormScriptsHandler(IScriptsRegisteringEvent @event)
        {
            var zoneEditor = @event.Sender as ZoneEditor;
            if (SystemManager.GetModule("Feather") != null && zoneEditor != null && zoneEditor.MediaType == DesignMediaType.Form && zoneEditor.Framework == PageTemplateFramework.Mvc)
            {
                @event.Scripts.Add(new ScriptReference(string.Format("~/Frontend-Assembly/{0}/Mvc/Scripts/Form/form.js", typeof(Initializer).Assembly.GetName().Name)));
            }
        }
    }
}
