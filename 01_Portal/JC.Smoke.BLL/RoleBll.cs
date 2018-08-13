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
    public class RoleBll
    {
        /*public static Role[] GetAllRoles()
        {
            List<Role> retValue = new List<Role>();
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
                            menu.children[i] = new Role(children[i]);
                            //role.children[i].state = "";
                        }
                    }
                    retValue.Add(role);
                }
            }
            if (retValue.Count > 0)
            {
                //retValue[0].state = "";
            }
            return retValue.ToArray();
        }*/

        private static string GetStringFrom(MenuPurview[] purviews)
        {
            string retValue = null;
            if (purviews != null && purviews.Length > 0)
            {
                foreach (MenuPurview purview in purviews)
                {
                    if (retValue == null)
                    {
                        retValue = String.Format("{0}", purview.purview);
                    }
                    else
                    {
                        retValue += String.Format(",{0}", purview.purview);
                    }
                }
            }
            return retValue;
        }

        /*
        public static RoleAuthIO[] GetAllRoleAuths()
        {
            List<RoleAuthIO> retValue = new List<RoleAuthIO>();
            using (SmartHealthEntities db = new SmartHealthEntities())
            {
                Menu[] menus = (from m in db.Menu
                                where m.parent_id == 0
                                select m).ToArray();
                foreach (Menu menu in menus)
                {
                    RoleAuthIO roleAuth = new RoleAuthIO(menu);
                    //获取菜单所包含权限
                    roleAuth.menu_purviews = MenuBll.GetMenuPurviews(menu.id);
                    roleAuth.full_purview_string = GetStringFrom(roleAuth.menu_purviews);
                    //child
                    Menu[] children = (from m in db.Menu
                                       where m.parent_id == menu.id
                                       select m).ToArray();
                    if (children != null && children.Length > 0)
                    {
                        roleAuth.children = new RoleAuthIO[children.Length];
                        for (int i = 0; i < children.Length; i++)
                        {
                            roleAuth.children[i] = new RoleAuthIO(children[i]);
                            //获取菜单所包含权限
                            roleAuth.children[i].menu_purviews = MenuBll.GetMenuPurviews(children[i].id);
                            roleAuth.children[i].full_purview_string = GetStringFrom(roleAuth.children[i].menu_purviews);
                            roleAuth.children[i].state = "";
                        }
                    }
                    retValue.Add(roleAuth);
                }
            }
            if (retValue.Count > 0)
            {
                retValue[0].state = "";
            }
            return retValue.ToArray();
        }

        public static RoleAuthIO[] GetRoleAuths(int roleID)
        {
            using (SmartHealthEntities db = new SmartHealthEntities())
            {
                return db.Database.SqlQuery<RoleAuthIO>(String.Format("SELECT menu_id AS id,purview FROM RoleAuth WHERE role_id={0}", roleID)).ToArray();
            }
        }

        public static bool SaveRoleAuths(int operatorID, int roleID, RoleAuthIO[] roleAuth)
        {
            int affectCount = 0;
            using (SmartHealthEntities db = new SmartHealthEntities())
            {
                try
                {
                    foreach (RoleAuthIO auth in roleAuth)
                    {
                        string rawSql = String.Format("SELECT COUNT(*) AS RecordCount FROM RoleAuth WHERE role_id={0} AND menu_id={1}", roleID, auth.id);
                        int exist = db.Database.SqlQuery<int>(rawSql).FirstOrDefault();
                        if (exist > 0)
                        {
                            if (auth.purview.Trim().Length > 0)
                            {
                                rawSql = "UPDATE RoleAuth SET purview=@P0,update_date=@P1,update_by=@P2 WHERE role_id=@P3 AND menu_id=@P4";
                                affectCount += db.Database.ExecuteSqlCommand(rawSql, new object[] { auth.purview, DateTime.Now, operatorID, roleID, auth.id });
                            }
                            else
                            {
                                rawSql = "DELETE FROM RoleAuth WHERE role_id=@P0 AND menu_id=@P1";
                                affectCount += db.Database.ExecuteSqlCommand(rawSql, new object[] { roleID, auth.id });
                            }
                        }
                        else if (auth.purview.Trim().Length > 0)
                        {
                            rawSql = "INSERT INTO RoleAuth(role_id,menu_id,purview,create_by,update_by) VALUES (@P0,@P1,@P2,@P3,@P3)";
                            affectCount += db.Database.ExecuteSqlCommand(rawSql, new object[] { roleID, auth.id, auth.purview, operatorID });
                        }
                    }
                    if (affectCount > 0)
                    {
                        //添加操作日志
                        EmployeeAction employeeAction = new EmployeeAction();
                        employeeAction.action = 2;
                        employeeAction.employee_id = operatorID;
                        employeeAction.menu_id = 26;
                        employeeAction.operation_date = DateTime.Now;
                        employeeAction.remark = "角色id: " + roleID;
                        db.EmployeeAction.Add(employeeAction);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return (affectCount>0);
        }

        */
    }
}
