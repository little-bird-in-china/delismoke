using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlueStone.Smoke.Entity
{
    public enum FileMasterType
    {
        [Description("文章")]
        Topic = 1,
        [Description("推荐位图片")]
        RecommandBannerImage = 2,
        [Description("企业基本信息")]
        CompanyBasic = 3,
    }
    public class FileInfo
    {

        public long SysNo { get; set; }
        /// <summary>
        /// 文件所属的主体类型："Company"表示公司的数据，"Topic"表示内容数据
        /// </summary>
        public FileMasterType? MasterType { get; set; }

        /// <summary>
        /// 文件所属主体的ID。此ID唯一标识由“MasterType(主体类型)”所有确定的主体类型下的指定主体
        /// </summary>
        public int MasterID { get; set; }


        public string CategoryName { get; set; }



        /// <summary>
        /// 文件相对文件服务器的路径
        /// </summary>
        public string FileRelativePath { get; set; }

        /// <summary>
        /// 文件上传时的名称，如：“数据库存设计.xlsx”
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Size { get; set; }


        /// <summary>
        /// 如：“application/msword”，“image/jpeg”
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CreateUserSysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreateTimeStr { get { return CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        /// <summary>
        /// 是否单文件上传
        /// </summary>
        public bool IsSingle { get; set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
    public class FileInfoFilter : QueryFilter
    {
        public int IncubatorsSysNo { get; set; }
        /// <summary>
        /// 主体类型  
        /// </summary>
        public FileMasterType? MasterType { get; set; }
        /// <summary>
        /// 主体编号  
        /// </summary>
        public int MasterID { get; set; }
        public List<int> MasterIDList { get; set; }
        public string CategoryName { get; set; }
        /// <summary>
        /// 文件名称  
        /// </summary>
        public string FileName { get; set; }


        /// <summary>
        /// 最后更新者系统编号
        /// </summary>
        public string InUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary> 
        public DateTime? BegInInDate { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary> 
        public DateTime? EndInDate { get; set; }
    }
    [Flags]
    public enum FilePower
    {
        None
    }

    public class UpdateFilePriorityInfo
    {
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 修改人编号
        /// </summary>
        public int UserSysNo { get; set; }

        /// <summary>
        /// 修改人名称
        /// </summary>
        public string UserName { get; set; }


        public List<FilePriorityModel> FilePriorityList { get; set; }

        public class FilePriorityModel
        {

            public int SysNo { get; set; }

            public int Priority { get; set; }
        }
    }
}
