using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using JC.Smoke.Model;
using SmartHealth.BLL;
using SmartHealth.Model;

namespace SmartHealth.Providers
{
    public class MyRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            FormsIdentity formIdentity = (FormsIdentity)HttpContext.Current.User.Identity;
            string roleString = formIdentity.Ticket.UserData;
            Dictionary<string, string> roles = System.Web.Helpers.Json.Decode(roleString, typeof(Dictionary<string, string>));
            List<string> retValue = new List<string>(72);
            //string[] retValue = new string[18];
            if (roles.ContainsKey("0"))
            {
                for (int i = 1; i < 15; i++)
                {
                    if (i != 1 && i != 2 && i != 3 && i != 4)
                    {
                        MenuPurview[] menu_purviews = MenuBll.GetMenuPurviews(i);
                        if (menu_purviews != null && menu_purviews.Length > 0)
                        {
                            foreach (MenuPurview purview in menu_purviews)
                            {
                                retValue.Add(String.Format("{0}-{1}", i, purview.purview));
                            }
                        }
                    }
                }
            }
            else
            {
                string[] keys = roles.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (Convert.ToInt32(keys[i]) > 0)
                    {
                        string value = roles[keys[i]];
                        string[] values = value.Split(',');
                        for (int j = 0; j < values.Length; j++)
                        {
                            retValue.Add(String.Format("{0}-{1}", keys[i], values[j]));
                        }
                    }
                }
            }
            return retValue.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}