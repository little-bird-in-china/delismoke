using System.ComponentModel;

namespace BlueStone.Smoke.Entity
{
    public enum CompanyStatus
    {
        [Description("待审核")]
        Init = 0,
        [Description("审核通过")]
        Authenticated = 10,
        [Description("审核驳回")]
        Invalid = 20,
    }
}
