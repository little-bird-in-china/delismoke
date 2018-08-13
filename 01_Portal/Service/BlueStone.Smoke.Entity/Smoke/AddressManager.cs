using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class AddressManager
    {

        public int ManagerSysNo { get; set; }

        public string AddressCode { get; set; }

        public int CompanySysNo { get; set; }

        public DateTime InTime { get; set; }

        public string UserFullName { get; set; }
    }
}