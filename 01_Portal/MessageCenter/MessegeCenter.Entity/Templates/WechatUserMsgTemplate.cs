using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Template
{
    public abstract class WechatUserMsgTemplate : BaseMsgTemplate
    {
        public WechatUserMsgTemplate()
            : base()
        {
            SendCount = -1;
        }
        public override MsgReceiverType ReceiverType
        {
            get
            {
                return MsgReceiverType.WechatUsr;
            }
        }
        public override string MasterName
        {
            get
            {
                return "SmokeDetector";
            }
        }
        #region 内容
        [MsgTemplateParameterDescription("设备ID")]
        public string SerID { get; set; }
        [MsgTemplateParameterDescription("标题")]
        public string Title { get; set; }


        [MsgTemplateParameterDescription("备注")]
        public string Memo { get; set; }

       
        [MsgTemplateParameterDescription("时间")]
        public string DateStr { get; set; }

        [MsgTemplateParameterDescription("地点")]
        public string Address { get; set; }

        [MsgTemplateParameterDescription("备注")]
        public string SmsMemo { get; set; }
        #endregion


        //public override  string GetRealUrl(string SerID)
        //{
        //    if (string.IsNullOrEmpty(SerID))
        //    {
        //        return Url + $"/Smoke/DeviceDetails?code={SerID}";
        //    }
        //    return string.Empty;
        //}
        protected override Dictionary<string, string> GetSMSParmatersMap()
        {
            Dictionary<string, string> d = new Dictionary<string, string>(4);
            d.Add("SerID", "1");
            d.Add("DateStr", "2");
            d.Add("Address", "3");
            d.Add("SmsMemo", "4");
            return d;
        }
        protected override Dictionary<string, string> GetWechatParmatersMap()
        {
            Dictionary<string, string> d = new Dictionary<string, string>(3);
            return d;
        }
    }
    public class WechatUserBindDevicesTemplate : WechatUserMsgTemplate
    {
         
        [MsgTemplateParameterDescription("绑定结果")]
        public string BindResult { get; set; }

        [MsgTemplateParameterDescription("绑定时间")]
        public string BindTimeStr { get; set; }

        public override string TemplateCode
        {
            get
            {
                return "WechatUserBindDevices";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "微信用户绑定烟感器设备";
            }
        }
        protected override Dictionary<string, string> GetWechatParmatersMap()
        {
            Dictionary<string, string> d = new Dictionary<string, string>(5);
            d.Add("Title", "first");
            d.Add("SerID", "keyword1");
            d.Add("BindResult", "keyword2");
            d.Add("BindTimeStr", "keyword3");
            d.Add("Memo", "remark");
            return d;
        }
    }

    public class WechatUserUntieDevicesTemplate : WechatUserMsgTemplate
    { 
        public override string TemplateCode
        {
            get
            {
                return "WechatUserUntieDevices";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "微信用户解绑烟感器设备";
            }
        }
        protected override Dictionary<string, string> GetWechatParmatersMap()
        {
            Dictionary<string, string> d = new Dictionary<string, string>(3);
            return d;
        }
    }



    public class DevicesOfflineTemplateTemplate : WechatUserMsgTemplate
    {

        [MsgTemplateParameterDescription("设备名称")]
        public string DeviceName { get; set; }

        [MsgTemplateParameterDescription("设备地址")]
        public string DeviceAddress { get; set; }
        [MsgTemplateParameterDescription("离线时间")]
        public string OffLineTimeStr { get; set; }

        public override string MasterName
        {
            get
            {
                return "SmokeDetector";
            }
        }
        public override string TemplateCode
        {
            get
            {
                return "DevicesOffLine";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "烟感器离线通知";
            }
        }
        protected override Dictionary<string, string> GetWechatParmatersMap()
        {
            Dictionary<string, string> d = new Dictionary<string, string>(6);
            d.Add("Title", "first");
            d.Add("SerID", "keyword1");
            d.Add("DeviceName", "keyword2");
            d.Add("DeviceAddress", "keyword3");
            d.Add("OffLineTimeStr", "keyword4");
            d.Add("Memo", "remark");
            return d;
        }
    }
    public class DevicesWarningTemplateTemplate : WechatUserMsgTemplate
    {
        [MsgTemplateParameterDescription("设备名称")]
        public string DeviceName { get; set; }
        [MsgTemplateParameterDescription("报警类型")]
        public string Type { get; set; }
        [MsgTemplateParameterDescription("报警时间")]
        public string WarningTime { get; set; }


        public override string TemplateCode
        {
            get
            {
                return "DevicesWarning";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "烟感器报警通知";
            }
        }
        protected override Dictionary<string, string> GetWechatParmatersMap()
        {
            Dictionary<string, string> d = new Dictionary<string, string>(6);
            d.Add("Title", "first");
            d.Add("SerID", "keyword1");
            d.Add("DeviceName", "keyword2");
            d.Add("Type", "keyword3");
            d.Add("WarningTime", "keyword4");
            d.Add("Memo", "remark");
            return d;
        }
    }

}
