using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueStone.Utility;
using BlueStone.Smoke.Entity;

namespace BlueStone.Smoke.Entity
{
    public class TopicCategory : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 类别ID，两位一级
        /// </summary>
        public string CategoryID { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

 

        /// <summary>
        /// 是否为叶节点
        /// </summary>
        public CommonYesOrNo? IsLeaf { get; set; }

        /// <summary>
        /// 顺序，越小越优先
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Summary { get; set; }

 

        /// <summary>
        /// 图片
        /// </summary>
        public string DefaultImage { get; set; }

 

        /// <summary>
        /// 用户自定义标识,供前台某些特殊需求时使用
        /// </summary>
        public string CustomFlag { get; set; }

 

        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除是将状态设置为-999
        /// </summary>
        public CommonStatus? CommonStatus { get; set; }

        public String MasterName { get; set; }



        public List<KeyValuePair<Nullable<TopicDisplayPosition>, string>> TopicDisplayPositionList
        {
            get
            {
                return BlueStone.Utility.EnumHelper.GetKeyValuePairs<TopicDisplayPosition>(EnumAppendItemType.None);
            }
        }
    }

    public class QF_TopicCategory : QueryFilter
    {
        public int? SysNo { get; set; }

        public string CategoryID { get; set; }

        public string ParentCategoryID { get; set; }

        public CommonStatus? CommonStatus { get; set; }

        public string TenantID { get; set; }
    }
}
