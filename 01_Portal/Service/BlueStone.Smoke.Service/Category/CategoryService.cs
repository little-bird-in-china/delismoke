using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Service
{
    public class CategoryService
    {
        /// <summary>
        /// 获取类别基础信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static Category LoadCategory(int sysNo)
        {
            return CategoryDA.LoadCategory(sysNo);
        }

        /// <summary>
        /// 更新类别
        /// </summary>
        /// <param name="entity"></param>
        public static void UpdateCategory(Category entity)
        {
            CategoryDA.UpdateCategory(entity);
        }
        /// <summary>
        /// 新增类别
        /// </summary>
        /// <param name="entity"></param>
        public static int InserCategory(Category entity)
        {
            if (string.IsNullOrEmpty(entity.ParentCategoryCode) || string.IsNullOrEmpty(entity.ParentCategoryCode.Trim()))
            {
                return InsertRootCategory(entity);
            }
            else
            {
                return InsertChildCategory(entity);
            }
        }

        /// <summary>
        /// 创建根Category
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="channelCategoryList"></param>
        public static int InsertRootCategory(Category entity)
        {
            CheckRootCategory(entity, true);
            entity.JianPin = BlueStone.Utility.PinYinHelper.GetFirstPinYin(entity.Name);
            entity.ParentCategoryCode = "";
            int sysno = CategoryDA.InsertRootCategory(entity);
            return sysno;
        }
        /// <summary>
        /// 创建子节点Category
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="channelCategoryList"></param>
        public static int InsertChildCategory(Category entity)
        {
            CheckChildCategory(entity, true);
            Category parentCategory = CategoryDA.LoadCategoryByCode(entity.ParentCategoryCode);
            if (parentCategory == null)
            {
                throw new BusinessException(LangHelper.GetText("父节点不存在！"));
            }
            if (parentCategory.CommonStatus != CommonStatus.Actived)
            {
                throw new BusinessException(LangHelper.GetText("父节点状态不是有效状态不能添加！"));
            }
            entity.JianPin = " ";
            int sysNo = CategoryDA.InsertChildCategory(entity);
            CategoryDA.UpdateCategoryIsLeaf(entity.ParentCategoryCode, CommonYesOrNo.No);
            return sysNo;
        }
        /// <summary>
        /// 检查子节点
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isCreate"></param>
        private static void CheckChildCategory(Category entity, bool isCreate)
        {
            if (string.IsNullOrEmpty(entity.ParentCategoryCode))
            {
                throw new BusinessException(LangHelper.GetText("父编码不能为空！"));
            }
            else if (entity.ParentCategoryCode.Trim().Length > 10)
            {
                throw new BusinessException(LangHelper.GetText("父编码长度不能超过10！"));
            }
            CheckRootCategory(entity, isCreate);
        }
        /// <summary>
        /// 检查根Category
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isCreate"></param>
        private static void CheckRootCategory(Category entity, bool isCreate)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new BusinessException(LangHelper.GetText("类别名称不能为空！"));
            }
            else if (entity.Name.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("类别名称长度不能超过40！"));
            }

            if (!string.IsNullOrWhiteSpace(entity.Memo) && entity.Memo.Length > 200)
            {
                throw new BusinessException(LangHelper.GetText("备注长度不能超过200！"));
            }

        }

        /// <summary>
        /// 删除类别
        /// </summary>
        public static void DeleteCategory(int sysNo)
        {
            var currentCategory = CategoryDA.LoadCategory(sysNo);
            var categorys = CategoryDA.GetCategoryList();

            CategoryDA.DeleteCategory(sysNo);

            //如果父节点下无子节点 则将父节点重置为叶子节点
            if (!string.IsNullOrEmpty(currentCategory.ParentCategoryCode))
            {
                var parentChildrens = categorys.Where(a => a.ParentCategoryCode == currentCategory.ParentCategoryCode && a.CommonStatus == CommonStatus.Actived);
                if (parentChildrens != null && parentChildrens.Count() == 1)
                {
                    CategoryDA.UpdateCategoryIsLeaf(currentCategory.ParentCategoryCode, CommonYesOrNo.Yes);
                }
            }
        }
        /// <summary> 
        /// 获取Category list信息
        /// </summary>
        public static List<Category> GetCategoryList()
        {
            return CategoryDA.GetCategoryList();

        }

        public static List<Category> GetCategoryActivedList()
        {
            return CategoryDA.GetCategoryActivedList();
        }

            
    }
}
