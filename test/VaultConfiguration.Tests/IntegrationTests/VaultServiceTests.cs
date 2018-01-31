using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using HashiCorp.Vault;
using HashiCorp.Vault.Authentication;
using HashiCorp.Vault.Authentication.Token;
using Xunit;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Tests.IntegrationTests {

    public class VaultServiceTests {
        private readonly Uri _vaultAddress = new Uri("http://127.0.0.1:8200");

        [Fact]
        public async Task ConnectionError() {
            var vault = new VaultService(new Uri("http://unknown-server:8200"));
            var result = await vault.HealthStatusAsync();
            Assert.Equal(VaultHealthStatus.ConnectionError, result);
        }

        [Fact]
        public async Task Status() {
            var vault = VaultService();
            var result = await vault.HealthStatusAsync();
            Assert.NotEqual(VaultHealthStatus.Unknown, result);
            Assert.NotEqual(VaultHealthStatus.ConnectionError, result);
        }

        [Fact]
        public async Task ReadSecretWithoutAuthThrows() {
            var vault = VaultService();
            await Assert.ThrowsAsync<AuthenticationException>(() => vault.ReadSecretAsync("secret/hello"));
        }

        [Fact]
        public async Task ReadSecret() {
            var vault = await AuthVaultService();
            var secret = await vault.ReadSecretAsync("secret/hello");
            Assert.Equal("world", secret.Data["value"].ToString());
        }

        [Fact]
        public async Task AuthInvalidTokenThrows() {
            var randomlyGeneretedInvalidToken = Guid.NewGuid().ToString();
            var vault = VaultService().AuthenticateUsingToken(randomlyGeneretedInvalidToken);
            var exc = await Assert.ThrowsAsync<HttpRequestException>(() => vault.ReadSecretAsync("secret/hello"));
            Assert.Equal("permission denied", exc.Message);
        }

        private DefaultVaultService VaultService() {
            return new VaultService(_vaultAddress);
        }

        private async Task<DefaultVaultService> AuthVaultService() {
            var token = "de24bef9-56f1-8391-98ce-f6fa9ab53df1";
            var vault = new VaultService(_vaultAddress).AuthenticateUsingToken(token) as DefaultVaultService;
            return vault;
        }
    }

}
