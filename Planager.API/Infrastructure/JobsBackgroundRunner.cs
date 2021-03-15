using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planager.API.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planager.API.Infrastructure
{
    public class JobsBackgroundRunner : BackgroundService
    {
        private readonly IJobsManagementService _jobsManagementService;
        private readonly ILogger<JobsBackgroundRunner> _logger;

        public JobsBackgroundRunner(
            IJobsManagementService jobsManagementService,
            ILogger<JobsBackgroundRunner> logger)
        {
            _jobsManagementService = jobsManagementService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _jobsManagementService.InitializeJobsAsync(stoppingToken);
                await _jobsManagementService.StartAllAsync(stoppingToken);

                while(!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured in JobsBackgroundRunner!");
            }
        }
    }
}
