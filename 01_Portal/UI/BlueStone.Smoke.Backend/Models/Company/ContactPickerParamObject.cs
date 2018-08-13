using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueStone.Smoke.Backend.Models
{
    public class ContactPickerParamObject:BlueStone.Smoke.Entity.Common.PickerParamObject
    {
        public int? CompanySysNo { get; set; }
    }
}