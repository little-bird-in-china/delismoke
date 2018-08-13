using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class ContentTypes
    {
        public const string Application_Json = "application/json";
        public const string Application_Form_URlEncoded = "application/x-www-form-urlencoded";
        public const string Application_Form_URlEncoded_UTF8 = "application/x-www-form-urlencoded;charset=utf-8";
        public const string Application_Xml = "application/xml";

    }

    public enum HttpMethod
    {
        GET,
        POST
    }

}
