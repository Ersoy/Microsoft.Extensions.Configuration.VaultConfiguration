using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Models {

    [DataContract]
    public class SecretBundle {

        [DataMember(Name = "request_id")]
        public Guid RequestId { get; set; }

        [DataMember(Name = "lease_id")]
        public string LeaseId { get; set; }

        [DataMember(Name = "renewable")]
        public bool Renewable { get; set; }

        [DataMember(Name = "lease_duration")]
        public int LeaseDuration { get; set; }

        [DataMember(Name = "data")]
        public JObject Data { get; set; }

        [DataMember(Name = "wrap_info")]
        public object WrapInfo { get; set; }

        [DataMember(Name = "warnings")]
        public object Warnings { get; set; }

        [DataMember(Name = "auth")]
        public AuthBundle Auth { get; set; }

    }

}
