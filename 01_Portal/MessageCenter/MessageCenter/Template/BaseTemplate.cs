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
        /// 模板名称，显示给用户看
        /// </summary>
        public abstract string TemplateName
        {
            get;
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
                    string n = string.IsNullOrWhiteSpace(desc.Name) ? p.Name : desc.Name;
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
            foreach (var p in GetParmaters())
            {
                content = content.Replace(p.Key, p.Value);
            }
            return content;
        }
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
                        Name = string.IsNullOrWhiteSpace(desc.Name) ? p.Name : desc.Name,
                        Index = desc.Index,
                        Value = v == null ? "" : v.ToString(),
                        Color = desc.Color,
                        DisplayName = desc.DisplayName
                    });
                }
            }
            return list;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MsgTemplateParameterDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        public MsgTemplateParameterDescriptionAttribute() { }

        public MsgTemplateParameterDescriptionAttribute(string displayName, int index)
        {
            this.DisplayName = displayName;

            this.Index = index;
        }
        public MsgTemplateParameterDescriptionAttribute(string displayName)
            : this(displayName, 0)
        {

        }
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
    }
}
