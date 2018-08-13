using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System.Collections.Generic;

namespace BlueStone.Smoke.Service
{
    public class CommonService
    {
        /// <summary>
        /// 获取所有有效Area
        /// </summary>
        public static List<Area> GetAreaList()
        {
            return CommonDA.GetAreaList();
        }



        public static List<Area> GetProvinceList()
        {
            return CommonDA.GetProvinceList();
        }



        public static int InsertFileInfo(FileInfo entity)
        {
            if (string.IsNullOrEmpty(entity.FileRelativePath))
            {
                return 0;
            }
            entity.Priority = 0;
            CheckFileInfo(entity, true);
            if (entity.IsSingle)
            {//删除之前上传的文件
                CommonDA.DeleteFileInfo(entity.MasterType.Value, entity.MasterID, entity.CategoryName);
            }
            return CommonDA.InsertFileInfo(entity);
        }
        /// <summary>
        /// 检查FileInfo信息
        /// </summary>
        private static void CheckFileInfo(FileInfo entity, bool isCreate)
        {
            if (entity == null)
            {
                throw new BusinessException(LangHelper.GetText("请传入实体！"));
            }
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if (!entity.MasterType.HasValue)
            {
                throw new BusinessException(LangHelper.GetText("主体类型不能为空！"));
            }

            if (entity.MasterID <= 0)
            {
                throw new BusinessException(LangHelper.GetText("主体ID不能为空！"));
            }
            if (entity.CategoryName != null && entity.CategoryName.Length > 64)
            {
                throw new BusinessException(LangHelper.GetText("文件分组编号长度不能超过64！"));
            }
            if (string.IsNullOrWhiteSpace(entity.FileRelativePath))
            {
                throw new BusinessException(LangHelper.GetText("文件相对路径不能为空！"));
            }
        }


        public static void DeleteFileInfo(FileMasterType type, int masterID, string categoryName)
        {
            CommonDA.DeleteFileInfo(type, masterID, categoryName);
        }
        public static FileInfo LoadFileInfoBySysNo(int sysNo)
        {
            return CommonDA.LoadFileInfoBySysNo(sysNo);
        }
    }
}
