using BBlueStone.Smoke.Service;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Models;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Service;
using BlueStone.Smoke.Service.ONENET;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    [Auth(FunctionKeys.PM_Smoke_All, FunctionKeys.Cus_Smoke_All)]
    public class DayReportController : BaseController
    {
        public ActionResult DayReport()
        {
            if (CurrUser.IsPMAdmin)
            {
              var complist=  CompanyService.LoadAllCompany();
               ViewBag.complist = complist;
            }
            return View();
        }

        public JsonResult QueryReport(QF_SmokeDayReport qF_SmokeDay)
        {
            if (!CurrUser.IsPMAdmin)
            {
                qF_SmokeDay.CompanySysNo = CurrUser.MasterSysNo.Value;
            };
            if ((!qF_SmokeDay.EndDayDate.HasValue) || !qF_SmokeDay.StartDayDate.HasValue)
            {
                throw new BusinessException("请输入查询日期");
            }
            var result = SmokeDetectorDayReportServices.QueryDayReport(qF_SmokeDay);
            ReportModel reportModel = new ReportModel
            {
                XAxisData=new List<string>(),
                SeriesTotal=new List<List<object>>(),
                SeriesOnline=new List<List<object>>(),
                SeriesLowPower=new  List<List<object>>(),
                SeriesFire=new List<List<object>>(),
                SeriesOffline=new List<List<object>>()
            };
            reportModel.XAxisData = result.TotalCount.Select(e => e.DayDate).ToList();

            result.TotalCount.ForEach(e =>
            {
                var ADJ = new List<object>
                {
                    e.DayDate,
                    e.Count,
                    e.Percent
                };
                reportModel.SeriesTotal.Add(ADJ);
            });
            result.OnlineCount.ForEach(e =>
            {
                var ADJ = new List<object>
                {
                    e.DayDate,
                    e.Count,
                    e.Percent
                };
                reportModel.SeriesOnline.Add(ADJ);
            });
            result.LowPowerCount.ForEach(e =>
            {
                var ADJ = new List<object>
                {
                    e.DayDate,
                    e.Count,
                    e.Percent
                };
                reportModel.SeriesLowPower.Add(ADJ);
            });
            result.FireCount.ForEach(e =>
            {
                var ADJ = new List<object>
                {
                    e.DayDate,
                    e.Count,
                    e.Percent
                };
                reportModel.SeriesFire.Add(ADJ);
            });
            result.OffLineCount.ForEach(e =>
            {
                var ADJ = new List<object>
                {
                    e.DayDate,
                    e.Count,
                    e.Percent
                };
                reportModel.SeriesOffline.Add(ADJ);
            });
            return Json(new AjaxResult { Success = true, Data = reportModel });
        }
    }
    public class ReportModel
    {
        public List<string> XAxisData { get; set; }

        public  List<List<object>> SeriesTotal { get; set; }
        public List<List<object>> SeriesOnline { get; set; }
        public List<List<object>> SeriesLowPower { get; set; }
        public List<List<object>> SeriesFire { get; set; }
        public List<List<object>> SeriesOffline { get; set; }
    }
}