using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuctionSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }        

        public ActionResult Auction()
        {
            ViewBag.Message = "Welcome to AuctionSite!";
            return View();
        }
        
    }
}