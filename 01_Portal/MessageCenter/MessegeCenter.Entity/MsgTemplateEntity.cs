using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class MsgTemplateEntity
    {
        public string TemplateContent { get; set; }
        public int MsgType { get; set; }
        public int LimitCount { get; set; }
        public int SendFrequency { get; set; }

        public string GetRealUrl(string SerID)
        {
            if (!string.IsNullOrEmpty(SerID))
            {
                return Url + $"/Smoke/DeviceDetails?code={SerID}";
            }
            return string.Empty;
        }

        public string Url { get; set; }
        public string ExternalTemplateID { get; set; }
    }
    /// <summary>
    /// 消息模版参数
    /// </summary>
    public class MsgTemplateParmater
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 字体颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 参数索引
        /// </summary>
        // public int Index { get; set; }
    }
    public class MsgTemplateEntity_QF
    {
        public int TenantID { get; set; }
        public string ActionCode { get; set; }
    }
}
