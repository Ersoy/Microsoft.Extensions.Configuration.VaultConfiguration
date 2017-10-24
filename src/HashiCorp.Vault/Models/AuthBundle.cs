using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HashiCorp.Vault.Models {

    [DataContract]
    public class AuthBundle {

        [DataMember(Name = "client_token")]
        public string Token { get; set; }

        [DataMember(Name = "policies")]
        public IEnumerable<string> Policies { get; set; }

        [DataMember(Name = "metadata")]
        public IDictionary<string, string> Metadata { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }

    }

}
