using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JC.Smoke.Model
{
    public class Role
    {
        public int id { get; set; }
        public string name { get; set; }
        public string no { get; set; }
        public string icon { get; set; }
        public string iconCls { get; set; }
        public string url { get; set; }
        public string date { get; set; }

        public Role[] children;
    }
}
