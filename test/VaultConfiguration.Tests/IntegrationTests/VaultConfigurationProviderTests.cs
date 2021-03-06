using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault;
using Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication.Token;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Tests.IntegrationTests
{
    public class VaultServerFixture : IDisposable
    {
        private readonly Process m_process;

        public VaultServerFixture()
        {
            m_process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"c:\vault\vault.exe",
                    Arguments = "server -dev",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            m_process.Start();

            do
            {
                var output = m_process.StandardOutput.ReadLine();
                if (output == null)
                {
                    break;
                }
                SetToken(output);
                SetAddress(output);
            } while (string.IsNullOrEmpty(Token));

        }

        private void SetToken(string outputLine)
        {
            const string rootToken = "Root Token: ";
            if (Token == null && !string.IsNullOrWhiteSpace(outputLine) && outputLine.StartsWith(rootToken))
            {
                Token = outputLine.Substring(rootToken.Length).Trim();
            }
        }
        private void SetAddress(string outputLine)
        {
            const string addressMarker = "set VAULT_ADDR=";
            if (Address == null && !string.IsNullOrWhiteSpace(outputLine) && outputLine.Contains(addressMarker))
            {
                Address = outputLine.Substring(outputLine.IndexOf(addressMarker, StringComparison.Ordinal) + addressMarker.Length).Trim();
            }
        }

        public string Token { get; private set; }
        public string Address { get; private set; }

        public void Dispose()
        {
            m_process.Kill();
        }
    }

    [CollectionDefinition("Vault collection")]
    public class VaultCollection : ICollectionFixture<VaultServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        // As described here: https://stackoverflow.com/questions/12976319/xunit-net-global-setup-teardown
    }

    [Collection("Vault collection")]
    public class VaultConfigurationProviderTests
    {
        private readonly Uri m_vaultAddress = new Uri("http://127.0.0.1:8200");
        private readonly IVaultService m_vault;
        private readonly VaultUtils m_vaultUtils;

        public VaultConfigurationProviderTests(VaultServerFixture vaultFixture)
        {
            m_vault = new VaultService(new Uri(vaultFixture.Address)).AuthenticateUsingToken(vaultFixture.Token);            
            m_vaultUtils = new VaultUtils(vaultFixture.Token, vaultFixture.Address);
        }

        [Fact]
        public void ReadConfiguration() {
            // GIVEN settings at secret/hello/level1 and level2
            m_vaultUtils.Write("hello/level1", "value", "l1");
            m_vaultUtils.Write("hello/level2", "value", "l2");

            // WHEN settings are retrieved
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(m_vault, "hello"));
            
            var config = configBuilder.Build();
            var level1 = config["level1:value"];
            var level2 = config["level2:value"];

            // THEN the values match!
            Assert.Equal("l1", level1);
            Assert.Equal("l2", level2);
        }

        [Fact]
        public void CanReadCollection()
        {
            // GIVEN array setting value at secret/hello/foods 
            m_vaultUtils.Write("hello/foods", "types", new object[]{"tacos", "arepas", "chili"});

            // WHEN setting is retrieved
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(m_vault, "hello"));
            
            var config = configBuilder.Build();
            var foodTypes = JsonConvert.DeserializeObject<string[]>(config["foods:types"]);

            // THEN the values match!
            Assert.Equal(3, foodTypes.Length);
            Assert.Equal("arepas", foodTypes[1]);
        }

        [Fact]
        public void CanReadMultipleKeys() {
            // GIVEN settings at secret/hello/level1 and level2
            m_vaultUtils.Write("hello/fishies", new Dictionary<string, string> {{"count", "13"}, {"species", "zebrafish"}});

            // WHEN settings are retrieved
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(m_vault, "hello"));
            
            var config = configBuilder.Build();
            var count = config["fishies:count"];
            var species = config["fishies:species"];

            // THEN the values match!
            Assert.Equal("13", count);
            Assert.Equal("zebrafish", species);
        }

        [Fact]
        public void ReadsNonExistentKey()
        {
            // GIVEN no settings

            // WHEN settings are retrieved
            var configBuilder = new ConfigurationBuilder();
            configBuilder.Add(new VaultConfigurationSource(m_vault, "hello"));
            
            var config = configBuilder.Build();
            var value = config["setting:count"];

            // THEN the values should be null
            Assert.Null(value);
        }

    }

}
