using System.Collections.Generic;
using System.Linq;
using BlueStone.Utility;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.DataAccess;
using System;
using System.Timers;

namespace BBlueStone.Smoke.Service
{
    public class SmokeDetectorDayReportServices
    {

        private static Timer BuildReportTimer = null;
        private static bool BuildReportIsRuning = false;
        /// <summary>
        /// 定时器每天22:00点执行一次生成报表。如果要单次生成报表请调用 BuildDayReport(DateTime? beginDate, DateTime? endDate) 方法。
        /// </summary>
        /// <param name="days">最近多少（除今天外）天的数据，同时会生成今天的数据，如果只生成今天的数据，则转入0</param>
        public static void BuildDayReportByTimer(int days)
        {
            BuildReportTimer = BuildReportTimer ?? new Timer(600000);
            int tdays = Math.Abs(days);
            BuildReportTimer.Elapsed += (sender, e) =>
            {
                if (DateTime.Now.Hour >= 22)
                {
                    if (BuildReportIsRuning) return;
                    BuildReportIsRuning = true;
                    BuildDayReport(DateTime.Now.AddDays(-tdays), DateTime.Now);
                    BuildReportIsRuning = false;
                }
                else if (BuildReportIsRuning && DateTime.Now.Hour >= 20)
                {
                    BuildReportIsRuning = false;
                }
            };
            BuildReportTimer.Start();
        }

        public static void BuildDayReport(DateTime? beginDate, DateTime? endDate)
        {
            if (!beginDate.HasValue)
            {
                beginDate = DateTime.Now.AddDays(-1);
            }
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }
            SmokeDetectorDayReportDA.BuildDayReport(beginDate.Value, endDate.Value);
        }


        public static CompanyDayReport QueryDayReport(QF_SmokeDayReport qF_Smoke)
        {
            var reportlist = SmokeDetectorDayReportDA.QueryDayReport(qF_Smoke);

            //var result = new List<CompanyDayReport>();
            //if(qF_Smoke.CompanySysNo.HasValue&& qF_Smoke.CompanySysNo.Value > 0)
            // {
            CompanyDayReport companyDayReport = new CompanyDayReport
            {
                TotalCount = new List<DayReport>(),
                OnlineCount = new List<DayReport>(),
                LowPowerCount = new List<DayReport>(),
                FireCount = new List<DayReport>(),
                OffLineCount=new List<DayReport>()
            };
            if (qF_Smoke.CompanySysNo.HasValue && qF_Smoke.CompanySysNo.Value > 0)
            {
                companyDayReport.CompanySysNo = qF_Smoke.CompanySysNo.Value;
            }
            reportlist.ForEach(e =>
            {
                companyDayReport.TotalCount.Add(new DayReport
                {
                    DayDate = e.DayDate,
                    Count = e.TotalCount,
                   Percent= " "
                });
                companyDayReport.OnlineCount.Add(new DayReport
                {
                    DayDate = e.DayDate,
                    Count = e.OnlineCount,
                    Percent = "(" + Math.Round((double)e.OnlineCount / e.TotalCount, 4) * 100 + "%)"
                });
                companyDayReport.LowPowerCount.Add(new DayReport
                {
                    DayDate = e.DayDate,
                    Count = e.LowPowerCount,
                    Percent =  "(" + Math.Round((double)e.LowPowerCount / e.TotalCount, 4) * 100 + "%)"
                });
                companyDayReport.FireCount.Add(new DayReport
                {
                    DayDate = e.DayDate,
                    Count = e.FireCount,
                    Percent = "(" + Math.Round((double)e.FireCount / e.TotalCount, 4) * 100 + "%)"
                });
                companyDayReport.OffLineCount.Add(new DayReport
                {
                    DayDate = e.DayDate,
                    Count = (e.TotalCount - e.OnlineCount),
                    Percent =  "(" + Math.Round((double)(e.TotalCount - e.OnlineCount) / e.TotalCount, 4) * 100 + "%)"
                });
            });
            // }
            return companyDayReport;
        }


        ///// <summary>
        ///// 创建SmokeDetectorDayReport信息
        ///// </summary>
        //public static int InsertSmokeDetectorDayReport(SmokeDetectorDayReport entity)
        //{
        //    CheckSmokeDetectorDayReport(entity,true); 
        //    return SmokeDetectorDayReportDA.InsertSmokeDetectorDayReport(entity);
        //}

        ///// <summary>
        ///// 加载SmokeDetectorDayReport信息
        ///// </summary>
        //public static SmokeDetectorDayReport LoadSmokeDetectorDayReport(int sysNo)
        //{
        //    return SmokeDetectorDayReportDA.LoadSmokeDetectorDayReport(sysNo);
        //}

        ///// <summary>
        ///// 更新SmokeDetectorDayReport信息
        ///// </summary>
        //public static void UpdateSmokeDetectorDayReport(SmokeDetectorDayReport entity)
        //{
        //    CheckSmokeDetectorDayReport(entity,false);
        //    SmokeDetectorDayReportDA.UpdateSmokeDetectorDayReport(entity);
        //}

        ///// <summary>
        ///// 删除SmokeDetectorDayReport信息
        ///// </summary>
        //public static void DeleteSmokeDetectorDayReport(int sysNo)
        //{
        //    SmokeDetectorDayReportDA.DeleteSmokeDetectorDayReport(sysNo);
        //}

        ///// <summary>
        ///// 分页查询SmokeDetectorDayReport信息
        ///// </summary>
        //public static QueryResult<SmokeDetectorDayReport> QuerySmokeDetectorDayReportList(QF_SmokeDetectorDayReport  filter)
        //{
        //    return SmokeDetectorDayReportDA.QuerySmokeDetectorDayReportList(filter);
        //}

        ///// <summary>
        ///// 检查SmokeDetectorDayReport信息
        ///// </summary>
        //private static void CheckSmokeDetectorDayReport(SmokeDetectorDayReport entity,bool isCreate)
        //{
        //    if (!isCreate && entity.SysNo==0)
        //    {
        //         throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
        //    }
        //    if(string.IsNullOrWhiteSpace(entity.DayDate))
        //    {
        //        throw new BusinessException(LangHelper.GetText("统计时间不能为空！"));
        //    }
        //    if(entity.DayDate.Length>0)
        //    {
        //        throw new BusinessException(LangHelper.GetText("统计时间长度不能超过0！"));
        //    }
        //}

        //public static void UpdateSmokeDetectorDayReportStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        //{
        //    if (sysNos == null || sysNos.Count() == 0)
        //    {
        //        throw new BusinessException("请传入要批量操作的数据编号");
        //    }
        //    SmokeDetectorDayReportDA.UpdateSmokeDetectorDayReportStatusBatch(sysNos, status);
        //}

        //public static void DeleteSmokeDetectorDayReportBatch(IEnumerable<int> sysNos)
        //{
        //    if (sysNos == null || sysNos.Count() == 0)
        //    {
        //        throw new BusinessException("请传入要批量删除的数据编号");
        //    }
        //    SmokeDetectorDayReportDA.DeleteSmokeDetectorDayReportBatch(sysNos);
        //}

    }
}