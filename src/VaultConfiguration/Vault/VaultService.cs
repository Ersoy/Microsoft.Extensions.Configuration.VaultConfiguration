using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Models;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault {

    /// <inheritdoc />
    public class VaultService : DefaultVaultService {

        public VaultService(Uri vaultAddress) 
            : base(vaultAddress) {
        }

        /// <inheritdoc />
        public override Task<SecretBundle> ReadSecretAsync(string path, object payload = null) {
            return RequestSecret(path, payload);
        }

        /// <inheritdoc />
        public override Task<SecretBundle<T>> ReadSecretAsync<T>(string path, object payload = null) {
            return RequestSecret<T>(path, payload);
        }
        
        /// <inheritdoc />
        public override async Task<IEnumerable<string>> ListAsync(string path) {
            var secretValue = await RequestSecret(path + "?list=true").ConfigureAwait(false);
            var keys = secretValue.Data["keys"].ToString();
            return JsonConvert.DeserializeObject<string[]>(keys);
        }

        public override async Task<HealthBundle> HealthAsync() {
            var response = await GetAsync("sys/health");
            var contentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<HealthBundle>(contentAsString);
            return result;
        }

        /// <inheritdoc />
        public override async Task<VaultHealthStatus> HealthStatusAsync() {
            try {
                var result = await GetAsync("sys/health");
                var statusCode = (int)result.StatusCode;
                return (VaultHealthStatus)statusCode;
            }
            catch (HttpRequestException) {
                return VaultHealthStatus.ConnectionError;
            }
        }
    }

}
