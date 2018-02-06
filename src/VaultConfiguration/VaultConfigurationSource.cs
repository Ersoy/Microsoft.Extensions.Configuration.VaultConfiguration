using System;
using HashiCorp.Vault;

namespace Microsoft.Extensions.Configuration.VaultConfiguration {

    public class VaultConfigurationSource : IConfigurationSource {
        private readonly string _prefix;
        private readonly IVaultService _vaultService;

        public VaultConfigurationSource(IVaultService vaultService, string prefix) {
            _vaultService = vaultService ?? throw new ArgumentNullException(nameof(vaultService));
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) {                        
            var source = new VaultConfigurationProvider(_vaultService, _prefix);            
            return source;
        }

    }

}
