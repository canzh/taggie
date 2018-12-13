using Content.Api.EFModels;
using Content.Api.EFModels.enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundPopulateTaskQueue
{
    public class PopulateTaskHostedService : BackgroundService
    {
        private readonly ILogger<PopulateTaskHostedService> _logger;
        private readonly ApiDbContext _context;
        private readonly int _populateIntervalInMinute;
        private readonly int _queueSize;

        public PopulateTaskHostedService(ILogger<PopulateTaskHostedService> logger,
            IConfiguration configuration,
            ApiDbContext context)
        {
            _logger = logger;
            _context = context;
            _populateIntervalInMinute = configuration.GetValue<int>("PopulateIntervalInMinute", 10);
            _queueSize = configuration.GetValue<int>("RedisProjectQueueSize", 1000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"PopulateTaskHostedService is starting.");

            stoppingToken.Register(() => _logger.LogInformation($"#1 PopulateTaskHostedService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"PopulateTaskHostedService background task is doing background work.");

                PopulateTaskQueue();

                await Task.Delay(_populateIntervalInMinute, stoppingToken);
            }

            _logger.LogInformation($"PopulateTaskHostedService background task is stopping.");

            await Task.CompletedTask;
        }

        private void PopulateTaskQueue()
        {
            var take = _queueSize * 2; // buffer for remove duplicated

            var projects = _context.Project.Where(d => d.Status == ProjectStatus.Active);

            foreach (var proj in projects)
            {
                DateTime start = DateTime.Now;

                var locked = RedisUtil.AcquireTaskQueueLock(proj.Id);
                if (locked == null)
                {
                    _logger.LogInformation($"Failed to acquire lock!");
                    continue;
                }

                try
                {
                    var pendingItems = _context.Projectitem.Where(d => d.ProjectId == proj.Id && d.Status == ProjectItemStatus.New).Take(take);

                    var queueIds = RedisUtil.GetAllItemsInProjectQueue(proj.Id);
                    var wipIds = RedisUtil.GetAllItemsInProjectWipQueue(proj.Id);

                    var toAddItems = pendingItems.Select(d => d.Id).Except(queueIds).Except(wipIds);

                    var size = _queueSize - queueIds.Length;

                    toAddItems = toAddItems.Take(size);

                    RedisUtil.QueueItems(proj.Id, toAddItems.ToArray());
                }
                finally
                {
                    RedisUtil.ReleaseTaskQueueLock(proj.Id, locked);
                }

                DateTime end = DateTime.Now;

                _logger.LogInformation("Finished queue item for {0}: elapsed: {1} ms", proj.ProjectName, (end - start).TotalMilliseconds);
            }
        }
    }
}
