using System;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using CommandLine;
using System.Threading.Tasks;

namespace Toss.AzureSetup
{
    class Program
    {
        public class Options
        {
            [Option('s', "client-secret", Required = true, HelpText = "Azure account client secret.")]
            public string ClientSecret { get; set; }

            [Option('i', "client-id", Required = true, HelpText = "Azure account client id.")]
            public string ClientId { get; set; }

            [Option('t', "tenant", Required = true, HelpText = "Azure tenant id.")]
            public string TenantId { get; set; }

            [Option('p', "prefix", Required = true, HelpText = "Ressource prefix name")]
            public string RessourcePrefix { get; set; }

            [Option('d', "drop", Default = false, Required = false, HelpText = "If set to true then the ressource group is removed at the end of the run")]
            public bool DropAtTheEnd { get; set; }

            [Option('r', "region", Required = false, HelpText = "Region on which the ressources must be created (default EuropeWest)")]
            public Region Region { get; set; } = Region.EuropeWest;

            public string GetResourceName(string suffix)
            {
                return RessourcePrefix + "-" + suffix;
            }
        }
        static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(async o =>
                   {
                       //connect
                       Console.WriteLine("Connecting");
                       var azureCredentials = new AzureCredentials(
                           new ServicePrincipalLoginInformation
                           {
                               ClientId = o.ClientId,
                               ClientSecret = o.ClientSecret
                           },
                            o.TenantId,
                            AzureEnvironment.AzureGlobalCloud);
                       var azure = Azure
                            .Configure()
                            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                            .Authenticate(azureCredentials)
                            .WithDefaultSubscription();
                       Console.WriteLine("Creating resource group");
                       //create ressource group
                       var resourceGroup = await azure.ResourceGroups
                            .Define(o.GetResourceName("rg"))
                            .WithRegion(o.Region)
                            .CreateAsync();
                       //create app insight
                       //drop ressource group if DropAtTheEnd = true
                       if (o.DropAtTheEnd)
                       {
                           azure.ResourceGroups.DeleteByName(resourceGroup.Name);
                       }
                   });

        }
    }
}
