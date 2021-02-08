using System.Collections.Generic;
using FluentAssertions;
using Keyfactor.Platform.Extensions.Agents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Keyfactor.Extensions.Orchestrator.WindowsUser.Tests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void ReturnsTheCorrectJobClassAndStoreType()
        {
            var inventory = new Inventory();
            inventory.GetJobClass().Should().Be("Inventory");
            inventory.GetStoreType().Should().Be("WinU");
        }

        [TestMethod]
        public void JobInvokesCorrectDelegate()
        {
            var inventory = new Mock<Inventory>() { CallBase = true };
            var mockInventoryDelegate = Mocks.GetSubmitInventoryDelegateMock();
            mockInventoryDelegate.Setup(m => m.Invoke(It.IsAny<List<AgentCertStoreInventoryItem>>())).Returns(true);
            var result = inventory.Object.processJob(Mocks.GetMockConfig(), mockInventoryDelegate.Object, Mocks.GetSubmitEnrollmentDelegateMock().Object, Mocks.GetSubmitDiscoveryDelegateMock().Object);

            mockInventoryDelegate.Verify(m => m(It.IsAny<List<AgentCertStoreInventoryItem>>()));
        }
    }
}
