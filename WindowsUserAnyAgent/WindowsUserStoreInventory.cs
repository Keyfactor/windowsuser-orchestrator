using CSS.Common.Logging;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Keyfactor.Platform.Extensions.Agents.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WindowsUserAnyAgent
{
    public class WindowsUserStoreInventory : LoggingClientBase, IAgentJobExtension
    {
        public string GetJobClass()
        {
            return "Inventory";
        }

        public string GetStoreType()
        {
            return "WinU";
        }

        public AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory, SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr)
        {
            return PerformInventory(config, submitInventory);
        }

        private AnyJobCompleteInfo PerformInventory(AnyJobConfigInfo config, SubmitInventoryUpdate inventoryCallback)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    List<AgentCertStoreInventoryItem> inventory = store.Certificates.OfType<X509Certificate2>().Select(c => new AgentCertStoreInventoryItem()
                    {
                        Alias = c.Thumbprint.Replace(" ", ""),
                        Certificates = new string[] {Convert.ToBase64String(c.RawData)},
                        ItemStatus = Keyfactor.Platform.Extensions.Agents.Enums.AgentInventoryItemStatus.Unknown,
                        PrivateKeyEntry = c.HasPrivateKey,
                        UseChainLevel = false
                    })
                    .ToList();

                    if(!inventoryCallback.Invoke(inventory))
                    {
                        throw new Exception("Error submitting updated inventory");
                    }
                }
            }
            catch (Exception)
            {
                Logger.Error("Error collecting new inventory");
                throw;
            }

            return new AnyJobCompleteInfo()
            {
                Status = 2, // Success
                Message = "Inventory completed successfully"
            };
        }
    }
}
