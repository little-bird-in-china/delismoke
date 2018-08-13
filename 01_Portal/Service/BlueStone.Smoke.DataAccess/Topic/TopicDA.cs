using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Data;
using System.Collections.Generic;

namespace BlueStone.Smoke.DataAccess
{
    public class TopicDA
    {
        /// <summary>
        /// 根据编号获取文章信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static TopicInfo LoadTopicInfoBySysNo(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadTopicInfoBySysNo");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            return cmd.ExecuteEntity<TopicInfo>();
        }
        public static QueryResult<QR_Topic> QueryTopicList(QF_Topic filter)
        {
            var cmd = new DataCommand("QueryTopicList");
            cmd.QuerySetCondition("t.Title", ConditionOperation.Like, DbType.String, filter.Title);
            //if (filter.CategoryID!="0") {
            //    cmd.QuerySetCondition("tc.CategoryID", ConditionOperation.Equal, DbType.String, filter.CategoryID);
            //}
            if (filter.CategorySysNo > 0)
            {
                cmd.QuerySetCondition("t.TopicCategorySysNo", ConditionOperation.Equal, DbType.Int32, filter.CategorySysNo);
            }
            //if (!string.IsNullOrEmpty(filter.MasterName) && !string.IsNullOrWhiteSpace(filter.MasterName))
            //{
            //    cmd.QuerySetCondition("tc.MasterName", ConditionOperation.Equal, DbType.String, filter.MasterName);
            //}
            cmd.QuerySetCondition("t.TopicStatus", ConditionOperation.Equal, DbType.Int32, filter.TopicStatus);
            cmd.QuerySetCondition("t.TopicStatus", ConditionOperation.NotEqual, DbType.Int32, TopicStatus.Delete);
            QueryResult<QR_Topic> result = cmd.Query<QR_Topic>(filter, "t.IsTop desc,t.EditDate DESC");
            return result;
        }
        /// <summary>
        /// 更新文章信息
        /// </summary>
        /// <param name="entity"></param>
        public static int SaveTopicInfo(TopicInfo entity)
        {
            DataCommand cmd = new DataCommand("UpdateTopicInfo");
            cmd.SetParameter<TopicInfo>(entity);
            cmd.ExecuteNonQuery();
            return entity.SysNo.Value;
        }
        /// <summary>
        /// 保存文章信息
        /// </summary>
        /// <param name="entity"></param>
        public static int CreateTopicInfo(TopicInfo topic)
        {
            DataCommand cmd = new DataCommand("CreateTopicInfo");
            cmd.SetParameter<TopicInfo>(topic);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }
        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static bool PublishTopic(TopicInfo topic)
        {
            var cmd = new DataCommand("PublishTopic");
            cmd.SetParameter("@SysNo", DbType.Int32, topic.SysNo);
            cmd.SetParameter("@TopicStatus", DbType.Int32, topic.TopicStatus);
            cmd.SetParameter("@EditDate", DbType.DateTime, topic.EditDate);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, topic.EditUserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, topic.EditUserName);
            return cmd.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 更新文章状态
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static bool UpdateTopicStatus(TopicInfo topic)
        {
            var cmd = new DataCommand("UpdateTopicStatus");
            cmd.SetParameter("@SysNo", DbType.Int32, topic.SysNo);
            cmd.SetParameter("@TopicStatus", DbType.Int32, topic.TopicStatus);
            cmd.SetParameter("@EditDate", DbType.DateTime, topic.EditDate);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, topic.EditUserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, topic.EditUserName);
            return cmd.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 保存新闻公告默认图片
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static bool SaveTopicDefaultImage(TopicInfo topic)
        {
            var cmd = new DataCommand("SaveTopicDefaultImage");
            cmd.SetParameter("@SysNo", DbType.Int32, topic.SysNo);
            cmd.SetParameter("@DefaultImage", DbType.String, topic.DefaultImage);
            cmd.SetParameter("@EditDate", DbType.DateTime, topic.EditDate);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, topic.EditUserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, topic.EditUserName);
            return cmd.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 查询所有的文章类别
        /// </summary>
        /// <returns></returns>
        public static List<TopicCategory> QueryAllTopicCategoryListByMasterName(string masterName)
        {
            DataCommand cmd = new DataCommand("QueryAllTopicCategoryListByMasterName");
            cmd.SetParameter("@MasterName", DbType.String, masterName);
            return cmd.ExecuteEntityList<TopicCategory>();
        }

        /// <summary>
        /// 查询文章类别
        /// </summary>
        /// <returns></returns>
        public static TopicCategory LoadTopicCategory(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadTopicCategory");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            return cmd.ExecuteEntity<TopicCategory>();
        }
        public static void SaveTopicCategoryPriority(TopicCategory info)
        {
            DataCommand cmd = new DataCommand("SaveTopicCategoryPriority");
            cmd.SetParameter("@SysNo", DbType.Int32, info.SysNo);
            cmd.SetParameter("@Priority", DbType.Int32, info.Priority);
            cmd.ExecuteNonQuery();

        }
        public static List<TopicInfo> LoadTopicInfoListByCategory(int categorySysNo)
        {
            DataCommand cmd = new DataCommand("LoadTopicInfoListByCategory");
            cmd.SetParameter("@CategorySysNo", DbType.Int32, categorySysNo);
            return cmd.ExecuteEntityList<TopicInfo>();
        }
        /// <summary>
        /// 删除文章类别
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static bool DeleteTopicCategory(TopicCategory topicCategory)
        {
            var cmd = new DataCommand("DeleteTopicCategory");
            cmd.SetParameter("@SysNo", DbType.Int32, topicCategory.SysNo);
            return cmd.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 更新新闻类别
        /// </summary>
        /// <param name="topicCategory"></param>
        /// <returns></returns>
        public static int UpdateTopicCategory(TopicCategory topicCategory)
        {
            DataCommand cmd = new DataCommand("UpdateTopicCategory");
            cmd.SetParameter<TopicCategory>(topicCategory);
            cmd.ExecuteNonQuery();
            return topicCategory.SysNo.Value;
        }
        public static bool CheckNameIsExist(TopicCategory topicCategory)
        {
            DataCommand cmd;
            if (topicCategory.SysNo > 0)
            {
                cmd = new DataCommand("CheckTopicCategoryNameIsExistForUpdate");
            }
            else
            {
                cmd = new DataCommand("CheckTopicCategoryNameIsExistForInsert");
            }
            cmd.SetParameter<TopicCategory>(topicCategory);
            return cmd.ExecuteScalar<int>() > 0;
        }
        /// <summary>
        /// 查询所有的文章类别
        /// </summary>
        /// <returns></returns>
        public static List<TopicCategory> QueryAllTopicCategoryListByParentID(string parentID)
        {
            DataCommand cmd = new DataCommand("QueryAllTopicCategoryListByParentID");
            cmd.SetParameter("@ParentCategoryID", DbType.String, parentID);
            return cmd.ExecuteEntityList<TopicCategory>();
        }
        /// <summary>
        /// 创建新闻类别
        /// </summary>
        /// <param name="topicCategory"></param>
        /// <returns></returns>
        public static int InsertTopicCategory(TopicCategory topicCategory)
        {
            DataCommand cmd = new DataCommand("InsertTopicCategory");
            cmd.SetParameter<TopicCategory>(topicCategory);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

    }
}
