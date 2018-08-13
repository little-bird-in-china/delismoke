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
    public class FileHelper
    {
        private static string _uploadFileBaseUrl;
        public static string UploadFileBaseUrl
        {
            get
            {
                if (_uploadFileBaseUrl == null)
                {
                    _uploadFileBaseUrl = (ConfigurationManager.AppSettings["ImageStorageServerDomain"] ?? "/f").Trim();
                    if (!_uploadFileBaseUrl.StartsWith("/") && !_uploadFileBaseUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _uploadFileBaseUrl = "/" + _uploadFileBaseUrl;
                    }
                    _uploadFileBaseUrl = _uploadFileBaseUrl ?? "";
                    _uploadFileBaseUrl = _uploadFileBaseUrl.TrimEnd('/', '\\');
                }
                return _uploadFileBaseUrl;

            }
        }

        public static string GetUploadFileUrl(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return null;
            return UploadFileBaseUrl + "/" + fileUrl.TrimStart('/', '\\');
        }

        private static string _uploadFolder;
        public static string UploadFolder
        {
            get
            {
                if (_uploadFolder == null)
                {
                    string path = (ConfigurationManager.AppSettings["UploadFileBaseFolder"] ?? "~/").Trim();
                    if (path.IndexOf(":") >= 0 || path.StartsWith("/"))
                    {
                        _uploadFolder = path;
                    }
                    if (path.IndexOf(":") > 0)
                    {
                        _uploadFolder = path;
                    }
                    else if (path.StartsWith("~"))
                    {
                        _uploadFolder = System.Web.HttpContext.Current.Server.MapPath(path);
                    }
                    else
                    {
                        path = path.TrimStart('\\', '/');
                        _uploadFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
                    }
                    if (!Directory.Exists(_uploadFolder))
                    {
                        Directory.CreateDirectory(_uploadFolder);
                    }
                }
                return _uploadFolder;

            }
        }

        public static void SaveFileData(string fileName, Stream fileContent, out bool isExists)
        {
            isExists = false;
            if (string.IsNullOrWhiteSpace(fileName)) return;
            string filePath = Path.Combine(UploadFolder, fileName);

            if (File.Exists(filePath))
            {
                isExists = true;
                return;
            }
            string d = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d);
            }
            fileContent.Position = 0;
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                int maxBufLength = 2048;
                int length = maxBufLength;
                do
                {
                    long lastLength = fileContent.Length - fileContent.Position;
                    byte[] buffer = new byte[length];
                    if (lastLength < length)
                    {
                        length = (int)lastLength;
                    }
                    fileContent.Read(buffer, 0, length);
                    stream.Write(buffer, 0, length);
                }
                while (length == maxBufLength);
            }
        }
        public static byte[] ReadFileData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return new byte[0];
            string filePath = Path.Combine(UploadFolder, fileName);
            byte[] buffer = null;
            if (File.Exists(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                }
                return buffer;
            }
            return null;
        }

        public static bool DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            string filePath = Path.Combine(UploadFolder, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }

        public static string SaveFile(string fileFullName, Stream fileContent)
        {
            string fileName = fileFullName.TrimStart('/', '\\');
            bool isExists;
            SaveFileData(fileName, fileContent, out  isExists);
            return fileFullName;
        }

        public static bool IsExists(string fileFullName)
        {
            string filePath = Path.Combine(UploadFolder, fileFullName.TrimStart(new char[] { '\\', '/' }));
            return File.Exists(filePath);
        }


        public static string SaveFile(string fileFullName, Stream stream, out long fileSize, Dictionary<int, int> imageSuiteSizes = null, string watermarkPic = null)
        {
            int i = fileFullName.LastIndexOf('/');
            string fileName = fileFullName.Substring(i + 1);
            string filePath = fileFullName.Substring(0, i + 1);
            string[] pics = { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

            var fileExtension = Path.GetExtension(fileName).ToLower();
            fileSize = stream.Length;

            //byte[] fbuffer = new byte[stream.Length];
            //stream.Position = 0;
            //stream.Read(fbuffer, 0, (int)stream.Length);
            //stream.Position = 0;

            string fileUrl = null;
            bool isImg = string.Join(",", pics).Contains(fileExtension);
            if (isImg)
            {
                using (Stream newStream = Rotate(stream))
                {
                    newStream.Position = 0;
                    string fileMD5Value = GetMD5Value(newStream).ToLower();
                    newStream.Position = 0;
                    fileName = fileMD5Value + fileExtension;
                    fileFullName = filePath + fileName;
                    fileSize = newStream.Length;
                    bool isExists = IsExists(fileFullName);
                    if (isExists)
                    {
                        if (fileFullName.StartsWith("/")) return fileFullName;
                        return "/" + fileFullName;
                    }
                    newStream.Position = 0;
                    fileUrl = SaveFile(fileFullName, newStream);
                }
            }
            else
            {
                stream.Position = 0;
                string fileMD5Value = GetMD5Value(stream).ToLower();
                stream.Position = 0;
                fileFullName = filePath + fileMD5Value + fileExtension;
                bool isExists = IsExists(fileFullName);
                if (isExists)
                {
                    if (fileFullName.StartsWith("/")) return fileFullName;
                    return "/" + fileFullName;
                }
            }

            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                stream.Position = 0;
                fileUrl = SaveFile(fileFullName, stream);
            }
            if (fileUrl.StartsWith("/")) return fileUrl;
            return "/" + fileUrl;
        }

        /// <summary>
        /// 解决 Iphone 拍照上传图片显示不正确，原因是浏览器无法根据图片的显示属性来显示图片，所以需要将图片旋转正确位置后保存。
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="imgPath"></param> 
        /// <returns></returns>
        public static Stream Rotate(Stream stream)
        {
            using (Image img = Image.FromStream(stream, true))
            {
                foreach (var p in img.PropertyItems)
                {
                    // Id=0x0112 表示图片查看方位信息
                    //PropertyItem np = p;// new PropertyItem() { Id = p.Id, Len = p.Len, Type = p.Type, Value = new byte[] { 1, 0 } };
                    if (p.Id == 274)
                    {
                        byte[] value = p.Value as byte[];
                        byte orientation = value[0];
                        PropertyItem np = p;
                        np.Value = new byte[] { 1, 0 };
                        switch (orientation)
                        {
                            case 3://180;
                                img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                img.SetPropertyItem(np);
                                break;
                            case 6:// 90;
                                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                img.SetPropertyItem(np);
                                break;
                            case 8://-90 or 270; // case 5:
                                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                img.SetPropertyItem(np);
                                break;
                        }
                        break;
                    }
                }
                ImageConverter imgconv = new ImageConverter();

                //Stream imgStream = (Stream)imgconv.ConvertTo(img, typeof(Stream));
                //return imgStream;
                MemoryStream imgStream = new MemoryStream();
                img.Save(imgStream, img.RawFormat);
                imgStream.Position = 0;
                return imgStream;
            }
        }

        public static string BytesToHexString(byte[] buffer)
        {
            if (buffer == null) return null;
            StringBuilder hexBuilder = new StringBuilder();
            foreach (byte b in buffer)
            {
                hexBuilder.Append(b.ToString("X2"));
            }
            return hexBuilder.ToString();
        }
        public static string GetMD5Value(Stream stream)
        {
            stream.Position = 0;
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBuffer = md5.ComputeHash(stream);
            return BytesToHexString(hashBuffer);
        }
    }

}