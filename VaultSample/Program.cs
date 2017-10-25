using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.VaultConfiguration;

namespace VaultSample 
{
    class Program
    {
        static void Main(string[] args) 
        {
            var config = new ConfigurationBuilder()
                .AddVault(
                    new Uri("http://127.0.0.1:8200"),
                    "de24bef9-56f1-8391-98ce-f6fa9ab53df1",
                    "")
                .Build();

            var result = config["secret:hello:level2"];
            Console.WriteLine(result);

            Console.ReadLine();
        }
    }
}
