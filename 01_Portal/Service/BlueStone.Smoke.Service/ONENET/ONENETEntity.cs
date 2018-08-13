using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace BlueStone.Smoke.Service.ONENET
{
    public class BaseResponse
    {
        /// <summary>
        /// 0=无错误,1=失败
        /// </summary>
        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("msg")]
        public string msg { get; set; }
        [JsonProperty("IsSuccess")]
        public bool IsSuccess { get; set; }
    }

    #region  request entity
    /// <summary>
    /// 创建设备请求实体
    /// </summary>
    public class CreateDeviceRequest
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        [JsonProperty("desc")]
        public string Desc { get; set; }
        /// <summary>
        /// 设备标签
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }
        /// <summary>
        /// 设备协议 固定值: LWM2M
        /// </summary>
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        /// <summary>
        /// {"lon": 106, "lat": 29, "ele": 370};
        /// </summary>
        [JsonProperty("location_lon")]
        public decimal LocationLon { get; set; }
        [JsonProperty("location_lat")]
        public decimal LocationLat { get; set; }
        [JsonProperty("location_ele")]
        public decimal LocationEle { get; set; }
        /// <summary>
        /// {"imei":"xxxxxxxxxxxxxx",imsi:"xxxx"}
        /// </summary>
        [JsonProperty("imei")]
        public string IMei { get; set; }

        [JsonProperty("imsi")]
        public string IMsi { get; set; }
        /// <summary>
        /// 是否订阅
        /// </summary>
        [JsonProperty("obsv")]
        public int Observe { get; set; }
        [JsonProperty("IsOnLine")]
        public int IsOnLine { get; set; }

    }
    /// <summary>
    /// 删除设备请求实体
    /// </summary>
    public class DeleteDeviceRequest
    {
        [JsonProperty("DeviceID")]
        public string DeviceID { get; set; }
    }
    /// <summary>
    /// 发送命令请求实体
    /// </summary>
    public class SendCmdRequest
    {
        [JsonProperty("imei")]
        public string Imei { get; set; }
    }

    public class UpdateDeviceRequest : CreateDeviceRequest
    {
        [JsonProperty("deviceid")]
        public string DeviceID { get; set; }

    }
    #endregion



    #region 【response entity】
    /// <summary>
    /// 创建设备返回结果
    /// </summary>
    public class CreateDeviceResponse : BaseResponse
    {
        /// <summary>
        /// Onenet的设备ID
        /// </summary>
        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }
    }
    /// <summary>
    /// 删除设备的返回结果
    /// </summary>
    public class DeleteDviceResponse : BaseResponse
    {
    }

    public class SendCmdResponse : BaseResponse
    {
        /// <summary>
        /// Onenet的设备ID
        /// </summary>
        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }
    }


    public class UpdateDeviceResponse : CreateDeviceResponse
    {
    }

    #endregion

    [Serializable]
    public class TargetUrlItem
    {
        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Method { get; set; }

        [XmlAttribute]
        public string Url { get; set; }
    }
    public class SyncDeviceStatusRequest
    {
        public long SFDPID
        {
            get;
            set;
        }
        [JsonProperty("dev_id")]
        public string DeviceID
        {
            get;
            set;
        }
        [JsonProperty("dev_value")]
        public string DeviceValue
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string Info
        {
            get; set;
        }
        [JsonProperty("desc")]
        public string Desc
        {
            get; set;
        }

        public int Level
        {
            get;
            set;
        }

        public string SerNo { get; set; }

        public string CDatetime { get; set; }
    }

    public class SyncDeviceStatusResponse
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("device_id")]
        public string DeviceID { get; set; }
    }
}
