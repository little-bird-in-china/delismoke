using BlueStone.Utility;
using System.ComponentModel;

namespace BlueStone.Smoke.Entity
{
    public enum TopicStatus
    {
        [Description("草稿")]
        Init = 0,
        [Description("发布")]
        Published = 1,
        [Description("撤下")]
        Offline = 11,
        [Description("作废")]
        Void = -1,
        [Description("删除")]
        [Display(false)]
        Delete = -999,

    }
    public enum TopicContentType
    {
        [Description("图片")]
        Image = 1,
        [Description("文件")]
        File = 2,
        [Description("视频")]
        Vedio = 3,
        [Description("图文混排")]
        Html = 4
    }
    public enum TopicDisplayPosition
    {
        [Description("默认")]
        Default = 0,
        [Description("首页")]
        Index = 1,
        [Description("帮助中心")]
        HelpCenter = 2,
    }


}
