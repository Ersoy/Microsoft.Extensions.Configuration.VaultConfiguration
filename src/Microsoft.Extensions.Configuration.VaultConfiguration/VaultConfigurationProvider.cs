using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HashiCorp.Vault;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Configuration.VaultConfiguration {

    public class VaultConfigurationProvider : ConfigurationProvider {

        public VaultConfigurationProvider(IVaultService vaultService, string prefix) {
            Service = vaultService ?? throw new ArgumentNullException(nameof(vaultService));
            Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

        /// <summary>
        /// A prefix used to filter vault secrets.
        /// </summary>
        public string Prefix { get; }

        protected IVaultService Service { get; }

        public override void Load() {
            var secrets = ReadSecrets().Result;
            Load(secrets).GetAwaiter().GetResult();
        }

        private async Task Load(IEnumerable<string> secrets) {
            Data = new Dictionary<string, string>();
            foreach (var secret in secrets) {
                var result = await Service.ReadSecretDataAsync(secret);                
                Data.Add(DenormalizePath(secret), JsonConvert.SerializeObject(result));
            }
        }

        private async Task<IEnumerable<string>> ReadSecrets() {
            var secrets = new List<string>();
            await ReadSecrets(secrets, $"secret/{Prefix}");
            return secrets;
        }

        private async Task ReadSecrets(IList<string> secrets, string path) {
            var result = await Service.ListAsync(path);

            foreach (var secret in result) {
                if (secret.EndsWith("/")) {
                    await ReadSecrets(secrets, $"{path}/{secret.TrimEnd('/')}");
                }
                else {
                    secrets.Add($"{path}/{secret}");
                }
            }
        }

        public override bool TryGet(string key, out string value) {
            return Data.TryGetValue(key, out value);
        }

        protected virtual string NormalizePath(string path) {
            return string.IsNullOrWhiteSpace(path) ? path : path.Replace(ConfigurationPath.KeyDelimiter, VaultPath.PathDelimiter);
        }

        protected virtual string DenormalizePath(string path) {
            return string.IsNullOrWhiteSpace(path) ? path : path.Replace(VaultPath.PathDelimiter, ConfigurationPath.KeyDelimiter);
        }

    }

    public class VaultPath {
        public static readonly string PathDelimiter = "/";
    }

}
