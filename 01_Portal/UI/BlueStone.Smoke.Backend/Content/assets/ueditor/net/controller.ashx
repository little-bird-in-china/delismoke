<%@ WebHandler Language="C#" Class="UEditorHandler" %>

using System;
using System.Web;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

public class UEditorHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        Handler action = null;
        switch (context.Request["action"])
        {
            case "config":
                action = new ConfigHandler(context);
                break;
            case "uploadimage":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = U_Config.GetStringList("imageAllowFiles"),
                    PathFormat = U_Config.GetString("imagePathFormat"),
                    SizeLimit = U_Config.GetInt("imageMaxSize"),
                    UploadFieldName = U_Config.GetString("imageFieldName")
                });
                break;
            case "uploadscrawl":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = new string[] { ".png" },
                    PathFormat = U_Config.GetString("scrawlPathFormat"),
                    SizeLimit = U_Config.GetInt("scrawlMaxSize"),
                    UploadFieldName = U_Config.GetString("scrawlFieldName"),
                    Base64 = true,
                    Base64Filename = "scrawl.png"
                });
                break;
            case "uploadvideo":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = U_Config.GetStringList("videoAllowFiles"),
                    PathFormat = U_Config.GetString("videoPathFormat"),
                    SizeLimit = U_Config.GetInt("videoMaxSize"),
                    UploadFieldName = U_Config.GetString("videoFieldName")
                });
                break;
            case "uploadfile":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = U_Config.GetStringList("fileAllowFiles"),
                    PathFormat = U_Config.GetString("filePathFormat"),
                    SizeLimit = U_Config.GetInt("fileMaxSize"),
                    UploadFieldName = U_Config.GetString("fileFieldName")
                });
                break;
            case "listimage":
                action = new ListFileManager(context, U_Config.GetString("imageManagerListPath"), U_Config.GetStringList("imageManagerAllowFiles"));
                break;
            case "listfile":
                action = new ListFileManager(context, U_Config.GetString("fileManagerListPath"), U_Config.GetStringList("fileManagerAllowFiles"));
                break;
            case "catchimage":
                action = new CrawlerHandler(context);
                break;
            default:
                action = new NotSupportedHandler(context);
                break;
        }
        action.Process();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}