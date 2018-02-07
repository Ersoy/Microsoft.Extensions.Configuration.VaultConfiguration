using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            Load(secrets).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task Load(IEnumerable<string> secrets)
        {
            Data = new Dictionary<string, string>();
            foreach (var secret in secrets)
            {
                var result = await Service.ReadSecretAsync(secret).ConfigureAwait(false);

                foreach (var data in result.Data)
                {
                    if (data.Value.Type == JTokenType.String)
                    {
                        Data.Add(DenormalizePath(VaultPath.Combine(secret, data.Key)), (string) data.Value);
                    }
                    else if (data.Value.Type == JTokenType.Array)
                    {
                        Data.Add(DenormalizePath(VaultPath.Combine(secret, data.Key)), JsonConvert.SerializeObject(data.Value));
                    }
                }
            }
        }

        private async Task<IEnumerable<string>> ReadSecrets() {
            var secrets = new List<string>();
            await ReadSecrets(secrets, $"secret/{Prefix}");
            return secrets;
        }

        private async Task ReadSecrets(ICollection<string> secrets, string path) {
            var result = await Service.ListAsync(path).ConfigureAwait(false);

            foreach (var secret in result) {
                if (secret.EndsWith(VaultPath.PathDelimiter)) {
                    var readPath = VaultPath.Combine(path, secret);
                    await ReadSecrets(secrets, readPath).ConfigureAwait(false);
                }
                else {
                    secrets.Add(VaultPath.Combine(path, secret));
                }
            }
        }

        public override bool TryGet(string key, out string value) {
            // return Data.TryGetValue(key, out value);
            return Data.TryGetValue(key.Replace($"secret:{Prefix}:", string.Empty), out value);
        }

        protected virtual string NormalizePath(string path) {
            return string.IsNullOrWhiteSpace(path) ? path : path.Replace(ConfigurationPath.KeyDelimiter, VaultPath.PathDelimiter);
        }

        protected virtual string DenormalizePath(string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? path
                : path
                    .Replace(VaultPath.PathDelimiter, ConfigurationPath.KeyDelimiter)
                    .Replace($"secret{ConfigurationPath.KeyDelimiter}{Prefix}{ConfigurationPath.KeyDelimiter}", string.Empty);
        }

    }

}
