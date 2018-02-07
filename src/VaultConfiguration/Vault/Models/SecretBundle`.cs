using System.Runtime.Serialization;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Models {

    [DataContract]
    public class SecretBundle<T> : SecretBundle {
        public new T Data { get; set; }
    }

}
