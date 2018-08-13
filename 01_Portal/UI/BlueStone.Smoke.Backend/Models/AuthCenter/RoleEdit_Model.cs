using BlueStone.Smoke.Entity.AuthCenter;
using System.Collections.Generic;

namespace BlueStone.Smoke.Backend.Models
{
    public class RoleEdit_Model
    {
        public RoleEdit_Model()
        {
            RoleInfo = new Role();
            allApps = new List<SystemApplication>();
        }

        public Role RoleInfo { get; set; }
        public List<SystemApplication> allApps { get; set; }
    }
}