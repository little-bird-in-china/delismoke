using BlueStone.Utility;
using BlueStone.Utility.Caching;
using BlueStone.Utility.HttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueStone.Smoke.Service.ONENET
{
    public class ONENETService
    {
        private static List<TargetUrlItem> m_targetList = null;

        static ONENETService()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Configuration/ONENET.config");
            m_targetList = CacheManager.GetWithLocalCache(path, () =>
            {
                List<TargetUrlItem> list = SerializeHelper.LoadFromXml<List<TargetUrlItem>>(path);
                if (list == null)
                {
                    list = new List<TargetUrlItem>();
                }
                return list;
            }, path);
        }

        public static CreateDeviceResponse CreateDevice(CreateDeviceRequest request)
        {
            TargetUrlItem urlItem = GetTargetUrlItem("CreateDevice");
            string requestStr = JsonConvert.SerializeObject(request);
            //Logger.WriteLog("请求参数：" + requestStr);
            AsyncResult<CreateDeviceResponse> response = HttpClient.Post<CreateDeviceResponse>(urlItem.Url, requestStr);
            return response.Result;
        }


        public static DeleteDviceResponse DeleteDevice(DeleteDeviceRequest request)
        {
            TargetUrlItem urlItem = GetTargetUrlItem("DeleteDevice");
            string requestUrl = string.Format(urlItem.Url, request.DeviceID);
            AsyncResult<DeleteDviceResponse> response = HttpClient.Get<DeleteDviceResponse>(requestUrl);
            return response.Result;
        }

        public static SendCmdResponse SendCmd(SendCmdRequest request)
        {
            TargetUrlItem urlItem = GetTargetUrlItem("SendCmdNBIOT");
            string requestUrl = string.Format(urlItem.Url, request.Imei);
            AsyncResult<SendCmdResponse> response = HttpClient.Get<SendCmdResponse>(requestUrl);
            return response.Result;
        }
        public static UpdateDeviceResponse UpdateDevice(UpdateDeviceRequest request)
        {
            TargetUrlItem urlItem = GetTargetUrlItem("UpdateDevice");
            string requestStr = JsonConvert.SerializeObject(request);
            AsyncResult<UpdateDeviceResponse> response = HttpClient.Post<UpdateDeviceResponse>(urlItem.Url, requestStr);
            return response.Result;
        }




        private static TargetUrlItem GetTargetUrlItem(string name)
        {
            if (m_targetList == null || m_targetList.Count <= 0)
            {
                throw new BusinessException("您还没有配置需要交互的接口列表");
            }
            TargetUrlItem urlItem = m_targetList.FirstOrDefault(f => f.Name == name);
            if (urlItem == null || string.IsNullOrEmpty(urlItem.Url))
            {
                throw new BusinessException(string.Format("您还没有配置【{0}】的接口。", name));
            }
            return urlItem;
        }
    }
}
