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
