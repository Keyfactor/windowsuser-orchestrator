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
    public class WindowsUserStoreManagement : LoggingClientBase, IAgentJobExtension
    {
        public string GetJobClass()
        {
            return "Management";
        }

        public string GetStoreType()
        {
            return "WinU";
        }

        public AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory, SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr)
        {
            if(config.Job.OperationType == Keyfactor.Platform.Extensions.Agents.Enums.AnyJobOperationType.Add)
            {
                return PerformAdd(config);
            }
            else if(config.Job.OperationType == Keyfactor.Platform.Extensions.Agents.Enums.AnyJobOperationType.Remove)
            {
                return PerformRemove(config);
            }
            else
            {
                throw new Exception($"Unexpected operation type: {config.Job.OperationType}");
            }
        }

        private AnyJobCompleteInfo PerformAdd(AnyJobConfigInfo config)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);

                    X509Certificate2 certToAdd = GetCertFromBase64String(config.Job.EntryContents, config.Job.PfxPassword);
                    string thumb = certToAdd.Thumbprint
                        .Replace(" ", "")
                        .Replace("\u200e", "");

                    Logger.Trace($"Searching for certificate with thumbprint {thumb}");
                    X509Certificate2Collection searchResults = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, false);

                    if(searchResults.Count == 0)
                    {
                        Logger.Trace("Adding certificate");
                        store.Add(certToAdd);
                    }
                    else
                    {
                        Logger.Warn($"Certificate with thumbprint {thumb} already exists in store. No action will be taken");
                        return new AnyJobCompleteInfo()
                        {
                            Status = 3, // Warning
                            Message = $"Certificate with thumbprint {thumb} already exists in store. No action will be taken"
                        };
                    }
                }
            }
            catch (Exception)
            {
                Logger.Error("Error adding new certificate");
                throw;
            }

            return new AnyJobCompleteInfo()
            {
                Status = 2, // Success
                Message = "Certificate added successfully"
            };
        }

        private AnyJobCompleteInfo PerformRemove(AnyJobConfigInfo config)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);

                    X509Certificate2 certToAdd = GetCertFromBase64String(config.Job.EntryContents, config.Job.PfxPassword);
                    string thumb = config.Job.Alias
                        .Replace(" ", "")
                        .Replace("\u200e", "");

                    Logger.Trace($"Searching for certificate with thumbprint {thumb}");
                    X509Certificate2Collection searchResults = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, false);

                    if (searchResults.Count == 0)
                    {
                        Logger.Warn($"Certificate with thumbprint {thumb} does not exist in store. No action will be taken");
                        return new AnyJobCompleteInfo()
                        {
                            Status = 3, // Warning
                            Message = $"Certificate with thumbprint {thumb} does not exist in store. No action will be taken"
                        };
                    }
                    else
                    {
                        Logger.Trace($"Removing certificate {thumb}");
                        store.Remove(searchResults[0]);
                    }
                }
            }
            catch (Exception)
            {
                Logger.Error("Error removing certificate");
                throw;
            }

            return new AnyJobCompleteInfo()
            {
                Status = 2, // Success
                Message = "Certificate removed successfully"
            };
        }

        private X509Certificate2 GetCertFromBase64String(string entryContents, string password)
        {
            //Logger.Debug($"Using password:{password}");
            return new X509Certificate2(Convert.FromBase64String(entryContents),
                password,
                X509KeyStorageFlags.UserKeySet
                    | X509KeyStorageFlags.PersistKeySet);
        }
    }
}
