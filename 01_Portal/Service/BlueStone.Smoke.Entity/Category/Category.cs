using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace BlueStone.Smoke.Entity
{
    public class Category : EntityBase
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 类别编码：类别编码，2位一级，下级在上级的基础上添加两位，如上级编码为“10”，则下级第一个编码为“1001”， 下级第二个编码为“1002”
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 类别父编码：父编码
        /// </summary>
        public string ParentCategoryCode { get; set; }

        /// <summary>
        /// 类别名称：名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 简拼：简拼，自动生成
        /// </summary>
        public string JianPin { get; set; }

        /// <summary>
        /// 备注：备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 状态：
        /// </summary>
        public CommonStatus CommonStatus { get; set; }

        /// <summary>
        /// 状态：
        /// </summary>
        public string CommonStatusStr { get { return EnumHelper.GetDescription(CommonStatus); } }

        /// <summary>
        /// ：
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public string DefaultImage { get; set; }

        public bool? IsLeaf { get; set; }

        public List<Category> CategoryList { get; set; }
    }
}
