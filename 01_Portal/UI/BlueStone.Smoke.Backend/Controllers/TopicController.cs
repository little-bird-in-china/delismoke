using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Controllers;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Service;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.ERP.Controllers
{
    [Auth(FunctionKeys.PM_Topic_All)]
    public class TopicController : BaseController
    {
        #region 视图控制
        /// <summary>
        /// 新闻列表
        /// </summary>
        public ActionResult TopicList()
        {
            ViewBag.CurrUser = CurrUser;
            return View();
        }


        /// <summary>
        /// 新闻维护
        /// </summary>
        [ValidateInput(false)]
        public ActionResult Maintain(int? sysno)
        {
            TopicInfo topic = new TopicInfo();
            if (sysno > 0)
            {
                topic = TopicService.LoadTopicInfoBySysNo(sysno.Value);
            }
            return View(topic);
        }

        public ActionResult TopicCategoryList(string MasterName="Topic")
        {
            ViewBag.MasterName = MasterName;
            List<TopicCategory> allCategory = new List<Smoke.Entity.TopicCategory>();
            List<TopicCategory> result = new List<Smoke.Entity.TopicCategory>();
            if (!string.IsNullOrEmpty(MasterName) && !string.IsNullOrWhiteSpace(MasterName))
            {
                allCategory = TopicService.QueryAllTopicCategoryListByMasterName(MasterName) ?? new List<Smoke.Entity.TopicCategory>();
                if (allCategory == null)
                {
                    allCategory = new List<TopicCategory>();
                }
                result = allCategory.Where(p => p.CommonStatus == CommonStatus.Actived).ToList();
            }
            return View(result);
        }
        public ActionResult TopicCategoryInfo(int sysNo, string MasterName)
        {
            TopicCategory info = new TopicCategory();
            if (sysNo > 0)
            {
                info = TopicService.LoadTopicCategory(sysNo) ?? new Smoke.Entity.TopicCategory();
                if (info.SysNo <= 0)
                {
                    info.MasterName = MasterName;
                }
            }
            else
            {
                info.MasterName = MasterName;
            }
            return PartialView("~/Views/Topic/_TopicCategoryInfo.cshtml", info);
        }
        #endregion

        #region 异步请求
        /// <summary>
        /// 查询新闻列表
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult QueryTopicList()
        {
            QF_Topic filter = BuildQueryFilterEntity<QF_Topic>();
            QueryResult<QR_Topic> result = TopicService.QueryTopicList(filter);
            return AjaxGridJson(result);
        }

        /// <summary>
        /// 新闻详情
        /// </summary>
        public ActionResult LoadDetail(long SysNo)
        {
            if (SysNo < 1)
            {
                throw new BusinessException("文章编号不存在");
            }
            TopicInfo data = TopicService.LoadTopicInfoBySysNo(int.Parse(SysNo.ToString()));
            if (data == null)
            {
                throw new BusinessException("文章不存在");
            }

            return Json(new
            {
                Success = true,
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存文章信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AjaxSaveTopicInfo(TopicInfo data, FileInfo[] files)
        {
            int topicSysNo = 0;
            //string dataString = Request.Form["Data"];
            //dataString = HttpUtility.UrlDecode(dataString);
            //TopicInfo data = SerializeHelper.JsonDeserialize<TopicInfo>(dataString);
            data.Title = data.Title;
            data.Content = data.Content;
            SetEntityBaseUserInfo(data);
            if (data.SysNo > 0)
            {
                data.TopicStatus = TopicStatus.Offline;
                topicSysNo = TopicService.SaveTopicInfo(data);
                return new JsonResult() { Data = topicSysNo };
            }
            else
            {
                data.TopicStatus = TopicStatus.Init;
                topicSysNo = TopicService.CreateTopicInfo(data);
                return new JsonResult() { Data = topicSysNo };
            }
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxPublishTopic()
        {
            int sysNo = 0;
            if (!int.TryParse(Request.QueryString["SysNo"], out sysNo))
            {
                throw new BusinessException(LangHelper.GetText("请输入正确的文章编号！"));
            }
            TopicInfo info = new TopicInfo()
            {
                SysNo = sysNo,
                TopicStatus = TopicStatus.Published
            };

            SetEntityBaseUserInfo(info);
            var result = TopicService.UpdateTopicStatus(info, CurrUser);
            return new JsonResult() { Data = result };
        }
        /// <summary>
        /// 撤下文章
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxOfflineTopic()
        {
            int sysNo = 0;
            if (!int.TryParse(Request.QueryString["SysNo"], out sysNo))
            {
                throw new BusinessException(LangHelper.GetText("请输入正确的文章编号！"));
            }
            TopicInfo info = new TopicInfo()
            {
                SysNo = sysNo,
                TopicStatus = TopicStatus.Offline
            };

            SetEntityBaseUserInfo(info);
            var result = TopicService.UpdateTopicStatus(info, CurrUser);
            return new JsonResult() { Data = result };
        }
        /// <summary>
        /// 作废文章
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxAbondonTopic()
        {
            int sysNo = 0;
            if (!int.TryParse(Request.QueryString["SysNo"], out sysNo))
            {
                throw new BusinessException(LangHelper.GetText("请输入正确的文章编号！"));
            }
            TopicInfo info = new TopicInfo()
            {
                SysNo = sysNo,
                TopicStatus = TopicStatus.Delete
            };

            SetEntityBaseUserInfo(info);
            var result = TopicService.UpdateTopicStatus(info, CurrUser);
            return new JsonResult() { Data = result };
        }

        /// <summary>
        /// 批量发布文章
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxBatchPublishTopic()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = SerializeHelper.JsonDeserialize<List<int>>(dataString);
            var result = TopicService.BatchUpdateTopicStatus(data, TopicStatus.Published, CurrUser);
            return new JsonResult() { Data = result };
        }
        /// <summary>
        /// 批量撤下文章
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxBatchOfflineTopic()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = SerializeHelper.JsonDeserialize<List<int>>(dataString);
            var result = TopicService.BatchUpdateTopicStatus(data, TopicStatus.Offline, CurrUser);
            return new JsonResult() { Data = result };
        }
        /// <summary>
        /// 批量作废文章
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxBatchAbondonTopic()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = SerializeHelper.JsonDeserialize<List<int>>(dataString);
            var result = TopicService.BatchUpdateTopicStatus(data, TopicStatus.Void, CurrUser);
            return new JsonResult() { Data = result };
        }
        public JsonResult AjaxBatchDeleteTopic()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = SerializeHelper.JsonDeserialize<List<int>>(dataString);
            var result = TopicService.BatchUpdateTopicStatus(data, TopicStatus.Delete, CurrUser);
            return new JsonResult() { Data = result };
        }
        /// <summary>
        /// 保存文章默认图片
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSaveTopicImage()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            TopicInfo data = SerializeHelper.JsonDeserialize<TopicInfo>(dataString);
            var result = TopicService.SaveTopicDefaultImage(data, CurrUser);
            return new JsonResult() { Data = result };
        }
        public JsonResult AjaxSaveFileInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            FileInfo data = SerializeHelper.JsonDeserialize<FileInfo>(dataString);
            var result = FileInfoService.InsertFileInfo(data);
            return new JsonResult() { Data = result };
        }
        public JsonResult SaveTopicCategoryPriority()
        {
            var info = SerializeHelper.JsonDeserialize<List<TopicCategory>>(HttpUtility.UrlDecode(Request.Form["Data"]));
            if (info != null && info.Count > 0)
            {
                TopicService.SaveTopicCategoryPriority(info);
            }
            return Json(new AjaxResult() { Success = true });
        }
        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCategory(string id)
        {
            int sysno = 0;
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out sysno))
            {
                TopicCategory tc = new TopicCategory();
                tc.SysNo = sysno;
                TopicService.DeleteTopicCategory(tc);
                return Json(new AjaxResult() { Success = true });
            }
            else
            {
                return Json(new AjaxResult() { Success = false, Message = "传入参数错误." });
            }
        }

        /// <summary>
        /// 维护Category信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MaintainCategory(TopicCategory model)
        {
            model.InUserSysNo = CurrUser.UserSysNo;//auth.UserSysNo;
            model.InUserName = CurrUser.UserDisplayName;// auth.UserDisplayName;
            model.EditUserSysNo = CurrUser.UserSysNo; ;// auth.UserSysNo;
            model.EditUserName = CurrUser.UserDisplayName;// auth.UserDisplayName;

            if (!model.Priority.HasValue)
            {
                model.Priority = 0;
            }
            if (model.SysNo.HasValue && model.SysNo.Value > 0)//编辑
            {
                TopicService.UpdateTopicCategory(model, CurrUser);
            }
            else//新增
            {
                model.SysNo = TopicService.InsertTopicCategory(model, CurrUser);
            }

            return Json(new AjaxResult() { Success = true, Data = model.SysNo });
        }

        #endregion


    }
}
