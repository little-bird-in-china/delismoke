using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BlueStone.Smoke.Entity;
using System;
using System.Text;

namespace BlueStone.Smoke.DataAccess
{
    public class SystemUserDA
    {
        /// <summary>
        /// 创建SystemUser信息
        /// </summary>
        public static int InsertSystemUser(SystemUser entity)
        {
            DataCommand cmd = new DataCommand("InsertSystemUser");
            cmd.SetParameter<SystemUser>(entity);
            //cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        public static int InsertSystemUser_Application(int user_sysno, string applicationid)
        {
            DataCommand cmd = new DataCommand("InsertSystemUser_Application");
            cmd.SetParameter("@UserSysNo", DbType.Int32, user_sysno);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationid);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        public static int DeleteSystemUser_Application(int user_sysno, string applicationid)
        {
            DataCommand cmd = new DataCommand("DeleteSystemUser_Application");
            cmd.SetParameter("@UserSysNo", DbType.Int32, user_sysno);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationid);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        public static int CountLoginName(string loginName,string applicationID)
        {
            DataCommand cmd = new DataCommand("CountLoginName");
            cmd.SetParameter("@LoginName", DbType.String, loginName);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 更新SystemUser信息
        /// </summary>
        public static int UpdateSystemUser(SystemUser entity, string applicationID)
        {
            DataCommand cmd = new DataCommand("UpdateSystemUser");
            cmd.SetParameter<SystemUser>(entity);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            return cmd.ExecuteScalar<int>();
        }

        public static int UpdateSystemUser(SystemUser entity)
        {
            DataCommand cmd = new DataCommand("UpdateSystemUserForAuthCenter");
            cmd.SetParameter<SystemUser>(entity);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 删除SystemUser信息
        /// </summary>
        public static void DeleteSystemUser(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteSystemUser");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteSystemUserBatch(IEnumerable<int> sysNos, string applicationID, int EditUserSysNo, string EditUserName)
        {
            DataCommand cmd = new DataCommand("DeleteSystemUserBatch");
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, EditUserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, EditUserName);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteSystemUserBatch(IEnumerable<int> sysNos)
        {
            DataCommand cmd = new DataCommand("DeleteSystemUserBatchForAuthCenter");
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

     

        public static void UpdateSystemUserStatusBatch(IEnumerable<int> sysNos, CommonStatus status ,CurrentUser current)
        {
            DataCommand cmd = new DataCommand("UpdateSystemUserStatusBatch");
            cmd.SetParameter("@Status", DbType.Int32, status);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, current.UserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, current.UserDisplayName);
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询SystemUser信息
        /// </summary>
        public static QueryResult<SystemUser> QuerySystemUserList(QF_SystemUser filter)
        {
            //if (string.IsNullOrEmpty(filter.ApplicationID))
            //{
                DataCommand cmd = new DataCommand("QuerySystemUserList");
                cmd.QuerySetCondition("u.SysNo", ConditionOperation.Equal, DbType.Int32, filter.SysNo);
                if(!string.IsNullOrEmpty(filter.KeyWords))
                {
                    cmd.QuerySetCondition($"And (u.CellPhone like '%{cmd.SetSafeParameter(filter.KeyWords)}%' OR u.UserFullName like '%{cmd.SetSafeParameter(filter.KeyWords)}%' OR u.LoginName like '%{cmd.SetSafeParameter(filter.KeyWords)}%' )");
                }
                //cmd.QuerySetCondition("u.Email", ConditionOperation.Equal, DbType.AnsiString, filter.Email);
                cmd.QuerySetCondition("u.CommonStatus", ConditionOperation.Equal, DbType.Int32, filter.CommonStatus);
                if (filter.MasterSysNo.HasValue)
                {
                    cmd.QuerySetCondition("u.MasterSysNo", ConditionOperation.Equal, DbType.Int32, filter.MasterSysNo);
                }
                else if(filter.IsPlatformManager)
                {
                    cmd.QuerySetCondition("And (u.MasterSysNo is NUll Or  u.MasterSysNo=0)");
                }
                
                

                QueryResult<SystemUser> result = cmd.Query<SystemUser>(filter, " u.SysNo DESC");
                if (result.data != null && result.data.Count > 0)
                {

                    List<SystemApplication> apps = AuthDA.GetSystemApplicationsByUserSysNo(from s in result.data select s.SysNo);
                    result.data.ForEach(x =>
                    {
                        x.Applications = apps.FindAll(y => y.BizSysNo == x.SysNo);
                    });
                }

                return result;
            //}
            //else
            //{
            //    DataCommand cmd = new DataCommand("QuerySystemUserListBuyApplication");
            //    cmd.QuerySetCondition("u.SysNo", ConditionOperation.Equal, DbType.Int32, filter.SysNo);
            //    cmd.QuerySetCondition("u.LoginName", ConditionOperation.Like, DbType.AnsiString, cmd.SetSafeParameter(filter.LoginName));
            //    cmd.QuerySetCondition("u.UserFullName", ConditionOperation.Like, DbType.String, cmd.SetSafeParameter(filter.UserFullName));
            //    cmd.QuerySetCondition("u.CellPhone", ConditionOperation.Like, DbType.String, cmd.SetSafeParameter(filter.CellPhone));
            //    cmd.QuerySetCondition("u.Email", ConditionOperation.Equal, DbType.AnsiString, cmd.SetSafeParameter(filter.Email));
            //    cmd.QuerySetCondition("u.CommonStatus", ConditionOperation.Equal, DbType.Int32, filter.CommonStatus);
            //    //cmd.QuerySetCondition("u.CommonStatus", ConditionOperation.NotEqual, DbType.Int32, CommonStatus.Deleted);
            //    cmd.QuerySetCondition("ua.ApplicationID", ConditionOperation.Equal, DbType.AnsiString, cmd.SetSafeParameter(filter.ApplicationID));
            //    if (filter.MasterSysNo.HasValue)
            //    {
            //        cmd.QuerySetCondition("u.MasterSysNo", ConditionOperation.Equal, DbType.Int32, filter.MasterSysNo);
            //    }
            //    else
            //    {
            //        cmd.QuerySetCondition("And (u.MasterSysNo is NUll Or  u.MasterSysNo=0)");
            //    }
            //    QueryResult<SystemUser> result = cmd.Query<SystemUser>(filter, " u.SysNo DESC");
            //    if (result.data != null && result.data.Count > 0)
            //    {

            //        List<SystemApplication> apps = AuthDA.GetSystemApplicationsByUserSysNo(from s in result.data select s.SysNo);
            //        result.data.ForEach(x =>
            //        {
            //            x.Applications = apps.FindAll(y => y.BizSysNo == x.SysNo);
            //        });
            //    }

            //    return result;
            //}


        }

        public static List<SystemUser> QuerySystemUserListBySysNos(IEnumerable<int> sysNos, string applicationID)
        {
            DataCommand cmd = new DataCommand("QuerySystemUserListBySysNos");
            cmd.SetParameter("@ApplicationID", DbType.String,applicationID);
            cmd.CommandText = cmd.CommandText.Replace("#SysNos#", string.Join(",", sysNos));
            List<SystemUser> result = cmd.ExecuteEntityList<SystemUser>();

            if (result != null && result.Count > 0)
            {
                List<SystemApplication> apps = AuthDA.GetSystemApplicationsByUserSysNo(from s in result select s.SysNo);
                result.ForEach(x =>
                {
                    x.Applications = apps.FindAll(y => y.BizSysNo == x.SysNo);
                });
            }
            return result;
        }

        /// <summary>
        /// 获取单个SystemUser信息
        /// </summary>
        public static SystemUser LoadSystemUser(int sysNo, string applicationID)
        {
            DataCommand cmd = new DataCommand("LoadSystemUser");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);

            DataSet ds = cmd.ExecuteDataSet();

            SystemUser result;
            if (ds.Tables[0].Rows.Count > 0)
            {
                result = DataMapper.GetEntity<SystemUser>(ds.Tables[0].Rows[0]);
                result.Applications = DataMapper.GetEntityList<SystemApplication, List<SystemApplication>>(ds.Tables[1].Rows);
            }
            else
            {
                result = new SystemUser();
            }

            return result;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="newPassword"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static int ResetSystemUserPassword(string loginName, string newPassword, string applicationKey, int EditUserSysNo, string EditUserName)
        {
            DataCommand cmd = new DataCommand("ResetSystemUserPassword");
            cmd.SetParameter("@LoginName", DbType.String, loginName);
            cmd.SetParameter("@NewPassword", DbType.String, newPassword);
            cmd.SetParameter("@ApplicationKey", DbType.String, applicationKey);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, EditUserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, EditUserName);
            return cmd.ExecuteNonQuery();
        }

        public static int ResetSystemUserPasswordBatch(Dictionary<int, string> sysno_password, string applicationKey, int EditUserSysNo, string EditUserName)
        {
            DataCommand cmd = new DataCommand("ResetSystemUserPasswordBatch");

            string insertTemplate =
            @"
                UPDATE `Authcenter`.`Systemuser` as u
                SET 
                `LoginPassword` = '{0}',
                `EditUserSysNo` = {1},
                `EditUserName` = '{2}',
                `EditDate` = NOW()
                Where `SysNo` = {3} And
                exists ( 
                select 1 
                from authcenter.user_application as u_a
                where u.SysNo = u_a.UserSysNo and u_a.ApplicationID = '{4}'
                ) ;
            ";

            StringBuilder sb = new StringBuilder();
            foreach (var item in sysno_password)
            {
                sb.Append(string.Format(insertTemplate, item.Value, EditUserSysNo, EditUserName, item.Key, applicationKey));
            }
            cmd.CommandText = cmd.CommandText.Replace("#InsertSQL#", sb.ToString());

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public static SystemUser LoadSystemUserByLoginName(string loginName, string applicationID)
        {
            DataCommand cmd = new DataCommand("LoadSystemUserByLoginName");
            cmd.SetParameter("@LoginName", DbType.String, loginName);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            return cmd.ExecuteEntity<SystemUser>();
        }


        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="newPassword"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static void FindSystemUserPwd(int sysNo, string newPassword)
        {
            DataCommand cmd = new DataCommand("FindSystemUserPwd");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.SetParameter("@NewPassword", DbType.String, newPassword);

            cmd.ExecuteNonQuery();
        }

       
    }
}
