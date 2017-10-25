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
    public abstract class DefaultVaultService : IVaultService {

        public const string XVaultToken = "X-Vault-Token";

        protected DefaultVaultService(Uri vaultAddress) {
            Client = new HttpClient {
                BaseAddress = new Uri(vaultAddress + "/v1/")
            };
        }

        public HttpClient Client { get; }

        protected SecureString AuthToken { get; set; }

        /// <inheritdoc />
        public abstract Task AuthenticateAsync(IVaultAuthentication vaultAuthentication);
        
        /// <inheritdoc />
        public abstract Task<SecretBundle> ReadSecretAsync(string path, object payload = null);

        /// <inheritdoc />
        public abstract Task<SecretBundle<T>> ReadSecretAsync<T>(string path, object payload = null);

        /// <inheritdoc />
        public abstract Task<IEnumerable<string>> ListAsync(string path);

        /// <inheritdoc />
        public abstract Task<HealthBundle> HealthAsync();

        /// <inheritdoc />
        public abstract Task<VaultHealthStatus> HealthStatusAsync();

        protected virtual async Task<HttpResponseMessage> GetAsync(string request) {
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

        protected virtual async Task<HttpResponseMessage> PostAsync(string request, HttpContent payload) {
            if (string.IsNullOrWhiteSpace(request)) {
                throw new ArgumentNullException(nameof(request));
            }

            try {
                if (AuthToken != null) {
                    Client.DefaultRequestHeaders.Add(XVaultToken, AuthToken.ToUnicodeString());
                }
                
                var response = await Client.PostAsync(request, payload).ConfigureAwait(false);

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

        protected virtual async Task<SecretBundle> RequestSecret(string request, object payload = null) {
            HttpResponseMessage response;

            if (payload != null) {
                response = await PostAsync(request, new StringContent(JsonConvert.SerializeObject(payload))).ConfigureAwait(false);
            }
            else {
                response = await GetAsync(request).ConfigureAwait(false);
            }

            response.EnsureSuccessStatusCode();
            var contentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<SecretBundle>(contentAsString);
        }

        protected virtual async Task<SecretBundle<T>> RequestSecret<T>(string request, object payload = null) {
            HttpResponseMessage response;

            if (payload != null) {
                response = await PostAsync(request, new StringContent(JsonConvert.SerializeObject(payload))).ConfigureAwait(false);
            }
            else {
                response = await GetAsync(request).ConfigureAwait(false);
            }

            response.EnsureSuccessStatusCode();
            var contentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<SecretBundle<T>>(contentAsString);
        }
    }
    
}
