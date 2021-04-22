// Copyright 2021 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using CSS.Common.Logging;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Keyfactor.Platform.Extensions.Agents.Interfaces;

namespace Keyfactor.Extensions.Orchestrator.WindowsUser
{
    public abstract class AgentJob : LoggingClientBase, IAgentJobExtension
    {
        public string GetJobClass()
        {
            var attr = GetType().GetCustomAttributes(true).First(a => a.GetType() == typeof(JobAttribute)) as JobAttribute;
            return attr?.JobClass ?? string.Empty;
        }

        public string GetStoreType() => WindowsUserAnyAgentConstants.STORE_TYPE_NAME;

        public abstract AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory, SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr);

        protected AnyJobCompleteInfo Success(string message = null)
        {

            return new AnyJobCompleteInfo()
            {
                Status = 2,
                Message = message ?? $"{GetJobClass()} Complete"
            };
        }

        protected AnyJobCompleteInfo ThrowError(Exception exception, string jobSection)
        {
            string message = FlattenException(exception);
            Logger.Error($"Error performing {jobSection} in {GetJobClass()} {GetStoreType()} - {message}");
            return new AnyJobCompleteInfo()
            {
                Status = 4,
                Message = message
            };
        }

        private string FlattenException(Exception ex)
        {
            string returnMessage = ex.Message;
            if (ex.InnerException != null)
                returnMessage += (" - " + FlattenException(ex.InnerException));

            return returnMessage;
        }
    }
}
