namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication.Ldap {

    namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication.Ldap {

        public static class LdapAuthenticationExtensions {
    
            public static IVaultService AuthenticateUsingLdap(this DefaultVaultService vaultService, string username, string password) {
                var auth = new LdapAuthentication(vaultService);
                auth.Credentials(username, password);
                vaultService.AuthenticateAsync(auth).ConfigureAwait(false).GetAwaiter().GetResult();
                return vaultService;
            }
        
        }

    }


}