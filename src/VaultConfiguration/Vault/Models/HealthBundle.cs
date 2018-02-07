using System;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Models {

    [DataContract]
    public class HealthBundle {
        [DataMember(Name ="cluster_id")]
        public Guid ClusterId { get; set; }

        [DataMember(Name = "cluster_name")]
        public string ClusterName { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "server_time_utc")]
        public string ServerTime { get; set; }

        [DataMember(Name = "standby")]
        public bool Standby { get; set; }

        [DataMember(Name = "sealed")]
        public bool Sealed { get; set; }

        [DataMember(Name = "initialized")]
        public bool Initialized { get; set; }
    }

}