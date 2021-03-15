using Microsoft.Extensions.Logging;
using Planager.API.Application.CommandModels;
using Planager.API.Application.ViewModels;
using Planager.API.Domain.Entities;
using Planager.API.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planager.API.Application.Services
{
    public class JobsManagementService : IJobsManagementService
    {
        private readonly JobsRegistry _jobsRegistry;
        private readonly IJobAppointmentsRepository _jobAppointmentsRepository;
        private readonly ILoggerFactory _loggerFactory;

        public JobsManagementService(
            JobsRegistry jobsRegistry,
            IJobAppointmentsRepository jobAppointmentsRepository,
            ILoggerFactory loggerFactory)
        {
            _jobsRegistry = jobsRegistry;
            _jobAppointmentsRepository = jobAppointmentsRepository;
            _loggerFactory = loggerFactory;
        }

        private void RegisterJob(JobAppointment jobAppointment)
        {
            var logger = _loggerFactory.CreateLogger($"{jobAppointment.Name}");

            _jobsRegistry.AddJob(new RunningJob(jobAppointment, logger));
        }

        public async Task InitializeJobsAsync(CancellationToken cancellationToken)
        {
            var jobAppointments = await _jobAppointmentsRepository
                .GetAllAsync()
                .ConfigureAwait(false);

            foreach (var jobAppointment in jobAppointments)
            {
                RegisterJob(jobAppointment);
            }
        }

        public Task StartAllAsync(CancellationToken cancellationToken)
        {
            _jobsRegistry.StartAll(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task<Guid> AddJobAsync(CreateJobAppointmentCommandModel model)
        {
            var jobAppointment = new JobAppointment(
                model.Name,
                model.Description,
                model.ShouldRunAt,
                model.Timezone,
                Enum.Parse<RepeatType>(model.RepeatType),
                model.RepeatInterval,
                model.ExternalUrl);

            await _jobAppointmentsRepository.AddAsync(jobAppointment);

            RegisterJob(jobAppointment);

            StartJob(jobAppointment.Id);

            return jobAppointment.Id;
        }

        public void StartJob(Guid id)
        {
            _jobsRegistry.Start(id);
        }

        public void StopJob(Guid id)
        {
            _jobsRegistry.Stop(id);
        }

        public JobStatusViewModel GetJobStatus(Guid id)
        {
            var job = _jobsRegistry.GetRegisteredJob(id);
            return new JobStatusViewModel(job.Id, job.Status.ToString());
        }

        public List<JobStatusViewModel> GetJobStatuses()
        {
            var jobs = _jobsRegistry.GetRegisteredJobs();

            return jobs
                .Select(job => new JobStatusViewModel(job.Id, job.Status.ToString()))
                .ToList();
        }

        public async Task<JobAppointmentViewModel?> GetJobAppointmentAsync(Guid id)
        {
            var jobAppointment = await _jobAppointmentsRepository.GetAsync(id);

            if (jobAppointment is null)
                return null;

            return new JobAppointmentViewModel()
            {
                Id = jobAppointment.Id,
                Name = jobAppointment.Name,
                RepeatType = jobAppointment.RepeatType.ToString()
            };
        }

        public async Task<List<JobAppointmentItemViewModel>> GetJobAppointmentsAsync()
        {
            var jobAppointments = await _jobAppointmentsRepository.GetAllAsync();

            return jobAppointments
                .Select(jobAppointment => new JobAppointmentItemViewModel()
                {
                    Id = jobAppointment.Id,
                    Name = jobAppointment.Name,
                    RepeatType = jobAppointment.RepeatType.ToString(),
                    ShouldRunAt = new DateTimeOffset(jobAppointment.ShouldRunAt, jobAppointment.Timezone)
                })
                .ToList();
        }

        public async Task RemoveAppointmentAsync(Guid id)
        {
            _jobsRegistry.RemoveJob(id);
            await _jobAppointmentsRepository.RemoveAsync(id);
        }
    }
}
