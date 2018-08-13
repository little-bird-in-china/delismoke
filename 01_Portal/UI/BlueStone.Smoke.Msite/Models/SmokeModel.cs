using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueStone.Smoke.Msite.Models
{
    public class DeviceListModel
    { 

     public   SmokeDetectorCount CountInfo { get; set; }
     public QueryResult<QR_SmokeDetector> DeviceList { get; set; }
    }

   
    public class NoticeModel
    {
        public DateTime? BeginInDate { get; set; }

        public string BeginInDateStr { get
            {
                if (BeginInDate != null)
                {
                    return BeginInDate.GetValueOrDefault().ToString("yyyy/MM/dd");
                }else
                {
                    return "";
                }
            }
        }

        public DateTime? EndInDate { get; set; }

        public string EndInDateStr
        {
            get
            {
                if (EndInDate != null)
                {
                    return EndInDate.GetValueOrDefault().ToString("yyyy/MM/dd");
                }
                else
                {
                    return "";
                }
            }
        }

        public List<MessageCenter.Entity.QR_Message> MassageList { get; set; }
    }

    public class AddDetectorModel
    {
        public List<Address> ItemList { get; set; }

        public string SelectCode { get; set; }
    }

    public class UIAddDetectorModel
    {
        public List<AddDetectorModel> List { get; set; }

        public List<Company> CompanyList { get; set; }

        public int SelectCompany { get; set; }

        public Address FirstAddress { get; set; }
    }
}