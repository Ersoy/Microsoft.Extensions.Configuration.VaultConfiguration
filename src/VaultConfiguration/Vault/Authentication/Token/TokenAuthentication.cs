using System;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Utilities;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication.Token {

    /// <summary>
    /// Represents a token authentication.
    /// </summary>
    public class TokenAuthentication : IVaultAuthentication {
        private readonly SecureString m_vaultToken;

        /// <summary>
        /// Sets the vault authentication token.
        /// </summary>
        /// <param name="vaultService">The vault service instance.</param>
        /// <param name="vaultToken">The vault token.</param>
        public TokenAuthentication(DefaultVaultService vaultService, string vaultToken) {
            VaultService = vaultService ?? throw new ArgumentNullException(nameof(vaultService));
            m_vaultToken = vaultToken.ToSecureString();
        }

        public DefaultVaultService VaultService { get; }

        public Task<SecureString> AuthenticateAsync() {
            return Task.FromResult(m_vaultToken);
            //var response = await VaultService.ReadSecretAsync($"auth/token/lookup/{_vaultToken.ToUnicodeString()}").ConfigureAwait(false);
            //var authBundle = response.Data.ToObject<AuthBundle>();
            //return authBundle.Token.ToSecureString();
        }

    }

}
