using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity.Common
{
    /// <summary>
    /// 选择器对象
    /// </summary>
    public class PickerParamObject
    {
        /// <summary>
        /// 选择器ID
        /// </summary>
        public string PickerID { get; set; }

        /// <summary>
        /// 选中值
        /// </summary>
        public string SelectValue { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMultiple { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// 选择器选择类型
        /// </summary>
        public string PickerType { get; set; }

        /// <summary>
        /// 是否执行JS回调函数
        /// </summary>
        public bool CallbackJSFunction { get; set; }

        public string Validator { get; set; }

        public string CssClass { get; set; }
    }
}
