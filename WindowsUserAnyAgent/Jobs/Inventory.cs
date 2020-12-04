using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Keyfactor.Platform.Extensions.Agents.Interfaces;

namespace Keyfactor.AnyAgent.WindowsUser
{
    [Job(JobTypes.INVENTORY)]
    public class Inventory : AgentJob, IAgentJobExtension
    {
        public override AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory, SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr)
        {
            return PerformInventory(submitInventory);
        }

        private AnyJobCompleteInfo PerformInventory(SubmitInventoryUpdate inventoryCallback)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    List<AgentCertStoreInventoryItem> inventory = store.Certificates.OfType<X509Certificate2>().Select(c => new AgentCertStoreInventoryItem()
                    {
                        Alias = c.Thumbprint.Replace(" ", ""),
                        Certificates = new string[] { Convert.ToBase64String(c.RawData) },
                        ItemStatus = Platform.Extensions.Agents.Enums.AgentInventoryItemStatus.Unknown,
                        PrivateKeyEntry = c.HasPrivateKey,
                        UseChainLevel = false
                    })
                    .ToList();

                    if (!inventoryCallback.Invoke(inventory))
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

