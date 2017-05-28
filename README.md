# AuctionSite
A simple website in ASP.NET MVC that uses SignalR 2.2
- each new bid adds 1 cent to the price and 10 seconds to the count-down time.
- The last user to bid before timer reaches 0, is the winner.

The repository contains also an example for unit tests on controllers and SignalR hub.

The project basically consists of ViewModel to store bid information, SignalR hub to perform all the communication between clients and server in real-time, View which initiates the communication with the hub and handles clients' input and displays server responses.

![alt text](http://i.imgur.com/FhLsOOM.png)
