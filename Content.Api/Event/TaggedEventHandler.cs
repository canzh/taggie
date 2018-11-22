using Content.Api.Common;
using Content.Api.EFModels;
using Content.Api.EFModels.dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.Event
{
    public class TaggedEventHandler : IEventHandler<TaggedEvent>
    {
        private readonly ApiDbContext _context;
        private ILogger _logger;
        private RedisUtil _redis;

        public TaggedEventHandler(
            ApiDbContext context,
            RedisUtil redis,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _redis = redis;
            _logger = loggerFactory.CreateLogger<TaggedEventHandler>();
        }

        public void Handle(TaggedEvent o)
        {
            // after finish tagging a item, there are several thing to do:
            // 1. update project item status
            // 1. create finished-by record: projectitemeffort
            // 1. add category link
            // 1. add subcategory link
            // 1. store keywords
            // 1. update taggie statistics
            // 1. update team statistics
            // 1. delete project item id in project wip queue
            // 1. delete project item id in user queue
            // 1. decrease project meta remaining

            var projectItem = _context.Projectitem.FirstOrDefault(d => d.Id == o.ProjectItemId);

            if (projectItem == null)
            {
                _logger.LogInformation("Project item is not found for id: {0}", o.ProjectItemId);
                return;
            }

            projectItem.Status = EFModels.enums.ProjectItemStatus.Tagged;
            _context.SaveChanges(); // save status quickly for racing condition

            Projectitemefforttaggie effort = new Projectitemefforttaggie
            {
                ProjectItemId = projectItem.Id,
                EffortUserId = o.TaggedUserId,
                CreatedDate = DateTime.Now,
                ProjectId = o.ProjectId,
                TeamId = o.TeamId,
                VerifiedStatus = EFModels.enums.ProjectItemVerifyStatus.Pending
            };

            _context.Projectitemefforttaggie.Add(effort);

            foreach (var id in o.CategoryIds)
            {
                Projectitemcategories clink = new Projectitemcategories
                {
                    ProjectItemId = o.ProjectItemId,
                    CategoryId = id,
                    AddedByRole = EFModels.enums.UserRoleType.Taggie
                };

                _context.Projectitemcategories.Add(clink);
            }

            foreach (var id in o.SubcategoryIds)
            {
                Projectitemsubcategories sclink = new Projectitemsubcategories
                {
                    ProjectItemId = o.ProjectItemId,
                    SubCategoryId = id,
                    AddedByRole = EFModels.enums.UserRoleType.Taggie
                };

                _context.Projectitemsubcategories.Add(sclink);
            }

            Projectitemkeywords keywords = new Projectitemkeywords
            {
                ProjectItemId = o.ProjectItemId,
                Keywords = string.Join(",", o.Keywords),
                AddedByRole = EFModels.enums.UserRoleType.Taggie
            };

            _context.Projectitemkeywords.Add(keywords);

            _context.SaveChanges();

            _redis.FinishTaggie(projectItem.ProjectId, o.TeamId, o.TaggedUserId, o.ProjectItemId);

            _logger.LogInformation("Finished project item: {0}, by user: {1} in team： {2}", projectItem.Id, o.TaggedUserId, o.TeamId);
        }
    }
}
