using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace BlueStone.Smoke.Backend.Untility
{
    public class PortalUtility
    {

        public static Stream GetUrlImage(string url)
        {
            try
            {

                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }
        private static Image ResizeImage(Image bmp, int newW, int newH)
        {
            try
            {
                Image img = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(img);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return img;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 本地文件上传到远程服务器
        /// </summary>
        /// <param name="localFile">本地文件路径</param>
        /// <param name="remoteUrl">远程图片服务器上传地址</param>
        /// <param name="dic">携带的上传参数</param>
        /// <returns></returns>
        public static string UploadFileToRemote(string localFile, string remoteUrl, Dictionary<string, string> dic)
        {
            if (!File.Exists(localFile))
            {
                Logger.WriteLog(string.Format("本地文件不存在。"));
                return string.Empty;
            }

            FileStream fileStream = new FileStream(localFile, FileMode.Open);

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(remoteUrl);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" +
            boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;



            Stream memStream = new System.IO.MemoryStream();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");


            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            foreach (string key in dic.Keys)
            {
                string formitem = string.Format(formdataTemplate, key, dic[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }


            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";

            var fileExtension = Path.GetExtension(localFile).ToLower();
            string header = string.Format(headerTemplate, "uploadfile", DateTime.Now.Ticks.ToString("x") + fileExtension);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            byte[] buffer = new byte[1024];

            int bytesRead = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            fileStream.Close();


            httpWebRequest.ContentLength = memStream.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();


            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string responseContent = string.Empty;

            using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            {
                responseContent = httpStreamReader.ReadToEnd();
            }

            return responseContent;

        }

    }
}