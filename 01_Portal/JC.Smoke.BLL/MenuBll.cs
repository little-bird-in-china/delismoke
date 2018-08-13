using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JC.Smoke.DAL;
using JC.Smoke.Model;
using MySql.Data.MySqlClient;

namespace SmartHealth.BLL
{
    public class MenuBll
    {
        public static Menu[] GetAllMenus()
        {//先去掉state树状态,just for test
            List<Menu> retValue = new List<Menu>();
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                Menu[] menus = MenuDal.GetMenusByParentID(0, conn);
                foreach (Menu menu in menus)
                {
                    //child
                    Menu[] children = MenuDal.GetMenusByParentID(menu.id, conn);
                    if (children != null && children.Length > 0)
                    {
                        menu.children = new Menu[children.Length];
                        for (int i = 0; i < children.Length; i++)
                        {
                            menu.children[i] = children[i];
                            //menu.children[i].state = "";
                        }
                    }
                    retValue.Add(menu);
                }
            }
            if (retValue.Count > 0)
            {
                //retValue[0].state = "";
            }
            return retValue.ToArray();
        }

        public static MenuPurview[] GetMenuPurviews(int menu_id)
        {
            MenuPurview[] retValue = null;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                retValue = MenuPurviewDal.GetPurviewsByMenuID(menu_id, conn);
            }
            return retValue;
        }
    }
}
