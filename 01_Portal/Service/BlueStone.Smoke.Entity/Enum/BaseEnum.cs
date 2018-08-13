using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BlueStone.Smoke.Entity
{
    public enum CommonStatus : int
    {
        [Description("有效")]
        Actived = 1,
        [Description("无效")]
        DeActived = 0,

        [Display(false)]
        [Description("已删除")]
        Deleted = -1
    }

    public enum CommonYesOrNo
    {
        [Description("是")]
        Yes = 1,
        [Description("否")]
        No = 0
    }

    public enum CommonHasOrNot
    {
        [Description("有")]
        Yes = 1,
        [Description("无")]
        No = 0
    }


    public enum Gender
    {
        [Description("未知")]
        Unknown = -1,
        [Description("男")]
        Male = 1,
        [Description("女")]
        Female = 0
    }
    /// <summary>
    /// 加密模式
    /// </summary>
    public enum EncryptMode
    {
        /// <summary>
        /// SHA1,并使用Salt，该方法为默认的加密方式
        /// </summary>
        [Description("SHA1加密")]
        SHA1 = 0,
        /// <summary>
        /// MD5加密算法，为了兼容KJT外部系统导入的密码
        /// </summary>
        [Description("MD5加密")]
        MD5 = 1
    }


    public enum ECConfigMode
    {
        [Description("单一")]
        SimpleValue = 0,
        [Description("集合")]
        Collection = 1
    }

    /// <summary>
    /// ERP账号类型
    /// </summary>
    public enum IsChild
    {
        [Description("子账号")]
        CHILD = 1,
        [Description("父账号")]
        PARENT = 0
    }
    /// <summary>
    /// 渠道类型
    /// </summary>
    public enum ChannelType
    {
        [Description("第三方")]
        ThirdParty = 0,
        [Description("自营")]
        SelfSupport = 1
    }

    /// <summary>
    /// 处理状态
    /// </summary>
    public enum DealStatus
    {
        [Description("未处理")]
        Init = 0,
        [Description("处理成功")]
        Succeed = 1,
        [Description("处理失败")]
        Failure = 2,
    }

    /// <summary>
    /// 第三方来源
    /// </summary>
    public enum ThirdPartSource
    {
        [Description("京东")]
        JD = 0,
        [Description("淘宝")]
        TB = 1,
        [Description("天猫")]
        TM = 2
    }

    public enum SiteType
    {
        [Description("PC")]
        PC = 0,
        [Description("APP")]
        APP = 2,
    }

    public enum SendStatus
    {
        [Description("待发送")]
        Wait = 0,
        [Description("已发送")]
        Send = 1
    }

    public enum MailPriority
    {
        [Description("普通")]
        Normal = 0,
        [Description("高")]
        High = 1,
        [Description("非常高")]
        VeryHigh = 2,
    }

    public enum ClientType
    {
        [Description("MSite")]
        Msite = 0,
        //[Description("后台")]
        //Backend = 1,
        [Description("PC网站")]
        PC = 2,
        [Description("Android")]
        Android = 3,
        [Description("IPhone")]
        IPhone = 4,
        [Description("WeiXin")]
        Wechat = 5
    }
    public enum ViewPermissions
    {
        [Description("无需登录（任何人）")]
        None = 0,
        [Description("登录查看（门店所有员工）")]
        Login = 1,
        [Description("仅主账号查看（门店老板）")]
        Master = 2,

    }

    public enum LinkType
    {
        [Description("文字链接")]
        Text = 1,
        [Description("图标链接")]
        Icon = 2,
    }
}
