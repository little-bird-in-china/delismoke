using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Models;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Service;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{


    [Auth(FunctionKeys.PM_Customer_All, FunctionKeys.Cus_MyInfo_All)]
    public class CompanyController : BaseController
    {
        SystemUserService user_service = new SystemUserService();
        RoleService usersrole = new RoleService();
        private static string m_DefaultCompanySysNo = ConfigurationManager.AppSettings["DummyCompanySysNo"];
        private static int defaultCompanySysNo = 0;
        public static string RoleSysNo = ConfigurationManager.AppSettings["RoleSysNo"];
        public int rolesysno = 0;
        public CompanyController()
        {
            int.TryParse(RoleSysNo, out rolesysno);
            int.TryParse(m_DefaultCompanySysNo, out defaultCompanySysNo);
        }

        [Auth(FunctionKeys.PM_Customer_All)]
        // GET: Company
        public ActionResult Index()
        {
            ViewBag.AreaList = CommonService.GetProvinceList();
            return View();
        }


        #region 地址管理

        public ActionResult AddressManager(int? companySysNo)
        {
            int no = companySysNo.GetValueOrDefault();
            if (!companySysNo.HasValue)
            {
                if (CurrUser.CompanySysNo > 0)
                {
                    no = CurrUser.CompanySysNo;
                }
            }
            AddressManagerModel model = new AddressManagerModel { CompanySysNo = no };

            var company = CompanyService.LoadCompany(no);
            if (company != null)
            {
                model.AreaSysNo = company.AreaSysNo;
                model.Address = company.Address;
            }
            else
            {
                model.HasError = true;
                model.ErrorMessage = "客户信息不存在可能已经被删除，请刷新页面重试。";
            }
            QF_SystemUser filter = new QF_SystemUser()
            {
                PageSize = 10000,
                PageIndex = 0,
                MasterSysNo = no,
                CommonStatus = CommonStatus.Actived
            };
            AddressFilter addFilter = new AddressFilter { PageSize = 5, AddressGrade = AddressGrade.Building, CompanySysNo = no };
            QueryResult<Address> addResult = AddressService.QueryAddressList(addFilter);
            if (addResult != null && addResult.data != null)
            {
                var root = addResult.data.Find(a => a.ParentSysNo.GetValueOrDefault() == 0);
                if (root != null)
                {
                    model.AreaSysNo = root.AreaSysNo;
                    model.Address = root.PathName;
                }
            }
            var result = user_service.QuerySystemUserList(filter);
            model.ManagerList = result.data;
            return View(model);
        }
        public ActionResult AddAdressManager(List<int> users, string code, int companySysNo)
        {
            AddressService.InsertAddressManager(users, code, companySysNo);
            return Json(new AjaxResult { Success = true });
        }
        public ActionResult LoadAddressManager(string code, int companySysNo)
        {
            List<AddressManager> info = AddressService.LoadAddressManagerByCode(code, companySysNo);
            return Json(new AjaxResult { Success = true, Data = info });
        }


        public ActionResult EditAddress(string data)
        {
            AjaxResult result = new AjaxResult
            {
                Success = true,
                Message = "保存成功",
                Data = null
            };
            Address addInfo = string.IsNullOrWhiteSpace(data) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<Address>(data, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            if (CurrUser.CompanySysNo > 0)
            {
                addInfo.CompanySysNo = CurrUser.CompanySysNo;
                addInfo.EditUserName = CurrUser.UserDisplayName;
                addInfo.EditUserSysNo = CurrUser.UserSysNo;
            }
            if (addInfo.Name == "#")
            {
                CompanyService.CreateRootAddress(addInfo.CompanySysNo);
            }
            else
            {
                if (addInfo.SysNo <= 0 && (!addInfo.ParentSysNo.HasValue || addInfo.ParentSysNo.Value <= 0))
                {
                    throw new BusinessException("父级节点不存在，请选择正确的父级节点。");
                }
                CompanyService.EditAddress(addInfo);
            }
            return Json(result);
        }

        public ActionResult CreateAddress(string data)
        {
            AddressNamePattern pattern = string.IsNullOrWhiteSpace(data) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<AddressNamePattern>(data, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            if (CurrUser.CompanySysNo > 0)
            {
                pattern.CompanySysNo = CurrUser.CompanySysNo;
            }

            string message = "";
            bool isSuccess = CompanyService.CreateAddress(pattern, CurrUser.UserSysNo, CurrUser.UserName, out message);
            AjaxResult result = new AjaxResult
            {
                Success = isSuccess,
                Message = message,
                Data = null
            };

            return Json(result);
        }
        public ActionResult DeleteAddress(int sysNo, bool isDeleteMore)
        {
            bool isSuccess = CompanyService.DeleteAddress(CurrUser.CompanySysNo, sysNo, isDeleteMore);

            AjaxResult result = new AjaxResult
            {
                Success = isSuccess,
                Data = null,
                Message = isSuccess ? "" : "删除失败，可能选中的地址下有设备存在。"
            };
            return Json(result);
        }

        public ActionResult GetAddressTree(AddressFilter filter)
        {
            AddressFilter addFilter = filter;

            if (filter == null) addFilter = new AddressFilter();
            if (CurrUser.CompanySysNo > 0)
            {
                addFilter.CompanySysNo = CurrUser.CompanySysNo;
            }
            addFilter.PageSize = 1000000;

            QueryResult<Address> result = null;
            List<Address> addList = null;
            if ((!filter.ParentSysNo.HasValue || filter.ParentSysNo <= 0) && filter.CompanySysNo == defaultCompanySysNo)
            {
                filter.SelectedAddressCode = "1003";
            }
            if (!string.IsNullOrWhiteSpace(filter.SelectedAddressCode))
            {
                addList = AddressService.LoadAddressByAddressCode(filter.SelectedAddressCode, addFilter.CompanySysNo);
            }
            if (addList == null)
            {
                result = AddressService.QueryAddressList(addFilter);
                addList = result?.data;
            }
            List<TreeInfoModel> treeList = new List<TreeInfoModel>();

            if (filter.ParentSysNo.HasValue && filter.ParentSysNo > 0)
            {

                foreach (var item in result.data)
                {
                    TreeInfoModel treeInfo = MappingTreeInfo(item, 0);
                    treeList.Add(treeInfo);
                    treeInfo.children = item.Grade != AddressGrade.Room;
                }
            }
            else
            {
                if (addList != null && addList.Count > 0)
                {
                    List<Address> firstList = addList.FindAll(item => !item.ParentSysNo.HasValue || item.ParentSysNo.Value == 0);
                    if (firstList != null && firstList.Count > 0)
                    {
                        foreach (var item in firstList)
                        {
                            TreeInfoModel treeInfo = MappingTreeInfo(item, 0);
                            treeList.Add(treeInfo);
                            SetChildTreeInfo(addList, treeInfo, item.SysNo, 0);
                        }
                    }
                }
            }
            return Json(treeList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置子节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="treeInfo"></param>
        /// <param name="parentCategoryCode">父节点CategoryID</param>
        /// <param name="selectedSysNo">选中的节点SysNo</param>
        private static void SetChildTreeInfo(List<Address> list, TreeInfoModel treeInfo, int parentID, int selectedSysNo)
        {
            if (treeInfo.children == null) treeInfo.children = true;
            if (list != null && list.Count > 0 && parentID > 0)
            {
                List<Address> childList = list.FindAll(child => child.ParentSysNo == parentID);
                if (childList != null && childList.Count > 0)
                {
                    List<TreeInfoModel> treeList = new List<TreeInfoModel>();
                    foreach (var item in childList)
                    {
                        TreeInfoModel tree = MappingTreeInfo(item, selectedSysNo);
                        treeList.Add(tree);
                        SetChildTreeInfo(list, tree, item.SysNo, selectedSysNo);
                    }
                    if (treeList != null && treeList.Count > 0)
                    {
                        treeInfo.state.opened = true;
                    }
                    treeInfo.children = treeList;
                }

            }
        }

        private static TreeInfoModel MappingTreeInfo(Address info, int selectedSysNo)
        {
            TreeInfoModel treeInfo = new TreeInfoModel();
            treeInfo.id = info.SysNo;
            treeInfo.state = new TreeStateModel();
            treeInfo.data = info;
            treeInfo.text = info.Name;
            if (info.SysNo == selectedSysNo)
            {
                treeInfo.state.selected = true;
            }
            else
            {
                treeInfo.state.selected = false;
            }
            //if (info.Grade == AddressGrade.Default)
            //{
            //    treeInfo.type = "default";
            //    treeInfo.icon = "icon-add icon-add-default";
            //}
            //else
            //{
            //    treeInfo.icon = "icon-add icon-add-" + info.Grade.ToString().ToLower();
            //}
            treeInfo.icon = "icon-add icon-add-" + info.Grade.GetValueOrDefault().ToString().ToLower();
            return treeInfo;
        }
        #endregion

        #region 图片标记

        public ActionResult SmokeMap(int? companySysNo)
        {
            int no = companySysNo.GetValueOrDefault(0);
            if (!companySysNo.HasValue)
            {
                if (CurrUser.MasterSysNo.HasValue && CurrUser.MasterSysNo > 0)
                {
                    no = CurrUser.MasterSysNo.Value;
                }
            }

            return View(no);
        }

        public JsonResult GetAddressMapSmokes(int addressSysNo, int? companySysNo = null)
        {
            if (!CurrUser.IsPMAdmin)
            {
                companySysNo = CurrUser.CompanySysNo;
            }

            var data = CompanyService.GetAddressMapSmokes(companySysNo.GetValueOrDefault(0), addressSysNo);
            return Json(new AjaxResult { Success = true, Data = data });
        }

        public JsonResult AddAddressMap(AddressMap data)
        {
            SetEntityBaseUserInfo(data);
            data.SysNo = AddressMapService.InsertAddressMap(data);
            return Json(new AjaxResult { Success = true, Data = data });
        }

        public JsonResult InsertOrUpdateAddressMapImg(AddressMap data)
        {
            SetEntityBaseUserInfo(data);
            data.SysNo = AddressMapService.InsertOrUpdateAddressMapImg(data);
            return Json(new AjaxResult { Success = true, Data = data });
        }



        public JsonResult UpdateAddressMapCoordinate(int sysNo, string markerStr)
        {
            AddressMapService.UpdateAddressMapCoordinate(sysNo, markerStr, CurrUser);
            return Json(new AjaxResult { Success = true });
        }

        public JsonResult DeleteAddressMap(int sysNo)
        {
            AddressMapService.DeleteAddressMap(sysNo);
            return Json(new AjaxResult { Success = true });
        }

        public JsonResult UpdateAddressMapName(int sysNo, string name)
        {
            AddressMapService.UpdateAddressMapName(sysNo, name, CurrUser);
            return Json(new AjaxResult { Success = true });
        }

        #endregion

        #region 客户中心

        #region 公司列表
        [ValidateInput(false)]
        public ActionResult QueryCompany()
        {
            QF_Company filter = BuildQueryFilterEntity<QF_Company>();

            var result = CompanyService.QueryCompanyList2(filter);

            return AjaxGridJson(result);
        }

        public ActionResult Maintain(int? sysNo)
        {
            Company company = new Company();
            if (CurrUser.MasterSysNo.HasValue && CurrUser.MasterSysNo.Value > 0)
            {
                sysNo = CurrUser.MasterSysNo.Value;
            }
            else
            {
                company.CompanyStatus = CompanyStatus.Init;
            }
            if (sysNo > 0)
            {
                company = CompanyService.LoadCompany(sysNo.Value);
            }
            return View(company);
        }
        [ValidateInput(false)]
        public ActionResult Save()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }
            Company entity = SerializationUtility.JsonDeserialize2<Company>(json);
            SetEntityBaseUserInfo(entity);
            entity.CompanyStatus = CompanyStatus.Init;
            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new BusinessException("请输入客户名称");
            }
            SystemUser customer = new SystemUser();
            //if (!string.IsNullOrEmpty(entity.ContactCellPhone))
            //{
            if (string.IsNullOrEmpty(entity.ContactName))
            {
                throw new BusinessException("请输入联系人姓名");
            }
            if (string.IsNullOrEmpty(entity.ContactCellPhone))
            {
                throw new BusinessException("请输入联系人手机号");
            }
            if (!WebPortalHelper.IsPhonenum(entity.ContactCellPhone))
            {
                throw new BusinessException("请输入正确的手机号!");
            }

            customer = SystemUserService.LoadSystemUserByLoginNameAndCellPhone(entity.ContactCellPhone, ConstValue.ApplicationID);
            // }

            Company company = CompanyService.LoadCompanyByName(entity.Name);
            if (entity.SysNo > 0)
            {
                Company curentcompany = CompanyService.LoadCompany(entity.SysNo, false);
                if (curentcompany == null)
                {
                    throw new BusinessException("未找到此公司的相关信息");
                }
                if (company != null && company.SysNo != curentcompany.SysNo)
                {
                    throw new BusinessException("系统中已存在当前客户，请重新输入客户名称!");
                }


                if (customer != null && customer.SysNo > 0 && ((!string.IsNullOrEmpty(entity.ContactCellPhone) && !string.Equals(customer.CellPhone, entity.ContactCellPhone)) || (entity.SysNo != customer.MasterSysNo)))
                {
                    throw new BusinessException("系统中已存在此手机号,请更换手机号重试!");
                }
                using (ITransaction it = TransactionManager.Create())
                {

                    //if (!string.IsNullOrEmpty(entity.ContactCellPhone))
                    //{
                    //    if (customer == null || !string.Equals(entity.ContactCellPhone, customer.CellPhone))
                    //    {
                    //        customer = new SystemUser();
                    //        customer.MasterSysNo = entity.SysNo;
                    //        customer.LoginName = customer.CellPhone = entity.ContactCellPhone;
                    //        customer.LoginPassword = AuthMgr.EncryptPassword(entity.ContactCellPhone);
                    //        customer.UserFullName = entity.ContactName;
                    //        customer.CommonStatus = CommonStatus.Actived;
                    //        SetEntityBaseUserInfo(customer);

                    //        var usersysno = user_service.InsertSystemUser(customer);
                    //        if (usersysno > 0)
                    //        {
                    //            if (rolesysno != 0)
                    //            {
                    //                List<Role> roles = new List<Role> { new Role { SysNo = rolesysno } };
                    //                usersrole.SaveUsersRole(usersysno, roles, ConstValue.ApplicationID);
                    //            }
                    //        }

                    //    }
                    //}
                    entity.CompanyStatus = curentcompany.CompanyStatus;
                    entity.AccountSysNo = curentcompany.AccountSysNo;
                    if (curentcompany.CompanyStatus == CompanyStatus.Invalid)
                    {
                        entity.CompanyStatus = CompanyStatus.Init;
                    }
                    CompanyService.UpdateCompany(entity);
                    it.Complete();
                }
            }
            else
            {
                if (company != null)
                {
                    throw new BusinessException("系统中已存在当前客户，请重新输入客户名称!");
                }
                if (customer != null && customer.SysNo > 0)
                {
                    throw new BusinessException("系统中已存在此手机号,请更换手机号重试!");
                }
                if (!string.IsNullOrEmpty(entity.ContactCellPhone))
                {
                    customer = new SystemUser();
                    customer.LoginName = entity.LoginName;
                    customer.CellPhone = "";
                    customer.LoginPassword = AuthMgr.EncryptPassword(entity.LoginName);
                    customer.UserFullName = entity.ContactName;
                    customer.CommonStatus = CommonStatus.Actived;
                    SetEntityBaseUserInfo(customer);
                }
                using (ITransaction it = TransactionManager.Create())
                {
                    entity.SysNo = CompanyService.InsertCompany(entity);
                    customer.MasterSysNo = entity.SysNo;
                    if (!string.IsNullOrEmpty(entity.ContactCellPhone))
                    {
                        var usersysno = user_service.InsertSystemUser(customer);
                        if (usersysno > 0)
                        {
                            entity.AccountSysNo = usersysno;
                            CompanyService.UpdateCompany(entity);

                            if (rolesysno != 0)
                            {
                                List<Role> roles = new List<Role>{new Role{SysNo=rolesysno}};
                                usersrole.SaveUsersRole(usersysno, roles, ConstValue.ApplicationID);
                            }
                        }

                    }
                    it.Complete();
                }
            }

            return Json(new AjaxResult { Success = true, Message = "保存成功", Data = entity.SysNo }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _EditLngLat(string lngStr, string latStr, string provincename, string cityname, string address)
        {
            double? lng = null;  //经度默认值
            double? lat = null;   //纬度默认值

            if (!string.IsNullOrEmpty(lngStr) && lngStr != "undefined")
            {
                lng = Convert.ToDouble(lngStr);
            }
            if (!string.IsNullOrEmpty(latStr) && latStr != "undefined")
            {
                lat = Convert.ToDouble(latStr);
            }

            ViewBag.Lng = lng;
            ViewBag.lat = lat;
            ViewBag.City = HttpUtility.UrlDecode(provincename).Trim() + HttpUtility.UrlDecode(cityname).Trim();
            ViewBag.Address = HttpUtility.UrlDecode(address);
            return PartialView("~/Views/Company/_EditLngLat.cshtml");
        }
        public ActionResult MarkAsInit()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要提交审核的企业");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            CompanyService.UpdateCompanyStatusBatch(sysNos, CompanyStatus.Init);
            return Json(new AjaxResult { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MarkAsAuthenticated()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要认证的企业");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            CompanyService.UpdateCompanyStatusBatch(sysNos, CompanyStatus.Authenticated);
            return Json(new AjaxResult { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MarkAsInvalid()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要审核驳回的企业");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            CompanyService.UpdateCompanyStatusBatch(sysNos, CompanyStatus.Invalid);
            return Json(new AjaxResult { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MarkAsDelete()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要删除的企业");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            CompanyService.DeleteCompanyBatch(sysNos);
            return Json(new AjaxResult { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult QueryInstallers()
        {
            QF_SystemUser filter = new QF_SystemUser { PageSize = 10000, CommonStatus = CommonStatus.Actived, IsPlatformManager = true };
            var result = user_service.QuerySystemUserList(filter);
            return Json(new AjaxResult { Success = true, Data = result.data });
        }

        public JsonResult QueryCompnayInstallers(int companySysNo)
        {
            var filter = new QF_CompanyInstaller { PageSize = 10000, CompanySysNo = companySysNo };
            var result = CompanyService.QueryCompanyInstallerList(filter);
            return Json(new AjaxResult { Success = true, Data = result.data });
        }

        public ActionResult UpdateInstaller(string companySysNostr, string installerSysNoStr)
        {
            var compnaySysNos = JsonConvert.DeserializeObject<List<int>>(companySysNostr);
            var installerSysNos = JsonConvert.DeserializeObject<List<int>>(installerSysNoStr);

            CompanyService.UpdateCompanyInstallerBatch(compnaySysNos, installerSysNos);
            return Json(new AjaxResult { Success = true });
        }


        #endregion

        #region 公司用户
        public ActionResult CompanyUserView(int sysno)
        {
            if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
            {
                sysno = CurrUser.MasterSysNo.Value;
            }
            if (sysno > 0)
            {
                var company = CompanyService.LoadCompany(sysno);
                if (company != null)
                {
                    ViewBag.AccountSysNo = company.AccountSysNo.GetValueOrDefault();
                }
            }
            return View();
        }


        public ActionResult QueryCompanyUser()
        {
            QF_SystemUser filter = BuildQueryFilterEntity<QF_SystemUser>();
            if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
            {
                filter.MasterSysNo = CurrUser.MasterSysNo;
            }
            var result = user_service.QuerySystemUserList(filter);
            return AjaxGridJson(result);
        }

        public ActionResult UpdateSystemUserStatus(string customerSysNos, CommonStatus status)
        {
            var sysnolist = JsonConvert.DeserializeObject<List<int>>(customerSysNos);
            if (sysnolist.Count == 0)
            {
                throw new BusinessException("请勾选账号后再操作");
            }
            if (status == CommonStatus.Deleted)
            {
                throw new BusinessException("传入状态不正确!请刷新重试!");
            }
            var cuslist = user_service.QuerySystemUserListBySysNos(sysnolist, ConstValue.ApplicationID);

            if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
            {
                cuslist = cuslist.Where(e => e.MasterSysNo == CurrUser.MasterSysNo.Value).ToList();
            }
            AjaxResult result = new AjaxResult();

            if (status == CommonStatus.Actived)
            {
                cuslist = cuslist.Where(e => e.CommonStatus == CommonStatus.DeActived).ToList();
            }
            if (status == CommonStatus.DeActived)
            {
                cuslist = cuslist.Where(e => e.CommonStatus == CommonStatus.Actived).ToList();
            }
            if (cuslist.Count == 0)
            {
                if (status == CommonStatus.Actived)
                {
                    throw new BusinessException("选择的账号已是启用状态!请重新勾选后再试!");
                }
                if (status == CommonStatus.DeActived)
                {
                    throw new BusinessException("选择的账号已是禁用状态!请重新勾选后再试!");
                }
            }
            var sysnos = cuslist.Select(e => e.SysNo).ToList();
            user_service.UpdateSystemUserStatusBatch(sysnos, status, CurrUser);
            return Json(new AjaxResult { Success = true, Data = sysnos.Count });
        }

        public ActionResult UserInfo(int sysNo = 0, int companysysno = 0)
        {
            SystemUser user = new SystemUser();
            if (companysysno == 0)
            {
                throw new BusinessException("请传入公司编号");
            }
            if (sysNo < 1)
            {
                user.CommonStatus = CommonStatus.Actived;
            }
            else
            {
                var userr = user_service.LoadSystemUserBySysNo(sysNo, ConstValue.ApplicationID);
                if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
                {
                    companysysno = CurrUser.MasterSysNo.Value;
                }
                if (userr == null || (!userr.MasterSysNo.HasValue) || (userr.MasterSysNo.Value != companysysno))
                {
                    throw new BusinessException("用户不存在");
                }
                user = userr;
            }
            user.MasterSysNo = companysysno;
            return PartialView("~/Views/Company/_UserInfo.cshtml", user);
        }

        /// <summary>
        /// 设为管理员 (注释掉)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //public ActionResult SetAdmin(int usersysno, int companysysno)
        //{
        //    AjaxResult result = new AjaxResult();
        //    if (usersysno <= 0 || companysysno <= 0)
        //    {
        //        throw new BusinessException("请选择正确的数据");
        //    }
        //    if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
        //    {
        //        companysysno = CurrUser.MasterSysNo.Value;
        //    }
        //    var company = CompanyService.LoadCompany(companysysno);
        //    if (company == null)
        //    {
        //        throw new BusinessException("未找到当前公司的相关信息");
        //    }
        //    var user = user_service.LoadSystemUser(usersysno);
        //    if (user == null)
        //    {
        //        throw new BusinessException("未找到当前用户的相关信息");
        //    }
        //    if (CurrUser.MasterSysNo.GetValueOrDefault() > 0 && user.MasterSysNo.Value != CurrUser.MasterSysNo.Value)
        //    {
        //        throw new BusinessException("未找到当前用户的相关信息");
        //    }
        //    if (user.CommonStatus == CommonStatus.DeActived)
        //    {
        //        throw new BusinessException("当前账号已被禁用，不能设为管理员");
        //    }
        //    company.AccountSysNo = usersysno;
        //    company.EditUserSysNo = CurrUser.UserSysNo;
        //    company.EditUserName = CurrUser.UserDisplayName;
        //    CompanyService.UpdateCompany(company);
        //    result.Success = true;
        //    return Json(result);
        //}

        public ActionResult SaveSystemUser(SystemUser user)
        {
            AjaxResult result = new AjaxResult();
            if (user == null || string.IsNullOrEmpty(user.LoginName))
            {
                throw new BusinessException("请输入正确的数据");
            }

            if (user.SysNo == 0)
            {
                if (string.IsNullOrEmpty(user.LoginPassword))
                {
                    user.LoginPassword = AuthMgr.EncryptPassword(user.LoginName);
                }
                else
                {
                    user.LoginPassword = AuthMgr.EncryptPassword(user.LoginPassword);
                }
                user.CellPhone = user.LoginName;
                SetEntityBaseUserInfo(user);
                if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
                {
                    user.MasterSysNo = CurrUser.MasterSysNo.Value;
                }
                var usersysno = user_service.InsertSystemUser(user);

                if (usersysno > 0)
                {
                    if (rolesysno != 0)
                    {
                        List<Role> roles = new List<Role>{
                        new Role
                        {
                            SysNo=rolesysno
                        }};
                        usersrole.SaveUsersRole(usersysno, roles, ConstValue.ApplicationID);
                    }
                    result.Data = usersysno;
                    result.Success = true;
                    return Json(result);
                }
            }
            else
            {
                var userr = user_service.LoadSystemUser(user.SysNo);
                if (userr == null)
                {
                    throw new BusinessException("未找到当前用户的相关信息");
                }
                if (CurrUser.MasterSysNo.GetValueOrDefault() > 0 && CurrUser.MasterSysNo.Value != userr.MasterSysNo)
                {
                    throw new BusinessException("未找到当前用户的相关信息");
                }
                userr.UserFullName = user.UserFullName;
                userr.CommonStatus = user.CommonStatus;
                userr.EditUserSysNo = CurrUser.UserSysNo;
                userr.EditUserName = CurrUser.UserDisplayName;
                user.EditDate = DateTime.Now;
                user_service.UpdateSystemUser(userr);
                if (rolesysno > 0)
                {
                    var roles = usersrole.GetAllRolesByUserSysNo(userr.SysNo);
                    var role = roles.Find(e => e.SysNo == rolesysno);
                    if (role == null)
                    {
                        roles.Add(new Role { SysNo = rolesysno });
                    }
                    usersrole.SaveUsersRole(userr.SysNo, roles, ConstValue.ApplicationID);
                }
                result.Data = userr.SysNo;
                result.Success = true;
                return Json(result);
            }

            result.Success = false;
            result.Message = "数据错误，请刷新后重试!";
            return Json(result);
        }


        public ActionResult ResetPwd(string loginname)
        {
            if (string.IsNullOrEmpty(loginname))
            {
                throw new BusinessException("未找到当前用户的相关信息");
            }
            var newpwd = AuthMgr.EncryptPassword(loginname);
            user_service.FindSystemUserPwd(loginname, newpwd, ConstValue.ApplicationID, CurrUser.MasterSysNo);
            return Json(new AjaxResult { Success = true });
        }
        #endregion

        #endregion

    }
}