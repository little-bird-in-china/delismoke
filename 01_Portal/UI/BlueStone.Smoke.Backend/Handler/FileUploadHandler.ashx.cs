using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BlueStone.Utility;

namespace BlueStone.Smoke.Backend
{
    /// <summary>
    /// FileUploadHandler 的摘要说明
    /// </summary>
    public class FileUploadHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            UploadResult uploadResult = null;

            if (context.Request.Files.Count == 0)
            {
                uploadResult = new UploadResult { state = UploadState.TypeNotAllow, message = "请指定上传的文件" };
                Json(uploadResult);
                return;
            }
            HttpPostedFile file = context.Request.Files[0];
            string fileUrl = string.Empty;
            long fileSize = 0;

            var originalFileName = file.FileName;
            try
            {
                //string newFileName = file.FileName.Replace(System.IO.Path.GetExtension(file.FileName), "") + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(file.FileName).ToLower();
                //string path = "UploadFiles\\";
                //string mapPath = context.Server.MapPath("~");
                //fileUrl = mapPath + "\\" + path + newFileName;
                //if (!Directory.Exists(mapPath + "\\" + path))
                //{
                //    Directory.CreateDirectory(mapPath + "\\" + path);
                //}
                //file.SaveAs(fileUrl);

                int ti = originalFileName.LastIndexOf('\\');
                if (ti >= 0)
                {
                    originalFileName = originalFileName.Substring(ti + 1);
                }
                else
                {
                    ti = originalFileName.LastIndexOf('/');
                    if (ti >= 0)
                    {
                        originalFileName = originalFileName.Substring(ti + 1);
                    }
                }
                var appName = context.Request.Params["appName"];
                string filePath = "/";

                if (!string.IsNullOrWhiteSpace(appName))
                {
                    filePath = filePath + appName + "/";
                }
                string fileFullPath = filePath + originalFileName;
                fileUrl = FileHelper.SaveFile(fileFullPath, file.InputStream, out fileSize);
            }
            catch (Exception ex)
            {
                uploadResult = new UploadResult()
                {
                    state = UploadState.ERROR,
                    url = "",
                    original = file.FileName,
                    message = ex.Message
                };
            }

            uploadResult = new UploadResult()
            {
                state = UploadState.Success,
                url = fileUrl,
                original = file.FileName,
                message = "",
                size = fileSize,
                title = originalFileName
            };
            Json(uploadResult);
        }

        private void Json(UploadResult result)
        {
            string json = SerializeHelper.JsonSerialize(new {
                state = result.state.ToString().ToUpper(),
                url = result.url,
                original = result.original,
                message = result.message,
                size = result.size,
                title = result.title
            });
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

}