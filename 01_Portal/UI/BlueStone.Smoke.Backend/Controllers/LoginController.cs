using BlueStone.Smoke.Service;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using BlueStone.Utility.Web.Utility;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    public class LoginController : Controller
    {
        // public static string LOGIN_COOKIE = ConfigurationManager.AppSettings["LOGIN_COOKIE"] == null ? "bluestone_login" : ConfigurationManager.AppSettings["LOGIN_COOKIE"];
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult Login()
        {
            string userName = Request["UserName"];
            string userPwd = Request["UserPwd"];
            string keepalive = Request["keepalive"];
            string verifyCode = Request["VerifyCode"];
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPwd))
            {
                throw new BusinessException("请输入账号或密码");
            }
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                throw new BusinessException("请输入验证码");
            }

            string encrptedPassword = AuthMgr.EncryptPassword(userPwd);
            var user = AuthMgr.Login(userName, encrptedPassword, verifyCode, !string.IsNullOrEmpty(keepalive) && keepalive.ToLower() == "true");
            if (user != null)
            {
                if (user.ExData != null && !user.ExData.ToString().Equals("0"))
                {
                    var company = CompanyService.LoadCompany((int)user.ExData, false);
                    if (company == null || company.CompanyStatus != Entity.CompanyStatus.Authenticated)
                    {
                        AuthMgr.Logout();
                        throw new BusinessException("您所在的公司还未认证!");
                    }
                    if(company!=null&& company.AccountSysNo.HasValue&& company.AccountSysNo.Value!= user.UserSysNo)
                    {
                        AuthMgr.Logout();
                        throw new BusinessException("您没有权限登录此系统!");
                    }
                }
            }

            // SystemUserService systemUserServic = new SystemUserService();
            //  var loginUser= systemUserServic.LoadSystemUserBySysNo(user.UserSysNo, Entity.ConstValue.ApplicationID);
            //  if (loginUser != null)
            //  {

            // user.ExData = loginUser.MasterSysNo;
            //CookieHelper.SaveCookie<AuthUserModel>(LOGIN_COOKIE, user, 7 * 24 * 60);
            // }
            //}
            AjaxResult result = new AjaxResult
            {
                Success = true
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoginOut()
        {
            AuthMgr.Logout();
            AjaxResult result = new AjaxResult
            {
                Success = true
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoginValidationCode()
        {
            string code = ValidationCodeHelper.CreateValidateCode(5);
            byte[] bytes = ValidationCodeHelper.CreateValidateGraphic(code, 52);
            CookieHelper.SaveCookie<string>(AuthMgr.LOGIN_VERIFYCODE_COOKIE, code.Trim());
            return File(bytes, @"image/jpeg");
        }
    }
}