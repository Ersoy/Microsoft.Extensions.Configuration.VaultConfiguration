using System;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using HashiCorp.Vault.Models;
using HashiCorp.Vault.Utilities;
using Newtonsoft.Json;

namespace HashiCorp.Vault.Authentication.Ldap {

    /// <summary>
    /// Represents the LDAP authentication.
    /// </summary>
    public class LdapAuthentication : IVaultAuthentication {

        private SecureString _userName;
        private SecureString _password;

        public LdapAuthentication(IVaultService vaultService) {
            VaultService = vaultService ?? throw new ArgumentNullException(nameof(vaultService));
        }

        protected IVaultService VaultService { get; }

        /// <summary>
        /// Sets ldap credentials.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public void Credentials(string userName, string password) {
            _userName = userName.ToSecureString();
            _password = password.ToSecureString();
        }

        public async Task<SecureString> AuthenticateAsync() {
            var response = await VaultService.Client.PostAsync(
                               $"auth/ldap/login/{_userName.ToUnicodeString()}",
                               new StringContent(
                                   JsonConvert.SerializeObject(new {password = _password.ToUnicodeString()})));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SecretBundle>(content);

            var authToken = result?.Auth?.Token;

            if (string.IsNullOrWhiteSpace(authToken)) {
                throw new InvalidOperationException("Empty/null authentication token is returned from the vault service.");
            }

            return authToken.ToSecureString();
        }

    }

}
