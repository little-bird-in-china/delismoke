using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BlueStone.Smoke.Entity.AuthEnum
{
    public enum PageType
    {
        [Description("菜单目录")]
        Category = 0,

        [Description("页面")]
        Page = 1
    }
}
