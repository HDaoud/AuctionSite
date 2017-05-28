using System;
using Moq;
using System.Dynamic;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuctionSite.Models;
//using Xunit;

namespace AuctionSite.Tests.Hubs
{
    [TestClass]
    public class AuctionHubTest
    {
        /// <summary>
        /// Check if the AuctionHub works correctly and updates all clients with the new bid info
        /// </summary>
        [TestMethod]
        public void AuctionTest()
        {
            AuctionViewModel auctionViewModel = new AuctionViewModel();
            
            bool updateCalled = false;
            var hub = new AuctionHub();            
            var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            hub.Clients = mockClients.Object;
            dynamic all = new ExpandoObject();
            //all.placebid = new action<string, string>((price, name) => {
            //    updatecalled = true;                
            //});
            all.addMessage = new Action<string>((message) =>
            {
                updateCalled = true;
            });
            mockClients.Setup(m => m.All).Returns((ExpandoObject)all);
            //hub.PlaceBid("20", "user");
            hub.AddMessage("Test Message");
            Assert.IsTrue(updateCalled);            
        }
    }
}
