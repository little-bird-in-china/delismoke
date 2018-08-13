using BlueStone.Smoke.Service;
using BlueStone.Smoke.Entity.AuthCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}