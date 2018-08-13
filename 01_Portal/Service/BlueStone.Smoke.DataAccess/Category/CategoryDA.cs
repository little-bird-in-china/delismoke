using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace BlueStone.Smoke.DataAccess
{

    public class CategoryDA
    {
        /// <summary>
        /// 获取Category list信息
        /// </summary>
        public static List<Category> GetCategoryList()
        {
            DataCommand cmd = new DataCommand("GetCategoryList");

            return cmd.ExecuteEntityList<Category>();
        }
        /// <summary>
        /// 新建根Category信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int InsertRootCategory(Category entity)
        {
            DataCommand cmd0 = new DataCommand("GenerateParentCategory");
            cmd0.SetParameter("@ParentCategory", DbType.String, entity.ParentCategoryCode);
            DataTable dt = cmd0.ExecuteDataTable();
            entity.CategoryCode = dt.Rows[0][0].ToString();


            DataCommand cmd = new DataCommand("InsertRootCategory");
            cmd.SetParameter<Category>(entity);
            int result = cmd.ExecuteScalar<int>();
            entity.SysNo = result;
            return result;
        }

        /// <summary>
        /// 新建Child Category信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int InsertChildCategory(Category entity)
        {
            DataCommand cmd0 = new DataCommand("GenerateChildCategory");
            cmd0.SetParameter("@ParentCategoryCode", DbType.AnsiStringFixedLength, entity.ParentCategoryCode);

            DataTable dt = cmd0.ExecuteDataTable();
            entity.CategoryCode = dt.Rows[0][0].ToString();

            DataCommand cmd = new DataCommand("InsertChildCategory");
            cmd.SetParameter<Category>(entity);
            int result = cmd.ExecuteScalar<int>();
            entity.SysNo = result;
            return result;
        }

        /// <summary>
        /// 更新Category信息
        /// </summary>
        public static void UpdateCategory(Category entity)
        {
            DataCommand cmd = new DataCommand("UpdateCategory");
            cmd.SetParameter<Category>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除Category信息
        /// </summary>
        public static void DeleteCategory(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteCategory");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取有效的类别列表
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCategoryActivedList()
        {
            DataCommand cmd = new DataCommand("GetCategoryActivedList");
            return cmd.ExecuteEntityList<Category>();
        }

        /// <summary>
        /// 获取单个Category信息
        /// </summary>
        public static Category LoadCategory(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadCategory");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            Category result = cmd.ExecuteEntity<Category>();
            return result;
        }

        /// <summary>
        /// 根据类别编号加载Category信息
        /// </summary>
        /// <param name="categoryCode"></param>
        /// <returns></returns>
        public static Category LoadCategoryByCode(string categoryCode)
        {
            DataCommand cmd = new DataCommand("LoadCategoryByCode");
            cmd.SetParameter("@CategoryCode", DbType.StringFixedLength, categoryCode);
            Category result = cmd.ExecuteEntity<Category>();
            return result;
        }




        /// <summary>
        /// 修改子节点状态
        /// </summary>
        /// <param name="parentCategoryCode"></param>
        /// <param name="status"></param>
        public static void UpdateChildStatus(string parentCategoryCode, string tenantID, CommonStatus status)
        {
            DataCommand cmd = new DataCommand("UpdateChildStatus");
            cmd.SetParameter("@CategoryCode", DbType.StringFixedLength, parentCategoryCode);
            cmd.SetParameter("@CommonStatus", DbType.Int32, status);
            cmd.SetParameter("@TenantID", DbType.String, tenantID);
            cmd.ExecuteNonQuery();
        }


        public static List<Category> GetCategoryListBySysNos(List<int> sysnos, string tenantID)
        {
            DataCommand cmd = new DataCommand("GetCategoryListBySysNos");
            cmd.QuerySetCondition("SysNo", ConditionOperation.In, DbType.Int32, sysnos);
            cmd.QuerySetCondition("TenantID", ConditionOperation.Equal, DbType.String, tenantID);
            return cmd.ExecuteEntityList<Category>("#DynamicParameters#");
        }

        public static void UpdateCategoryIsLeaf(string categoryCode, CommonYesOrNo isLeaf)
        {

            DataCommand cmd = new DataCommand("UpdateCategoryIsLeaf");
            cmd.SetParameter("@CategoryCode", DbType.String, categoryCode);
            cmd.SetParameter("@IsLeaf", DbType.Int32, isLeaf);
            cmd.ExecuteNonQuery();
        }


    }
}
