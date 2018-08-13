using MessageCenter.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Template
{
    public abstract class BaseMsgTemplate
    {
        public BaseMsgTemplate()
        {
        }

        /// <summary>
        /// 同一条消息同一个接受者最大的发送次数
        /// </summary>
        public int LimitCount { get; set; }
        /// <summary>
        /// 消息发送的频率（单位为秒）
        /// </summary>
        public int SendFrequency { set; get; }

        #region 微信消息

        /// <summary>
        /// 调用微信api的Token
        /// </summary>
        public string JsApiToken { get; set; }

        /// <summary>
        /// 点击消息跳转的url
        /// </summary>
        public string Url { get; set; }


        ///// <summary>
        ///// 得到真正的url
        ///// </summary>
        //public virtual string GetRealUrl(string pr) { return Url; }

        #endregion
        /// <summary>
        /// 模板名称，显示给用户看
        /// </summary>
        public abstract string TemplateName
        {
            get;
        }
        /// <summary>
        /// 模板名称，显示给用户看
        /// </summary>
        public abstract string MasterName
        {
            get;
        }
        /// <summary>
        /// 模板名称，显示给用户看
        /// </summary>
        public string MasterID
        {
            get; set;
        }
        /// <summary>
        /// 模板标识，作为数据库中的对应关系字段， 只能由字母和数字组成
        /// </summary>
        public virtual string TemplateCode
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public abstract MsgReceiverType ReceiverType
        {
            get;
        }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; }
        /// <summary>
        /// 同模版重试发送次数
        /// </summary>
        public virtual int RetryCount { get; set; }
        /// <summary>
        /// 同IP同客户端24小时发送的条数
        /// 小于0：无限
        /// 大于0：限制条数
        /// </summary>
        public virtual int SendCount { get; set; }
        /// <summary>
        /// 模版内容
        /// </summary>
        public string Template { get; set; }

        public virtual List<MsgTemplateParmater> GetParmaterList()
        {
            Type type = GetType();
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<MsgTemplateParmater> list = new List<MsgTemplateParmater>(4);
            foreach (var p in ps)
            {
                var desca = p.GetCustomAttribute(typeof(MsgTemplateParameterDescriptionAttribute));
                if (desca != null)
                {
                    MsgTemplateParameterDescriptionAttribute desc = desca as MsgTemplateParameterDescriptionAttribute;
                    object v = p.GetValue(this);
                    list.Add(new MsgTemplateParmater
                    {
                        Name = p.Name,
                        Value = v == null ? "" : v.ToString(),
                        Color = desc.Color,
                        DisplayName = desc.DisplayName
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 属性名称与短信模板的映射关系
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, string> GetSMSParmatersMap();

        /// <summary>
        /// 属性名称与微信模板的映射关系
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, string> GetWechatParmatersMap();

        /// <summary>
        /// 模版参数
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetParmaters()
        {
            Type type = GetType();
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (var p in ps)
            {
                var desca = p.GetCustomAttribute(typeof(MsgTemplateParameterDescriptionAttribute));
                if (desca != null)
                {
                    object v = p.GetValue(this);
                    MsgTemplateParameterDescriptionAttribute desc = desca as MsgTemplateParameterDescriptionAttribute;
                    string n = p.Name;
                    list.Add("{" + p.Name + "}", v == null ? "" : v.ToString());
                }
            }
            return list;
        }
        /// <summary>
        /// 模版ID
        /// </summary>
        public string TemplateID { get; set; }
        /// <summary>
        /// 构建消息内容
        /// </summary>
        /// <returns></returns>
        public string BuildeMessage()
        {
            string content = Template;
            if (string.IsNullOrEmpty(content)) { return content; }
            foreach (var p in GetParmaters())
            {
                content = content.Replace(p.Key, p.Value);
            }
            return content;
        }
        public virtual List<MsgTemplateParmater> GetParmaterList(MsgType smgType)
        {
            Type type = GetType();
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<MsgTemplateParmater> list = GetParmaterList();

            Dictionary<string, string> parmaterMapping = null;
            switch (smgType)
            {
                case MsgType.SMS: parmaterMapping = GetSMSParmatersMap(); break;
                case MsgType.WeiXin: parmaterMapping = GetWechatParmatersMap(); break;
                default:
                    return list;
            }
            if (parmaterMapping == null)
            {
                return list;
            }

            List<MsgTemplateParmater> newPList = new List<MsgTemplateParmater>(parmaterMapping.Count);
            foreach (var m in parmaterMapping)
            {
                var msgP = list.Find(p => p.Name.Equals(m.Key, StringComparison.CurrentCultureIgnoreCase));
                if (msgP == null) continue;

                newPList.Add(new MsgTemplateParmater
                {
                    Name = m.Value,
                    Value = msgP.Value,
                    Color = msgP.Color,
                    DisplayName = msgP.DisplayName
                });
            }
            return newPList;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MsgTemplateParameterDescriptionAttribute : Attribute
    {
        ///// <summary>
        ///// 参数名称
        ///// </summary>
        //public string Name { get; set; }

        ///// <summary>
        ///// 参数索引
        ///// </summary>
        //public int Index { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        public MsgTemplateParameterDescriptionAttribute() { }

        public MsgTemplateParameterDescriptionAttribute(string displayName)
        {
            this.DisplayName = displayName;

            //this.Index = index;
        }
        //public MsgTemplateParameterDescriptionAttribute(string displayName)
        //    : this(displayName, 0)
        //{

        //}
    }

    [Flags]
    public enum MsgReceiverType
    {
        None = 0,
        /// <summary>
        /// 生产商
        /// </summary>
        Manufacturer = 1,
        /// <summary>
        /// 分销商
        /// </summary>
        Distibutor = 2,
        /// <summary>
        /// 顾客
        /// </summary>
        Cusmtomer = 4,
        /// <summary>
        /// 微信用户
        /// </summary>
        WechatUsr = 5
    }
}
