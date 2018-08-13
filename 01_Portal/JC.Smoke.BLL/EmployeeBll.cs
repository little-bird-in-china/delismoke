using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JC.Smoke.DAL;
using JC.Smoke.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using SmartHealth.Model;

namespace SmartHealth.BLL
{
    public class EmployeeBll
    {
        public static EmployeeWithAuthes Login(string username, string password)
        {
            //这个函数返回的类型具有层次结构，故直接实体类返回还是比较合理，这种情况用dynamic确实不合适
            EmployeeWithAuthes retValue = new EmployeeWithAuthes();
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {//db该不该释放用Iqueryable就知道了
                try
                {
                    retValue.employee = EmployeeDal.Login(username, password, conn);
                    if (retValue.employee != null && !username.Equals("admin"))
                    {
                        //非管理员查询对应权限
                        retValue.roleAuthes = RoleAuthDal.GetRoleAuths(retValue.employee.role_id, conn);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return retValue;
        }

        public static bool ChangePassword(int uid, string oldpwd, string newpwd)
        {
            bool retValue = false;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                if (EmployeeDal.CheckPwd(uid, oldpwd, conn))
                {
                    retValue = EmployeeDal.UpdatePwd(uid, newpwd, conn)>0;
                }
            }
            return true;
        }

        public static JArray GetAllMenus()
        {
            JArray retValue = new JArray();
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                try
                {
                    Menu[] dbMenus =  MenuDal.GetAllMenus(conn);

                    if (dbMenus != null && dbMenus.Length > 0)
                    {
                        //dbMenus为单级权限菜单,需要组装成二级菜单
                        Dictionary<int, List<Menu>> menuDict = new Dictionary<int, List<Menu>>();
                        foreach (Menu dbmenu in dbMenus)
                        {
                            List<Menu> menuGroup = null;
                            if (menuDict.ContainsKey(dbmenu.parent_id))
                            {
                                menuGroup = menuDict[dbmenu.parent_id];
                                //选择合适位置插入
                                int insertIndex = 0;
                                for (; insertIndex < menuGroup.Count; insertIndex++)
                                {
                                    if (string.Compare(menuGroup[insertIndex].menu_no, dbmenu.menu_no, StringComparison.OrdinalIgnoreCase) > 0)
                                    {
                                        break;
                                    }
                                }
                                menuGroup.Insert(insertIndex, dbmenu);
                            }
                            else
                            {
                                Menu parent = MenuDal.GetMenu(dbmenu.parent_id, conn);
                                if (parent != null)
                                {
                                    menuGroup = new List<Menu>() { parent, dbmenu };
                                    menuDict.Add(dbmenu.parent_id, menuGroup);
                                }
                            }
                        }
                        //生成json数组
                        int[] keys = menuDict.Keys.ToArray();
                        for (int i = 0; i < keys.Length; i++)
                        {
                            Menu parent = menuDict[keys[i]][0];
                            JArray jChildren = new JArray();
                            for (int j = 1; j < menuDict[keys[i]].Count; j++)
                            {
                                Menu child = menuDict[keys[i]][j];
                                jChildren.Add(new JObject(new JProperty("id", child.menu_no),
                                                          new JProperty("name", child.name),
                                                          new JProperty("icon", child.icon_class),
                                                          new JProperty("url", child.action_url)));
                            }
                            JObject jMenuGroup = new JObject(new JProperty("id", parent.menu_no),
                                                             new JProperty("icon", parent.icon_class),
                                                             new JProperty("name", parent.name),
                                                             new JProperty("children", jChildren));
                            //父级菜单还需一次排序
                            int insertIndex = 0;
                            for (; insertIndex < retValue.Count; insertIndex++)
                            {
                                JObject item = (JObject)retValue[insertIndex];
                                if (string.Compare(item["id"].ToString(), jMenuGroup["id"].ToString(), StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    break;
                                }
                            }
                            retValue.Insert(insertIndex, jMenuGroup);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return retValue;
        }


        /*public static int GetTotalEmployees(int shopID)
        {
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                if (shopID > 0)
                {
                    return (from m in db.Employee
                            where m.status == 1 && m.shop_id.Equals(shopID)
                            select m).Count();
                }
                else
                {
                    return (from m in db.Employee
                            where m.status == 1 && m.id>1
                            select m).Count();
                }
                
            }
        }
        public static EmployeeIO GetEmployee(int id)
        {
            EmployeeIO retValue = null;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                string rawSql = String.Format("SELECT A.shop_id,A.role_id,B.name AS shop_name FROM Employee AS A" +
                                                " LEFT OUTER JOIN Shop AS B ON A.shop_id=B.id" +
                                                " WHERE A.id={0}", id);
                retValue = db.Database.SqlQuery<EmployeeIO>(rawSql).FirstOrDefault();
            }
            return retValue;
        }
        //分页查询语句需要调整
        public static EmployeeIO[] GetEmployees(string nameFilter, int state, int department,
                                                    int pageCount, int pageIndex, ref int totalCount)
        {
            //dynamic性能肯定好于Dictionory,低于var,如何dynamic可以直接转化为Json还是强烈建议用的，况且这里返回的数据层次单一
            //如果存储过程raw query出来的结果可以直接转化为dynamic还是直接用存储过程, 至少不用去学linq过于复杂的语法
            EmployeeIO[] retValue = null;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                string rawSql = String.Format("EXEC dbo.P_SH_GetEmployees '{0}',{1},{2},{3},{4}",
                                                nameFilter, state, department, pageCount, pageIndex);
                retValue = db.Database.SqlQuery<EmployeeIO>(rawSql).ToArray();
                rawSql = String.Format("EXEC dbo.P_SH_GetEmployeesCount '{0}',{1},{2}",
                                                nameFilter, state, department);
                totalCount = db.Database.SqlQuery<int>(rawSql).FirstOrDefault();
            }
            return retValue;
        }

        public static int EnableEmployee(int id, int operatorID)
        {
            int retValue = 0;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                Employee employee = (from m in db.Employee
                                     where (m.id == id && (m.status & 1) == 0)
                                     select m).FirstOrDefault();
                if (employee != null)
                {
                    employee.status = 1;
                    db.Entry<Employee>(employee).Property("status").IsModified = true;
                    employee.update_by = operatorID;
                    db.Entry<Employee>(employee).Property("update_by").IsModified = true;
                    employee.update_date = DateTime.Now;
                    db.Entry<Employee>(employee).Property("update_date").IsModified = true;
                    retValue = db.SaveChanges();

                    if (retValue > 0)
                    {
                        //添加操作日志
                        EmployeeAction employeeAction = new EmployeeAction();
                        employeeAction.action = 4;
                        employeeAction.employee_id = operatorID;
                        employeeAction.menu_id = 21;
                        employeeAction.operation_date = DateTime.Now;
                        employeeAction.remark = "id: " + employee.id;
                        db.EmployeeAction.Add(employeeAction);
                        db.SaveChanges();
                    }
                }
            }
            return retValue;
        }

        public static int DisableEmployee(int id, int operatorID)
        {
            int retValue = 0;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                Employee employee = (from m in db.Employee
                                     where (m.id == id && (m.status & 1) > 0)
                                     select m).FirstOrDefault();
                if (employee != null)
                {
                    employee.status = 0;
                    db.Entry<Employee>(employee).Property("status").IsModified = true;
                    employee.update_by = operatorID;
                    db.Entry<Employee>(employee).Property("update_by").IsModified = true;
                    employee.update_date = DateTime.Now;
                    db.Entry<Employee>(employee).Property("update_date").IsModified = true;
                    retValue = db.SaveChanges();

                    if (retValue > 0)
                    {
                        //添加操作日志
                        EmployeeAction employeeAction = new EmployeeAction();
                        employeeAction.action = 5;
                        employeeAction.employee_id = operatorID;
                        employeeAction.menu_id = 21;
                        employeeAction.operation_date = DateTime.Now;
                        employeeAction.remark = "id: " + employee.id;
                        db.EmployeeAction.Add(employeeAction);
                        db.SaveChanges();
                    }
                }
            }
            return retValue;
        }

        public static int AddEmployee(string name, string mobile_no, string password, int role_id, int department_id, int shop_id,
                                           string telephone, string email, string birth_day,string entry_date, string note, int operatorID)
        {
            int retValue = 0;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                int exsistedCount = (from m in db.Employee
                                     where (m.mobile_no.Trim().Equals(mobile_no.Trim()))
                                     select m).Count();
                if (exsistedCount > 0)
                {
                    retValue = -2;
                }
                else
                {
                    Employee employee = new Employee();
                    employee.name = name.Trim();
                    employee.mobile_no = mobile_no.Trim();
                    if (password != null && password.Length > 0)
                    {
                        employee.password = password;
                    }
                    else
                    {
                        employee.password = "670B14728AD9902AECBA32E22FA4F6BD";
                    }
                    
                    employee.role_id = role_id;
                    employee.department_id = department_id;
                    employee.shop_id = shop_id;
                    employee.telephone = telephone;
                    employee.email = email;
                    if (birth_day!=null && birth_day.Trim().Length>0) {
                        employee.birth_day = Convert.ToDateTime(birth_day);
                    } else {
                        employee.birth_day = null;
                    }
                    if (entry_date!=null && entry_date.Trim().Length>0) {
                        employee.entry_date = Convert.ToDateTime(entry_date);
                    } else {
                        employee.entry_date = null;
                    }
                    employee.note = note.Trim();
                    employee.status = 1;
                    employee.create_by = operatorID;
                    employee.create_date = DateTime.Now;
                    employee.update_by = operatorID;
                    employee.update_date = DateTime.Now;
                    db.Employee.Add(employee);
                    db.SaveChanges();

                    retValue = employee.id;

                    if (retValue > 0)
                    {
                        //添加操作日志
                        EmployeeAction employeeAction = new EmployeeAction();
                        employeeAction.action = 2;
                        employeeAction.employee_id = operatorID;
                        employeeAction.menu_id = 21;
                        employeeAction.operation_date = DateTime.Now;
                        employeeAction.remark = "id: " + employee.id;
                        db.EmployeeAction.Add(employeeAction);
                        db.SaveChanges();
                    }
                }
            }
            return retValue;
        }

        public static int ModifyEmployee(int id, string name, string mobile_no, string password, int role_id, int department_id, int shop_id,
                                           string telephone, string email, string birth_day, string entry_date, string note, int operatorID)
        {
            int retValue = 0;
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                int exsistedCount = (from m in db.Employee
                                     where (m.mobile_no.Trim().Equals(mobile_no.Trim()) &&
                                            (!m.id.Equals(id)))
                                     select m).Count();
                if (exsistedCount > 0)
                {
                    retValue = -2;
                }
                else
                {
                    Employee employee = EmployeeDal.GetEmployee(id, conn);
                    if (employee != null)
                    {
                        employee.name = name;
                        db.Entry<Employee>(employee).Property("name").IsModified = true;
                        employee.mobile_no = mobile_no.Trim();
                        db.Entry<Employee>(employee).Property("mobile_no").IsModified = true;
                        if (password != null && password.Length > 0)
                        {
                            employee.password = password;
                            db.Entry<Employee>(employee).Property("password").IsModified = true;
                        }
                        employee.role_id = role_id;
                        db.Entry<Employee>(employee).Property("role_id").IsModified = true;
                        employee.department_id = department_id;
                        db.Entry<Employee>(employee).Property("department_id").IsModified = true;
                        employee.shop_id = shop_id;
                        db.Entry<Employee>(employee).Property("shop_id").IsModified = true;
                        employee.telephone = telephone;
                        db.Entry<Employee>(employee).Property("telephone").IsModified = true;
                        employee.email = email;
                        db.Entry<Employee>(employee).Property("email").IsModified = true;
                        if (birth_day != null && birth_day.Trim().Length > 0)
                        {
                            employee.birth_day = Convert.ToDateTime(birth_day);
                        }
                        else
                        {
                            employee.birth_day = null;
                        }
                        db.Entry<Employee>(employee).Property("birth_day").IsModified = true;
                        if (entry_date != null && entry_date.Trim().Length > 0)
                        {
                            employee.entry_date = Convert.ToDateTime(entry_date);
                        }
                        else
                        {
                            employee.entry_date = null;
                        }
                        db.Entry<Employee>(employee).Property("entry_date").IsModified = true;
                        employee.note = note.Trim();
                        db.Entry<Employee>(employee).Property("note").IsModified = true;

                        employee.update_by = operatorID;
                        db.Entry<Employee>(employee).Property("update_by").IsModified = true;
                        employee.update_date = DateTime.Now;
                        db.Entry<Employee>(employee).Property("update_date").IsModified = true;
                        retValue = db.SaveChanges();

                        if (retValue > 0)
                        {
                            //添加操作日志
                            EmployeeAction employeeAction = new EmployeeAction();
                            employeeAction.action = 3;
                            employeeAction.employee_id = operatorID;
                            employeeAction.menu_id = 21;
                            employeeAction.operation_date = DateTime.Now;
                            employeeAction.remark = "id: " + employee.id;
                            db.EmployeeAction.Add(employeeAction);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return retValue;
        }*/

        public static JArray GetMenusByRoles(Dictionary<string, string> roles)
        {
            JArray retValue = new JArray();
            using (MySqlConnection conn = DBUtility.OpenConnection())
            {
                try
                {
                    Menu[] dbMenus = null;
                    if (roles.ContainsKey("0"))
                    {
                        //admin
                        dbMenus = MenuDal.GetAllMenus(conn);
                    }
                    else
                    {
                        string menuIDs = string.Join(",", roles.Keys.ToArray());
                        dbMenus = MenuDal.GetMenus(menuIDs, conn);
                    }
                    if (dbMenus != null && dbMenus.Length > 0)
                    {
                        //dbMenus为单级权限菜单,需要组装成二级菜单
                        Dictionary<int, List<Menu>> menuDict = new Dictionary<int, List<Menu>>();
                        foreach (Menu dbmenu in dbMenus)
                        {
                            List<Menu> menuGroup = null;
                            if (menuDict.ContainsKey(dbmenu.parent_id))
                            {
                                menuGroup = menuDict[dbmenu.parent_id];
                                //选择合适位置插入
                                int insertIndex = 0;
                                for (; insertIndex < menuGroup.Count; insertIndex++)
                                {
                                    if (string.Compare(menuGroup[insertIndex].menu_no, dbmenu.menu_no, StringComparison.OrdinalIgnoreCase) > 0)
                                    {
                                        break;
                                    }
                                }
                                menuGroup.Insert(insertIndex, dbmenu);
                            }
                            else
                            {
                                Menu parent = MenuDal.GetMenu(dbmenu.parent_id, conn);
                                if (parent != null)
                                {
                                    menuGroup = new List<Menu>() { parent, dbmenu };
                                    menuDict.Add(dbmenu.parent_id, menuGroup);
                                }
                            }
                        }
                        //生成json数组
                        int[] keys = menuDict.Keys.ToArray();
                        for (int i = 0; i < keys.Length; i++)
                        {
                            Menu parent = menuDict[keys[i]][0];
                            JArray jChildren = new JArray();
                            for (int j = 1; j < menuDict[keys[i]].Count; j++)
                            {
                                Menu child = menuDict[keys[i]][j];
                                jChildren.Add(new JObject(new JProperty("id", child.menu_no),
                                                          new JProperty("name", child.name),
                                                          new JProperty("icon", child.icon_class),
                                                          new JProperty("url", child.action_url)));
                            }
                            JObject jMenuGroup = new JObject(new JProperty("id", parent.menu_no),
                                                             new JProperty("icon", parent.icon_class),
                                                             new JProperty("name", parent.name),
                                                             new JProperty("children", jChildren));
                            //父级菜单还需一次排序
                            int insertIndex = 0;
                            for (; insertIndex < retValue.Count; insertIndex++)
                            {
                                JObject item = (JObject)retValue[insertIndex];
                                if (string.Compare(item["id"].ToString(), jMenuGroup["id"].ToString(), StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    break;
                                }
                            }
                            retValue.Insert(insertIndex, jMenuGroup);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return retValue;
        }
    }
}
