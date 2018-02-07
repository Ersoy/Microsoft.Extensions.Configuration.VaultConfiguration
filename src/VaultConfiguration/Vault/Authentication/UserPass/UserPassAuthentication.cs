using System;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Models;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Utilities;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication.UserPass {

    /// <summary>
    /// Represents the 'userpass' authentication.
    /// </summary>
    public class UserPassAuthentication : IVaultAuthentication {

        private SecureString m_userName;
        private SecureString m_password;

        public UserPassAuthentication(IVaultService vaultService) {
            VaultService = vaultService ?? throw new ArgumentNullException(nameof(vaultService));
        }

        protected IVaultService VaultService { get; }

        /// <summary>
        /// Sets ldap credentials.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public void Credentials(string userName, string password) {
            m_userName = userName.ToSecureString();
            m_password = password.ToSecureString();
        }

        public async Task<SecureString> AuthenticateAsync() {
            var response = await VaultService.Client.PostAsync(
                $"auth/userpass/login/{m_userName.ToUnicodeString()}",
                new StringContent(
                    JsonConvert.SerializeObject(new {password = m_password.ToUnicodeString()})));

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
