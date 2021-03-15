using Microsoft.Extensions.Logging;
using Planager.API.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planager.API.Application.Services
{
    public delegate Task JobAction(CancellationTokenSource cancellationToken);

    public class RunningJob : IDisposable
    {
        private readonly JobAppointment _jobAppointment;
        private readonly JobAction _actionReference;
        private readonly ILogger _logger;

        private Task? _taskReference;
        private bool _disposedValue;
        private CancellationTokenSource? _cts;

        public RunningJob(JobAppointment jobAppointment, ILogger logger)
        {
            _jobAppointment = jobAppointment;
            _logger = logger;
            Id = jobAppointment.Id;
            Status = JobStatus.Starting;
            _actionReference = jobAppointment.RepeatType == RepeatType.NoRepeat
                ? DoOnetimeWorkAsync
                : DoPeriodicWorkAsync;
        }

        public Guid Id { get; init; }
        public JobStatus Status { get; private set; }
        public DateTime? StartedAt { get; private set; }
        public DateTime? StoppedAt { get; private set; }
        public JobAppointment JobAppointment => _jobAppointment;

        private async Task DoOnetimeWorkAsync(CancellationTokenSource cancellationToken)
        {
            _logger.LogInformation($"Onetime job {Id} started at {DateTime.UtcNow}");

            //Some work
            await Task.Delay(10_000, cancellationToken.Token);

            StoppedAt = DateTime.UtcNow;
            Status = JobStatus.Stopped;
        }

        private async Task DoPeriodicWorkAsync(CancellationTokenSource cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Periodic job {Id} started at {DateTime.UtcNow}");

                    //Some work
                    await Task.Delay(5_000, cancellationToken.Token);
                }
                catch (OperationCanceledException)
                {
                    StoppedAt = DateTime.UtcNow;
                    Status = JobStatus.Stopped;
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Periodic job {_jobAppointment.Id} failed at {DateTime.UtcNow}");
                    StoppedAt = DateTime.UtcNow;
                    Status = JobStatus.Faulted;
                    break;
                }
            }
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (Status == JobStatus.Running)
                throw new Exception($"Job with Id: {Id} is already running.");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (_taskReference is not null)
            {
                _taskReference.Dispose();
                _taskReference = null;
            }

            _taskReference = _actionReference(_cts);

            StartedAt = DateTime.UtcNow;
            Status = JobStatus.Running;
        }

        public void Stop()
        {
            if (Status == JobStatus.Stopped)
                return;

            if (_cts is null)
                throw new Exception($"Not started job with Id: {Id} cannot be stopped.");

            StoppedAt = DateTime.UtcNow;
            Status = JobStatus.Stopped;

            _cts.Cancel();
        }

        public void FailRegistration()
        {
            Status = JobStatus.FailedRegistration;
        }

        public void FailRemoval()
        {
            Status = JobStatus.FailedRemoval;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _taskReference?.Dispose();
                    _taskReference = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
