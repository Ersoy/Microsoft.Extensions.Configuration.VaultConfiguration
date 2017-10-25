using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HashiCorp.Vault.Authentication;
using HashiCorp.Vault.Models;

namespace HashiCorp.Vault {

    /// <summary>
    /// The vault service. This is the entry point for vault communication.
    /// </summary>
    public interface IVaultService {

        /// <summary>
        /// Gets the rest client.
        /// </summary>
        HttpClient Client { get; }
        
        /// <summary>
        /// Authenticate to vault.
        /// </summary>
        /// <param name="vaultAuthentication">Authentication token.</param>        
        Task AuthenticateAsync(IVaultAuthentication vaultAuthentication);

        /// <summary>
        /// Read data from Vault.
        /// </summary>
        /// <typeparam name="T">Type of returned result.</typeparam>
        /// <param name="path">Path to a backend.</param>
        /// <returns>
        /// Reads data at the given path from Vault. This can be used to read 
        /// secrets and configuration as well as generate dynamic values from 
        /// materialized backends. Please reference the documentation for the 
        /// backends in use to determine key structure.
        /// </returns>
        Task<IDictionary<string, string>> ReadSecretDataAsync(string path);

        /// <summary>
        /// Read data as <see cref="SecretBundle"/> from Vault.
        /// </summary>        
        /// <param name="path">Path to a backend.</param>
        /// <returns>
        /// Reads data at the given path from Vault.This can be used to read 
        /// secrets and configuration as well as generate dynamic values from 
        /// materialized backends. Please reference the documentation for the 
        /// backends in use to determine key structure.
        /// </returns>
        Task<SecretBundle> ReadSecretAsync(string path);

        /// <summary>
        /// Read data as <see cref="SecretBundle"/> from Vault.
        /// </summary>        
        /// <param name="path">Path to a backend.</param>
        /// <returns>
        /// Reads data at the given path from Vault.This can be used to read 
        /// secrets and configuration as well as generate dynamic values from 
        /// materialized backends. Please reference the documentation for the 
        /// backends in use to determine key structure.
        /// <typeparam name="T">The type of the <code>Data</code> property.</typeparam>
        /// </returns>
        Task<SecretBundle<T>> ReadSecretAsync<T>(string path);

        /// <summary>
        /// List data from Vault.
        /// </summary>        
        /// <param name="path">Path to a backend.</param>
        /// <returns> Retrieve a listing of available data. The data returned, 
        /// if any, is backend-and endpoint-specific. </returns>
        Task<IEnumerable<string>> ListAsync(string path);

        /// <summary>
        /// Gets health status info of Vault.
        /// </summary>
        /// <returns>Returns <see cref="HealthBundle"/> that describes the Vault's health info.</returns>
        Task<HealthBundle> HealthAsync();

        /// <summary>
        /// Check the health status of Vault.
        /// </summary>
        /// <returns>Returns an enumeration that describes the Vault status.</returns>
        Task<VaultHealthStatus> HealthStatusAsync();
    }

}
