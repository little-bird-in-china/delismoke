using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Service;
using BlueStone.Smoke.Service.ONENET;
using BlueStone.Utility;
using MessageCenter.Processor;
using MessageCenter.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BlueStone.Smoke.Backend.Controllers
{
    public class CommonApiController : ApiController
    {

        private static string[] SMOKE_STATUS_CODE = { "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1112", "1113" };
        public HttpResponseMessage SyncDeviceStatus()
        {
            Stream reqstream = HttpContext.Current.Request.InputStream;
            reqstream.Seek(0, System.IO.SeekOrigin.Begin);
            string body = new StreamReader(reqstream).ReadToEnd();
            Logger.WriteLog("API接收数据：" + body);
            List<SyncDeviceStatusRequest> list = JsonConvert.DeserializeObject<List<SyncDeviceStatusRequest>>(body);
            SyncDeviceStatusResponse response = new SyncDeviceStatusResponse();

            if (list == null || list.Count <= 0)
            {
                response.IsSuccess = false;
                response.Message = "参数错误。";
            }
            else
            {
                SyncDeviceStatusRequest request = null;
                foreach (var item in list)
                {
                    if (SMOKE_STATUS_CODE.Contains(item.Status))
                    {
                        request = item;
                        break;
                    }
                }
                if (request == null)
                {
                    response.IsSuccess = false;
                    response.Message = "参数错误。";
                }
                else
                {
                    SmokeDetector detecor = SmokeDetectorServices.LoadSmokeDetailByDeviceID(request.DeviceID);

                    if (detecor == null)
                    {
                        response.Message = "设备不存在。";
                    }
                    else
                    {
                        SmokeDetectorStatus status = MatchDeviceStatus(request.Status);
                        if (detecor.Status != status)//两次设备状态不一致
                        {
                            SmokeDetectorStatusLog lastLog = SmokeDetectorServices.LoadSmokeDetectorStatusLogByDeviceCode(detecor.Code);

                            #region  更新设备状态,写状态变更日志
                            DateTime dt;
                            DateTime.TryParse(request.CDatetime, out dt);
                            SmokeDetectorStatusLog statusLog = new SmokeDetectorStatusLog()
                            {
                                PreStatus = detecor.Status.Value,
                                Status = status,
                                BeginTime = dt == DateTime.MinValue ? DateTimeHelper.GetTimeZoneNow() : dt,
                                ReceivedJsonData = body,
                                SmokeDetectorCode = detecor.Code
                            };
                            detecor.Status = status;
                            if (lastLog != null && statusLog.BeginTime.HasValue && lastLog.BeginTime.HasValue)
                            {

                                statusLog.DurationSeconds = (int)(statusLog.BeginTime.Value - lastLog.BeginTime.Value).TotalSeconds;
                            }
                            using (var trans = TransactionManager.Create())
                            {
                                SmokeDetectorServices.InsertSmokeDetectorStatusLog(statusLog);
                                SmokeDetectorServices.UpdateSmokeDetector(detecor);
                                trans.Complete();
                            }
                            #endregion

                            #region 更新首页地图数据缓存

                            (new MapDataService(detecor.CompanySysNo)).DataChangeAsync();

                            #endregion

                            #region 发送消息

                            if (detecor.Status == SmokeDetectorStatus.Warning
                                || detecor.Status == SmokeDetectorStatus.TestWarning)
                            {
                                Task.Factory.StartNew(() =>//火灾报警
                                {
                                    DevicesWarningTemplateTemplate devicesWarningTemplate = new DevicesWarningTemplateTemplate
                                    {
                                        SerID = detecor.Code,
                                        DeviceName = "烟感设备报警",
                                        Type = "火灾报警",
                                        WarningTime = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                                        Memo = $"此设备地址为{detecor.AddressName}",
                                        MasterID = detecor.Code,
                                        DateStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),//短信通知时间
                                        Address = detecor.AddressName
                                    };
                                    if (detecor.InstallerSysNo.HasValue && detecor.InstallerSysNo < 0)
                                    {
                                        devicesWarningTemplate.Address = detecor.Position;
                                        devicesWarningTemplate.Memo= detecor.Position;
                                    }
                                    SendMessageService.SendMessage(devicesWarningTemplate, detecor.Code);

                                });
                            }
                            else if (detecor.Status == SmokeDetectorStatus.Lost
                                || detecor.Status == SmokeDetectorStatus.Offline
                                || detecor.Status == SmokeDetectorStatus.OutNet)
                            {
                                Task.Factory.StartNew(() =>//离线报警
                                {

                                    DevicesOfflineTemplateTemplate devicesOfflintTemplate = new DevicesOfflineTemplateTemplate
                                    {
                                        SerID = detecor.Code,
                                        DeviceName = "烟感设备离线",
                                        DeviceAddress = $"此设备地址为{detecor.AddressName}",
                                        OffLineTimeStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                                        Memo = "请尽快处理!",
                                        MasterID = detecor.Code,
                                        DateStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),//短信通知时间
                                        Address = detecor.AddressName

                                    };
                                    if (detecor.InstallerSysNo.HasValue && detecor.InstallerSysNo < 0)
                                    {
                                        devicesOfflintTemplate.Address = detecor.Position;
                                        devicesOfflintTemplate.Memo = detecor.Position;
                                    }
                                    SendMessageService.SendMessage(devicesOfflintTemplate, detecor.Code);

                                });
                            }
                            else if (detecor.Status == SmokeDetectorStatus.LowPower)
                            {
                                Task.Factory.StartNew(() =>//低电压报警
                                {
                                    DevicesWarningTemplateTemplate devicesWarningTemplate = new DevicesWarningTemplateTemplate
                                    {
                                        SerID = detecor.Code,
                                        DeviceName = "烟感设备电量过低",
                                        Type = "低电量报警",
                                        WarningTime = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                                        Memo = $"此设备地址为{detecor.AddressName}",
                                        MasterID = detecor.Code,
                                        DateStr = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),//短信通知时间
                                        Address = detecor.AddressName
                                    };
                                    if (detecor.InstallerSysNo.HasValue && detecor.InstallerSysNo < 0)
                                    {
                                        devicesWarningTemplate.Address = detecor.Position;
                                        devicesWarningTemplate.Memo = detecor.Position;
                                    }
                                    SendMessageService.SendMessage(devicesWarningTemplate, detecor.Code);

                                });
                            }
                            #endregion
                        }
                        response.IsSuccess = true;
                    }

                }

            }
            string result = JsonConvert.SerializeObject(response);
            return new HttpResponseMessage() { Content = new StringContent(result, Encoding.UTF8, "application/json") };
        }

        private SmokeDetectorStatus MatchDeviceStatus(string status)
        {
            switch (status)
            {
                case "0000":
                    return SmokeDetectorStatus.Reserve;
                case "0001":
                    return SmokeDetectorStatus.Start;
                case "0010":
                    return SmokeDetectorStatus.Beat;
                case "0011":
                    return SmokeDetectorStatus.Warning;
                case "0100":
                    return SmokeDetectorStatus.TestWarning;
                case "0101":
                    return SmokeDetectorStatus.LowPower;
                case "0110":
                    return SmokeDetectorStatus.CancelWarning;
                case "0111":
                    return SmokeDetectorStatus.Mute;
                case "1000":
                    return SmokeDetectorStatus.InNet;
                case "1001":
                    return SmokeDetectorStatus.OutNet;
                case "1010":
                    return SmokeDetectorStatus.EditServer;
                case "1011":
                    return SmokeDetectorStatus.ActiveBeat;
                case "1100":
                    return SmokeDetectorStatus.LampstandLowPower;
                case "1101":
                    return SmokeDetectorStatus.Lost;
                case "1112":
                    return SmokeDetectorStatus.Offline;
                case "1113":
                    return SmokeDetectorStatus.Online;
                default:
                    return SmokeDetectorStatus.Reserve;
            }
        }

        [HttpGet]
        public string SendMessages()
        {
            MessageProcessor.Instance.PreLoadMessages();
            return "{Message:\"正在发送消息\"}";
        }
    }
}