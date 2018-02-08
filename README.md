# Microsoft.Extensions.Configuration.VaultConfiguration
[HashiCorp Vault](https://www.vaultproject.io/) configuration provider implementation for Microsoft.Extensions.Configuration.

### Usage
When this package is installed, it allows calling `AddVault()` as described below to tell the configuration builder to hydrate config values from Vault (as opposed to JSON files or environment variables). 

```
var vaultUri = Environment.GetEnvironmentVariable("VAULT_ADDR");
var vaultAccessToken = Environment.GetEnvironmentVariable("VAULT_TOKEN");
var prefix = "MyApp"; // each application should use its own prefix, in general

var builder = new ConfigurationBuilder()
    .AddVault(vaultUri, vaultAccessToken, prefix);
```

## Change history

### Version 1.2.0
* Moved code that interfaces with Vault from separate project into the main project, under Vault namespace
* Added change history to readme
* Some internal changes to naming to match Buildium's code standards
