// Copyright 2021 Keyfactor
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System;
using System.Runtime.CompilerServices;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Moq;

[assembly: InternalsVisibleTo("Keyfactor.Extensions.Orchestrator.WindowsUser")]
namespace Keyfactor.Extensions.Orchestrator.WindowsUser.Tests
{
    public static class Mocks
    {
        public static Mock<SubmitInventoryUpdate> GetSubmitInventoryDelegateMock() => new Mock<SubmitInventoryUpdate>();

        public static Mock<SubmitEnrollmentRequest> GetSubmitEnrollmentDelegateMock() => new Mock<SubmitEnrollmentRequest>();

        public static Mock<SubmitDiscoveryResults> GetSubmitDiscoveryDelegateMock() => new Mock<SubmitDiscoveryResults>();

        public static AnyJobConfigInfo GetMockConfig()
        {
            var ajJob = new AnyJobJobInfo { OperationType = Platform.Extensions.Agents.Enums.AnyJobOperationType.Create, Alias = "testJob", JobId = Guid.NewGuid(), JobTypeId = Guid.NewGuid(), };
            var ajServer = new AnyJobServerInfo { Username = "testUsername", Password = "testPassword", UseSSL = true };
            var ajc = new AnyJobConfigInfo()
            {
                Job = ajJob,
                Server = ajServer
            };

            return ajc;
        }
    }
}
