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
    public class AddressDA
    {
        /// <summary>
        /// 创建Address信息
        /// </summary>
        public static int InsertAddress(Address entity)
        {
            DataCommand cmd = new DataCommand("InsertAddress");
            cmd.SetParameter<Address>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 创建Address信息
        /// </summary>
        public static void InsertAddress(int companySysNo, int parentSysNo, ref List<Address> addList, int userSysNo, string userName)
        {
            DataCommand cmd = new DataCommand("InsertAddress");

            StringBuilder sqlBuilder = new StringBuilder(@"
drop TEMPORARY TABLE if exists AddressTB;
CREATE TEMPORARY TABLE if not exists AddressTB (
  `Code` varchar(32)   primary key,
  `Name` varchar(40)  NULL,
  `PathName` varchar(100)  NULL,
  `Grade` int(11) NULL,
  `Number` varchar(20)  NULL
); 

INSERT INTO AddressTB(`Code`,   `Name`, `PathName`, `Grade`, `Number`)
values ");

            for (int i = 0; i < addList.Count; i++)
            {
                var add = addList[i];
                if (i > 0) sqlBuilder.Append(",");
                sqlBuilder.AppendFormat($"('{add.Code}','{cmd.SetSafeParameter(add.Name)}','{cmd.SetSafeParameter(add.PathName)}',{(int)add.Grade},'{add.Number}')");
            }
            sqlBuilder.Append($@";
update  smoke.Address o   inner join AddressTB n on o.CompanySysNo=@CompanySysNo and o.Number=n.Number AND o.ParentSysNo=@ParentSysNo 
set o.Name=n.Name,
o.`PathName`=n.`PathName`;

INSERT INTO smoke.Address
                ( `CompanySysNo`,
                `Code`,
                `ParentSysNo`,
                `Name`,
                `PathName`,
                AreaSysNo,
                `Grade`,
                `Number`,
                EditUserSysNo,
                EditUserName,
                EditDate)  
select @CompanySysNo,
                n.`Code`,
                @ParentSysNo,
                n.`Name`,
                n.`PathName`,
                @AreaSysNo,
                n.`Grade`,
                n.`Number`,
                @EditUserSysNo,
                @EditUserName,
                now(3)  from AddressTB n left join smoke.Address o on o.CompanySysNo=@CompanySysNo and o.Number=n.Number AND o.ParentSysNo=@ParentSysNo where o.SysNo is Null;
select p.SysNo,p.Code,p.Number,Max(c.Code) EditUserName  -- 子节点最大的编码暂存在 EditUserName
from (select SysNo,Code,Number  from smoke.Address where CompanySysNo=@CompanySysNo and ParentSysNo=@ParentSysNo) p
LEFT join smoke.Address  c on c.ParentSysNo=p.SysNo group by p.SysNo;");

            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentSysNo);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, userSysNo);
            cmd.SetParameter("@EditUserName", DbType.AnsiString, userName);
            cmd.CommandText = sqlBuilder.ToString();
            var result = cmd.ExecuteEntityList<Address>();

            foreach (var add in addList)
            {
                var newAdd = result.Find(t => add.Number.Equals(t.Number, StringComparison.CurrentCultureIgnoreCase));
                if (newAdd != null)
                {
                    add.SysNo = newAdd.SysNo;
                    add.CompanySysNo = companySysNo;
                    add.ParentSysNo = parentSysNo;
                    add.Code = newAdd.Code;
                    add.EditUserName = newAdd.EditUserName; //子节点最大的编码暂存在 EditUserName
                }
            }

        }
        /// <summary>
        /// 更新Address信息
        /// </summary>
        public static void UpdateAddress(Address entity)
        {
            DataCommand cmd = new DataCommand("UpdateAddress");
            cmd.SetParameter<Address>(entity);
            cmd.ExecuteNonQuery();
        }

        public static void CreateRootAddress(int companySysNo)
        {
            DataCommand cmd = new DataCommand("CreateRootAddress");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个Address信息
        /// </summary>
        public static Address LoadAddress(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadAddress");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            Address result = cmd.ExecuteEntity<Address>();
            return result;
        }

        public static string GetChildrenMaxCode(int companySysNo, int parentSysNo)
        {
            DataCommand cmd = new DataCommand("AddressGetChildrenMaxCode");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentSysNo);
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            string result = cmd.ExecuteScalar<string>();
            return result;
        }


        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="sysNo">要删除的节点编号</param>
        /// <param name="isDeleteMoreNode">是否同时删除同级节点中比此节点以后的节点</param>
        public static bool DeleteAddress(int sysNo, bool isDeleteMoreNode = true)
        {
            DataCommand cmd = new DataCommand("DeleteAddress");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.SetParameter("@IsDeleteMore", DbType.Boolean, @isDeleteMoreNode);
            return cmd.ExecuteNonQuery() > 0;
        }



        /// <summary>
        /// 分页查询Address信息
        /// </summary>
        public static QueryResult<Address> QueryAddressList(AddressFilter filter)
        {
            DataCommand cmd = new DataCommand("QueryAddressList");
            cmd.QuerySetCondition("CompanySysNo", ConditionOperation.Equal, DbType.Int32, filter.CompanySysNo);
            if (filter.ParentSysNo.HasValue && filter.ParentSysNo.Value > 0)
            {
                cmd.QuerySetCondition("ParentSysNo", ConditionOperation.Equal, DbType.Int32, filter.ParentSysNo);
            }
            else
            {
                cmd.QuerySetCondition("Grade", ConditionOperation.LessThanEqual, DbType.Int32, (int)filter.AddressGrade.Value);
            }

            if (filter.SelectRoot.HasValue && filter.SelectRoot.Value)
            {
                cmd.QuerySetCondition("AND ParentSysNo IS NULL");
            }

            QueryResult<Address> result = cmd.Query<Address>(filter, "Code ASC");
            return result;
        }


        /// <summary>
        /// 加载客户除楼和房间号地址信息
        /// </summary>
        /// <param name="companuSysNo"></param>
        /// <returns></returns>
        public static List<Address> LoadCompanyAddress(int companySysNo)
        {
            DataCommand cmd = new DataCommand("LoadCompanyAddress");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            List<Address> list = cmd.ExecuteEntityList<Address>();
            return list;
        }

        /// <summary>
        /// 查询地址下级
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<Address> LoadSubsetAddressByAddressSysNo(int companySysNo, int addressSysNo)
        {
            DataCommand cmd = new DataCommand("LoadSubsetAddressByAddressSysNo");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.SetParameter("@AddressSysNo", DbType.Int32, addressSysNo);
            return cmd.ExecuteEntityList<Address>();
        }




        public static List<Address> GetAddressBySysNos(List<int> SysNos)
        {
            DataCommand cmd = new DataCommand("GetAddressBySysNos");
            cmd.CommandText = cmd.CommandText.Replace("#SysNos", string.Join(",", SysNos));
            var result = cmd.ExecuteEntityList<Address>();
            return result;
        }

        public static List<Address> GetTopLevelChildren(int sysNo)
        {
            DataCommand cmd = new DataCommand("GetTopLevelChildren");
            cmd.SetParameter("SysNo", DbType.Int32, sysNo);
            var result = cmd.ExecuteEntityList<Address>();
            return result;

        }

        public static void InsertAddressManager(List<int> users, string code, int companySysNo)
        {
            DataCommand cmd = new DataCommand("InsertAddressManager");
            string s = cmd.CommandText;
            StringBuilder builder = new StringBuilder();
            foreach (var item in users)
            {
                string b = s.Replace("@ManagerSysNo", item.ToString());
                b = b.Replace("@AddressCode", "'" + code.ToString() + "'");
                b = b.Replace("@CompanySysNo", companySysNo.ToString());
                builder.Append(b);
            };
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();
        }

        public static List<AddressManager> LoadAddressManagerByCode(string addressCode, int companySysNo)
        {
            DataCommand cmd = new DataCommand("LoadAddressManagerByCode");
            cmd.SetParameter("@AddressCode", DbType.String, addressCode);
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            return cmd.ExecuteEntityList<AddressManager>();
        }


        public static void DeleteAddressManagerByCode(string addressCode, int companySysNo)
        {
            DataCommand cmd = new DataCommand("DeleteAddressManagerByCode");
            cmd.SetParameter("@AddressCode", DbType.String, addressCode);
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.ExecuteNonQuery();
        }


        public static List<MapDataAddress> GetMapDataAddress(int companySysNo)
        {
            //非离线状态 且 非空状态的都是在线状态
            var offlineStatus = new List<int> { (int)SmokeDetectorStatus.OutNet, (int)SmokeDetectorStatus.Offline, (int)SmokeDetectorStatus.Lost, };
            var offlineStatusStr = string.Join(",", offlineStatus);

            var warnStatus = new List<int> { (int)SmokeDetectorStatus.Warning, (int)SmokeDetectorStatus.TestWarning, };

            DataCommand cmd = new DataCommand("GetMapDataAddress");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.CommandText = cmd.CommandText.Replace("#OfflineStatus", offlineStatusStr);
            cmd.CommandText = cmd.CommandText.Replace("#WarnStatus", string.Join(",", warnStatus));
            return cmd.ExecuteEntityList<MapDataAddress>();
        }

        /// <summary>
        /// 根据Code获取地址树
        /// </summary>
        /// <param name="addressCode"></param>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static List<Address> LoadAddressByAddressCode(string addressCode, int companySysNo)
        {
            DataCommand cmd = new DataCommand("LoadAddressByAddressCode");
            cmd.SetParameter("@AddressCode", DbType.String, addressCode);
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            return cmd.ExecuteEntityList<Address>();
        }





    }
}