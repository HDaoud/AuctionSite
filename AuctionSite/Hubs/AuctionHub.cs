using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using AuctionSite.Models;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AuctionSite
{
    [HubName("Auction")]
    public class AuctionHub : Hub
    {
        #region Properties
        //static private Timer timer;
        public static bool initialized = false;
        public static object initLock = new object(); 
        //AuctionViewModel is used to store and keep track of bid information
        static public AuctionViewModel auctionViewModel;
        public static int secs_10 = 10000;
        public static int sec = 1000;
        public static bool isOver = false;
        #endregion

        #region Initialization
        public AuctionHub()
        {           
            if (initialized)
                return;

            lock (initLock)
            {
                if (initialized)
                    return;

                InitializeAuction();
            }
        }

        private void InitializeAuction()
        {            
            // Initialize model
            auctionViewModel = new AuctionViewModel(0, 10, DateTime.Now.AddSeconds(60), 10);

            //timer = new System.Threading.Timer(TimerExpired, null, sec, 0);
            var Generator = Observable.Interval(TimeSpan.FromSeconds(1)).
                TakeWhile(_ => { return auctionViewModel.TimeRemaining >= 0; }).Publish().RefCount();            

            var obsrvr = Generator.Subscribe(_ =>
            {                
                    Clients.All.updateRemainingTime(string.Format("{0:hh\\:mm\\:ss}", auctionViewModel.GetTimeRemaining()));               
            }        
            , 
            () => 
            {
                isOver = true;
                Clients.All.updateRemainingTime("00:00:00");
                Clients.All.finishBidding();
                Clients.All.addMessage("Time Expired!");
                if (!String.IsNullOrEmpty(auctionViewModel.LastUserBid))
                    Clients.All.addMessage(string.Format("Congratulations {0}! \n {0} has won the auction with {1}$ \n on {2}", auctionViewModel.LastUserBid, auctionViewModel.ValueLastBid, auctionViewModel.LastBid));
                else
                    Clients.All.addMessage(string.Format("Oh, poor product! Somebody will take you home someday!"));

                Clients.All.addMessage("\n-------");
            });

            initialized = true;
        }
        #endregion

        #region ServerToClient
        /// <summary>
        /// Thread method to show the remaining time till the end of the bid
        /// It send bid results when the bid is over and update clients to prevent new bids
        /// </summary>
        /// <param name="state"></param>
        //public void TimerExpired(object state)
        //{
        //    if (auctionViewModel.TimeRemaining > 0)
        //    {
        //        Clients.All.updateRemainingTime(string.Format("{0:hh\\:mm\\:ss}", auctionViewModel.GetTimeRemaining()));
        //        timer.Change(sec, 0);
        //    }
        //    else
        //    {
        //        timer.Dispose();
        //        isOver = true;
        //        Clients.All.updateRemainingTime("00:00:00");
        //        Clients.All.finishBidding();
        //        AddMessage("Time Expired");
        //        if (!String.IsNullOrEmpty(auctionViewModel.LastUserBid))
        //            AddMessage(string.Format("Congratulations {0}! \n {0} has won the auction with {1}$ \n on {2}", auctionViewModel.LastUserBid, auctionViewModel.ValueLastBid, auctionViewModel.LastBid));
        //        else
        //            AddMessage(string.Format("Oh, poor product! Somebody will take you home someday!"));
        //    }
        //}

        /// <summary>
        /// Server messages to clients. Now it only report bid results
        /// </summary>
        /// <param name="msg">The message</param>
        public void AddMessage(string msg)
        {
            Clients.All.addMessage(msg);
        }

        /// <summary>
        /// Called after each client's bid
        /// </summary>
        /// <param name="user">user name of last bid</param>
        /// <param name="value">the value of last bid</param>
        public void NotifyNewBid(string user, decimal value)
        {
            Clients.All.notifyNewBid(string.Format("{0} did a new bid of {1:c} at {2:T}", user, value, DateTime.Now));
        }
        #endregion

        #region ClientToServer
        /// <summary>
        /// Called by client when pressing bid button
        /// </summary>
        /// <param name="valueBid">The value of the bid (last bid + 1 cent)</param>
        /// <param name="user">The user name</param>
        public void PlaceBid(string valueBid, string user)
        {
            auctionViewModel.PlaceBid(decimal.Parse(valueBid), user);

            NotifyNewBid(user, decimal.Parse(valueBid));

            CallRefresh();
            
        }

        /// <summary>
        /// This is the method called by clients to retrieve information about bid current status
        /// </summary>
        public void CallRefresh()
        {
            Clients.All.auctionRefresh(auctionViewModel);
            if(isOver)
            {
                Clients.All.updateRemainingTime("00:00:00");
                Clients.All.finishBidding();
                AddMessage("Time Expired");
            }
        }
        #endregion

    }
}