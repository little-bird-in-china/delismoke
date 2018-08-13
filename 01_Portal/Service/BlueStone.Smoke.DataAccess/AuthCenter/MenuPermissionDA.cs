using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.DataAccess
{
  public  class MenuPermissionDA
    {
        public static int InsertMenuPermission(MenuPermission entity) {
            DataCommand cmd = new DataCommand("InsertMenuPermission");
            cmd.SetParameter<MenuPermission>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        public static void UpdateMenuPermission(MenuPermission entity)
        {
            DataCommand cmd = new DataCommand("UpdateMenuPermission");
            cmd.SetParameter<MenuPermission>(entity);
            cmd.ExecuteNonQuery();
        }
    }
}
