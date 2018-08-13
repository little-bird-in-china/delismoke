using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Models;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Service;
using BlueStone.Smoke.Service.ONENET;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    [Auth(FunctionKeys.PM_Smoke_All, FunctionKeys.Cus_Smoke_All)]
    public class SmokeController : BaseController
    {
        public ActionResult List(int? companySysNo)
        {
            int no = companySysNo.GetValueOrDefault();
            if (!companySysNo.HasValue)
            {
                if (CurrUser.CompanySysNo > 0)
                {
                    no = CurrUser.CompanySysNo;
                }
            }
            AddressManagerModel model = new AddressManagerModel { CompanySysNo = no };

            var company = CompanyService.LoadCompany(no);
            if (company != null)
            {
                model.AreaSysNo = company.AreaSysNo;
                model.Address = company.Address;
            }
            else
            {
                model.HasError = true;
                model.ErrorMessage = "客户信息不存在可能已经被删除，请刷新页面重试。";
            }
            QF_SystemUser filter = new QF_SystemUser()
            {
                PageSize = 10000,
                PageIndex = 0,
                MasterSysNo = no,
                CommonStatus = CommonStatus.Actived
            };
            AddressFilter addFilter = new AddressFilter { PageSize = 5, AddressGrade = AddressGrade.Building, CompanySysNo = no };
            QueryResult<Address> addResult = AddressService.QueryAddressList(addFilter);
            if (addResult != null && addResult.data != null)
            {
                var root = addResult.data.Find(a => a.ParentSysNo.GetValueOrDefault() == 0);
                if (root != null)
                {
                    model.AreaSysNo = root.AreaSysNo;
                    model.Address = root.PathName;
                }
            }
            SystemUserService user_service = new SystemUserService();
            var result = user_service.QuerySystemUserList(filter);
            model.ManagerList = result.data;
            return View(model);
        }
        [ValidateInput(false)]
        public ActionResult QuerySmoke()
        {
            //CreateDeviceRequest request = new CreateDeviceRequest()
            //{
            //    Desc = "创建测试设备描述",
            //    IMei = "1354789845",
            //    IMsi = "1354789845",
            //    IsOnLine = 1,
            //    LocationEle = 0,
            //    LocationLat = 0,
            //    LocationLon = 0,
            //    Observe = 1,
            //    Protocol = "LWM2M",
            //    Tags = "1354789845,147890123331",
            //    Title = "创建测试设备名称"
            //};

            //CreateDeviceResponse create = ONENETService.CreateDevice(request);
            //if (create != null && !string.IsNullOrEmpty(create.DeviceId) && create.IsSuccess)
            //{
            //    UpdateDeviceRequest upRequest = new UpdateDeviceRequest()
            //    {
            //        Desc = "更新测试设备描述",
            //        IMei = "1364789845",
            //        IMsi = "1364789845",
            //        IsOnLine = 1,
            //        LocationEle = 1,
            //        LocationLat = 2,
            //        LocationLon = 3,
            //        Observe = 1,
            //        Protocol = "LWM2M",
            //        Tags = "1364789845,1364789845",
            //        Title = "更新测试设备名称",
            //        DeviceID = create.DeviceId
            //    };
            //    UpdateDeviceResponse upResponse = ONENETService.UpdateDevice(upRequest);
            //    if (upResponse != null && !string.IsNullOrEmpty(upResponse.DeviceId) && upResponse.IsSuccess)
            //    {
            //        SendCmdRequest send = new SendCmdRequest()
            //        {
            //            StrCmd = "008013870005013D58FF",
            //            DeviceID = create.DeviceId
            //        };
            //        SendCmdResponse sendRes = ONENETService.SendCmd(send);
            //        if (sendRes != null && !string.IsNullOrEmpty(sendRes.DeviceId) && sendRes.IsSuccess)
            //        {
            //            DeleteDeviceRequest delete = new DeleteDeviceRequest()
            //            {
            //                DeviceID = sendRes.DeviceId
            //            };
            //            DeleteDviceResponse deleteRes = ONENETService.DeleteDevice(delete);

            //        }
            //    }
            //}

            QF_SmokeDetector filter = BuildQueryFilterEntity<QF_SmokeDetector>();

            if (CurrUser.CompanySysNo > 0)
            {
                filter.CompanySysNo = CurrUser.CompanySysNo;
            }
            var result = SmokeDetectorServices.QuerySmokeDetectorList(filter);
            return AjaxGridJson(result);
        }

        public ActionResult Detail(string code)
        {

            //WebsocketService.SendMessage();
            var model = new SmokeDetailModel();
            model.DetectorInfo = SmokeDetectorServices.LoadSmokeDetail(code);
            if (model.DetectorInfo == null
                || (CurrUser.CompanySysNo > 0 && CurrUser.CompanySysNo != model.DetectorInfo.CompanySysNo))
            {
                model.HasError = true;
                model.ErrorMessage = "设备信息不存在";
            }
            else
            {
                ClientFilter cFilter = new ClientFilter { SmokeDetectorCode = code, PageIndex = 0, PageSize = int.MaxValue };
                if (CurrUser != null && CurrUser.CompanySysNo > 0)
                {
                    cFilter.CompanySysNo = CurrUser.CompanySysNo;

                }
                model.ClientList = ClientService.QueryClientList(cFilter).data;
                //model.LogList = SmokeDetectorServices.QueryDeviceNoticeList(code);
            }
            return View(model);
        }

        public ActionResult AjaxQueryDeviceNoticeInfo()
        {
            QF_SmokeDetectorStatusLog filter = BuildQueryFilterEntity<QF_SmokeDetectorStatusLog>();
            var resul = SmokeDetectorServices.QueryDeviceNoticeList(filter);
            return AjaxGridJson(resul);
        }

        public ActionResult AjaxQueryDeviceUserNoticeInfo()
        {
            QF_SmokeDetectorMessage filter = BuildQueryFilterEntity<QF_SmokeDetectorMessage>();
            filter.MasterName = "SmokeDetector";
            var resul = SmokeDetectorServices.QuerySmokeDetectorMessage(filter);
            return AjaxGridJson(resul);
        }

        public ActionResult CancelWarning(string code)
        {
            SmokeDetector detector = SmokeDetectorServices.LoadSmokeDetail(code);
            if (detector == null)
            {
                throw new BusinessException(string.Format("编号为【{0}】的设备不存在", code));
            }
            if (detector.Status != SmokeDetectorStatus.Warning && detector.Status != SmokeDetectorStatus.TestWarning)
            {
                throw new BusinessException(string.Format("编号为【{0}】的设备不是报警状态", code));
            }

            if (detector.Status == SmokeDetectorStatus.TestWarning)
            {
                //更新本地设备状态
                SmokeDetectorStatusLog lastLog = SmokeDetectorServices.LoadSmokeDetectorStatusLogByDeviceCode(detector.Code);
                SmokeDetectorStatusLog statusLog = new SmokeDetectorStatusLog()
                {
                    PreStatus = detector.Status.Value,
                    Status = SmokeDetectorStatus.CancelWarning,
                    BeginTime = DateTimeHelper.GetTimeZoneNow(),
                    ReceivedJsonData = "",
                    SmokeDetectorCode = detector.Code
                };
                detector.Status = SmokeDetectorStatus.CancelWarning;
                if (lastLog != null && statusLog.BeginTime.HasValue && lastLog.BeginTime.HasValue)
                {

                    statusLog.DurationSeconds = (int)(statusLog.BeginTime.Value - lastLog.BeginTime.Value).TotalSeconds;
                }
                //更新本地设备状态
                SmokeDetectorServices.UpdateSmokeDetector(detector);

                //更新首页数据
                (new MapDataService(detector.CompanySysNo)).DataChangeAsync();

                //写设备状态更改日志
                SmokeDetectorServices.InsertSmokeDetectorStatusLog(statusLog);
                return Json(new AjaxResult() { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SendCmdRequest send = new SendCmdRequest()
                {
                    Imei = detector.Code
                };
                SendCmdResponse sendRes = ONENETService.SendCmd(send);
                if (sendRes.IsSuccess)
                {
                    //更新本地设备状态
                    SmokeDetectorStatusLog lastLog = SmokeDetectorServices.LoadSmokeDetectorStatusLogByDeviceCode(detector.Code);
                    SmokeDetectorStatusLog statusLog = new SmokeDetectorStatusLog()
                    {
                        PreStatus = detector.Status.Value,
                        Status = SmokeDetectorStatus.CancelWarning,
                        BeginTime = DateTimeHelper.GetTimeZoneNow(),
                        ReceivedJsonData = "",
                        SmokeDetectorCode = detector.Code
                    };
                    detector.Status = SmokeDetectorStatus.CancelWarning;
                    if (lastLog != null && statusLog.BeginTime.HasValue && lastLog.BeginTime.HasValue)
                    {

                        statusLog.DurationSeconds = (int)(statusLog.BeginTime.Value - lastLog.BeginTime.Value).TotalSeconds;
                    }
                    //更新本地设备状态
                    SmokeDetectorServices.UpdateSmokeDetector(detector);

                    //更新首页数据
                    (new MapDataService(detector.CompanySysNo)).DataChangeAsync();

                    //写设备状态更改日志
                    SmokeDetectorServices.InsertSmokeDetectorStatusLog(statusLog);
                    return Json(new AjaxResult() { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new AjaxResult() { Success = false, Message = "取消失败！" }, JsonRequestBehavior.AllowGet);
                }

            }

        }

        public ActionResult UpdateSmokeInfo(string code, string newDeviceID, string addressSysNo,string position)
        {
            if (addressSysNo == null)
            {
                throw new BusinessException("位置信息错误，请重新选择后再试。");
            }
            int sysNo = int.Parse(addressSysNo);
            if (sysNo == 0)
            {
                throw new BusinessException("位置信息错误，请重新选择后再试。");
            }
            Address address = AddressService.LoadAddress(sysNo);
            if (address == null)
            {
                throw new BusinessException("位置信息错误，请重新选择后再试。");
            }
            if (string.IsNullOrWhiteSpace(newDeviceID))
            {
                throw new BusinessException("请填写ONENET编号！。");
            }
            SmokeDetector result = SmokeDetectorServices.LoadSmokeDetectorByDeviceID(newDeviceID);
            if (result != null && result.Code != code)
            {
                throw new BusinessException("已存在该ONENET编号！。");
            }
            SmokeDetector smoke = SmokeDetectorServices.IsUniquenessCode(code);
            if (smoke != null)
            {
                //List<Address> addressList = AddressService.LoadSubsetAddressByAddressSysNo(smoke.CompanySysNo, sysNo);
                //if (addressList != null && addressList.Count > 0)
                //{
                //    throw new BusinessException("请选择详细地址！。");
                //}
                smoke.DeviceId = newDeviceID;
                smoke.AddressCode = address.Code;
                smoke.AddressName = address.PathName;
                smoke.Position = position;
                SmokeDetectorServices.UpdateSmokeDetector(smoke);
                return Json(new AjaxResult() { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new AjaxResult() { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult DeleteDetector(string code, bool isdelete)
        {
            SmokeDetector detector = SmokeDetectorServices.LoadSmokeDetail(code);
            if (detector == null)
            {
                throw new BusinessException(string.Format("编号为【{0}】的设备不存在", code));
            }

            if (isdelete)
            {
                DeleteDeviceRequest send = new DeleteDeviceRequest()
                {
                    DeviceID = detector.DeviceId
                };
                DeleteDviceResponse sendRes = ONENETService.DeleteDevice(send);
                SmokeDetectorServices.DeleteSmokeDetector(detector, CurrUser);
                SmokeDetectorServices.DeleteClientSmokeDetectorBycode(code);
                return Json(new AjaxResult() { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DeleteDeviceRequest send = new DeleteDeviceRequest()
                {
                    DeviceID = detector.DeviceId
                };
                DeleteDviceResponse sendRes = ONENETService.DeleteDevice(send);
                if (sendRes.IsSuccess)
                {
                    //更新本地设备状态
                    SmokeDetectorServices.DeleteSmokeDetector(detector, CurrUser);
                    SmokeDetectorServices.DeleteClientSmokeDetectorBycode(code);
                    return Json(new AjaxResult() { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new AjaxResult() { Success = false, Message = "删除失败，ONENET平台返回消息：" + sendRes.msg }, JsonRequestBehavior.AllowGet);

                }
            }



        }

        public ActionResult _Modify(string code)
        {
            SmokeDetector detector = SmokeDetectorServices.LoadSmokeDetail(code) ?? new SmokeDetector();
            return PartialView("_Modify", detector);
        }

        public ActionResult ExportSmokeDeviceList(string exportType)
        {
            QF_SmokeDetector filter = JsonConvert.DeserializeObject<QF_SmokeDetector>(this.Request["data"]);
            if (filter.CompanySysNo <= 0)
            {
                if (CurrUser.CompanySysNo > 0)
                {
                    filter.CompanySysNo = CurrUser.CompanySysNo;
                }
            }
            // 1=导出当页，2=导出全部,默认导出当前页
            if (exportType == "2")
            {
                filter.PageIndex = 0;
                filter.PageSize = 10000;
            }
            QueryResult<QR_SmokeDetector> result = SmokeDetectorServices.QuerySmokeDetectorList(filter);
            ExcelFileExporter<QR_SmokeDetector> exporter = new ExcelFileExporter<QR_SmokeDetector>();
            List<ColumnSetting<QR_SmokeDetector>> columnSettings = new List<ColumnSetting<QR_SmokeDetector>>();
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "设备编号", PropertyName = "Code" });
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "设备状态", PropertyName = "UIStatusStr" });
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "位置", PropertyName = "PositionAddress" });
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "ONENET平台编号", PropertyName = "DeviceID" });
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "安装人员", PropertyName = "InstallerName" });
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "绑定客户", PropertyName = "ClientName" });
            columnSettings.Add(new ColumnSetting<QR_SmokeDetector>() { ColumnName = "安装时间", PropertyName = "InDateStr" });
            string fileName = exporter.Export(result.data, columnSettings, "烟感设备列表");
            fileName = Url.Content(ExcelFileExporterHelper.GetExportFileUrl(fileName));
            return Content(fileName);
        }
    }
}