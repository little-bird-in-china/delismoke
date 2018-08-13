using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using System.Collections.Generic;
using BlueStone.Smoke.Entity;

namespace BlueStone.Smoke.Service
{
    public class SysPermissionService
    {
        /// <summary>
        /// 创建SysPermission信息
        /// </summary>
        public int InsertSysPermission(SysPermission entity)
        {
            CheckSysPermission(entity, true);
            MenuPermission menuPermission = new MenuPermission()
            {
                MenuSysNo = entity.MenuSysNo,
                PermissionSysNo = entity.SysNo
            };
            MenuPermissionDA.InsertMenuPermission(menuPermission);
            return SysPermissionDA.InsertSysPermission(entity);
        }

        /// <summary>
        /// 更新SysPermission信息
        /// </summary>
        public void UpdateSysPermission(SysPermission entity)
        {
            CheckSysPermission(entity, false);
            MenuPermission menuPermission = new MenuPermission()
            {
                MenuSysNo = entity.MenuSysNo,
                PermissionSysNo = entity.SysNo
            };
            MenuPermissionDA.UpdateMenuPermission(menuPermission);
            SysPermissionDA.UpdateSysPermission(entity);
        }

        /// <summary>
        /// 删除SysPermission信息
        /// </summary>
        public void DeleteSysPermission(int sysNo)
        {
            SysPermissionDA.DeleteSysPermission(sysNo);
        }

        /// <summary>
        /// 分页查询SysPermission信息
        /// </summary>
        public QueryResult<SysPermission> QuerySysPermissionList(QF_SysPermission filter)
        {
            var result = SysPermissionDA.QuerySysPermissionList(filter);

            return result;
        }

        /// <summary>
        /// 加载SysPermission信息
        /// </summary>
        public SysPermission LoadSysPermission(int sysNo)
        {
            return SysPermissionDA.LoadSysPermission(sysNo);
        }

        /// <summary>
        /// 检查SysPermission信息
        /// </summary>
        private void CheckSysPermission(SysPermission entity, bool isCreate)
        {
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if (isCreate && entity.FunctionSysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("没有所属功能编号！"));
            }
            if (isCreate && string.IsNullOrWhiteSpace(entity.FunctionSysCode))
            {
                throw new BusinessException(LangHelper.GetText("没有所属功能编码！"));
            }
            if (isCreate && !string.IsNullOrWhiteSpace(entity.FunctionSysCode))
            {
                if (entity.FunctionSysCode.Length > 30)
                {
                    throw new BusinessException(LangHelper.GetText("所属功能系统Code长度不能超过30！"));
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
            if (string.IsNullOrWhiteSpace(entity.PermissionName))
            {
                throw new BusinessException(LangHelper.GetText("权限名称不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.PermissionName))
            {
                if (entity.PermissionName.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("权限名称长度不能超过200！"));
                }
            }
            if (string.IsNullOrWhiteSpace(entity.PermissionKey))
            {
                throw new BusinessException(LangHelper.GetText("权限Key不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.PermissionKey))
            {
                if (entity.PermissionKey.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("权限Key长度不能超过200！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.Memo))
            {
                if (entity.Memo.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("备注长度不能超过200！"));
                }
            }
        }

        public void DeleteSysPermissionBatch(IEnumerable<int> sysNos)
        {
            SysPermissionDA.DeleteSysPermissionBatch(sysNos);
        }

        public void UpdateSysPermissionStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        {
            SysPermissionDA.UpdateSysPermissionStatusBatch(sysNos, status);
        }
    }
}
