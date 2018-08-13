using BlueStone.Utility;
using System.Collections.Generic;

namespace BlueStone.Smoke.Entity
{
    public class Company : EntityBase
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 企业名称：企业名称必须系统唯一
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 企业电话：电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 系统管理员登陆账号
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 联系人：
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机：
        /// </summary>
        public string ContactCellPhone { get; set; }

        public string Logo { get; set; }
        /// <summary>
        /// 公司地区：
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 公司地址：
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经纬度：经纬度“Lng|Lat”分隔
        /// </summary>
        public string LngLat { get; set; }

        /// <summary>
        /// 企业介绍：
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 企业状态：
        /// </summary>
        public CompanyStatus? CompanyStatus { get; set; }

        /// <summary>
        /// 企业状态：
        /// </summary>
        public string CompanyStatusStr { get { return EnumHelper.GetDescription(CompanyStatus); } }

        /// <summary>
        /// 登录账号编号：
        /// </summary>
        public int? AccountSysNo { get; set; }

        public List<FileInfo> FileList { get; set; }
    }

    public class QF_Company : QueryFilter
    {

        /// <summary>
        ///  系统编号：系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        ///  企业名称：企业名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CompanyStatus? CompanyStatus { get; set; }

    }
}