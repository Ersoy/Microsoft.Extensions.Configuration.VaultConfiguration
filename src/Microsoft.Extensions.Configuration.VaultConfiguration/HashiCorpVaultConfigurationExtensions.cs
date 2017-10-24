using System;
using HashiCorp.Vault;
using HashiCorp.Vault.Authentication;

namespace Microsoft.Extensions.Configuration.VaultConfiguration {

    /// <summary>
    /// Extension methods for registering <see cref="VaultConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class HashiCorpVaultConfigurationExtensions {
        
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the HashiCorp Vault. This provider uses 'token' authentication.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The HashiCorp Vault uri.</param>
        /// <param name="token">The authentication token.</param>
        /// <param name="prefix">The prefix to use when retrieving secrets from the Vault.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder configurationBuilder, Uri vaultUri, string token, string prefix) {
            if (vaultUri == null) {
                throw new ArgumentNullException(nameof(vaultUri));
            }
            if (string.IsNullOrWhiteSpace(token)) {
                throw new ArgumentNullException(nameof(token));
            }

            var vault = new VaultService(vaultUri);
            var source = new VaultConfigurationSource(vault, prefix);
            vault.AuthenticateAsync(new TokenAuthentication(token)).ConfigureAwait(false).GetAwaiter().GetResult();

            return configurationBuilder.Add(source);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the HashiCorp Vault. This provider uses 'userpass' authentication.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The HashiCorp Vault uri.</param>
        /// <param name="userName">The user name for userpass authentication.</param>
        /// <param name="password">The password for userpass authentication.</param>
        /// <param name="prefix">The prefix to use when retrieving secrets from the Vault.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder configurationBuilder, Uri vaultUri, string userName, string password, string prefix) {
            if (vaultUri == null) {
                throw new ArgumentNullException(nameof(vaultUri));
            }
            if (string.IsNullOrWhiteSpace(userName)) {
                throw new ArgumentNullException(nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentNullException(nameof(password));
            }

            var vault = new VaultService(vaultUri);
            var source = new VaultConfigurationSource(vault, prefix);

            var authentication = new UserPassAuthentication(vault);
            authentication.Credentials(userName, password);
            vault.AuthenticateAsync(authentication).ConfigureAwait(false).GetAwaiter().GetResult();

            return configurationBuilder.Add(source);
        }
    }

}
