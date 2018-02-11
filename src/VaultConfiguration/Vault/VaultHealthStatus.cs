namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault {

    public enum VaultHealthStatus {
        Unknown,
        ConnectionError,
        Active = 200,
        UnsealedStandBy = 429,
        Uninitialized = 501,
        Sealed = 503
    }

}
