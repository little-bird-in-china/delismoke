using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Models;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Service;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    [Auth(FunctionKeys.PM_Customer_All, FunctionKeys.Cus_MyInfo_All)]
    public class ClientController : BaseController
    {
        public ActionResult ClientList()
        {
            return View();
        }

        public ActionResult QueryAllClientList()
        {
            ClientFilter filter = BuildQueryFilterEntity<ClientFilter>();
            var data = ClientService.QueryAllClientList(filter);
            return AjaxGridJson(data);
        }
        public ActionResult SmokeList(int sysno)
        {
            var list = SmokeDetectorServices.LoadSmokeDetectorsByClientSysNo(sysno);
            return PartialView("~/views/client/_smokeList.cshtml", list);
        }
    }
}