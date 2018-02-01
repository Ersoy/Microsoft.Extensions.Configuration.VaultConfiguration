using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Tests.IntegrationTests
{
    /// <summary>
    /// The tests need to write to vault in order to set up proper state
    /// </summary>
    public class VaultUtils
    {
        private readonly string m_token;
        private readonly string m_address;

        public VaultUtils(string token, string address)
        {
            m_token = token;
            m_address = address;
        }

        public void Write(string path, string key, string value)
        {
            var uri = new Uri($"{m_address}/v1/secret/{path}");
            var dataAsJson = JsonConvert.SerializeObject(new Dictionary<string, string> {{key, value}});

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Vault-Token", m_token);
                var response = client.PostAsJsonAsync(uri, new Dictionary<string, string> { { key, value } }).ConfigureAwait(false).GetAwaiter().GetResult();
                var responseBody = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}
