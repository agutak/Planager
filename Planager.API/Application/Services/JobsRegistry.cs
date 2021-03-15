using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Planager.API.Application.Services
{
    public class JobsRegistry
    {
        private readonly ConcurrentDictionary<Guid, RunningJob> _jobs;
        private readonly ILogger<JobsRegistry> _logger;

        private CancellationTokenSource? _cancellationToken;

        public JobsRegistry(ILogger<JobsRegistry> logger)
        {
            _jobs = new();
            _logger = logger;
        }

        public void AddJob(RunningJob runningJob)
        {
            if (!_jobs.TryAdd(runningJob.Id, runningJob))
            {
                _logger.LogWarning($"Job with Name: {runningJob.JobAppointment.Name} cannot be registered.");
                return;
            }
        }

        public void RemoveJob(Guid id)
        {
            if (!_jobs.TryRemove(id, out var job))
            {
                _logger.LogWarning($"Job with Id: {id} cannot be removed.");
                return;
            }

            job.Stop();
            job.Dispose();
        }

        public RunningJob? GetRegisteredJob(Guid id)
        {
            if (!_jobs.TryGetValue(id, out var job))
                _logger.LogWarning($"Job with Id: {id} cannot be found.");

            return job;
        }

        public ICollection<RunningJob> GetRegisteredJobs()
        {
            return _jobs.Values;
        }

        public void StartAll(CancellationToken cancellationToken)
        {
            _cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            foreach (var job in _jobs.Values)
            {
                job.Start(_cancellationToken.Token);
            }
        }

        public void Start(Guid id)
        {
            if (!_jobs.TryGetValue(id, out var runningJob))
                throw new ArgumentException($"Job with Id: {id} is not registered.");

            if (_cancellationToken is null)
                throw new Exception("CancellationTokenSource is not initialized.");

            runningJob.Start(_cancellationToken.Token);
        }

        public void Stop(Guid id)
        {
            if (!_jobs.TryGetValue(id, out var runningJob))
                throw new ArgumentException($"Job with Id: {id} is not registered.");

            runningJob.Stop();
        }
    }
}
