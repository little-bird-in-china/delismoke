using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;

namespace BlueStone.Smoke.DataAccess
{
    public class CommonDA
    {

        /// <summary>
        /// 获取所有有效Area
        /// </summary>
        public static List<Area> GetAreaList()
        {
            DataCommand cmd = new DataCommand("GetAreaList");

            return cmd.ExecuteEntityList<Area>();
        }


        #region 异步获取Area
        public static List<Area> GetProvinceList()
        {
            DataCommand cmd = new DataCommand("GetProvinceList");
            cmd.SetParameter("@Status", DbType.Int32, CommonStatus.Actived);
            return cmd.ExecuteEntityList<Area>();
        }

        #endregion

        //}
        public static int InsertFileInfo(FileInfo entity)
        {
            DataCommand cmd = new DataCommand("InsertFileInfo");
            cmd.SetParameter<FileInfo>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        public static QueryResult<FileInfo> QueryFileInfoList(FileInfoFilter condition)
        {
            DataCommand command = new DataCommand("QueryFileInfoList");

            command.QuerySetCondition("CategoryName", ConditionOperation.Equal, DbType.AnsiString, condition.CategoryName);
            command.QuerySetCondition("FileName", ConditionOperation.Like, DbType.AnsiString, condition.FileName);
            command.QuerySetCondition("MasterType", ConditionOperation.Equal, DbType.Int32, condition.MasterType);
            if (condition.MasterID > 0)
            {
                command.QuerySetCondition("MasterID", ConditionOperation.Equal, DbType.Int32, condition.MasterID);
            }
            else if (condition.MasterIDList != null && condition.MasterIDList.Count > 0)
            {
                command.QuerySetCondition(string.Format(" AND MasterID in ( '{0}') ", string.Join("','", condition.MasterIDList)));
            }
            command.QuerySetCondition("CreateUserName", ConditionOperation.Equal, DbType.AnsiString, condition.InUserName);
            command.QuerySetCondition("CreateUserSysNo", ConditionOperation.Equal, DbType.Int32, condition.InUserSysNo);
            command.QuerySetCondition("CreateTime", ConditionOperation.MoreThan, DbType.DateTime, condition.BegInInDate);
            command.QuerySetCondition("CreateTime", ConditionOperation.LessThan, DbType.DateTime, condition.EndInDate);

            QueryResult<FileInfo> result = command.Query<FileInfo>(condition, "Priority asc,CreateTime desc");
            return result;
        }


        public static void DeleteFileInfo(FileMasterType masterType, int masterID, string categoryName)
        {
            DataCommand cmd = new DataCommand("DeleteOthersFileInfo");
            cmd.SetParameter("@MasterType", DbType.Int32, (int)masterType);
            cmd.SetParameter("@MasterID", DbType.Int32, masterID);
            cmd.SetParameter("@CategoryName", DbType.String, categoryName);
            cmd.ExecuteNonQuery();
        }

        public static FileInfo LoadFileInfoBySysNo(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadFileInfoBySysNo");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            return cmd.ExecuteEntity<FileInfo>();

        }
    }
}
