using System.Runtime.Serialization;

namespace HashiCorp.Vault.Models {

    [DataContract]
    public class DatabaseCredentials {

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

    }

}
