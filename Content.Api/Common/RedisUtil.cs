using Content.Api.EFModels.dto;
using Content.Api.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CSRedis.CSRedisClient;

namespace Content.Api.Common
{
    public class RedisUtil
    {
        public const string TAGGIE_PROJECT_QUEUE_SIZE_KEY = "taggie:project:queue:size";
        public const string TAGGIE_PROJECT_QUEUE_PATTERN_KEY = "taggie:project:queue:{0}"; // projectid
        public const string TAGGIE_PROJECT_METADATA_PATTERN_KEY = "taggie:project:metadata:{0}"; // projectid
        public const string TAGGIE_USER_QUEUE_PATTERN_KEY = "taggie:user:queue:{0}:{1}"; // projectid:userid
        public const string TAGGIE_ONLINE_USER_COUNT_KEY = "taggie:online:user:count";
        public const string TAGGIE_CATEGORIES_PATTERN_KEY = "taggie:categories:{0}"; // projectid
        public const string TAGGIE_SUBCATEGORIES_PATTERN_KEY = "taggie:subcategories:{0}"; // projectid
        public const string TAGGIE_KEYWORDS_PATTERN_KEY = "taggie:keywords:{0}"; // projectid
        public const string TAGGIE_CHANNEL_TAGGED = "taggie:channel:tagged";
        public const string TAGGIE_PROJECT_NAME = "project-name";
        public const string TAGGIE_PROJECT_TOTAL = "total-items";
        public const string TAGGIE_PROJECT_REMAINING = "remaining-items";
        public const string TAGGIE_TEAM_STATISTICS_PATTERN = "taggie:team:statistics:{0}:{1}"; // projectid:teamid
        public const string TAGGIE_TEAM_TYPE_FIELD = "team:type";
        public const string TAGGIE_TEAM_FINISHED = "team:finished";
        public const string TAGGIE_TEAM_INCORRECT = "team:incorrect";
        public const string TAGGIE_TEAM_USER_FINISHED = "{0}:finished"; // userid
        public const string TAGGIE_TEAM_USER_INCORRECT = "{0}:incorrect"; // userid
        public const string TAGGIE_PROJECT_QUEUE_WIP_PATTERN = "taggie:project:queue:wip:{0}"; // projectid
        public const string TAGGIE_USER_SUBMITTED_PATTERN = "taggie:user:submit:{0}:{1}"; // projectid:userid

        public async Task<bool> IfItemIdAssignedToUser(int projectId, string userId, int projectItemId)
        {
            var userQueueKey = string.Format(TAGGIE_USER_QUEUE_PATTERN_KEY, projectId, userId);

            return await RedisHelper.ZScoreAsync(userQueueKey, projectItemId) != null;
            //return RedisHelper.Get<int>(string.Format(TAGGIE_USER_QUEUE_PATTERN_KEY, projectId, userId));
        }

        public async Task PublishTaggedItem(TaggedEvent submission)
        {
            await RedisHelper.PublishAsync(TAGGIE_CHANNEL_TAGGED, JsonConvert.SerializeObject(submission));
        }

        public async Task SetUserSubmitted(int projectId, string userId, bool b)
        {
            var submitted = string.Format(TAGGIE_USER_SUBMITTED_PATTERN, projectId, userId);
            await RedisHelper.SetAsync(submitted, b ? 1 : 0);
        }

        public async Task<int> AssignQueueItemToUser(int projectId, string userId)
        {
            var submitted = string.Format(TAGGIE_USER_SUBMITTED_PATTERN, projectId, userId);
            var userQueueKey = string.Format(TAGGIE_USER_QUEUE_PATTERN_KEY, projectId, userId);

            if (await RedisHelper.ExistsAsync(submitted) && await RedisHelper.GetAsync<int>(submitted) == 0)
            {
                var previousId = await RedisHelper.ZRevRangeAsync<int>(userQueueKey, 0, 0);
                return previousId.Length > 0 ? previousId[0] : 0;
            }

            var projectItemId = await RedisHelper.LPopAsync<int>(string.Format(TAGGIE_PROJECT_QUEUE_PATTERN_KEY, projectId));

            if (projectItemId == 0) return 0; // no more items in queue

            var wipQueueKey = string.Format(TAGGIE_PROJECT_QUEUE_WIP_PATTERN, projectId);
            await RedisHelper.RPushAsync(wipQueueKey, projectItemId);

            await RedisHelper.ZAddAsync(userQueueKey, (DateTime.Now.Ticks, projectItemId));

            await SetUserSubmitted(projectId, userId, false);

            return projectItemId;
        }

