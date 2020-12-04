using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Moq;

[assembly: InternalsVisibleTo("Keyfactor.AnyAgent.WindowsUser")]
namespace Keyfactor.AnyAgent.WindowsUser.Tests
{
    public static class Mocks
    {
        public static Mock<SubmitInventoryUpdate> GetSubmitInventoryDelegateMock() => new Mock<SubmitInventoryUpdate>();

        public static Mock<SubmitEnrollmentRequest> GetSubmitEnrollmentDelegateMock() => new Mock<SubmitEnrollmentRequest>();

        public static Mock<SubmitDiscoveryResults> GetSubmitDiscoveryDelegateMock() => new Mock<SubmitDiscoveryResults>();

        public static AnyJobConfigInfo GetMockConfig()
        {
            var ajStore = new AnyJobStoreInfo()
            {
                Properties = new
                {
                    VaultUrl = "https://test.vault",
                    TenantId = "8b74a908-b153-41dc-bfe5-3ea7b22b9678",
                    ClientSecret = "testClientSecret",
                    ApplicationId = Guid.NewGuid().ToString(),
                    SubscriptionId = Guid.NewGuid().ToString(),
                    VaultName = "testVaultName",
                    ResourceGroupName = "testResourceGroupName",
                    APIObjectId = Guid.NewGuid().ToString(),
                },
                Inventory = new List<AnyJobInventoryItem>(),
                StorePath = "http://test.vault",
                Storetype = 1,
            };
            var ajJob = new AnyJobJobInfo { OperationType = Platform.Extensions.Agents.Enums.AnyJobOperationType.Create, Alias = "testJob", JobId = Guid.NewGuid(), JobTypeId = Guid.NewGuid(), };
            var ajServer = new AnyJobServerInfo { Username = "testUsername", Password = "testPassword", UseSSL = true };
            var ajc = new AnyJobConfigInfo()
            {
                Store = ajStore,
                Job = ajJob,
                Server = ajServer
            };

            return ajc;
        }
    }
}
