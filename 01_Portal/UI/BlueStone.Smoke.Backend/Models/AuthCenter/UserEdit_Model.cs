using BlueStone.Smoke.Entity.AuthCenter;
using System.Collections.Generic;

namespace BlueStone.Smoke.Backend.Models
{
    public class UserEdit_Model
    {
        public UserEdit_Model()
        {
            UserInfo = new SystemUser();
            UserRoles = new List<Role>();
            allApps = new List<SystemApplication>();
        }
        public SystemUser UserInfo { get; set; }
        public List<Role> UserRoles { get; set; }
        public List<SystemApplication> allApps { get; set; }
    }
}