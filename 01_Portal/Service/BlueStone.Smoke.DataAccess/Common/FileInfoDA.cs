using BlueStone.Smoke.Entity;
using BlueStone.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.DataAccess
{
    public class FileInfoDA
    {
        /// <summary>
        /// 创建FileInfo信息
        /// </summary>
        public static int InsertFileInfo(FileInfo entity)
        {
            DataCommand cmd = new DataCommand("InsertFileInfo");
            cmd.SetParameter<FileInfo>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }
    }
}
