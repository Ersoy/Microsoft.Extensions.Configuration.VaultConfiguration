using System.Threading.Tasks;

namespace HashiCorp.Vault.Authentication.UserPass {

    public static class UserPassAuthenticationExtensions {

        public static IVaultService AuthenticateUsingUserPass(this DefaultVaultService vaultService, string userName, string password) {
            var auth = new UserPassAuthentication(vaultService);
            auth.Credentials(userName, password);
            vaultService.AuthenticateAsync(auth).ConfigureAwait(false).GetAwaiter().GetResult();
            return vaultService;
        }

    }

}
