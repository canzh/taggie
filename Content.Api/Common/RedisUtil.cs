using Content.Api.EFModels.dto;
using Content.Api.Event;
using Microsoft.Extensions.Logging;
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
        public const string TAGGIE_TEAM_TOTOL_ASSIGNED = "team:total:assigned";
        public const string TAGGIE_TEAM_FINISHED = "team:finished";
        public const string TAGGIE_TEAM_CORRECT = "team:correct";
        public const string TAGGIE_TEAM_INCORRECT = "team:incorrect";
        public const string TAGGIE_TEAM_USER_FINISHED = "{0}:finished"; // userid
        public const string TAGGIE_TEAM_USER_CORRECT = "{0}:correct"; // userid
        public const string TAGGIE_TEAM_USER_INCORRECT = "{0}:incorrect"; // userid
        public const string TAGGIE_PROJECT_QUEUE_WIP_PATTERN = "taggie:project:queue:wip:{0}"; // projectid
        public const string TAGGIE_USER_SUBMITTED_PATTERN = "taggie:user:submit:{0}:{1}"; // projectid:userid
        public const string TAGGIE_LOCK_TASK_QUEUE_PATTERN = "taggie:lock:task:queue:{0}"; // projectid
        public const string TAGGIE_TEAM_TASK_QUEUE_PATTERN = "taggie:team:queue:{0}:{1}"; // projectid:teamid

        private readonly ILogger _logger;

        public RedisUtil(ILogger<RedisUtil> logger)
        {
            _logger = logger;
        }

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

        public async Task<int> AssignQueueItemToUser(int projectId, string userId, int teamId)
        {
            // check previous assigned task finished or not
            var submitted = string.Format(TAGGIE_USER_SUBMITTED_PATTERN, projectId, userId);
            var userQueueKey = string.Format(TAGGIE_USER_QUEUE_PATTERN_KEY, projectId, userId);

            if (await RedisHelper.ExistsAsync(submitted) && await RedisHelper.GetAsync<int>(submitted) == 0)
            {
                var previousId = await RedisHelper.ZRevRangeAsync<int>(userQueueKey, 0, 0);
                return previousId.Length > 0 ? previousId[0] : 0;
            }

            // To synchronize the assign process and finish process in order to garrente the assignment
            // for the whole team, we calculate: team_finished + team_queue <= assignment 

            // synchronize with finish taggie
            var lockName = string.Format(TAGGIE_LOCK_TASK_QUEUE_PATTERN, projectId);

            var locked = AcquireLock(lockName);
            if (locked == null)
            {
                return -1; // failed to acquire lock
            }

            // check team assignment has finished or not
            var teamStatistics = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, teamId);
            var numbers = await RedisHelper.HMGetAsync(teamStatistics, TAGGIE_TEAM_TOTOL_ASSIGNED, TAGGIE_TEAM_FINISHED);
            if (numbers.Any(d => d == null))
            {
                ReleaseLock(lockName, locked);
                throw new Exception("failed to determine team assignment info, not able to alocate task!");
            }

            var teamWipKey = string.Format(TAGGIE_TEAM_TASK_QUEUE_PATTERN, projectId, teamId);

            var teamWipCount = await RedisHelper.HLenAsync(teamWipKey);
            if (long.Parse(numbers[0]) <= long.Parse(numbers[1]) + teamWipCount)
            {
                ReleaseLock(lockName, locked);
                return 0; // no more tasks
            }

            // allocate task using queue
            var projectItemId = await RedisHelper.LPopAsync<int>(string.Format(TAGGIE_PROJECT_QUEUE_PATTERN_KEY, projectId));

            if (projectItemId == 0)
            {
                ReleaseLock(lockName, locked);
                return 0; // no more items in queue
            }

            // project wip queue
            var wipQueueKey = string.Format(TAGGIE_PROJECT_QUEUE_WIP_PATTERN, projectId);
            await RedisHelper.RPushAsync(wipQueueKey, projectItemId);

            // user wip queue
            await RedisHelper.ZAddAsync(userQueueKey, (DateTime.Now.Ticks, projectItemId));

            // team wip queue
            await RedisHelper.HSetAsync(teamWipKey, projectItemId.ToString(), userId);

            await SetUserSubmitted(projectId, userId, false);

            ReleaseLock(lockName, locked);

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
            var projectMetadataKey = string.Format(TAGGIE_PROJECT_METADATA_PATTERN_KEY, projectId);
            var teamStatisticsKey = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, teamId);
            var userField = string.Format(TAGGIE_TEAM_USER_FINISHED, userId);
            var userQueue = string.Format(TAGGIE_USER_QUEUE_PATTERN_KEY, projectId, userId);
            var projectWipQueue = string.Format(TAGGIE_PROJECT_QUEUE_WIP_PATTERN, projectId);
            var teamWipKey = string.Format(TAGGIE_TEAM_TASK_QUEUE_PATTERN, projectId, teamId);

            // synchronize with assign
            var lockName = string.Format(TAGGIE_LOCK_TASK_QUEUE_PATTERN, projectId);

            string locked = null;

            while (locked == null)
            {
                locked = AcquireLock(lockName);

                if (locked == null)
                {
                    _logger.LogError("Failed to acquire lock for: {0}", lockName);
                }
            }

            RedisHelper.StartPipe()
                .LRem(projectWipQueue, 1, projectItemId)
                .ZRem(userQueue, projectItemId)
                .HIncrBy(teamStatisticsKey, TAGGIE_TEAM_FINISHED, 1)
                .HIncrBy(teamStatisticsKey, userField, 1)
                .HIncrBy(projectMetadataKey, TAGGIE_PROJECT_REMAINING, -1)
                .HDel(teamWipKey, projectItemId.ToString())
                .EndPipe();

            ReleaseLock(lockName, locked);
        }

        // TODO: not finished
        public void FinishQA(int projectId, int qaTeamId, int taggieTeamId, string qaUserId, string taggieUserId, bool correct)
        {
            var qaTeamStatistics = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, qaTeamId);
            var qaUserField = string.Format(TAGGIE_TEAM_USER_FINISHED, qaUserId);

            var pipe = RedisHelper.StartPipe()
                .HIncrBy(qaTeamStatistics, TAGGIE_TEAM_FINISHED, 1)
                .HIncrBy(qaTeamStatistics, qaUserField, 1);

            var taggieTeamStatistics = string.Format(TAGGIE_TEAM_STATISTICS_PATTERN, projectId, taggieTeamId);

            if (correct)
            {
                var taggieUserCorrect = string.Format(TAGGIE_TEAM_USER_CORRECT, taggieUserId);

                pipe.HIncrBy(taggieTeamStatistics, TAGGIE_TEAM_CORRECT, 1)
                    .HIncrBy(taggieTeamStatistics, taggieUserCorrect, 1);
            }
            else
            {
                var taggieUserIncorrect = string.Format(TAGGIE_TEAM_USER_INCORRECT, taggieUserId);

                pipe.HIncrBy(taggieTeamStatistics, TAGGIE_TEAM_INCORRECT, 1)
                    .HIncrBy(taggieTeamStatistics, taggieUserIncorrect, 1);
            }

            pipe.EndPipe();
        }

        public string AcquireLock(string lockName, int acquireTimeout = 10, int lockTimeout = 10)
        {
            string identifier = Guid.NewGuid().ToString();

            DateTime end = DateTime.Now.AddSeconds(acquireTimeout);

            while (DateTime.Now < end)
            {
                if (RedisHelper.SetNx(lockName, identifier))
                {
                    RedisHelper.Expire(lockName, lockTimeout);
                    return identifier;
                }
                else if (RedisHelper.Ttl(lockName) > 0)
                {
                    RedisHelper.Expire(lockName, lockTimeout);
                }

                Task.Delay(1);
            }

            return null;
        }

        public bool ReleaseLock(string lockName, string identifier)
        {
            if (RedisHelper.Get<string>(lockName) == identifier)
            {
                RedisHelper.Del(lockName);
                return true;
            }

            return false;
        }
    }
}
