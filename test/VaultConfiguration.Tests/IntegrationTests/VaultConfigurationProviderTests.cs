using System;
using HashiCorp.Vault;
using HashiCorp.Vault.Authentication.Token;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Tests.IntegrationTests {

    public class VaultConfigurationProviderTests {
        private readonly Uri _vaultAddress = new Uri("http://127.0.0.1:8200");
        private readonly IVaultService _vault;

        public VaultConfigurationProviderTests()
        {
            
            var token = "590254fd-d75f-f545-05c2-fbfc023ddc49";
            var address = _vaultAddress;
            _vault = new VaultService(address).AuthenticateUsingToken(token);            
        }

        [Fact]
        public void ReadConfiguration() {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(_vault, "hello"));
            
            var config = configBuilder.Build();
            var level1 = config["level1:value"];
            var level2 = config["level2:value"];

            Assert.Equal("l1", level1);
            Assert.Equal("l2", level1);
        }

        [Fact]
        public void CanReadSimpleObject()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(_vault, "hello"));
            
            var config = configBuilder.Build();

        }
    }

}
