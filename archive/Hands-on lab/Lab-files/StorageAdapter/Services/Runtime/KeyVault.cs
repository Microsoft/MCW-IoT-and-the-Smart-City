using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime
{
    public class KeyVault
    {

        // Key Vault details and access
        private readonly string name;
        private readonly string clientId;
        private readonly string clientSecret;
        private ILogger log;

        // Key Vault Client
        private readonly KeyVaultClient keyVaultClient;

        // Constants
        private const string KEY_VAULT_URI = "https://{0}.vault.azure.net/secrets/{1}";

        public KeyVault(
            string name,
            string clientId,
            string clientSecret,
            ILogger logger)
        {
            this.name = name;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.log = logger;
            this.keyVaultClient = new KeyVaultClient(
                                    new KeyVaultClient.AuthenticationCallback(this.GetToken));
        }

        public string GetSecret(string secretKey)
        {
            secretKey = secretKey.Split(':').Last();
            var uri = string.Format(KEY_VAULT_URI, this.name, secretKey);

            try
            {
                return this.keyVaultClient.GetSecretAsync(uri).Result.Value;
            }
            catch (Exception e)
            {
                this.log.Error($"Secret {secretKey} not found in Key Vault.", () => { });
                return null;
            }
        }

        //the method that will be provided to the KeyVaultClient
        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(this.clientId, this.clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                this.log.Debug($"Failed to obtain authentication token from key vault.", () => { });
                throw new System.InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}