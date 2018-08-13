using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Service
{
    public class FileInfoService
    {
        /// <summary>
        /// 创建FileInfo信息
        /// </summary>
        public static int InsertFileInfo(FileInfo entity)
        {
            entity.Priority = 0;
            CheckFileInfo(entity, true);
            entity.CreateTime = DateTime.Now;
            return FileInfoDA.InsertFileInfo(entity);
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
            if (entity.MasterID<=0)
            {
                throw new BusinessException(LangHelper.GetText("主体ID必须大于0！"));
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

    }
}
