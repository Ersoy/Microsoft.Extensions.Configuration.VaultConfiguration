using System;
using HashiCorp.Vault;
using HashiCorp.Vault.Authentication.Token;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Tests.IntegrationTests {

    public class VaultConfigurationProviderTests {
        private readonly Uri _vaultAddress = new Uri("http://127.0.0.1:8200");

        [Fact]
        public void ReadConfiguration() {
            var token = "de24bef9-56f1-8391-98ce-f6fa9ab53df1";
            var address = _vaultAddress;
            var vault = new VaultService(address).AuthenticateUsingToken(token);            

            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(vault, "hello"));
            
            var config = configBuilder.Build();
            var level1 = config["secret:hello:level1"];
            var level2 = config["secret:hello:level2"];

            Assert.Equal("l1", JsonConvert.DeserializeObject<JObject>(level1)["value"]);
            Assert.Equal("l2", JsonConvert.DeserializeObject<JObject>(level2)["value"]);
        }
        
    }

}
