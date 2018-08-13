using BlueStone.Utility;
using System;
using System.Collections.Generic;

namespace BlueStone.Smoke.Entity
{
    [Serializable]
    public class TopicInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 类别系统编号
        /// </summary>
        public int? TopicCategorySysNo { get; set; }

        public string CategoryName { get; set; }

        /// <summary>
        /// 政策级别编号
        /// </summary>
        public int PolicyLevelSysNo { get; set; }

        /// <summary>
        /// 政策支持产业编号
        /// </summary>
        public int IndustryTypeSysNo { get; set; }

        public string MasterName { get; set; }
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
        /// 来源地址
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// 文章缩略
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }



        public string ExplainedContent { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 政策发文单位名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 政策发文时间
        /// </summary>
        public DateTime? OrgPublishDate { get; set; }

        public string OrgPublishDateStr
        {
            get
            {
                if (OrgPublishDate.HasValue)
                {
                    return OrgPublishDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

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
        public int PageViews { get; set; }

        /// <summary>
        /// 有效期起
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 有效期止
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 文章状态
        /// </summary>
        public TopicStatus? TopicStatus { get; set; }


        public string StartTimeStr
        {
            get
            {
                if (StartTime.HasValue)
                {
                    return StartTime.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string EndTimeStr
        {
            get
            {
                if (EndTime.HasValue)
                {
                    return EndTime.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        public List<FileInfo> FileList { get; set; }

    }
}
