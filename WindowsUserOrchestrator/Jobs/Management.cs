// Copyright 2021 Keyfactor
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System;
using System.Security.Cryptography.X509Certificates;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Keyfactor.Platform.Extensions.Agents.Enums;
using Keyfactor.Platform.Extensions.Agents.Interfaces;

namespace Keyfactor.Extensions.Orchestrator.WindowsUser
{
    [Job(JobTypes.MANAGEMENT)]
    public class Management : AgentJob, IAgentJobExtension
    {
        public override AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory, SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr)
        {

            AnyJobCompleteInfo complete = new AnyJobCompleteInfo()
            {
                Status = 4,
                Message = "Invalid Management Operation"
            };

            switch (config.Job.OperationType)
            {
                case AnyJobOperationType.Add:
                    complete = PerformAddition(config.Job.EntryContents, config.Job.PfxPassword);
                    break;
                case AnyJobOperationType.Remove:
                    complete = PerformRemoval(config.Job.Alias);
                    break;
            }
            return complete;
        }

        protected virtual AnyJobCompleteInfo PerformAddition(string entryContents, string pfxPassword)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);

                    X509Certificate2 certToAdd = GetCertFromBase64String(entryContents, pfxPassword);
                    string thumb = certToAdd.Thumbprint
                        .Replace(" ", "")
                        .Replace("\u200e", "");

                    Logger.Trace($"Searching for certificate with thumbprint {thumb}");
                    X509Certificate2Collection searchResults = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, false);

                    if (searchResults.Count == 0)
                    {
                        Logger.Trace("Adding certificate");
                        store.Add(certToAdd);
                    }
                    else
                    {
                        var msg = $"Certificate with thumbprint {thumb} already exists in store. No action will be taken";
                        Logger.Warn(msg);
                        return new AnyJobCompleteInfo()
                        {
                            Status = 3, // Warning
                            Message = msg
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

        protected virtual AnyJobCompleteInfo PerformRemoval(string alias)
        {
            try
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);

                    string thumb = alias
                        .Replace(" ", "")
                        .Replace("\u200e", "");

                    Logger.Trace($"Searching for certificate with thumbprint {thumb}");
                    X509Certificate2Collection searchResults = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, false);

                    if (searchResults.Count == 0)
                    {
                        var msg = $"Certificate with thumbprint {thumb} does not exist in store. No action will be taken";
                        Logger.Warn(msg);
                        return new AnyJobCompleteInfo()
                        {
                            Status = 3, // Warning
                            Message = msg
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

