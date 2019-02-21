using Microsoft.AspNetCore.Components.Builder;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public static class ServiceCollectionConfigurationBuilderExtension
    {
        public static void AddEnvironmentConfiguration<TResource>(
            this IServiceCollection serviceCollection,
            Func<EnvironmentChooser> environmentChooserFactory)
        {
            serviceCollection.AddSingleton<IConfiguration>((s) =>
            {
                var environementChooser = environmentChooserFactory();
                var uri = new Uri(s.GetRequiredService<IUriHelper>().GetAbsoluteUri());
                System.Reflection.Assembly assembly = typeof(TResource).Assembly;
                string environment = environementChooser.GetCurrent(uri);
                var ressourceNames = new[]
                {
                    assembly.GetName().Name + ".Configuration.appsettings.json",
                    assembly.GetName().Name + ".Configuration.appsettings." + environment + ".json"
                };
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "Environment", environment }
                });
                Console.WriteLine(string.Join(",", assembly.GetManifestResourceNames()));
                Console.WriteLine(string.Join(",", ressourceNames));
                foreach (var resource in ressourceNames)
                {

                    if (assembly.GetManifestResourceNames().Contains(resource))
                    {
                        configurationBuilder.AddJsonFile(
                            new InMemoryFileProvider(assembly.GetManifestResourceStream(resource)), resource, false, false);
                    }
                }
                return configurationBuilder.Build();
            });
        }

        public static void InitEnvironmentConfiguration(this IComponentsApplicationBuilder app)
        {
            //force config initialisation with current uri
            IConfiguration config = app.Services.GetService<IConfiguration>();
        }
        //Even though we load it from embedded ressource, net core configuration still needs a file provider
        // inspired from Rob Lyndon answer https://stackoverflow.com/a/52405277/277067
        public class InMemoryFileProvider : IFileProvider
        {
            private class InMemoryFile : IFileInfo
            {
                private readonly Stream _data;
                public InMemoryFile(Stream stream) => _data = stream;
                public Stream CreateReadStream() => _data;
                public bool Exists { get; } = true;
                public long Length => _data.Length;
                public string PhysicalPath { get; } = string.Empty;
                public string Name { get; } = string.Empty;
                public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;
                public bool IsDirectory { get; } = false;
            }

            private readonly IFileInfo _fileInfo;
            public InMemoryFileProvider(Stream stream) => _fileInfo = new InMemoryFile(stream);
            public IFileInfo GetFileInfo(string _) => _fileInfo;
            public IDirectoryContents GetDirectoryContents(string _) => null;
            public IChangeToken Watch(string _) => NullChangeToken.Singleton;
        }
    }
}
