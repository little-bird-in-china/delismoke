using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class QF_SysFunction : QueryFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus? CommonStatus { get; set; }
    }
}
