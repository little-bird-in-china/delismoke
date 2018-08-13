using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueStone.Smoke.Entity
{
    public class TreeInfoModel
    {
        public int id { get; set; }

        public string text { get; set; }
        /// <summary>
        /// 特殊处理请定义
        /// </summary>
        public string icon { get; set; } 
        /// <summary>
        /// 类型default file 要使用type就不能定义icon
        /// </summary>
        public string type { get; set; } 
         
        public TreeStateModel state { get; set; }

        /// <summary>
        ///  类型可以是List&lt;TreeInfoModel&gt;(为子节点列表) 或是 bool 类型(true表示动态加载子节点，false 表示没有子节点)
        /// </summary>
        public object children { get; set; }

        public object data { get; set; }

        public int sortIndex { get; set; }
    }

    public class TreeInfoAppModel
    {
        public int SysNo { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }


        public List<TreeInfoAppModel> children { get; set; }
    }

    public class TreeStateModel
    {
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool opened { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool selected { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; }
    }
}