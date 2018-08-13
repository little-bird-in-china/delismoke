using System.Collections.Generic;
using System.Linq;
using BlueStone.Utility;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.DataAccess;

namespace BlueStone.BizProcessor
{
    public class ClientSmokeDetectorProcessor
    {

        /// <summary>
        /// 创建ClientSmokeDetector信息
        /// </summary>
        public static int InsertClientSmokeDetector(ClientSmokeDetector entity)
        {
            CheckClientSmokeDetector(entity,true); 
            return ClientSmokeDetectorDA.InsertClientSmokeDetector(entity);
        }



        /// <summary>
        /// 加载ClientSmokeDetector信息
        /// </summary>
        public static List<ClientSmokeDetector> LoadClientSmokeDetectorByClientSysNo(int ClientSysNo)
        {

            return ClientSmokeDetectorDA.LoadClientSmokeDetectorByClientSysNo(ClientSysNo);
        }

        public static List<ClientSmokeDetector> LoadAllBindClientUser(string SmokeDetectorCode)
        {
            return ClientSmokeDetectorDA.LoadAllBindClientUser(SmokeDetectorCode); ;
        }

        ///// <summary>
        ///// 更新ClientSmokeDetector信息
        ///// </summary>
        //public static void UpdateClientSmokeDetector(ClientSmokeDetector entity)
        //{
        //    CheckClientSmokeDetector(entity,false);
        //    ClientSmokeDetectorDA.UpdateClientSmokeDetector(entity);
        //}

        ///// <summary>
        ///// 删除ClientSmokeDetector信息
        ///// </summary>
        //public static void DeleteClientSmokeDetector(int sysNo)
        //{
        //    ClientSmokeDetectorDA.DeleteClientSmokeDetector(sysNo);
        //}

        ///// <summary>
        ///// 分页查询ClientSmokeDetector信息
        ///// </summary>
        //public static QueryResult<ClientSmokeDetector> QueryClientSmokeDetectorList(QF_ClientSmokeDetector  filter)
        //{
        //    return ClientSmokeDetectorDA.QueryClientSmokeDetectorList(filter);
        //}

        /// <summary>
        /// 检查ClientSmokeDetector信息
        /// </summary>
        private static void CheckClientSmokeDetector(ClientSmokeDetector entity,bool isCreate)
        {
            if (!isCreate && entity.SysNo==0)
            {
                 throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if(string.IsNullOrWhiteSpace(entity.SmokeDetectorCode))
            {
                throw new BusinessException(LangHelper.GetText("烟感器编码不能为空！"));
            }
            if(entity.SmokeDetectorCode.Length>32)
            {
                throw new BusinessException(LangHelper.GetText("烟感器编码长度不能超过32！"));
            }
            if(entity.CellPhone!=null&&entity.CellPhone.Length>15)
            {
                throw new BusinessException(LangHelper.GetText("手机1长度不能超过15！"));
            }
            if(entity.CellPhone2!=null&&entity.CellPhone2.Length>15)
            {
                throw new BusinessException(LangHelper.GetText("手机2长度不能超过15！"));
            }
            if(entity.CellPhone3!=null&&entity.CellPhone3.Length>15)
            {
                throw new BusinessException(LangHelper.GetText("手机3长度不能超过15！"));
            }
        }

        //public static void UpdateClientSmokeDetectorStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        //{
        //    if (sysNos == null || sysNos.Count() == 0)
        //    {
        //        throw new BusinessException("请传入要批量操作的数据编号");
        //    }
        //    ClientSmokeDetectorDA.UpdateClientSmokeDetectorStatusBatch(sysNos, status);
        //}

        //public static void DeleteClientSmokeDetectorBatch(IEnumerable<int> sysNos)
        //{
        //    if (sysNos == null || sysNos.Count() == 0)
        //    {
        //        throw new BusinessException("请传入要批量删除的数据编号");
        //    }
        //    ClientSmokeDetectorDA.DeleteClientSmokeDetectorBatch(sysNos);
        //}

    }
}