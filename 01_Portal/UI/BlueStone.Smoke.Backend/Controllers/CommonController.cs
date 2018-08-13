using BlueStone.Smoke.Service;
using BlueStone.Utility;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    public class CommonController : BaseController
    {
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadFile()
        {
            string file = Request["filePath"];
            if (string.IsNullOrEmpty(file))
            {
                throw new BusinessException("没有找到文件" + file);
            }
            string filePath;
            string fileName = file.Substring(file.LastIndexOf("/") + 1, file.Length - file.LastIndexOf("/") - 1);
            if (file.ToLower().IndexOf("http") > 0)
            {
                filePath = file;
            }
            filePath = Server.MapPath(file);
            return File(filePath, "application/octet-stream", fileName);
        }


        public JsonResult SaveFileInfo(string data)
        {
            data = data == null ? null : HttpUtility.UrlDecode(data);
            BlueStone.Smoke.Entity.FileInfo fileInfo = data == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<BlueStone.Smoke.Entity.FileInfo>(data, new Newtonsoft.Json.JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            });
            if (data == null)
            {
                return new JsonResult() { Data = 0 };
            }

            fileInfo.CreateUserName = CurrUser.UserDisplayName;
            fileInfo.CreateUserSysNo = CurrUser.UserSysNo;
            int result = 0;
            if (fileInfo.IsDeleted)
            {
                CommonService.DeleteFileInfo(fileInfo.MasterType.Value, fileInfo.MasterID, fileInfo.CategoryName);
            }
            else
            {
                result = CommonService.InsertFileInfo(fileInfo);

            }

            fileInfo.SysNo = result;
            return Json(new AjaxResult { Success = true, Message = "保存成功", Data = result });
        }

        public JsonResult BatchSaveFileInfo(List<BlueStone.Smoke.Entity.FileInfo> files)
        {
            if (files == null || files.Count <= 0)
            {
                return new JsonResult() { Data = 0 };
            }

            foreach (var fileInfo in files)
            {
                if (!string.IsNullOrEmpty(fileInfo.FileRelativePath))
                {
                    fileInfo.CreateUserName = CurrUser.UserDisplayName;
                    fileInfo.CreateUserSysNo = CurrUser.UserSysNo;
                    if (fileInfo.IsDeleted)
                    {
                        CommonService.DeleteFileInfo(fileInfo.MasterType.Value, fileInfo.MasterID, fileInfo.CategoryName);
                    }
                    else
                    {
                        CommonService.InsertFileInfo(fileInfo);
                    }
                }
            }
            return Json(new AjaxResult { Success = true, Message = "保存成功" });
        }

        public JsonResult DeleteFile(long key)
        {
            //CommonService.DeleteFileInfo(key, CurrUser.CompanySysNo);
            return Json(new AjaxResult { Success = true, Message = "修改成功" });

        }

        [HttpGet]
        public ActionResult DownloadRemoteFile(int? sysNo)
        {
            if (!sysNo.HasValue || sysNo <= 0)
            {
                throw new BusinessException("没有检测到要下载的文件。");
            }
            BlueStone.Smoke.Entity.FileInfo file = CommonService.LoadFileInfoBySysNo(sysNo.Value);
            if (file == null || file.SysNo <= 0)
            {
                throw new BusinessException("没有检测到要下载的文件。");
            }
            string fileHost = BlueStone.Utility.AppSettingManager.GetSetting("base", "ImageStorageServerDomain");
            string remoteUrl = fileHost + file.FileRelativePath;
            byte[] fileData;
            try
            {
                using (WebClient client = new WebClient())
                {
                    fileData = client.DownloadData(remoteUrl);
                    return File(fileData, "text/plain", file.FileName);
                }
            }
            catch
            {
                throw new BusinessException("文件下载失败。");
            }
        }
    }
}