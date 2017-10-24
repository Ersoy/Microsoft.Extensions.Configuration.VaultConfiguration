using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Authentication;
using System.Threading.Tasks;
using HashiCorp.Vault.Authentication;
using HashiCorp.Vault.Models;
using HashiCorp.Vault.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HashiCorp.Vault {

    /// <summary>
    /// Default implementation of the <see cref="IVaultService"/> interface.
    /// </summary>
    public class VaultService : IVaultService {

        public const string XVaultToken = "X-Vault-Token";

        public VaultService(Uri vaultAddress) {
            Client = new HttpClient {
                BaseAddress = new Uri(vaultAddress + "/v1/")
            };
        }

        public HttpClient Client { get; }

        protected SecureString AuthToken { get; set; }

        /// <inheritdoc />
        public async Task AuthenticateAsync(IVaultAuthentication vaultAuthentication) {
            AuthToken = await vaultAuthentication.AuthenticateAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IDictionary<string, string>> ReadSecretDataAsync(string path) {
            var secretValue = await ReadSecretAsync(path).ConfigureAwait(false);
            return secretValue.Data.ToObject<Dictionary<string, string>>();
        }

        /// <inheritdoc />
        public Task<SecretBundle> ReadSecretAsync(string path) {
            return RequestSecret(path);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> ListAsync(string path) {
            var secretValue = await RequestSecret(path + "?list=true").ConfigureAwait(false);
            var keys = secretValue.Data["keys"].ToString();
            return JsonConvert.DeserializeObject<string[]>(keys);
        }

        public async Task<HealthBundle> HealthAsync() {
            var response = await Request("sys/health");
            var contentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<HealthBundle>(contentAsString);
            return result;
        }

        /// <inheritdoc />
        public async Task<VaultHealthStatus> HealthStatusAsync() {
            try {
                var result = await Request("sys/health");
                var statusCode = (int)result.StatusCode;
                return (VaultHealthStatus)statusCode;
            }
            catch (HttpRequestException) {
                return VaultHealthStatus.ConnectionError;
            }            
        }

        protected virtual async Task<HttpResponseMessage> Request(string request) {
            if (string.IsNullOrWhiteSpace(request)) {
                throw new ArgumentNullException(nameof(request));
            }

            try {
                if (AuthToken != null) {
                    Client.DefaultRequestHeaders.Add(XVaultToken, AuthToken.ToUnicodeString());
                }

                var response = await Client.GetAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode) {
                    var json = await response.Content.ReadAsStringAsync();
                    var r = JsonConvert.DeserializeObject<JObject>(json);

                    var errors = r["errors"]?.Any() == true ? r["errors"] : null;
                    
                    if (errors?[0].ToString() == "missing client token") {
                        throw new AuthenticationException(
                            "Not authenticated (AuthenticateAsync is not invoked before making this request) to Vault.");
                    }
                    if (response.StatusCode == HttpStatusCode.Forbidden) {
                        throw new HttpRequestException(errors?[0].ToString());
                    }
                }

                return response;
            }
            finally {
                Client.DefaultRequestHeaders.Remove(XVaultToken);
            }
        }

        protected virtual async Task<SecretBundle> RequestSecret(string request) {
            var response = await Request(request);
            response.EnsureSuccessStatusCode();
            var contentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<SecretBundle>(contentAsString);
        }
    }

}
