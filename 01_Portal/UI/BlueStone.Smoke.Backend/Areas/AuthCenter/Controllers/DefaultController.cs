﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class DefaultController : Controller
    {
        // GET: AuthCenter/Default
        public ActionResult Index()
        {
            return View();
        }
    }
}