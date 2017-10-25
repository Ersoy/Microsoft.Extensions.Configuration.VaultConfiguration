using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HashiCorp.Vault.Authentication;
using HashiCorp.Vault.Models;
using Newtonsoft.Json;

namespace HashiCorp.Vault {

    /// <inheritdoc />
    public class VaultService : DefaultVaultService {

        public VaultService(Uri vaultAddress) 
            : base(vaultAddress) {
        }

        /// <inheritdoc />
        public override async Task AuthenticateAsync(IVaultAuthentication vaultAuthentication) {
            AuthToken = await vaultAuthentication.AuthenticateAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<IDictionary<string, string>> ReadSecretDataAsync(string path) {
            var secretValue = await ReadSecretAsync(path).ConfigureAwait(false);
            return secretValue.Data.ToObject<Dictionary<string, string>>();
        }

        /// <inheritdoc />
        public override Task<SecretBundle> ReadSecretAsync(string path) {
            return RequestSecret(path);
        }

        public override Task<SecretBundle<T>> ReadSecretAsync<T>(string path) {
            return RequestSecret<T>(path);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<string>> ListAsync(string path) {
            var secretValue = await RequestSecret(path + "?list=true").ConfigureAwait(false);
            var keys = secretValue.Data["keys"].ToString();
            return JsonConvert.DeserializeObject<string[]>(keys);
        }

        public override async Task<HealthBundle> HealthAsync() {
            var response = await Request("sys/health");
            var contentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<HealthBundle>(contentAsString);
            return result;
        }

        /// <inheritdoc />
        public override async Task<VaultHealthStatus> HealthStatusAsync() {
            try {
                var result = await Request("sys/health");
                var statusCode = (int)result.StatusCode;
                return (VaultHealthStatus)statusCode;
            }
            catch (HttpRequestException) {
                return VaultHealthStatus.ConnectionError;
            }
        }
    }

}
