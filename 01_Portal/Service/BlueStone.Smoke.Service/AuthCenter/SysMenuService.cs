using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Service
{
    public class SysMenuService
    {
        private static readonly object codeLock = new object();

        /// <summary>
        /// 创建SysMenu信息
        /// </summary>
        public int InsertSysMenu(SysMenu entity)
        {
            CheckSysMenu(entity, true);
            int result = 0;
            lock (codeLock)
            {
                CreateCode(entity);
                result = SysMenuDA.InsertSysMenu(entity);
            }
            return result;
        }

        /// <summary>
        /// 系统生成编码每级3位
        /// </summary>
        /// <param name="entity"></param>
        private void CreateCode(SysMenu entity)
        {
            string BrotherCode = string.Empty;
            string ParentCode = string.Empty;
            SysMenuDA.GetBuildSysCode(entity.ParentSysNo, out ParentCode, out BrotherCode, entity.ApplicationID);
            if (string.IsNullOrWhiteSpace(ParentCode) && string.IsNullOrWhiteSpace(BrotherCode))
            {
                entity.SysCode = "001";
            }
            else if (string.IsNullOrWhiteSpace(ParentCode) && !string.IsNullOrWhiteSpace(BrotherCode))
            {
                entity.SysCode = (int.Parse(BrotherCode) + 1).ToString().PadLeft(3, '0');
            }
            else if (!string.IsNullOrWhiteSpace(ParentCode) && string.IsNullOrWhiteSpace(BrotherCode))
            {
                entity.SysCode = ParentCode + "001";
            }
            else if (!string.IsNullOrWhiteSpace(ParentCode) && !string.IsNullOrWhiteSpace(BrotherCode))
            {
                entity.SysCode = ParentCode
                + (int.Parse(BrotherCode.Substring(BrotherCode.Length - 3)) + 1).ToString().PadLeft(3, '0');
            }
        }

        public List<SysMenu> DisposableLoadMenus(int parentsysno)
        {
            return SysMenuDA.DisposableLoadMenus(parentsysno);
        }


        /// <summary>
        /// 更新SysMenu信息
        /// </summary>
        public void UpdateSysMenu(SysMenu entity)
        {
            CheckSysMenu(entity, false);
            SysMenuDA.UpdateSysMenu(entity);
        }

        public List<SysMenu> DynamicLoadMenus(int parentsysno,string ApplicationID)
        {
            return SysMenuDA.DynamicLoadMenus(parentsysno, ApplicationID);
        }

        /// <summary>
        /// 删除SysMenu信息
        /// </summary>
        public void DeleteSysMenu(int sysNo)
        {
            if (SysMenuDA.CountMenusChildrens(sysNo) > 0)
            {
                throw new BusinessException("不能直接删除拥有子节点的菜单,请先删除其子节点");
            }
            SysMenuDA.DeleteSysMenu(sysNo);
        }

        /// <summary>
        /// 分页查询SysMenu信息
        /// </summary>
        public QueryResult<SysMenu> QuerySysMenuList(QF_SysMenu filter)
        {
            return SysMenuDA.QuerySysMenuList(filter);
        }



        /// <summary>
        /// 加载SysMenu信息
        /// </summary>
        public SysMenu LoadSysMenu(int sysNo)
        {
            return SysMenuDA.LoadSysMenu(sysNo);
        }



        /// <summary>
        /// 检查SysMenu信息
        /// </summary>
        private void CheckSysMenu(SysMenu entity, bool isCreate)
        {
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            
            if (string.IsNullOrWhiteSpace(entity.MenuName))
            {
                throw new BusinessException(LangHelper.GetText("菜单名称不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.MenuName))
            {
                if (entity.MenuName.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("菜单名称长度不能超过50！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.IconStyle))
            {
                if (entity.IconStyle.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("图标样式长度不能超过50！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.LinkPath))
            {
                if (entity.LinkPath.Length > 500)
                {
                    throw new BusinessException(LangHelper.GetText("链接地址长度不能超过500！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.Memo))
            {
                if (entity.Memo.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("备注长度不能超过200！"));
                }
            }
            if (isCreate && string.IsNullOrWhiteSpace(entity.ApplicationID))
            {
                throw new BusinessException(LangHelper.GetText("不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.ApplicationID))
            {
                if (entity.ApplicationID.Length > 40)
                {
                    throw new BusinessException(LangHelper.GetText("长度不能超过40！"));
                }
            }

        }

        public void SaveMenusPermission(int menuSysNo , List<SysPermission> list)
        {
            if (menuSysNo == 0)
            {
                throw new BusinessException("请传入菜单编号!");
            }
            //if (permissionSysNo == 0) {
            //    throw new BusinessException("请传入权限ID!");
            //}
            SysPermissionDA.SaveMenusPermission(menuSysNo, list);
        }

        public static List<SysPermission> LoadAllSysPermissionsByMenuSysNo(int menuSysNo)
        {
            if (menuSysNo == 0)
            {
                throw new BusinessException("请传入菜单编号!");
            }
            return SysPermissionDA.LoadAllSysPermissionsByMenuSysNo(menuSysNo);
        }

        public static List<SysFunction> LoadAllFunctionsWithPermission(string appkey)
        {
            if (string.IsNullOrWhiteSpace(appkey))
            {
                throw new BusinessException("请传入ApplicationID！");
            }
            return SysFunctionDA.LoadAllFunctionsWithPermission(appkey);
        }

        public static List<SysMenu> LoadAllMenusWithPermission(string appkey)
        {
            if (string.IsNullOrWhiteSpace(appkey))
            {
                throw new BusinessException("请传入ApplicationID！");
            }
            return SysMenuDA.LoadAllMenusWithPermission(appkey);
        }

        public void DeleteMenusPermission(int sysno)
        {
            SysMenuDA.DeleteMenusPermission(sysno);
        }
    }
}
