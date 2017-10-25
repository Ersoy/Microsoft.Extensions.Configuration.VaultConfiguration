using System.Runtime.Serialization;

namespace HashiCorp.Vault.Models {

    [DataContract]
    public class SecretBundle<T> : SecretBundle {
        public new T Data { get; set; }
    }

}
