using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundPopulateTaskQueue
{
    public class RedisUtil
    {
        public const string TAGGIE_LOCK_TASK_QUEUE_PATTERN = "taggie:lock:task:queue:{0}"; // projectid
        public const string TAGGIE_PROJECT_QUEUE_PATTERN_KEY = "taggie:project:queue:{0}"; // projectid
        public const string TAGGIE_PROJECT_QUEUE_WIP_PATTERN = "taggie:project:queue:wip:{0}"; // projectid

        public static string AcquireTaskQueueLock(int projectId, int acquireTimeout = 10, int lockTimeout = 10)
        {
            var lockName = string.Format(TAGGIE_LOCK_TASK_QUEUE_PATTERN, projectId);
            return AcquireLock(lockName, acquireTimeout, lockTimeout);
        }

        public static string AcquireLock(string lockName, int acquireTimeout = 10, int lockTimeout = 10)
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

        public static bool ReleaseTaskQueueLock(int projectId, string identifier)
        {
            var lockName = string.Format(TAGGIE_LOCK_TASK_QUEUE_PATTERN, projectId);
            return ReleaseLock(lockName, identifier);
        }

        public static bool ReleaseLock(string lockName, string identifier)
        {
            if (RedisHelper.Get<string>(lockName) == identifier)
            {
                RedisHelper.Del(lockName);
                return true;
            }

            return false;
        }

        public static int[] GetAllItemsInProjectQueue(int projectId)
        {
            var projQueueKey = string.Format(TAGGIE_PROJECT_QUEUE_PATTERN_KEY, projectId);

            var queueItems = RedisHelper.LRange<int>(projQueueKey, 0, -1);

            return queueItems;
        }

        public static int[] GetAllItemsInProjectWipQueue(int projectId)
        {
            var projWipQueueKey = string.Format(TAGGIE_PROJECT_QUEUE_WIP_PATTERN, projectId);

            var wipQueueItems = RedisHelper.LRange<int>(projWipQueueKey, 0, -1);

            return wipQueueItems;
        }

        public static void QueueItems(int projectId, int[] projectItemIds)
        {
            var projQueueKey = string.Format(TAGGIE_PROJECT_QUEUE_PATTERN_KEY, projectId);

            RedisHelper.RPush<int>(projQueueKey, projectItemIds);
        }
    }
}
