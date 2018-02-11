using System;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Models {

    [DataContract]
    public class AuthBundle {

        [DataMember(Name = "accessor")]
        public Guid Accessor { get; set; }

        [DataMember(Name = "creation_time")]
        public int CreationTime { get; set; }

        [DataMember(Name = "creation_ttl")]
        public int CreationTimeToLive { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "expire_time")]
        public string ExpireTime { get; set; }

        [DataMember(Name = "explicit_max_ttl")]
        public int ExplicitMaxTimeToLive { get; set; }

        [DataMember(Name = "id")]
        public string Token { get; set; }

        [DataMember(Name = "meta")]
        public string Meta { get; set; }

        [DataMember(Name = "num_uses")]
        public string NumberOfUses { get; set; }

        [DataMember(Name = "orphan")]
        public bool Orphan { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "policies")]
        public string[] Policies { get; set; }

        [DataMember(Name = "ttl")]
        public string TimeToLive { get; set; }
    }

}
