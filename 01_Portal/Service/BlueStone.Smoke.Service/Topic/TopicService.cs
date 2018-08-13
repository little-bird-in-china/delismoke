using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlueStone.Smoke.Service
{
    public class TopicService
    {
        /// <summary>
        /// 根据编号获取文章信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static TopicInfo LoadTopicInfoBySysNo(int sysNo)
        {
            TopicInfo info = TopicDA.LoadTopicInfoBySysNo(sysNo);
            if (info == null)
            {
                throw new BusinessException("未找到文章。");
            }
            return info;
        }
        public static QueryResult<QR_Topic> QueryTopicList(QF_Topic filter)
        {
            return TopicDA.QueryTopicList(filter);
        }
        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static int SaveTopicInfo(TopicInfo topic)
        {
            int sysNo = 0;
            ValidTopicInfo(topic);
            topic.Content = topic.Content ?? "";
            TopicInfo info = LoadTopicInfoBySysNo(topic.SysNo.Value);
            if (info.TopicStatus == TopicStatus.Init || info.TopicStatus == TopicStatus.Void)
            {
                topic.TopicStatus = info.TopicStatus;
            }
            sysNo = TopicDA.SaveTopicInfo(topic);
            return sysNo;
        }
        /// <summary>
        /// 创建文章
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static int CreateTopicInfo(TopicInfo topic)
        {
            ValidTopicInfo(topic);
            int sysNo = 0;
            topic.Content = topic.Content ?? "";
            sysNo = TopicDA.CreateTopicInfo(topic);
            return sysNo;
        }

        private static void ValidTopicInfo(TopicInfo topic)
        {
            if (string.IsNullOrWhiteSpace(topic.Title))
            {
                throw new BusinessException("标题不能为空");
            }
            //if (string.IsNullOrWhiteSpace(topic.SubTitle))
            //{
            //    throw new BusinessException("副标题不能为空");
            //}
            //if (topic.TopicCategorySysNo <= 0)
            //{
            //    throw new BusinessException("类别不正确");
            //}

        }

        /// <summary>
        /// 更改文章状态
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool UpdateTopicStatus(TopicInfo topicInfo, CurrentUser user)
        {
            //AjaxResult ajaxResult = new AjaxResult() { Success = true, Message=string.Empty};
            bool result = true;
            #region check

            var topic = LoadTopicInfoBySysNo(topicInfo.SysNo.Value);
            if (topicInfo.TopicStatus == TopicStatus.Published && topic.TopicStatus != TopicStatus.Init && topic.TopicStatus != TopicStatus.Offline)
            {
                throw new BusinessException(LangHelper.GetText("只有草稿和撤下状态才能发布！"));
                //ajaxResult.Success = false;
                //ajaxResult.Message = "只有草稿和撤下状态才能发布！";
                //return ajaxResult;
            }

            #endregion

            topicInfo.EditUserSysNo = user.UserSysNo;
            topicInfo.EditUserName = user.UserDisplayName;
            topicInfo.EditDate = DateTime.Now;
            if (topicInfo.TopicStatus == TopicStatus.Published)
            {
                result = TopicDA.PublishTopic(topicInfo);
            }
            else
            {
                result = TopicDA.UpdateTopicStatus(topicInfo);
            }
            return result;
        }
        /// <summary>
        /// 批量修改文章状态
        /// </summary>
        /// <param name="?"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool BatchUpdateTopicStatus(List<int> topicSysNoList, TopicStatus status, CurrentUser user)
        {
            //AjaxResult ajaxResult = new AjaxResult() { Success = true, Message = string.Empty };
            List<TopicInfo> topicList = new List<TopicInfo>();

            #region check
            foreach (int topicSysNo in topicSysNoList)
            {
                var topic = LoadTopicInfoBySysNo(topicSysNo);
                if (status == TopicStatus.Published && topic.TopicStatus != TopicStatus.Init && topic.TopicStatus != TopicStatus.Offline)
                {
                    throw new BusinessException(LangHelper.GetText("只有草稿和撤下状态才能发布！"));
                    //ajaxResult.Success = false;
                    //ajaxResult.Message = "只有草稿和撤下状态才能发布！";
                    //return ajaxResult;
                }
                if (status == TopicStatus.Void && topic.TopicStatus != TopicStatus.Init && topic.TopicStatus != TopicStatus.Offline)
                {
                    throw new BusinessException(LangHelper.GetText("只有草稿和撤下状态才能作废！"));
                    //ajaxResult.Success = false;
                    //ajaxResult.Message = "只有草稿和撤下状态才能发布！";
                    //return ajaxResult;
                }
                if (status == TopicStatus.Delete && topic.TopicStatus != TopicStatus.Init && topic.TopicStatus != TopicStatus.Offline && topic.TopicStatus != TopicStatus.Void)
                {
                    throw new BusinessException(LangHelper.GetText("只有草稿，撤下以及作废状态才能删除！"));
                    //ajaxResult.Success = false;
                    //ajaxResult.Message = "只有草稿和撤下状态才能发布！";
                    //return ajaxResult;
                }
                topicList.Add(new TopicInfo()
                {
                    SysNo = topicSysNo,
                    TopicStatus = status,
                    EditUserSysNo = user.UserSysNo,
                    EditUserName = user.UserDisplayName,
                    EditDate = DateTime.Now
                });
            }

            #endregion

            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (var topic in topicList)
                {
                    if (status == TopicStatus.Published)
                    {
                        TopicDA.PublishTopic(topic);
                    }
                    else
                    {
                        TopicDA.UpdateTopicStatus(topic);
                    }
                }
                transaction.Complete();
            }

            return true;
        }

        /// <summary>
        /// 保存内容默认图片
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool SaveTopicDefaultImage(TopicInfo topic, CurrentUser user)
        {
            bool flag = false;
            topic.EditUserSysNo = user.UserSysNo;
            topic.EditUserName = user.UserDisplayName;
            topic.EditDate = DateTime.Now;
            flag = TopicDA.SaveTopicDefaultImage(topic);
            return flag;
        }
        /// <summary>
        /// 查询所有的文章类别根据MasterName
        /// </summary>
        /// <returns></returns>
        public static List<TopicCategory> QueryAllTopicCategoryListByMasterName(string masterName)
        {
            return TopicDA.QueryAllTopicCategoryListByMasterName(masterName);
        }

        public static TopicCategory LoadTopicCategory(int sysNo)
        {
            return TopicDA.LoadTopicCategory(sysNo);
        }
        public static void SaveTopicCategoryPriority(List<TopicCategory> list)
        {
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    TopicDA.SaveTopicCategoryPriority(item);
                }
            }
        }
        /// <summary>
        /// 删除文章类别
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static bool DeleteTopicCategory(TopicCategory topicCategory)
        {
            topicCategory = TopicDA.LoadTopicCategory(topicCategory.SysNo.Value);
            //查询当前分类下是否有关联文章
            List<TopicInfo> topicList = TopicDA.LoadTopicInfoListByCategory(topicCategory.SysNo.Value);
            if (topicList != null && topicList.Count > 0)
            {
                throw new BusinessException("当前类别下有关联文章，不能删除此类别！");
            }
            else
            {
                bool result = true;
                result = TopicDA.DeleteTopicCategory(topicCategory);
                return result;
            }

        }
        /// <summary>
        /// 更新新闻类别
        /// </summary>
        /// <param name="topicCategory"></param>
        /// <returns></returns>
        public static int UpdateTopicCategory(TopicCategory topicCategory, CurrentUser user)
        {
            return TopicDA.UpdateTopicCategory(topicCategory);
        }
        /// <summary>
        /// 创建新闻类别
        /// </summary>
        /// <param name="topicCategory"></param>
        /// <returns></returns>
        public static int InsertTopicCategory(TopicCategory topicCategory, CurrentUser user)
        {
            int sysNo = 0;
            using (ITransaction transaction = TransactionManager.Create())
            {
                bool nameExist = TopicDA.CheckNameIsExist(topicCategory);
                if (nameExist)
                {
                    throw new BusinessException(LangHelper.GetText("类别名称已存在！"));
                }
                string pCategoryID = string.IsNullOrEmpty(topicCategory.ParentCategoryID) ? "" : topicCategory.ParentCategoryID;
                string categoryID = pCategoryID + "01";

                List<TopicCategory> list = QueryAllTopicCategoryListByParentID(pCategoryID);

                if (list != null && list.Count > 0)
                {
                    TopicCategory tc = list.OrderByDescending(p => p.SysNo).FirstOrDefault();
                    int index = int.Parse(tc.CategoryID.Substring(tc.CategoryID.Length - 2, 2)) + 1;
                    categoryID = pCategoryID + (index < 10 ? "0" + index.ToString() : index.ToString());
                }

                topicCategory.ParentCategoryID = pCategoryID;
                topicCategory.CategoryID = categoryID;
                sysNo = TopicDA.InsertTopicCategory(topicCategory);
                transaction.Complete();
            }
            return sysNo;
        }
        /// <summary>
        /// 查询所有的文章类别
        /// </summary>
        /// <returns></returns>
        public static List<TopicCategory> QueryAllTopicCategoryListByParentID(string parentID)
        {
            parentID = string.IsNullOrEmpty(parentID) ? "" : parentID;
            return TopicDA.QueryAllTopicCategoryListByParentID(parentID);
        }
    }
}
