using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;  
using System.Text.RegularExpressions;
using BlueStone.Utility;
using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;

namespace BlueStone.Smoke.Service
{
    public class ClientService
    {

        /// <summary>
        /// 创建Client信息
        /// </summary>
        public static int InsertClient(Client entity)
        {
            CheckClient(entity,true);
            return ClientDA.InsertClient(entity);
        }

        /// <summary>
        /// 加载Client信息
        /// </summary>
        public static Client LoadClient(int sysNo)
        {
            return ClientDA.LoadClient(sysNo);
        }
        public static Client LoadClientByAppCustomerID(string appCustomerID)
        {
            return ClientDA.LoadClientByAppCustomerID(appCustomerID);
        }
        /// <summary>
        /// 更新Client信息
        /// </summary>
        public static void UpdateClient(Client entity)
        {
            CheckClient(entity,false);
            ClientDA.UpdateClient(entity);
        }

        /// <summary>
        /// 删除Client信息
        /// </summary>
        public static void DeleteClient(int sysNo)
        {
            ClientDA.DeleteClient(sysNo);
        }

        /// <summary>
        /// 分页查询Client信息
        /// </summary>
        public static QueryResult<Client> QueryClientList(ClientFilter filter)
        {
            return ClientDA.QueryClientList(filter);
        }
        public static QueryResult<Client> QueryAllClientList(ClientFilter filter)
        {
            return ClientDA.QueryAllClientList(filter);
        }

        /// <summary>
        /// 检查Client信息
        /// </summary>
        private static void CheckClient(Client entity,bool isCreate)
        {
            if (!isCreate && entity.SysNo==0)
            {
                 throw new BusinessException(LangHelper.GetText("记录不存在，请刷新页面重试！"));
            }
            if(string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new BusinessException(LangHelper.GetText("用户名称不能为空！"));
            }
            if(entity.Name.Length>15)
            {
                throw new BusinessException(LangHelper.GetText("用户名称长度不能超过15！"));
            }
            if(entity.HeaderImage!=null&&entity.HeaderImage.Length>500)
            {
                throw new BusinessException(LangHelper.GetText("头像地址长度不能超过500！"));
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

        public static void UpdateClientStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的数据编号");
            }
            ClientDA.UpdateClientStatusBatch(sysNos, status);
        }

        public static void DeleteClientBatch(IEnumerable<int> sysNos)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量删除的数据编号");
            }
            ClientDA.DeleteClientBatch(sysNos);
        }


        /// <summary>
        /// 更新ClientContact信息
        /// </summary>
        public static void UpdateClientContact(Client entity)
        {
            ClientDA.UpdateClientContact(entity);
        }

    }
}