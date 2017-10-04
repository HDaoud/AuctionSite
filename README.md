### On the Server side:
Use Observable.Interval instead of Timer:

```csharp
            var Generator = Observable.Interval(TimeSpan.FromSeconds(1)).
                TakeWhile(_ => { return auctionViewModel.TimeRemaining >= 0; }).Publish().RefCount();
```            


Subscribe to this observable to update users with the remaining time, and also with the result when the bidding is over:


```csharp
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
            });
```

### On client side:
User Observable to capture user's clicks of bid button

```javascript
            //The interaction of client with hub server
            var bidClickStream = Rx.Observable.fromEvent(bidButton, 'click');
            var publishBid = bidClickStream.subscribe(function () {
                if (!biddingActive) {
                    alert("Auction has ended!");
                    return;
                }

                if (!validBid()) {
                    alert("Please Insert your name!");
                    $('#user').focus();
                    return;
                }

                auction.server.placeBid($('#price').val(), $('#user').val());                        
            });
```            
[![Signalr_Rx_Auction_Site.png](https://s1.postimg.org/40fuoyun4v/Signalr_Rx_Auction_Site.png)](https://postimg.org/image/123klgmdnf/)
