using JC.Smoke.Model;
using SmartHealth.BLL;
using SmartHealth.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;

namespace SmartHealth.Providers
{
    public class MyMembershipProvider : MembershipProvider
    {
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        private void addCookieForEmployee(Employee employee)
        {
            var cookie = new HttpCookie("uinfo");
            cookie.Values.Add("uname", HttpContext.Current.Server.UrlEncode(employee.mobile_no));
            cookie.Values.Add("tname", HttpContext.Current.Server.UrlEncode(employee.name));
            cookie.HttpOnly = false;//保存到客户端
            cookie.Expires = DateTime.Now.AddDays(1);//IE客户端必须设置过期时间才能保存
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public override bool ValidateUser(string username, string password)
        {
            EmployeeWithAuthes employeeWithAuthes = EmployeeBll.Login(username, password);
            if (employeeWithAuthes.employee != null && employeeWithAuthes.employee.id > 0)
            {
                if (username.Equals("admin"))
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddMinutes(20), true, "{\"0\":\"all\"}", "/");
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    addCookieForEmployee(employeeWithAuthes.employee);

                    HttpContext.Current.Session["login_code"] = 0;
                    return true;
                }
                else if (employeeWithAuthes.roleAuthes != null && employeeWithAuthes.roleAuthes.Length > 0)
                {
                    Dictionary<string, string> purviews = new Dictionary<string, string>(employeeWithAuthes.roleAuthes.Length);
                    foreach (RoleAuth auth in employeeWithAuthes.roleAuthes)
                    {
                        purviews.Add(auth.menu_id.ToString(), auth.purview);
                    }
                    //add userid
                    purviews.Add("-1", employeeWithAuthes.employee.id.ToString());
                    string roleString = Json.Encode(purviews);
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddMinutes(20), true, roleString, "/");
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    addCookieForEmployee(employeeWithAuthes.employee);

                    HttpContext.Current.Session["login_code"] = 0;
                    return true;
                }
                else
                {
                    //未激活或权限未分配
                    HttpContext.Current.Session["login_code"] = -2;
                }
            }
            else
            {
                //用户名或密码错误
                HttpContext.Current.Session["login_code"] = -1;
            }
            return false;
        }
    }
}