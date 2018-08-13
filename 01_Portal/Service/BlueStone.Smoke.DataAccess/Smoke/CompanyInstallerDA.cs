using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;

namespace BlueStone.Smoke.DataAccess
{
    public class CompanyInstallerDA
    {
        /// <summary>
        /// 加载安装人员需安装公司
        /// </summary>
        /// <param name="installerSysNo"></param>
        /// <returns></returns>
        public static List<Company> LoadInstallerCompany(int installerSysNo)
        {
            DataCommand cmd = new DataCommand("LoadInstallerCompany");
            cmd.SetParameter("@InstallerSysNo", DbType.Int32, installerSysNo);
            List<Company> list = cmd.ExecuteEntityList<Company>();
            return list;

        }



        /// <summary>
        /// 分页查询CompanyInstaller信息
        /// </summary>
        public static QueryResult<CompanyInstaller> QueryCompanyInstallerList(QF_CompanyInstaller filter)
        {
            DataCommand cmd = new DataCommand("QueryCompanyInstallerList");
            cmd.QuerySetCondition("CompanySysNo", ConditionOperation.Equal, DbType.Int32, filter.CompanySysNo);

            QueryResult<CompanyInstaller> result = cmd.Query<CompanyInstaller>(filter, " InstallerSysNo DESC");
            return result;
        }


        public static void UpdateCompanyInstallerBatch(List<int> companySysNos,List<int> installerSysNos) {
            DataCommand cmd = new DataCommand("UpdateCompanyInstallerBatch");

            var sqlAdd = new StringBuilder();
            sqlAdd.Append($"DELETE FROM smoke.CompanyInstaller WHERE CompanySysNo IN({string.Join(",", companySysNos)});");
            companySysNos.ForEach(companySysNo =>
            {
                installerSysNos.ForEach(installerSysNo =>
                {
                    sqlAdd.Append($" INSERT INTO smoke.CompanyInstaller(CompanySysNo,InstallerSysNo,InTime) values({companySysNo},{installerSysNo},now());");
                });
            });

            cmd.CommandText =sqlAdd.ToString();
            cmd.ExecuteNonQuery();
        }


    }
}