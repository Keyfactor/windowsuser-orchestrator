using System.Collections.Generic;
using FluentAssertions;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace Keyfactor.AnyAgent.WindowsUser.Tests
{
    [TestClass]
    public class ManagementTests
    {
        [TestMethod]
        public void ReturnsTheCorrectJobClassAndStoreType()
        {
            var management = new Management();
            management.GetJobClass().Should().Be("Management");
            management.GetStoreType().Should().Be("WinU");
        }

        [TestMethod]
        public void JobForAddOnlyCallsPerformAddition()
        {
            var managementMock = new Mock<Management>() { CallBase = true };
            managementMock.Protected().Setup<AnyJobCompleteInfo>("PerformRemoval", ItExpr.IsAny<string>()).Returns(new AnyJobCompleteInfo());
            managementMock.Protected().Setup<AnyJobCompleteInfo>("PerformAddition", ItExpr.IsAny<string>(), ItExpr.IsAny<string>()).Returns(new AnyJobCompleteInfo());

            var config = Mocks.GetMockConfig();
            config.Job.OperationType = AnyJobOperationType.Add;

            var result = managementMock.Object.processJob(config, Mocks.GetSubmitInventoryDelegateMock().Object, Mocks.GetSubmitEnrollmentDelegateMock().Object, Mocks.GetSubmitDiscoveryDelegateMock().Object);
            managementMock.Protected().Verify("PerformRemoval", Times.Never(), ItExpr.IsAny<string>());
            managementMock.Protected().Verify("PerformAddition", Times.Once(), ItExpr.IsAny<string>(), ItExpr.IsAny<string>());
        }


        [TestMethod]
        public void JobForRemoveOnlyCallsPerformRemove()
        {
            var managementMock = new Mock<Management>() { CallBase = true };
            managementMock.Protected().Setup<AnyJobCompleteInfo>("PerformRemoval", ItExpr.IsAny<string>()).Returns(new AnyJobCompleteInfo());
            managementMock.Protected().Setup<AnyJobCompleteInfo>("PerformAddition", ItExpr.IsAny<string>(), ItExpr.IsAny<string>()).Returns(new AnyJobCompleteInfo());

            var config = Mocks.GetMockConfig();
            config.Job.OperationType = AnyJobOperationType.Remove;

            var result = managementMock.Object.processJob(config, Mocks.GetSubmitInventoryDelegateMock().Object, Mocks.GetSubmitEnrollmentDelegateMock().Object, Mocks.GetSubmitDiscoveryDelegateMock().Object);
            managementMock.Protected().Verify("PerformRemoval", Times.Once(), ItExpr.IsAny<string>());
            managementMock.Protected().Verify("PerformAddition", Times.Never(), ItExpr.IsAny<string>(), ItExpr.IsAny<string>());
        }
    }
}
