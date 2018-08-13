using BlueStone.BizProcessor;
using BlueStone.DataAdapter;
using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Msite.Models;
using BlueStone.Smoke.Service;
using BlueStone.Smoke.Service.ONENET;
using BlueStone.Utility;
using MessageCenter.Processor;
using MessageCenter.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Msite.Controllers
{
    /// <summary>
    /// 微信前台
    /// </summary>
    public class SmokeController : SSLControllerBase
    {

        public ActionResult DeviceList(UISmokeDetectorStatus? uiStatus)
        {
            ViewBag.status = uiStatus;
            DeviceListModel model = new DeviceListModel();
            if (curentUser.UserType == UserType.Installer)
            {
                QF_SmokeDetector filter = new QF_SmokeDetector();
                filter.InstallerSysNo = curentUser.ManagerSysNo.Value;
                filter.Status = uiStatus;
                filter.PageIndex = 0;
                filter.PageSize = 10;
                filter.SortFields = "InstalledTime desc";
                var allsmks = SmokeDetectorServices.LoadSmokeDetectorsByInstaller(curentUser.ManagerSysNo.Value).Where(e => e.Status != SmokeDetectorStatus.Delete).ToList();
                model.CountInfo = new SmokeDetectorCount
                {
                    ALLSmokeCount = allsmks.Count,
                    OfflineCount = (allsmks.Where(e => e.UIStatus == UISmokeDetectorStatus.OffLine) ?? new List<SmokeDetector>()).Count(),
                    OnlineCount = (allsmks.Where(e => e.UIStatus == UISmokeDetectorStatus.Online) ?? new List<SmokeDetector>()).Count(),
                    LowPowerCount = (allsmks.Where(e => e.UIStatus == UISmokeDetectorStatus.LowPowerWarning) ?? new List<SmokeDetector>()).Count(),
                    WarningCount = (allsmks.Where(e => e.UIStatus == UISmokeDetectorStatus.FireWarning) ?? new List<SmokeDetector>()).Count(),

                };
                //model.CountInfo.OnlineCount = model.CountInfo.ALLSmokeCount - model.CountInfo.OfflineCount;
                var result = SmokeDetectorServices.QuerySmokeDetectorList(filter);
                model.DeviceList = new QueryResult<QR_SmokeDetector>();
                model.DeviceList = result;
            }
            else
            {

                QF_UserDetector filter = new QF_UserDetector();
                filter.PageIndex = 0;
                filter.PageSize = 10;
                filter.Status = uiStatus;
                filter.ClientSysNo = curentUser.UserSysNo;
                model.CountInfo = SmokeDetectorServices.LoadUserSmokeDetectorCount(curentUser.UserSysNo);
                QueryResult<QR_SmokeDetector> list = SmokeDetectorServices.LoadUserSmokeDeletetorList(filter);
                model.DeviceList = new QueryResult<QR_SmokeDetector>();
                model.DeviceList = list;

            }
            ViewBag.UserType = curentUser.UserType;
            return View(model);
        }

        public ActionResult QueryDeviceList(int pageIndex, UISmokeDetectorStatus? uiStatus)
        {
            QueryResult<QR_SmokeDetector> list = new QueryResult<QR_SmokeDetector>();
            if (curentUser.UserType == UserType.Installer)
            {
                QF_SmokeDetector filter = new QF_SmokeDetector();
                filter.InstallerSysNo = curentUser.ManagerSysNo.Value;
                filter.Status = uiStatus;
                filter.PageIndex = pageIndex;
                filter.PageSize = 10;
                filter.SortFields = "status desc,InstalledTime desc";
                list = SmokeDetectorServices.QuerySmokeDetectorList(filter);
            }
            else
            {
                QF_UserDetector filter = new QF_UserDetector();
                filter.PageIndex = pageIndex;
                filter.PageSize = 10;
                filter.ClientSysNo = curentUser.UserSysNo;
                filter.Status = uiStatus;
                list = SmokeDetectorServices.LoadUserSmokeDeletetorList(filter);
            }
            return PartialView("~/Views/Smoke/_DeviceItem.cshtml", list.data);
        }
        public ActionResult Notice(DateTime? BeginInDate, DateTime? EndInDate)
        {
            MessageCenter.Entity.QF_Message filter = new MessageCenter.Entity.QF_Message()
            {
                MasterName = "SmokeDetector",
                EndInDate = EndInDate,
                BeginInDate = BeginInDate,
                MsgReceiver = curentUser.AppCustomerID,
                MsgType = MessageCenter.Entity.MsgType.WeiXin
            };
            if (BeginInDate == null && EndInDate == null)
            {
                filter.BeginInDate = DateTime.Now.AddDays(-30);
                filter.EndInDate = DateTime.Now;
            }
            if (EndInDate != null)
            {
                filter.EndInDate = EndInDate.GetValueOrDefault().AddDays(1);
            }
            List<MessageCenter.Entity.QR_Message> list = SMSProcessor.LoadMessageByMasterIDAndMasterName(filter);
            NoticeModel model = new NoticeModel();
            model.MassageList = list;
            model.BeginInDate = BeginInDate;
            model.EndInDate = EndInDate;
            return View(model);
        }
        public ActionResult DeviceDetails(string code)
        {

            DateTime now = DateTime.Now;
            QF_SmokeDetectorStatusLog filter = new QF_SmokeDetectorStatusLog()
            {
                DeviceCode = code,
                PageIndex = 0,
                PageSize = 9999
            };

            QueryResult<SmokeDetectorStatusLog> list = SmokeDetectorServices.QueryDeviceNoticeList(filter);
            SmokeDetector info = SmokeDetectorServices.LoadSmokeDetail(code);
            info.MessageList = list.data;
            ViewBag.UserType = curentUser.UserType;
            return View(info);
        }
        public ActionResult CancelWarning(string code)
        {
            SmokeDetector detector = SmokeDetectorServices.LoadSmokeDetail(code);
            if (detector == null)
            {
                return Json(new AjaxResult { Success = false, Message = string.Format("编号为【{0}】的设备不存在", code) }, JsonRequestBehavior.AllowGet);
            }
            if (detector.Status != SmokeDetectorStatus.Warning && detector.Status != SmokeDetectorStatus.TestWarning)
            {
                return Json(new AjaxResult { Success = false, Message = string.Format("编号为【{0}】的设备不是报警状态", code) }, JsonRequestBehavior.AllowGet);
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

        public ActionResult RelieveDetector(List<string> codes)
        {
            if (codes == null || codes.Count == 0)
            {
                return Json(new AjaxResult() { Success = false, Message = "请选择设备!" });
            };
            SmokeDetectorServices.DeleteClientSmokeDetector(codes, curentUser.UserSysNo);
            return Json(new AjaxResult() { Success = true });
        }
        public ActionResult UserInfo()
        {
            return View();
        }
        public JsonResult DeleteDetector(string code)
        {
            //if (curentUser.UserType != UserType.Installer)
            //{
            //    throw new BusinessException("您不是安装人员，不能删除当前设备");
            //}
            SmokeDetector detector = SmokeDetectorServices.LoadSmokeDetail(code);
            if (detector == null)
            {
                throw new BusinessException(string.Format("编号为【{0}】的设备不存在", code));
            }
            if (curentUser.UserType == UserType.Installer)
            {
                if (detector.InstallerSysNo != curentUser.ManagerSysNo)
                {
                    throw new BusinessException("您没有安装过当前设备,无权删除");
                }
            }
            else
            {
                if (detector.InstallerSysNo != -curentUser.UserSysNo)
                {
                    throw new BusinessException("您没有安装过当前设备,无权删除");
                }
            }

            DeleteDeviceRequest send = new DeleteDeviceRequest()
            {
                DeviceID = detector.DeviceId
            };
            DeleteDviceResponse sendRes = ONENETService.DeleteDevice(send);
            CurrentUser current = new CurrentUser
            {
                UserSysNo = curentUser.UserSysNo,
                UserDisplayName = curentUser.UserDisplayName
            };
            SmokeDetectorServices.DeleteSmokeDetector(detector, current);
            SmokeDetectorServices.DeleteClientSmokeDetectorBycode(code);
            return Json(new AjaxResult() { Success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddDevices()
        {
            Client user = ClientService.LoadClient(curentUser.UserSysNo);
            UIAddDetectorModel model = new UIAddDetectorModel();
            model.CompanyList = new List<Company>();
            model.List = new List<AddDetectorModel>();
            if (user == null)
            {
                UserMgr.Logout();
                throw new BusinessException("登录超时，请退出公众号重新进入。");
            }
            if (user != null && user.ManagerSysNo > 0 && curentUser.UserType == UserType.Installer)
            {
                //model.CompanyList = AddressService.LoadInstallerCompany(user.ManagerSysNo.GetValueOrDefault());
                SystemUserService service = new SystemUserService();
                SystemUser systemuser = service.LoadSystemUser(curentUser.ManagerSysNo.GetValueOrDefault(), ConstValue.ApplicationID);
                if (systemuser != null && systemuser.MasterSysNo > 0)
                {
                    model.CompanyList.Add(new Company() { SysNo = systemuser.MasterSysNo.GetValueOrDefault() });
                }
                SmokeDetector info = SmokeDetectorServices.LoadSmokeDetectorByInstaller(curentUser.ManagerSysNo.GetValueOrDefault());
                if (info != null && !string.IsNullOrWhiteSpace(info.AddressCode))
                {
                    List<Address> list = AddressService.LoadAddressByAddressCode(info.AddressCode, info.CompanySysNo);
                    string code = info.AddressCode;
                    List<AddDetectorModel> modelList = new List<AddDetectorModel>();
                    string scode = code;

                    for (var i = code.Length - 2; i >= 2; i = i - 2)
                    {
                        AddDetectorModel item = new AddDetectorModel();
                        string ncode = code.Substring(0, i);
                        List<Address> list1 = list.Where(x => x.Code.StartsWith(ncode) && x.Code.Length == i + 2).ToList();
                        item.ItemList = list1;
                        item.SelectCode = scode;
                        scode = ncode;
                        modelList.Add(item);
                        if (ncode.Length == 2)
                        {
                            List<Address> list2 = list.Where(x => x.Code == ncode).ToList();
                            model.FirstAddress = list2[0];
                        }
                    }
                    if (code.Length == 2)
                    {
                        model.FirstAddress = list[0];
                    }

                    modelList.Reverse();
                    model.List = modelList;
                    model.SelectCompany = info.CompanySysNo;
                }
            }
            else
            {
                string sysNo = ConfigurationManager.AppSettings["DummyCompanySysNo"];
                int conpanySysNo = 1;
                int.TryParse(sysNo, out conpanySysNo);
                model.CompanyList.Add(new Company() { SysNo = conpanySysNo });
            }
            return View(model);
        }

        public ActionResult AddSmokeDetector(SmokeDetector entity)
        {
            long deviceCode = 0;
            long.TryParse(entity.Code, out deviceCode);
            if (deviceCode <= 0)
            {
                return Json(new AjaxResult() { Success = false, Message = "请扫描正确的设备二维码。" });
            }
            //if (curentUser.UserType != UserType.Installer)
            //{
            //    return Json(new AjaxResult() { Success = false, Message = "您不是管理员，请绑定管理员账号。" });
            //}
            //if ((entity.AddressSysNo == null || entity.AddressSysNo <= 0) && (entity.Memo == "" || entity.Memo == null))
            //{
            //    return Json(new AjaxResult() { Success = false, Message = "请选择详细地址或者填写备注。" });
            //}
            if ((!entity.AddressSysNo.HasValue || entity.AddressSysNo <= 0) && string.IsNullOrWhiteSpace(entity.Position))
            {
                return Json(new AjaxResult() { Success = false, Message = "请选择地址或填写设备具体位置。" });
            }
            Address address = AddressDA.LoadAddress(entity.AddressSysNo.GetValueOrDefault());
            if (address == null)
            {
                return Json(new AjaxResult() { Success = false, Message = "位置信息错误，请重新选择后再试。" });
            }

            entity.AddressCode = address.Code;
            entity.AddressName = address.PathName;
            if ((address.ParentSysNo == 0 || address.ParentSysNo == null) && (address.PathName == "" || address.PathName == null))
            {
                entity.AddressName = address.Name;
            }
            entity.Status = SmokeDetectorStatus.Offline;
            if (curentUser.UserType == UserType.Installer)
            {
                entity.InstallerSysNo = curentUser.ManagerSysNo;
            }
            else
            {
                entity.InstallerSysNo = -curentUser.UserSysNo;
            }

            entity.InstallerName = HttpUtility.UrlDecode(curentUser.UserID);
            entity.InUserSysNo = curentUser.UserSysNo;
            entity.InUserName = HttpUtility.UrlDecode(curentUser.UserID);
            SmokeDetector smoke = SmokeDetectorServices.IsUniquenessCode(entity.Code);
            SystemUserService service = new SystemUserService();
            SystemUser systemuser = service.LoadSystemUser(curentUser.ManagerSysNo.GetValueOrDefault(), ConstValue.ApplicationID);
            if (systemuser == null)
            {
                UserMgr.Logout();
                return Json(new AjaxResult() { Success = false, Message = "登录超时，请退出公众号重新进入。" });
            }
            //if (smoke != null && smoke.CompanySysNo != systemuser.MasterSysNo)
            //{
            //    return Json(new AjaxResult() { Success = false, Message = "设备已安装，不能再安装。" });
            //}
            //else 
            if (smoke != null)
            {
                if (smoke.Status == SmokeDetectorStatus.Delete)
                {
                    entity.Status = SmokeDetectorStatus.Offline;

                }
                else
                {
                    entity.Status = smoke.Status;
                    if (smoke.InstallerSysNo <= 0)
                    {
                        if (smoke.InstallerSysNo != -curentUser.UserSysNo)
                        {
                            return Json(new AjaxResult() { Success = false, Message = "设备已安装，不能再安装。" });
                        }
                    }
                    else
                    {
                        if (!(smoke.CompanySysNo == systemuser.MasterSysNo && curentUser.UserType == UserType.Installer))
                        {
                            return Json(new AjaxResult() { Success = false, Message = "设备已安装，不能再安装。" });
                        }

                    }


                };
                CreateDeviceRequest reques = new CreateDeviceRequest()
                {
                    IMei = entity.Code,
                    IMsi = entity.Code,
                    Desc = "测试设备",
                    IsOnLine = 1,
                    Observe = 1,
                    Protocol = "LWM2M",
                    Title = entity.Code,
                    Tags = entity.Code + "," + entity.Code
                };
                CreateDeviceResponse result = ONENETService.CreateDevice(reques);
                if (result.IsSuccess)
                {
                    entity.DeviceId = result.DeviceId;
                }
                else
                {
                    entity.DeviceId = smoke.DeviceId;
                }

                entity.SysNo = smoke.SysNo;
                SmokeDetectorServices.UpdateSmokeDetector(entity);
                if (curentUser.UserType != UserType.Installer)
                {
                    Logger.WriteLog("Code:" + entity.Code);
                    BindingDevicesApi(entity.Code);
                }
                return Json(new AjaxResult() { Success = true, Message = "修改成功！" });
                //}
                //else
                //{
                //    return Json(new AjaxResult() { Success = false, Message = "系统异常！" });
                //}
            }
            else
            {
                CreateDeviceRequest reques = new CreateDeviceRequest()
                {
                    IMei = entity.Code,
                    IMsi = entity.Code,
                    Desc = "测试设备",
                    IsOnLine = 1,
                    Observe = 1,
                    Protocol = "LWM2M",
                    Title = entity.Code,
                    Tags = entity.Code + "," + entity.Code
                };
                CreateDeviceResponse result = ONENETService.CreateDevice(reques);
                if (result.IsSuccess)
                {
                    entity.DeviceId = result.DeviceId;
                }
                int i = SmokeDetectorServices.InsertSmokeDetector(entity);
                SmokeDetectorStatusLog statusLog = new SmokeDetectorStatusLog()
                {
                    Status = entity.Status,
                    BeginTime = DateTimeHelper.GetTimeZoneNow(),
                    ReceivedJsonData = "",
                    SmokeDetectorCode = entity.Code
                };
                if (curentUser.UserType != UserType.Installer)
                {
                    Logger.WriteLog("Code:" + entity.Code);
                    BindingDevicesApi(entity.Code);
                }

                return Json(new AjaxResult() { Success = true, Message = "添加成功！" });
            }
        }
        public ActionResult TopicDetail(int? sysNo)
        {
            if (!sysNo.HasValue)
            {
                sysNo = 1;
            }
            TopicInfo info = TopicService.LoadTopicInfoBySysNo(sysNo.Value);
            if (info != null)
            {
                info.Content = info.Content.Replace("nowrap", "normal");
            }
            return View(info);
        }

        public ActionResult TopicList()
        {
            QF_Topic filter = new QF_Topic()
            {
                TopicStatus = TopicStatus.Published,
                PageIndex = 0,
                PageSize = 9999
            };
            QueryResult<QR_Topic> result = TopicService.QueryTopicList(filter);

            return View(result.data);
        }

        public ActionResult LoadCompanyAddress(int companySysNo)
        {
            List<Address> list = new List<Address>();
            list = AddressService.LoadCompanyAddress(companySysNo);
            return Json(new AjaxResult() { Success = true, Data = list });
        }

        public ActionResult LoadSubsetAddress(int addressSysNo, int companySysNo)
        {
            List<Address> list = new List<Address>();
            list = AddressService.LoadSubsetAddressByAddressSysNo(companySysNo, addressSysNo);
            return Json(new AjaxResult() { Success = true, Data = list });
        }
        public ActionResult BondeDevices()
        {
            return View();
        }

        public ActionResult AdminBinding()
        {
            return View();
        }

        public ActionResult AdminBinding2()
        {
            return View();
        }
        public ActionResult BindingAdminID(string ID, string password)
        {
            if (string.IsNullOrWhiteSpace(ID) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new AjaxResult() { Success = false, Message = "请输入账号和密码！" });
            }
            string pd = SecurityHelper.GetMD5Value(password);

            #region 【验证账号有效性】
            SystemUser result = AuthDA.LoadSystemUserByIDAndPassword(ID, pd, ConstValue.ApplicationID);
            if (result == null)
            {
                return Json(new AjaxResult() { Success = false, Message = "账号或密码错误！" });
            }
            if (result.CommonStatus != CommonStatus.Actived)
            {
                return Json(new AjaxResult() { Success = false, Message = "账号已被禁用，请联系您的管理员。" });
            }
            if (result.MasterSysNo == null || result.MasterSysNo <= 0)
            {
                return Json(new AjaxResult() { Success = false, Message = "不存在此账号。" });
            }
            #endregion

            Client user = ClientService.LoadClient(curentUser.UserSysNo);
            if (user == null)
            {
                UserMgr.Logout();
                return Json(new AjaxResult() { Success = false, Message = "登录超时，请退出公众号重新进行。" });
            }

            #region 【绑定后台账号到client】
            user.ManagerSysNo = result.SysNo;
            ClientService.UpdateClient(user);
            #endregion

            #region 【更新cookie】

            Company company = CompanyService.GetCompanyUser(result.SysNo);
            if (company != null && company.SysNo > 0)
            {
                curentUser.UserType = UserType.Installer;
            }
            else
            {
                curentUser.UserType = UserType.Manager;
            }
            AppUserInfo loginUser = curentUser;
            loginUser.ManagerSysNo = result.SysNo;
            loginUser.ManagerLoginName = result.LoginName;
            loginUser.ManagerName = HttpUtility.UrlEncode(result.UserFullName);
            UserMgr.Logout();
            UserMgr.WriteUserInfo(loginUser);

            #endregion

            return Json(new AjaxResult() { Success = true, Message = "绑定成功！" });
        }

        public ActionResult RelieveManager()
        {
            Client user = ClientService.LoadClient(curentUser.UserSysNo);
            if (user == null)
            {
                return Json(new AjaxResult() { Success = false, Message = "请退出重新登录。" });
            }
            user.ManagerSysNo = null;
            ClientService.UpdateClient(user);
            AppUserInfo loginUser = curentUser;
            loginUser.ManagerSysNo = null;

            loginUser.UserType = UserType.Common;

            loginUser.ManagerLoginName = null;
            loginUser.ManagerName = null;
            UserMgr.Logout();
            UserMgr.WriteUserInfo(loginUser);
            return Json(new AjaxResult() { Success = true, Message = "解绑成功！" });
        }

        public ActionResult ScanDevicesQR(string serID)
        {
            if (string.IsNullOrEmpty(serID))
            {
                throw new BusinessException("读取设备id失败,请重新扫描");
            }
            var ClientSmokeDetectorList = ClientSmokeDetectorProcessor.LoadAllBindClientUser(serID);
            var temp = ClientSmokeDetectorList.Find(e => string.Equals(e.ClientSysNo, curentUser.UserSysNo));
            if (temp != null)
            {
                throw new BusinessException("你已绑定过当前设备");
            }
            if (ClientSmokeDetectorList.Count > 0)
            {
                throw new BusinessException($"设备已经绑定到“{ClientSmokeDetectorList.FirstOrDefault().Name}”,不能再次绑定");
            }
            var smoke = SmokeDetectorServices.LoadSmokeDetail(serID);
            if (smoke == null || smoke.Status == SmokeDetectorStatus.Delete)
            {
                return Json(new AjaxResult { Success = true, Code = 1, Message = "设备还未安装入网,请添加设备,如果您是公司用户,请绑定管理员!" });
                //throw new BusinessException("设备还未安装入网");
            }

            string addr = smoke.AddressName + smoke.Position;
            if (smoke.InstallerSysNo.HasValue && smoke.InstallerSysNo < 0)
            {
                addr = smoke.Position;
            }
            return Json(new AjaxResult { Success = true, Data = addr });
        }

        #region 设备通知信息

        public ActionResult BindingDevicesApi(string serID)
        {
            if (string.IsNullOrEmpty(serID))
            {
                throw new BusinessException("读取设备id失败,请重新扫描");
            }
            var user = ClientService.LoadClient(curentUser.UserSysNo);
            if (user == null)
            {
                UserMgr.Logout();
                throw new BusinessException("登录信息已过期，请退出重新进入公众号");
            }
            var ClientSmokeDetectorList = ClientSmokeDetectorProcessor.LoadAllBindClientUser(serID);

            var temp = ClientSmokeDetectorList.Find(e => string.Equals(e.ClientSysNo, curentUser.UserSysNo));
            if (temp != null)
            {
                throw new BusinessException("你已绑定过当前设备");
            }
            if (ClientSmokeDetectorList.Count > 0)
            {
                throw new BusinessException($"设备已经绑定到“{ClientSmokeDetectorList.FirstOrDefault().Name}”,不能再次绑定");
            }
            var smoke = SmokeDetectorServices.LoadSmokeDetail(serID);
            if (smoke == null || string.IsNullOrEmpty(smoke.AddressCode) || smoke.Status == SmokeDetectorStatus.Delete)
            {
                return Json(new AjaxResult { Success = true, Code = 1, Message = "设备还未安装入网,请添加设备,如果您是公司用户,请绑定管理员!" });
                // throw new BusinessException("设备还未安装入网");
            }
            if (user != null)
            {
                ClientSmokeDetector clientSmokeDetector = new ClientSmokeDetector
                {
                    ClientSysNo = user.SysNo,
                    SmokeDetectorCode = serID,
                    IsDefaultCellPhone = true,
                    CellPhone = user.CellPhone,
                    CellPhone2 = user.CellPhone2,
                    CellPhone3 = user.CellPhone3
                };
                ClientSmokeDetectorProcessor.InsertClientSmokeDetector(clientSmokeDetector);
                List<MessageCenter.Entity.ReceiverInfo> receivers = new List<MessageCenter.Entity.ReceiverInfo>
                {
                    new MessageCenter.Entity.ReceiverInfo
                    {
                         ReceiverNo = user.AppCustomerID,
                    MsgType = MessageCenter.Entity.MsgType.WeiXin
                    }
                };
                var cellphone = clientSmokeDetector.CellPhone ?? clientSmokeDetector.CellPhone2 ?? clientSmokeDetector.CellPhone3 ?? null;
                if (!string.IsNullOrEmpty(cellphone))
                {
                    receivers.Add(new MessageCenter.Entity.ReceiverInfo
                    {
                        ReceiverNo = cellphone,
                        MsgType = MessageCenter.Entity.MsgType.SMS
                    });
                }
                Task.Factory.StartNew(() =>
                {
                    WechatUserBindDevicesTemplate wechatUserBind = new WechatUserBindDevicesTemplate
                    {
                        Title = $"尊敬的{HttpUtility.UrlDecode(curentUser.UserID)}用户",
                        SerID = serID,
                        BindResult = "绑定成功",
                        BindTimeStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                        Memo = $"此设备地址为{smoke.AddressName}",
                        //  Url = $"http://tfs-code2.chinacloudapp.cn//Smoke/Notice?code= {serID}",
                        MasterID = serID,
                        DateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//短信通知时间
                        Address = smoke.AddressName,
                    };

                    if (smoke.InstallerSysNo.HasValue && smoke.InstallerSysNo < 0)
                    {
                        wechatUserBind.Address = smoke.Position;
                        wechatUserBind.Memo = smoke.Position;
                    }

                    SendMessageService.SendMessageOnce(wechatUserBind, receivers);
                });

            }
            return Json(new AjaxResult { Success = true });
        }

        public ActionResult DevicesOffLine(string SerID)
        {
            if (string.IsNullOrEmpty(SerID))
            {
                throw new BusinessException("请传入设备Code");
            }
            var smoke = SmokeDetectorServices.LoadSmokeDetail(SerID);
            if (smoke == null || string.IsNullOrEmpty(smoke.AddressCode))
            {
                throw new BusinessException("当前设备还未安装入网");
            }
            DevicesOfflineTemplateTemplate OffLineTemplate = new DevicesOfflineTemplateTemplate
            {
                SerID = SerID,
                DeviceName = "烟感器离线",
                DeviceAddress = $"此设备地址为{smoke.AddressName}",
                OffLineTimeStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                Memo = "请尽快处理!",
                // Url = "http://tfs-code2.chinacloudapp.cn/Smoke/Notice",
                MasterID = SerID,
                DateStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),//短信通知时间
                Address = smoke.AddressName,
            };
            if (smoke.InstallerSysNo.HasValue && smoke.InstallerSysNo < 0)
            {
                OffLineTemplate.Address = smoke.Position;
                OffLineTemplate.Memo = smoke.Position;
            }
            SendMessageService.SendMessage(OffLineTemplate, SerID);
            return Json(new AjaxResult { Success = true });
        }


        public ActionResult SendEmergencyMessage(string SerID)
        {
            if (string.IsNullOrEmpty(SerID))
            {
                throw new BusinessException("请传入设备Code");
            }
            var smoke = SmokeDetectorServices.LoadSmokeDetail(SerID);
            if (smoke == null || string.IsNullOrEmpty(smoke.AddressCode))
            {
                throw new BusinessException("当前设备还未安装入网");
            }
            DevicesWarningTemplateTemplate devicesWarningTemplate = new DevicesWarningTemplateTemplate
            {
                SerID = SerID,
                DeviceName = "烟感器报警",
                Type = "感应报警",
                WarningTime = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                Memo = $"此设备地址为{smoke.AddressName}",
                // Url = "http://tfs-code2.chinacloudapp.cn/Smoke/Notice",
                MasterID = SerID,
                DateStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),//短信通知时间
                Address = smoke.AddressName,
            };
            if (smoke.InstallerSysNo.HasValue && smoke.InstallerSysNo < 0)
            {
                devicesWarningTemplate.Address = smoke.Position;
                devicesWarningTemplate.Memo = smoke.Position;
            }
            SendMessageService.SendMessage(devicesWarningTemplate, SerID);
            return Json(new AjaxResult { Success = true });
        }

        #endregion



        public ActionResult LoginOut()
        {
            UserMgr.Logout();
            return RedirectToAction("DeviceList");
        }


        #region 紧急联系人

        public ActionResult Contact()
        {
            var client = ClientService.LoadClient(curentUser.UserSysNo);
            return View(client);
        }

        public ActionResult SaveContact(Client model)
        {
            ClientService.UpdateClientContact(model);
            return Json(new AjaxResult { Success = true });
        }

        #endregion


        //public ActionResult InstallSmokedetectorList()
        //{
        //    return View();
        //}
        public JsonResult QueryInstallList()
        {
            QF_SmokeDetector filter = JsonConvert.DeserializeObject<QF_SmokeDetector>(this.Request["data"]);
            filter.InstallerSysNo = curentUser.UserSysNo;// curentUser.UserSysNo;
            filter.PageSize = 7;
            filter.SortFields = "InstalledTime desc";
            var result = SmokeDetectorServices.QuerySmokeDetectorList(filter);
            return Json(new AjaxResult { Success = true, Data = result });
        }
        public ActionResult Test()
        {
            return View();
        }

        public JsonResult SendTest(string id, int offline)
        {
            if (offline == 1)
            {
                DevicesOffLine(id);
            }
            else
            {
                SendEmergencyMessage(id);
            }
            return Json(new AjaxResult { Success = true });
        }
    }
}
