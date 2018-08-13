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
    public class SmokeDetectorDayReportDA
    {

        public static void BuildDayReport(DateTime startdate, DateTime enddate)
        {
            DataCommand cmd = new DataCommand("BuildDayReport");
            cmd.SetParameter("@startdate",DbType.Date,startdate);
            cmd.SetParameter("@enddate", DbType.Date, enddate);
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// 创建SmokeDetectorDayReport信息
        /// </summary>
        public static int InsertSmokeDetectorDayReport(SmokeDetectorDayReport entity)
        {
            DataCommand cmd = new DataCommand("InsertSmokeDetectorDayReport");
            cmd.SetParameter<SmokeDetectorDayReport>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        public static List<SmokeDetectorDayReport> QueryDayReport(QF_SmokeDayReport qF_Smoke)
        {
            if (qF_Smoke.CompanySysNo.HasValue && qF_Smoke.CompanySysNo.Value > 0)
            {
                DataCommand cmd = new DataCommand("QueryCompanyDayReport");
                cmd.SetParameter("@StartDayDate", DbType.Date, qF_Smoke.StartDayDate);
                cmd.SetParameter("@EndDayDate", DbType.Date, qF_Smoke.EndDayDate);
                cmd.SetParameter("@CompanySysNo", DbType.Int32, qF_Smoke.CompanySysNo.Value);
                var result = cmd.ExecuteEntityList<SmokeDetectorDayReport>();
                return result;
            }
            else
            {
                DataCommand cmd = new DataCommand("QueryDayReport");
                cmd.SetParameter("@StartDayDate", DbType.Date, qF_Smoke.StartDayDate);
                cmd.SetParameter("@EndDayDate", DbType.Date, qF_Smoke.EndDayDate);
                var result = cmd.ExecuteEntityList<SmokeDetectorDayReport>();
                return result;
            }
        }

        /// <summary>
        /// 更新SmokeDetectorDayReport信息
        /// </summary>
        public static void UpdateSmokeDetectorDayReport(SmokeDetectorDayReport entity)
        {
            DataCommand cmd = new DataCommand("UpdateSmokeDetectorDayReport");
            cmd.SetParameter<SmokeDetectorDayReport>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个SmokeDetectorDayReport信息
        /// </summary>
        public static SmokeDetectorDayReport LoadSmokeDetectorDayReport(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetectorDayReport");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            SmokeDetectorDayReport result = cmd.ExecuteEntity<SmokeDetectorDayReport>();
            return result;
        }

        /// <summary>
        /// 删除SmokeDetectorDayReport信息
        /// </summary>
        public static void DeleteSmokeDetectorDayReport(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteSmokeDetectorDayReport");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

    }
}