namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication.Token {

    public static class TokenAuthenticationExtensions {

        public static IVaultService AuthenticateUsingToken(this DefaultVaultService vaultService, string token) {
            var auth = new TokenAuthentication(vaultService, token);
            vaultService.AuthenticateAsync(auth).ConfigureAwait(false).GetAwaiter().GetResult();
            return vaultService;
        }

    }

}
