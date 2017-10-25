using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
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
        public virtual async Task AuthenticateAsync(IVaultAuthentication vaultAuthentication) {
            AuthToken = await vaultAuthentication.AuthenticateAsync().ConfigureAwait(false);
            try {
                var response = await ReadSecretAsync($"auth/token/lookup-self").ConfigureAwait(false);
                var authBundle = response.Data.ToObject<AuthBundle>();
                if (authBundle.Token != AuthToken.ToUnicodeString()) {
                    Debug.Assert(authBundle.Token == AuthToken.ToUnicodeString());
                }
            }
            catch (HttpException e) {
                throw new HttpException(e.StatusCode, "Invalid token.", e);
            }
        }

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

        protected virtual Task<HttpResponseMessage> GetAsync(string request) => InvokeAsync(() => Client.GetAsync(request));

        protected virtual Task<HttpResponseMessage> PostAsync(string request, HttpContent payload) => InvokeAsync(() => Client.PostAsync(request, payload));

        protected virtual async Task<HttpResponseMessage> InvokeAsync(Func<Task<HttpResponseMessage>> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            
            try {
                if (AuthToken != null) {
                    Client.DefaultRequestHeaders.Add(XVaultToken, AuthToken.ToUnicodeString());
                }

                var response = await request().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode) {
                    var json = await response.Content.ReadAsStringAsync();
                    var r = JsonConvert.DeserializeObject<JObject>(json);

                    var errors = r["errors"]?.Any() == true ? r["errors"] : null;

                    if (errors?[0].ToString() == "missing client token") {
                        throw new HttpException(response.StatusCode, "Not authenticated (AuthenticateAsync is not invoked before making this request) to Vault.");
                    }
                    if (response.StatusCode == HttpStatusCode.Forbidden) {
                        throw new HttpException(response.StatusCode, errors?[0].ToString());
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
