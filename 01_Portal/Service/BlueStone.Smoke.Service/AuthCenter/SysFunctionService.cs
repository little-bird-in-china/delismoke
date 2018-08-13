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
    public class SysFunctionService
    {
        private static readonly object codeLock = new object();
        /// <summary>
        /// 创建SysFunction信息
        /// </summary>
        public int InsertSysFunction(SysFunction entity)
        {
            CheckSysFunction(entity, true);
            int result = 0;
            lock (codeLock)
            {
                CreateCode(entity);
                result = SysFunctionDA.InsertSysFunction(entity);
            }
            return result;
        }

        /// <summary>
        /// 系统生成编码每级3位
        /// </summary>
        /// <param name="entity"></param>
        private void CreateCode(SysFunction entity)
        {
            string BrotherCode = string.Empty;
            string ParentCode = string.Empty;
            SysFunctionDA.GetBuildSysCode(entity.ParentSysNo, out ParentCode, out BrotherCode, entity.ApplicationID);
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

        /// <summary>
        /// 更新SysFunction信息
        /// </summary>
        public void UpdateSysFunction(SysFunction entity)
        {
            CheckSysFunction(entity, false);
            SysFunctionDA.UpdateSysFunction(entity);
        }

        /// <summary>
        /// 删除SysFunction信息
        /// </summary>
        public void DeleteSysFunction(int sysNo)
        {
            if (SysFunctionDA.CountFunctionsChildren(sysNo) >0)
            {
                throw new BusinessException("不能直接删除拥有子节点的菜单,请先删除其子节点");
            }
            SysFunctionDA.DeleteSysFunction(sysNo);
        }

        /// <summary>
        /// 分页查询SysFunction信息
        /// </summary>
        public QueryResult<SysFunction> QuerySysFunctionList(QF_SysFunction filter)
        {
            return SysFunctionDA.QuerySysFunctionList(filter);
        }

        /// <summary>
        /// 加载SysFunction信息
        /// </summary>
        public SysFunction LoadSysFunction(int sysNo)
        {
            return SysFunctionDA.LoadSysFunction(sysNo);
        }

        /// <summary>
        /// 检查SysFunction信息
        /// </summary>
        private void CheckSysFunction(SysFunction entity, bool isCreate)
        {
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if (string.IsNullOrWhiteSpace(entity.FunctionName))
            {
                throw new BusinessException(LangHelper.GetText("功能名称不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.FunctionName))
            {
                if (entity.FunctionName.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("功能名称长度不能超过50！"));
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
            if (!string.IsNullOrWhiteSpace(entity.Memo))
            {
                if (entity.Memo.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("备注长度不能超过200！"));
                }
            }
        }

        public List<SysFunction> DynamicLoadFunctions(int parentsysno,string ApplicationID)
        {
            return SysFunctionDA.DynamicLoadFunctions(parentsysno, ApplicationID);
        }

        public List<SysFunction> LoadAllFunctions(string applicationID)
        {
            if (string.IsNullOrWhiteSpace(applicationID))
            {
                throw new BusinessException(LangHelper.GetText("请传入ApplicationID！"));
            }
            return SysFunctionDA.LoadAllFunctions(applicationID);
        }
    }
}
