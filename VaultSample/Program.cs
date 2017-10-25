using System;
using HashiCorp.Vault;
using HashiCorp.Vault.Authentication;
using HashiCorp.Vault.Authentication.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.VaultConfiguration;

namespace VaultSample
{
    class Program
    {
        static void Main(string[] args) {
            var vault = new VaultService(new Uri("http://127.0.0.1:8200"))
                .AuthenticateUsingToken("c51eff3f-43e8-91b0-5bc3-4d9587e14de4");
            
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(vault, "hello"));
            var config = configBuilder.Build();

            var result = config["hello/level2"];
            Console.WriteLine(result);

            Console.ReadLine();
        }
    }
}
