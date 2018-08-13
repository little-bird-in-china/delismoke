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
    public class AddressService
    {

        /// <summary>
        /// 创建Address信息
        /// </summary>
        public static int InsertAddress(Address entity)
        {
            CheckAddress(entity);
            return AddressDA.InsertAddress(entity);
        }

        /// <summary>
        /// 检查Address信息
        /// </summary>
        private static void CheckAddress(Address entity)
        {
            if (entity.Name != null && entity.Name.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("名称长度不能超过40个字符！"));
            }
        }


        public static void EditAddress(Address entity)
        {
            CheckAddress(entity);
            if (entity.SysNo > 0)
            {
                AddressDA.UpdateAddress(entity);
            }
            else
            {
                AddressDA.InsertAddress(entity);
            }
        }
        /// <summary>
        /// 加载Address信息
        /// </summary>
        public static Address LoadAddress(int sysNo)
        {
            return AddressDA.LoadAddress(sysNo);
        }

        /// <summary>
        /// 更新Address信息
        /// </summary>
        public static void UpdateAddress(Address entity)
        {
            CheckAddress(entity);
            AddressDA.UpdateAddress(entity);
        }

        /// <summary>
        /// 删除Address信息
        /// </summary>
        public static void DeleteAddress(int sysNo, bool isDeleteMoreNode = true)
        {
            AddressDA.DeleteAddress(sysNo, isDeleteMoreNode);
        }

        /// <summary>
        /// 分页查询Address信息
        /// </summary>
        public static QueryResult<Address> QueryAddressList(AddressFilter filter)
        {
            return AddressDA.QueryAddressList(filter);
        }
        /// <summary>
        /// 加载客户除楼和房间号地址信息
        /// </summary>
        /// <param name="companuSysNo"></param>
        /// <returns></returns>
        public static List<Address> LoadCompanyAddress(int companySysNo)
        {
            return AddressDA.LoadCompanyAddress(companySysNo);
        }

        /// <summary>
        /// 加载安装人员需安装公司
        /// </summary>
        /// <param name="installerSysNo"></param>
        /// <returns></returns>
        public static List<Company> LoadInstallerCompany(int installerSysNo)
        {
            return CompanyInstallerDA.LoadInstallerCompany(installerSysNo);
        }


        public static void InsertAddressManager(List<int> users, string code, int companySysNo)
        {
            using (ITransaction transaction = TransactionManager.Create())
            {
                AddressDA.DeleteAddressManagerByCode(code, companySysNo);
                if (users != null && users.Count > 0)
                {
                    AddressDA.InsertAddressManager(users, code, companySysNo);
                }
                transaction.Complete();
            }
        }

        public static List<AddressManager> LoadAddressManagerByCode(string code, int companySysNo)
        {
            return AddressDA.LoadAddressManagerByCode(code, companySysNo);
        }


        public static List<MapDataAddress> GetMapDataAddress(int companySysNo)
        {
            return AddressDA.GetMapDataAddress(companySysNo);
        }
        /// <summary>
        /// 根据Code获取地址树
        /// </summary>
        /// <param name="addressCode"></param>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static List<Address> LoadAddressByAddressCode(string addressCode, int companySysNo)
        {
            return AddressDA.LoadAddressByAddressCode(addressCode, companySysNo);
        }
        /// <summary>
        /// 查询地址下级
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<Address> LoadSubsetAddressByAddressSysNo(int companySysNo, int addressSysNo)
        {
            return AddressDA.LoadSubsetAddressByAddressSysNo(companySysNo, addressSysNo);
        }
    }
}