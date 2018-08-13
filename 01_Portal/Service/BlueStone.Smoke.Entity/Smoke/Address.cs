using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class Address
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 公司编号：
        /// </summary>
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 烟感器编号：在公司维度下编码，用数字和字母来编码，2位一级，下级在上级的基础上添加两位，如上级编码为“10”，则下级第一个编码为“1001”， 下级第二个编码为“1002”
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 所在地区编号：
        /// </summary>
        public int? ParentSysNo { get; set; }

        /// <summary>
        /// 所有的地区名称：
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路径名称：从根节点到此节点的名称，名称间用">"分隔
        /// </summary>
        public string PathName { get; set; }
        public int AreaSysNo { get; set; }
        public string AreaAddress { get; set; }
        public Area Area { get; set; }
        /// <summary>
        /// 地址级别：
        /// </summary>
        public AddressGrade? Grade { get; set; }

        /// <summary>
        /// 地址级别：
        /// </summary>
        public string GradeStr { get { return EnumHelper.GetDescription(Grade); } }

        /// <summary>
        /// 编号：当按规则生成时的编号是多少。
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 修改人编号：最后修改人系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 修改人：最后修改人显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 修改时间：最后修改时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 修改时间：最后修改时间
        /// </summary>
        public string EditDateStr { get { return EditDate.HasValue ? EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }
    }

    public enum AddressGrade
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        Default = 0,
        /// <summary>
        /// 楼栋
        /// </summary>
        [Description("楼栋")]
        Building = 1,
        /// <summary>
        /// 楼层
        /// </summary>
        [Description("楼层")]
        Floor = 2,
        /// <summary>
        /// 房间
        /// </summary>
        [Description("房间")]
        Room = 3
    }

    public class AddressFilter : QueryFilter
    {
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 如果ParentSysNo有值，则AddressGrade将无效，否则 AddressGrade 有效
        /// </summary>
        public int? ParentSysNo { get; set; }

        /// <summary>
        /// 查询值小于或等于此值的数据
        /// </summary>
        public AddressGrade? AddressGrade { get; set; }

        public AddressFilter() { AddressGrade = Entity.AddressGrade.Floor; }

        /// <summary>
        /// 查询根节点
        /// </summary>
        public bool? SelectRoot { get; set; }

        /// <summary>
        /// 选中的地址Code
        /// </summary>
        public string SelectedAddressCode { get; set; }
    }


    /// <summary>
    /// 地址名称规则
    /// </summary>
    public class AddressNamePattern
    {
        /// <summary>
        /// 公司编号
        /// </summary>
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 父级编号
        /// </summary>
        public int ParentSysNo { get; set; }
        /// <summary>
        /// 地址级别
        /// </summary>
        public AddressGrade? Grade { get; set; }
        /// <summary>
        /// 编号类型
        /// </summary>
        public AddressNameNoType NoType { get; set; }
        /// <summary>
        /// 是否使用父级编号作为前缀
        /// </summary>
        public bool ParentNoAsPreName { get; set; }

        public string ParentNo { get; set; }

        /// <summary>
        /// 前缀名称 或 当父级编号作为前缀时与当前编号的分隔符
        /// </summary>
        public string PreName { get; set; }

        public int NoLength { get; set; }
        /// <summary>
        /// 后缀名称
        /// </summary>
        public string SufName { get; set; }
        /// <summary>
        /// 起始编号
        /// </summary>
        public string BeginNo { get; set; }
        /// <summary>
        /// 结束编号
        /// </summary>
        public string EndNo { get; set; }

        public AddressNamePattern ChildPattern { get; set; }
    }
    /// <summary>
    /// 编号类型：数字或字母
    /// </summary>
    public enum AddressNameNoType
    {
        /// <summary>
        /// 数字
        /// </summary>
        Digit = 0,
        /// <summary>
        /// 字母
        /// </summary>
        Char = 1
    }
}