using Content.Api.Common;
using Content.Api.EFModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api
{
    public class RedisSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApiDbContext context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                await RedisHelper.DelAsync(RedisUtil.TAGGIE_PROJECT_QUEUE_SIZE_KEY);

                // queue size
                var projectQueueSize = configuration.GetValue<int>("RedisProjectQueueSize");
                await RedisHelper.SetAsync(RedisUtil.TAGGIE_PROJECT_QUEUE_SIZE_KEY, projectQueueSize);

                var projects = context.Project.ToList();

                foreach (var p in projects)
                {
                    // project metadata info
                    var finishedItems = context.Projectitem.Count(d => d.ProjectId == p.Id && d.Status == EFModels.enums.ProjectItemStatus.Tagged);
                    var projectMetadataKey = string.Format(RedisUtil.TAGGIE_PROJECT_METADATA_PATTERN_KEY, p.Id);

                    await RedisHelper.DelAsync(projectMetadataKey);

                    await RedisHelper.HMSetAsync(projectMetadataKey,
                        RedisUtil.TAGGIE_PROJECT_NAME, p.ProjectName,
                        RedisUtil.TAGGIE_PROJECT_TOTAL, p.TotalItems,
                        RedisUtil.TAGGIE_PROJECT_REMAINING, p.TotalItems - finishedItems);

                    // project task queue
                    var redisProjectQueueKey = string.Format(RedisUtil.TAGGIE_PROJECT_QUEUE_PATTERN_KEY, p.Id);

                    await RedisHelper.DelAsync(redisProjectQueueKey);

                    var itemIds = context.Projectitem
                        .Where(d => d.ProjectId == p.Id && d.Status == EFModels.enums.ProjectItemStatus.New)
                        .Take(projectQueueSize)
                        .Select(d => (object)d.Id)
                        .ToArray();
                    await RedisHelper.RPushAsync(redisProjectQueueKey, itemIds);

                    // project categories
                    var projectCategoryKey = string.Format(RedisUtil.TAGGIE_CATEGORIES_PATTERN_KEY, p.Id);

                    await RedisHelper.DelAsync(projectCategoryKey);

                    if (!await RedisHelper.ExistsAsync(projectCategoryKey))
                    {
                        var categories = context.Projectcategory.Where(d => d.ProjectId == p.Id && d.Status == EFModels.enums.TagStatus.Active).ToList();

                        var pipe = RedisHelper.StartPipe();

                        foreach (var c in categories)
                        {
                            pipe.HSet(projectCategoryKey, c.Name, c.Id);
                        }

                        pipe.EndPipe();
                    }

                    // project subcategories
                    var projectSubcategoryKey = string.Format(RedisUtil.TAGGIE_SUBCATEGORIES_PATTERN_KEY, p.Id);

                    await RedisHelper.DelAsync(projectSubcategoryKey);

                    if (!await RedisHelper.ExistsAsync(projectSubcategoryKey))
                    {
                        var subcategories = context.Projectsubcategory.Where(d => d.ProjectId == p.Id && d.Status == EFModels.enums.TagStatus.Active)
                          .ToList();
                        //await RedisHelper.RPushAsync(projectSubcategoryKey, subcategories);

                        var pipe = RedisHelper.StartPipe();

                        foreach (var c in subcategories)
                        {
                            pipe.HSet(projectSubcategoryKey, c.Name, c.Id);
                        }

                        pipe.EndPipe();
                    }

                    // project assigned to team
                    var teamAssignment = context.Teamprojects.Where(d => d.ProjectId == p.Id).ToList();

                    foreach (var assign in teamAssignment)
                    {
                        var teamStatisticsKey = string.Format(RedisUtil.TAGGIE_TEAM_STATISTICS_PATTERN, p.Id, assign.TeamId);

                        // only seed project assigned item count here
                        await RedisHelper.DelAsync(teamStatisticsKey);
                        await RedisHelper.HSetAsync(teamStatisticsKey, RedisUtil.TAGGIE_TEAM_TOTOL_ASSIGNED, assign.AssignedProjectItems);
                    }

                    // TODO: calculate user effort statistics

                    // project keywords
                    //var projectKeywordsKey = string.Format(RedisUtil.TAGGIE_KEYWORDS_PATTERN_KEY, p.Id);
                    //if (!await RedisHelper.ExistsAsync(projectKeywordsKey))
                    //{
                    //    var keywords = context.Projectkeyword.Where(d => d.ProjectId == p.Id && d.Status == EFModels.enums.TagStatus.Active)
                    //      .Select(d => d.Name).ToList();
                    //    await RedisHelper.RPushAsync(projectKeywordsKey, keywords);
                    //}
                }
            }
        }
    }
}
