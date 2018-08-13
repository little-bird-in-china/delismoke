using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlueStone.Smoke.Service
{
    public class CompanyService
    {

        /// <summary>
        /// 创建Company信息
        /// </summary>
        public static int InsertCompany(Company entity)
        {
            CheckCompany(entity, true);
            entity.SysNo = CompanyDA.InsertCompany(entity);
            AddressDA.CreateRootAddress(entity.SysNo);
            return entity.SysNo;
        }

        /// <summary>
        /// 加载Company信息
        /// </summary>
        public static Company LoadCompany(int sysNo, bool loadFileinfo = true)
        {
            Company company = CompanyDA.LoadCompany(sysNo);
            if (company == null)
            {
                return null;
            }
            //获取企业系统管理员账号

            var sysUserInfo = SystemUserDA.LoadSystemUser(company.AccountSysNo.GetValueOrDefault(), ConstValue.ApplicationID);
            if (sysUserInfo != null && sysUserInfo.SysNo > 0)
            {
                company.LoginName = sysUserInfo.LoginName;
            }
            if (loadFileinfo)
            {
                var result = CommonDA.QueryFileInfoList(new FileInfoFilter { PageSize = 100, MasterType = FileMasterType.CompanyBasic, MasterID = sysNo });
                company.FileList = result?.data;
            }
            return company;
        }

        /// <summary>
        /// 更新Company信息
        /// </summary>
        public static void UpdateCompany(Company entity)
        {
            CheckCompany(entity, false);
            Company company = CompanyDA.LoadCompany(entity.SysNo);

            if (company == null)
            {
                throw new BusinessException("查找不到企业");
            }
            CompanyDA.UpdateCompany(entity);
            AddressDA.CreateRootAddress(entity.SysNo);
        }
        public static List<Company> LoadAllCompany()
        {
            return CompanyDA.LoadAllCompany();
        }
        public static Company LoadCompanyByName(string name)
        {
            return CompanyDA.LoadCompanyByName(name);
        }
        /// <summary>
        /// 分页查询Company信息
        /// </summary>
        public static QueryResult<Company> QueryCompanyList2(QF_Company filter)
        {
            return CompanyDA.QueryCompanyList2(filter);
        }


        /// <summary>
        /// 分页查询Company信息
        /// </summary>
        public static QueryResult<Company> QueryCompanyList(QF_Company filter)
        {
            return CompanyDA.QueryCompanyList(filter);
        }

        /// <summary>
        /// 检查Company信息
        /// </summary>
        private static void CheckCompany(Company entity, bool isCreate)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new BusinessException(LangHelper.GetText("企业名称不能为空！"));
            }
            if (entity.Name.Length > 100)
            {
                throw new BusinessException(LangHelper.GetText("企业名称长度不能超过100！"));
            }

            if (entity.Phone != null && entity.Phone.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("企业电话长度不能超过40！"));
            }

            if (entity.ContactName != null && entity.ContactName.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("联系人长度不能超过40！"));
            }
            if (entity.ContactCellPhone != null && entity.ContactCellPhone.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("联系人手机长度不能超过40！"));
            }
            if (entity.Address != null && entity.Address.Length > 100)
            {
                throw new BusinessException(LangHelper.GetText("办公地址长度不能超过100！"));
            }
            if (entity.LngLat != null && entity.LngLat.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("经纬度长度不能超过40！"));
            }

            if (entity.InUserName != null && entity.InUserName.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("创建人长度不能超过40！"));
            }
            if (entity.EditUserName != null && entity.EditUserName.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("修改人长度不能超过40！"));
            }
        }


        public static void UpdateCompanyStatusBatch(IEnumerable<int> sysNos, CompanyStatus status)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的数据编号");
            }
            int errorCount = 0;
            StringBuilder errMsg = new StringBuilder();
            foreach (int sysNo in sysNos)
            {
                try
                {
                    UpdateCompanyStatus(sysNo, status);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    errMsg.AppendFormat("经销商编号：{0}{1}。", sysNo, ex.Message);
                }
            }
            if (errorCount > 0)
            {
                throw new BusinessException(string.Format("经销商操作成功{0}个，失败{1}个，异常信息：{2}", (sysNos.Count() - errorCount), errorCount, errMsg));
            }

            //CompanyDA.UpdateCompanyStatusBatch(sysNos, status);
        }

        public static void UpdateCompanyStatus(int sysNo, CompanyStatus status)
        {
            var company = CompanyDA.LoadCompany(sysNo);
            if (company == null)
            {
                throw new BusinessException("查找不到企业！");
            }
            if (status == CompanyStatus.Invalid)
            {
                if (company.CompanyStatus == CompanyStatus.Invalid)
                {
                    throw new BusinessException("已标记为认证未通过！");
                }
            }
            else if (status == CompanyStatus.Authenticated)
            {
                if (company.CompanyStatus == CompanyStatus.Authenticated)
                {
                    throw new BusinessException("已标记为认证通过");
                }
            }
            else if (status == CompanyStatus.Init)
            {
                if (company.CompanyStatus == CompanyStatus.Init)
                {
                    throw new BusinessException("已标记为待认证");
                }
            }
            CompanyDA.UpdateCompanyStatus(sysNo, status);
        }

        public static List<FileInfo> LoadCompanyFiles(int sysNo)
        {
            var result = CommonDA.QueryFileInfoList(new FileInfoFilter { PageSize = 100, MasterType = FileMasterType.CompanyBasic, MasterID = sysNo });
            return result?.data;
        }

        public static void DeleteCompanyBatch(IEnumerable<int> sysNos)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要删除的客户");
            }
            CompanyDA.DeleteCompanyBatch(sysNos);
        }

        private void CreateRootAddress(int companySysNo, string companyName, int userSysNo, string userName)
        {


        }

        private static bool CheckAddressPattern(AddressNamePattern pattern, out string message)
        {
            message = null;
            if (pattern.NoType == AddressNameNoType.Char)
            {
                string beginNo = pattern.BeginNo == null ? "" : pattern.BeginNo.Trim();
                if (beginNo.Length != 1)
                {
                    message = "起始编号为空或格式不正确，为字母时只能包括一个字母。";
                    return false;
                }
                string endNo = pattern.EndNo == null ? "" : pattern.EndNo.Trim();
                if (endNo.Length != 1)
                {
                    message = "结束编号为空或格式不正确，为字母时只能包括一个字母。";
                    return false;
                }


                beginNo = beginNo.ToUpper();
                pattern.BeginNo = beginNo;
                endNo = endNo.ToUpper();
                pattern.EndNo = endNo;
                if (beginNo[0] < 'A' || endNo[0] > 'Z')
                {
                    message = "开始和结束编号为空或格式不正确，为字母时只能包括一个字母。";
                    return false;
                }
                if (beginNo[0] > endNo[0])
                {
                    message = "结束编号不能大于开始编号。";
                    return false;
                }
            }
            else
            {
                int beginNo = 0;
                if (!int.TryParse(pattern.BeginNo, out beginNo) || beginNo < 0)
                {
                    message = "起始编号为空或格式不正确，编号为数字且不能小于0。";
                    return false;
                }
                int endNo = 0;
                if (!int.TryParse(pattern.EndNo, out endNo) || endNo < 0)
                {
                    message = "结束编号为空或格式不正确，编号为数字且不能小于0。";
                    return false;
                }
                if (beginNo > endNo)
                {
                    message = "结束编号不能大于结束编号。";
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(pattern.PreName) && pattern.PreName.Length > 10)
            {
                message = pattern.ParentNoAsPreName ? "与父级编号的分隔符长度不能超过10个字符" : "名称前缀不能超过10个字符！";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(pattern.SufName) && pattern.SufName.Length > 10)
            {
                message = "名称后缀不能超过10个字符！";
                return false;
            }
            bool result = false;
            if (pattern.ChildPattern != null)
            {
                result = CheckAddressPattern(pattern.ChildPattern, out message);
                if (!result)
                {
                    return result;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iniCode">初始编码</param>
        /// <param name="length">GenerateCode长度</param>
        /// <param name="codeType">1:仅数字编码，2:仅字母编码，3:数字和字母编码</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        private static string GenerateCode(string iniCode, int length, int codeType = 1, int step = 1)
        {
            if (length < 1) return "";
            if (step < 1) return iniCode;
            string numberList = "0123456789";
            string charList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string allList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string codeCharList = codeType == 1 ? numberList : (codeType == 2 ? charList : allList);

            int charsLength = codeCharList.Length;
            char padChar = codeCharList[0];
            if (string.IsNullOrWhiteSpace(iniCode))
            {
                iniCode = "".PadLeft(length, padChar);
            }
            iniCode = iniCode.Trim().ToUpper();
            char[] noChars = iniCode.ToCharArray();
            char[] newNoChars = new char[noChars.Length];
            for (int i = 0; i < newNoChars.Length; i++)
            {
                newNoChars[i] = noChars[i];
            }

            int nextStep = step; //进位数
            int thisIndex = 0;//当前位字符在charString中的Index;

            for (int i = noChars.Length - 1; i >= 0; i--)
            {
                int thisStep = nextStep;
                thisIndex = codeCharList.IndexOf(noChars[i]);
                thisIndex = thisIndex + thisStep;

                nextStep = thisIndex / charsLength;

                if (nextStep > 0)
                {
                    thisIndex = thisIndex % charsLength;
                }

                newNoChars[i] = codeCharList[thisIndex];
                if (nextStep == 0)
                {
                    break;
                }
            }
            string newNoString = new string(newNoChars);
            while (nextStep > 0)
            {
                thisIndex = nextStep;
                if (thisIndex >= charsLength)
                {
                    thisIndex = nextStep % charsLength;
                }
                newNoString = codeCharList[thisIndex] + newNoString;
                nextStep = nextStep / charsLength;
            }
            return newNoString.PadLeft(length, padChar);
        }

        private static List<Address> BuildAddress(AddressNamePattern pattern, Address parent, string beginAddressCode)
        {

            string beginNo = pattern.BeginNo.Trim().ToUpper();
            string endNo = pattern.EndNo.Trim().ToUpper();
            string nameTemplate = pattern.PreName + "{0}" + pattern.SufName;
            if (pattern.ParentNoAsPreName)
            {
                nameTemplate = parent.Number + pattern.PreName + "{0}" + pattern.SufName;
            }
            List<Address> addList = new List<Address>();
            string code = null;
            string parentPathName = parent.PathName;
            if (parent.ParentSysNo.GetValueOrDefault() == 0)
            {
                parentPathName = parent.Name;
            }
            if (pattern.NoType == AddressNameNoType.Digit)
            {
                int bNo = int.Parse(beginNo);
                int eNo = int.Parse(endNo);
                for (int i = bNo; i <= eNo; i++)
                {
                    if (string.IsNullOrWhiteSpace(beginAddressCode))
                    {
                        code = parent.Code + "00";
                    }
                    else
                    {
                        code = GenerateCode(beginAddressCode, beginAddressCode.Length, 3);
                    }
                    beginAddressCode = code;
                    string number = i.ToString().PadLeft(pattern.NoLength, '0');
                    string name = string.Format(nameTemplate, number);

                    Address add = new Address()
                    {
                        CompanySysNo = pattern.CompanySysNo,
                        Grade = pattern.Grade,
                        Number = i.ToString(),
                        Name = name,
                        ParentSysNo = parent.SysNo,
                        Code = code,
                        PathName = parentPathName + ">" + name,
                    };
                    addList.Add(add);
                }
            }
            else
            {
                char bNo = char.Parse(beginNo);
                char eNo = char.Parse(endNo);
                for (char i = bNo; i <= eNo; i++)
                {
                    if (string.IsNullOrWhiteSpace(beginAddressCode))
                    {
                        code = parent.Code + "00";
                    }
                    else
                    {
                        code = GenerateCode(beginAddressCode, beginAddressCode.Length, 3);
                    }
                    beginAddressCode = code;
                    string number = i.ToString().PadLeft(pattern.NoLength, 'A');
                    string name = string.Format(nameTemplate, number);
                    Address add = new Address()
                    {
                        CompanySysNo = pattern.CompanySysNo,
                        Grade = pattern.Grade,
                        Number = i.ToString(),
                        Name = name,
                        ParentSysNo = pattern.ParentSysNo,
                        Code = code,
                        PathName = parentPathName + ">" + name,
                    };
                    addList.Add(add);
                }
            }
            return addList;
        }

        private static Dictionary<int, object> CompanyAddressCreateLocker = new Dictionary<int, object>();
        private static Dictionary<int, int> CompanyAddressCreateCount = new Dictionary<int, int>();

        private static object LockerForCompanyAddressCreateLocker = new object();

        private static object GetCompanyAddressCreateLockerByKey(int companySysNo)
        {
            object locker = null;
            if (CompanyAddressCreateLocker.ContainsKey(companySysNo))
            {
                locker = CompanyAddressCreateLocker[companySysNo];
            }
            else
            {
                lock (LockerForCompanyAddressCreateLocker)
                {
                    if (CompanyAddressCreateLocker.ContainsKey(companySysNo))
                    {
                        locker = CompanyAddressCreateLocker[companySysNo];
                    }
                    else
                    {
                        locker = new object();
                        CompanyAddressCreateLocker[companySysNo] = locker;
                    }
                }
            }
            return locker;
        }

        public static bool CreateAddress(AddressNamePattern pattern, int userSysNo, string userName, out string message)
        {
            message = null;
            bool result = CheckAddressPattern(pattern, out message);
            if (!result)
            {
                return false;
            }
            int companySysNo = pattern.CompanySysNo;

            if (companySysNo <= 0)
            {
                message = "客户信息不存在，请选择客户后再创建地址信息。";
                return false;
            }

            object locker = GetCompanyAddressCreateLockerByKey(companySysNo);
            lock (locker)
            {
                if (CompanyAddressCreateCount.ContainsKey(companySysNo))
                {
                    CompanyAddressCreateCount[companySysNo] = CompanyAddressCreateCount[companySysNo] + 1;
                }
                else
                {
                    CompanyAddressCreateCount.Add(companySysNo, 1);
                }
                try
                {
                    Address parent = AddressDA.LoadAddress(pattern.ParentSysNo);

                    if (parent == null || parent.CompanySysNo != companySysNo)
                    {
                        message = "按规则创建地址时，请指定父级地址。如果是根地址，则只能单个创建。";
                        return false;
                    }

                    string beginAddressCode = AddressDA.GetChildrenMaxCode(companySysNo, pattern.ParentSysNo);

                    List<Address> addList = BuildAddress(pattern, parent, beginAddressCode);

                    AddressDA.InsertAddress(companySysNo, pattern.ParentSysNo, ref addList, userSysNo, userName);
                    if (pattern.ChildPattern != null) pattern.ChildPattern.CompanySysNo = companySysNo;
                    Task task = new Task(() =>
                    {
                        var aList = addList;
                        var p = pattern.ChildPattern;
                        var uno = userSysNo;
                        var uname = userName;
                        BuildAddSaveChildrenAddress(aList, p, uno, uname);
                    });
                    task.Start();
                }
                finally
                {
                    CompanyAddressCreateCount[companySysNo] = CompanyAddressCreateCount[companySysNo] - 1;
                }
            }
            return true;
        }
        private static void BuildAddSaveChildrenAddress(List<Address> pAddList, AddressNamePattern childPattern, int userSysNo, string userName)
        {
            if (childPattern == null)
            {
                return;
            }
            foreach (var p in pAddList)
            {
                if (p.SysNo < 1) continue;
                //p.EditUserName 中暂存了子节点最大的编码
                List<Address> addList = BuildAddress(childPattern, p, p.EditUserName);
                AddressDA.InsertAddress(p.CompanySysNo, p.SysNo, ref addList, userSysNo, userName);
                if (childPattern.ChildPattern != null) childPattern.ChildPattern.CompanySysNo = childPattern.CompanySysNo;
                BuildAddSaveChildrenAddress(addList, childPattern.ChildPattern, userSysNo, userName);
            }
        }

        /// <summary>
        /// 删除Address信息
        /// </summary>
        public static bool DeleteAddress(int companySysNo, int sysNo, bool isDeleteMoreNode = true)
        {
            Address addInfo = AddressDA.LoadAddress(sysNo);
            if (addInfo == null) return true;
            if (companySysNo > 0 && addInfo.CompanySysNo != companySysNo)
            {
                return true;
            }

            return AddressDA.DeleteAddress(sysNo, isDeleteMoreNode);
        }


        /// <summary>
        /// 检查Address信息
        /// </summary>
        private static void CheckAddress(Address entity)
        {
            if (entity.Name != null && entity.Name.Length > 40)
            {
                throw new BusinessException(LangHelper.GetText("名称长度不能超过40个字符！"));
            }
        }

        public static void CreateRootAddress(int companySysNo)
        {
            AddressDA.CreateRootAddress(companySysNo);
        }
        public static void EditAddress(Address entity)
        {
            CheckAddress(entity);
            if (entity.SysNo > 0)
            {
                var o = AddressDA.LoadAddress(entity.SysNo);
                if (o == null || o.CompanySysNo != entity.CompanySysNo)
                {
                    return;
                }
                Address parent = o.ParentSysNo.GetValueOrDefault() <= 0 ? null : AddressDA.LoadAddress(o.ParentSysNo.Value);
                entity.PathName = entity.Name;
                if (parent != null)
                {
                    string parentPathName = parent.PathName;
                    if (parent.ParentSysNo.GetValueOrDefault() == 0)
                    {
                        parentPathName = parent.Name;
                    }
                    entity.PathName = parentPathName + ">" + entity.Name;
                    //  entity.AreaSysNo = 0;
                }
                entity.ParentSysNo = o.ParentSysNo;
                AddressDA.UpdateAddress(entity);
            }
            else
            {
                string code = "";
                int parentSysNo = entity.ParentSysNo.GetValueOrDefault();
                string beginAddressCode = AddressDA.GetChildrenMaxCode(entity.CompanySysNo, parentSysNo);
                Address parent = parentSysNo <= 0 ? null : AddressDA.LoadAddress(parentSysNo);
                if (parentSysNo > 0 && (parent == null || parent.CompanySysNo != entity.CompanySysNo))
                {
                    throw new BusinessException("父级节点不存在，请选择正确的父级节点。");
                }
                if (string.IsNullOrWhiteSpace(beginAddressCode))
                {

                    code = parent == null ? "10" : (parent.Code + "00");
                }
                else
                {
                    code = GenerateCode(beginAddressCode, beginAddressCode.Length, 3);
                }
                entity.Code = code;
                entity.PathName = entity.Name;
                if (parent != null)
                {
                    string parentPathName = parent.PathName;
                    if (parent.ParentSysNo.GetValueOrDefault() == 0)
                    {
                        parentPathName = parent.Name;
                    }
                    entity.PathName = parentPathName + ">" + entity.Name;
                    // entity.AreaSysNo = 0;
                }
                entity.SysNo = AddressDA.InsertAddress(entity);
            }
        }

        public static SmokeMap GetAddressMapSmokes(int companySysNo, int addressId)
        {
            var model = new SmokeMap();
            model.AddressNo = addressId;
            model.AddressMaps = AddressMapDA.QueryAddressMapList(new AddressMapFilter { AddressSysNo = addressId, PageSize = 100000 }).data;

            //标记点分两种 烟感器 和 下级地址
            model.Smokes = new List<AddressMapMarker>();
            model.Markers = new List<AddressMapMarker>();
            var smokes = SmokeDetectorDA.GetSmokeDetectorByAddressSysNo(companySysNo, new List<int> { addressId });
            if (smokes != null)
            {
                smokes.ForEach(a =>
                {
                    model.Smokes.Add(new AddressMapMarker { SysNo = a.SysNo, Name = string.IsNullOrEmpty(a.Position) ? a.Code : a.Position, Type = AddressMapMarkerType.SmokeDetector, });
                });
            }

            var address = AddressDA.GetTopLevelChildren(addressId);
            if (address != null)
            {
                address.ForEach(a =>
                {
                    model.Smokes.Add(new AddressMapMarker { SysNo = a.SysNo, Name = a.Name, Type = AddressMapMarkerType.Address, });
                });
            }

            return model;
        }

        /// <summary>
        /// 分页查询CompanyInstaller信息
        /// </summary>
        public static QueryResult<CompanyInstaller> QueryCompanyInstallerList(QF_CompanyInstaller filter)
        {
            return CompanyInstallerDA.QueryCompanyInstallerList(filter);
        }


        public static void UpdateCompanyInstallerBatch(List<int> companySysNos, List<int> installerSysNos)
        {
            CompanyInstallerDA.UpdateCompanyInstallerBatch(companySysNos, installerSysNos);
        }


        /// <summary>
        /// 获取公司的用户SysNo
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static string GetCompanyUserNoStr(int companySysNo)
        {
            return CompanyDA.GetCompanyUserNoStr(companySysNo);
        }


        public static Company GetCompanyUser(int systemusersysno)
        {
            return CompanyDA.GetCompanyUser(systemusersysno);
        }

    }
}