using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class QF_Topic : QueryFilter
    {

        public string Title { get; set; }

        public string CategoryID { get; set; }

        public TopicStatus? TopicStatus { get; set; }

        public int? CategorySysNo { get; set; }

        public string MasterName { get; set; }
    }

    public class QR_Topic
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 类别系统编号
        /// </summary>
        public int? TopicCategorySysNo { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 缺省图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 文章缩略
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 是否标红
        /// </summary>
        public int? IsRed { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public int? IsTop { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int? PageViews { get; set; }

        /// <summary>
        /// 有效期起
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 有效期止
        /// </summary>
        public DateTime? EndTime { get; set; }

        public string StartTimeStr
        {
            get
            {
                if (this.StartTime.HasValue)
                    return this.StartTime.Value.ToString("yyyy-MM-dd");
                return "";
            }
        }

        public string EndTimeStr
        {
            get
            {
                if (this.EndTime.HasValue)
                    return this.EndTime.Value.ToString("yyyy-MM-dd");
                return "";
            }
        }
        public SiteType SiteType { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string ValidityPeriod
        {
            get
            {
                if (this.StartTime.HasValue && this.EndTime.HasValue)
                {
                    return string.Format("{0} - {1}", this.StartTime.Value.ToShortDateString(), this.EndTime.Value.ToShortDateString());
                }
                else if (this.StartTime.HasValue)
                {
                    return string.Format(LangHelper.GetText("{0}起"), this.StartTime.Value.ToShortDateString());
                }
                else if (this.EndTime.HasValue)
                {
                    return string.Format(LangHelper.GetText("{0}止"), this.EndTime.Value.ToShortDateString());
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 文章状态
        /// </summary>
        public TopicStatus? TopicStatus { get; set; }
        public string TopicStatusStr
        {
            get
            {
                if (this.TopicStatus.HasValue)
                    return this.TopicStatus.GetDescription();
                return "N/A";
            }
        }

        /// <summary>
        /// 创建者系统编号
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者显示名
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }
        public string InDateStr
        {
            get
            {
                if (this.InDate.HasValue)
                    return this.InDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                return "";
            }
        }

        /// <summary>
        /// 最后修改人系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后修改人显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? EditDate { get; set; }
        public string EditDateStr
        {
            get
            {
                if (this.EditDate.HasValue)
                    return this.EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                return "";
            }
        }

        /// <summary>
        /// 发布人系统编号
        /// </summary>
        public int? PublishUserSysNo { get; set; }

        /// <summary>
        /// 发布人显示名
        /// </summary>
        public string PublishUserName { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishDate { get; set; }

        public TopicContentType? TopicContentType { get; set; }
    }
}
