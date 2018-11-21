using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.Common
{
    public class UserAccessValidation
    {
        private readonly RedisUtil _redis;
        private readonly IHttpContextAccessor _httpContext;

        public UserAccessValidation(RedisUtil redis, IHttpContextAccessor httpContext)
        {
            _redis = redis;
            _httpContext = httpContext;
        }

        public string GetUserSubId()
        {
            var userId = _httpContext.HttpContext.User.FindFirst("sub")?.Value;

            if (userId == null)
            {
                throw new Exception("User id can't be extracted!");
            }

            return userId;
        }

        public int GetUserTeamId()
        {
            var teamId = _httpContext.HttpContext.User.FindFirst("team")?.Value;

            if (teamId == null)
            {
                throw new Exception("User team id can't be extracted!");
            }

            return int.Parse(teamId);
        }

        public string GetUserName()
        {
            var userName = _httpContext.HttpContext.User.FindFirst("name")?.Value;

            if (userName == null)
            {
                throw new Exception("User name can't be extracted!");
            }

            return userName;
        }

        public async Task<bool> IfQueueItemAssignedToUser(int projectId, int projectItemId)
        {
            var userId = GetUserSubId();

            return await _redis.IfItemIdAssignedToUser(projectId, userId, projectItemId);
        }
    }
}
