using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    [Auth]
    public class BaseController : Controller
    {

        public CurrentUser CurrUser;

        public BaseController()
        {
            this.ViewBag.IsUserLogin = true;
            //加载登录信息
            var userInfo = AuthMgr.ReadUserInfo();

            if (userInfo != null)
            {
                //获取登陆账户所在企业

                CurrUser = new CurrentUser()
                {
                    UserName = userInfo.UserID,
                    UserSysNo = userInfo.UserSysNo,
                    UserDisplayName = userInfo.UserDisplayName,
                    AvatarImageUrl = userInfo.AvatarImageUrl,
                    LoginTime = userInfo.LoginTime.ToString()
                };
                if (userInfo.ExData != null)
                {
                    CurrUser.MasterSysNo = (int)userInfo.ExData;
                    Company company = Service.CompanyService.LoadCompany(CurrUser.MasterSysNo.Value, false);
                    if (company != null && company.SysNo > 0)
                    {
                        CurrUser.AvatarImageUrl = company.Logo;
                    }
                }
                if (userInfo.ExData != null && ((int)userInfo.ExData > 0))
                {
                    ViewBag.IsPMAdmin = false;
                    CurrUser.IsPMAdmin = false;
                }
                else
                {
                    ViewBag.IsPMAdmin = true;
                    CurrUser.IsPMAdmin = true;
                }
                this.ViewBag.CurrUser = CurrUser;
            }
        }


        protected T BuildQueryFilterEntity<T>() where T : class
        {
            return BuildQueryFilterEntity<T>(null);

        }

        protected T BuildQueryFilterEntity<T>(Action<T> manualMapping) where T : class
        {
            object t = Activator.CreateInstance(typeof(T));
            if (!string.IsNullOrEmpty(this.Request["queryfilter[]"]))
            {
                t = JsonConvert.DeserializeObject<T>(this.Request["queryfilter[]"], new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,

                });
            }
            if (t is QueryFilter)
            {
                //每页显示条数:
                int pageSize = Convert.ToInt32(Request["length"]);
                //当前页码:
                int pageIndex = Convert.ToInt32(Request["start"]) % pageSize == 0 ? Convert.ToInt32(Request["start"]) / pageSize : Convert.ToInt32(Request["start"]) / pageSize + 1;
                //排序:
                string sortBy = null;
                if (!string.IsNullOrEmpty(Request["order[0][column]"]))
                {
                    string colIndex = Request["order[0][column]"];
                    string sortByField = string.IsNullOrEmpty(Request[string.Format("columns[{0}][name]", colIndex)]) ? Request[string.Format("columns[{0}][data]", colIndex)] : Request[string.Format("columns[{0}][name]", colIndex)];
                    string sortDir = Request["order[0][dir]"];
                    sortBy = string.Format("{0} {1}", sortByField, sortDir.ToUpper());
                }
                ((QueryFilter)t).PageSize = pageSize;
                ((QueryFilter)t).PageIndex = pageIndex;
                ((QueryFilter)t).SortFields = GetOrderText();
            }

            if (manualMapping != null)
            {
                manualMapping((T)t);
            }
            return (T)t;

        }

        //获取前端多个排序 拼接排序字符串
        private string GetOrderText()
        {
            string orderText = string.Empty;
            var orderColumnNoKeys = this.Request.Form.AllKeys.Where(a => a.StartsWith("order") && a.EndsWith("[column]"));
            var orderDirKeys = this.Request.Form.AllKeys.Where(a => a.StartsWith("order") && a.EndsWith("[dir]"));

            if (orderColumnNoKeys != null && orderDirKeys != null)
            {
                if (orderColumnNoKeys.Count() == orderDirKeys.Count())
                {
                    for (int i = 0; i < orderColumnNoKeys.Count(); i++)
                    {
                        if (i > 0)
                        {
                            orderText += ",";
                        }

                        string colIndex = Request[orderColumnNoKeys.ElementAt(i)];
                        string sortByField = string.IsNullOrEmpty(Request[string.Format("columns[{0}][name]", colIndex)]) ? Request[string.Format("columns[{0}][data]", colIndex)] : Request[string.Format("columns[{0}][name]", colIndex)];
                        string sortDir = Request[orderDirKeys.ElementAt(i)];
                        orderText += string.Format(" {0} {1} ", sortByField, sortDir.ToUpper());
                    }
                }
            }

            return orderText;
        }

        protected JsonResult AjaxGridJson<T>(QueryResult<T> result) where T : class
        {
            return Json(new
            {
                draw = result.draw,
                recordsTotal = result.recordsTotal,
                recordsFiltered = result.recordsFiltered,
                aaData = result.data,
            }, JsonRequestBehavior.AllowGet);
        }


        protected void SetEntityBaseUserInfo(EntityBase entityBase)
        {
            entityBase.InUserSysNo = CurrUser.UserSysNo;
            entityBase.InUserName = CurrUser.UserDisplayName;
            entityBase.InDate = DateTimeHelper.GetTimeZoneNow();
            entityBase.EditUserSysNo = CurrUser.UserSysNo;
            entityBase.EditUserName = CurrUser.UserDisplayName;
            entityBase.EditDate = DateTimeHelper.GetTimeZoneNow();
        }
    }
}