        public async Task<ProjectStatisticInfo> GetProjectInfo(int projectId)
        {
            var values = await RedisHelper.HMGetAsync(string.Format(TAGGIE_PROJECT_METADATA_PATTERN_KEY, projectId),
                TAGGIE_PROJECT_NAME, TAGGIE_PROJECT_TOTAL, TAGGIE_PROJECT_REMAINING);

            return new ProjectStatisticInfo
            {
                ProjectName = values[0],
                TotalItems = int.Parse(values[1]),
                RemainingItems = int.Parse(values[2])
            };
        }

        public async Task<string[]> GetProjectCategories(int projectId)
        {
            var key = string.Format(TAGGIE_CATEGORIES_PATTERN_KEY, projectId);

            return await RedisHelper.HKeysAsync(key);
        }

        public async Task<string[]> GetProjectSubcategories(int projectId)
        {
            var key = string.Format(TAGGIE_SUBCATEGORIES_PATTERN_KEY, projectId);

            return await RedisHelper.HKeysAsync(key);
        }

        public async Task<int[]> ConvertCategoryNames(int projectId, string[] categories)
        {
            var projectCategoryKey = string.Format(TAGGIE_CATEGORIES_PATTERN_KEY, projectId);

            return await ConvertNamesToIds(projectCategoryKey, categories);
        }

        public async Task<int[]> ConvertSubcategoryNames(int projectId, string[] subcategories)
        {
            var projectSubcategoryKey = string.Format(TAGGIE_SUBCATEGORIES_PATTERN_KEY, projectId);

            return await ConvertNamesToIds(projectSubcategoryKey, subcategories);
        }

        private async Task<int[]> ConvertNamesToIds(string key, string[] names)
        {
            var values = await RedisHelper.HMGetAsync(key, names);

            if (values.Any(d => d == null))
            {
                throw new Exception(string.Format("There are invalid values! Values: {0}", string.Join(",", names)));
            }

            return values.Select(d => int.Parse(d)).ToArray();
        }

        public void FinishTaggie(int projectId, int teamId, string userId, int projectItemId)
        {
            var teamStatisticsKey = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, teamId);
            var userField = string.Format(TAGGIE_TEAM_USER_FINISHED, userId);
            var wipQueue = string.Format(TAGGIE_PROJECT_QUEUE_WIP_PATTERN, projectId);
            var userQueue = string.Format(TAGGIE_USER_QUEUE_PATTERN_KEY, projectId, userId);

            RedisHelper.StartPipe()
                .LRem(wipQueue, 1, projectItemId)
                .ZRem(userQueue, projectItemId)
                .HIncrBy(teamStatisticsKey, TAGGIE_TEAM_FINISHED, 1)
                .HIncrBy(teamStatisticsKey, userField, 1)
                .EndPipe();
        }

        public void FinishQA(int projectId, int qaTeamId, int taggieTeamId, string qaUserId, string taggieUserId, bool correct)
        {
            var qaStatisticsKey = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, qaTeamId);
            var qaUserField = string.Format(TAGGIE_TEAM_USER_FINISHED, qaUserId);

            var pipe = RedisHelper.StartPipe()
                .HIncrBy(qaStatisticsKey, TAGGIE_TEAM_FINISHED, 1)
                .HIncrBy(qaStatisticsKey, qaUserField, 1);

            if (!correct)
            {
                var taggieStatisticsKey = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, taggieTeamId);
                var taggieUserField = string.Format(TAGGIE_TEAM_USER_INCORRECT, taggieUserId);

                pipe.HIncrBy(taggieStatisticsKey, TAGGIE_TEAM_INCORRECT, 1)
                    .HIncrBy(taggieStatisticsKey, taggieUserField, 1);
            }

            pipe.EndPipe();
        }
    }
}
