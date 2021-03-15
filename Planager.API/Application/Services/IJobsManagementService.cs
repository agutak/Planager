using Planager.API.Application.CommandModels;
using Planager.API.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planager.API.Application.Services
{
    public interface IJobsManagementService
    {
        Task InitializeJobsAsync(CancellationToken cancellationToken);
        Task StartAllAsync(CancellationToken cancellationToken);
        Task<Guid> AddJobAsync(CreateJobAppointmentCommandModel model);
        void StartJob(Guid id);
        void StopJob(Guid id);
        JobStatusViewModel GetJobStatus(Guid id);
        List<JobStatusViewModel> GetJobStatuses();
        Task<JobAppointmentViewModel?> GetJobAppointmentAsync(Guid id);
        Task<List<JobAppointmentItemViewModel>> GetJobAppointmentsAsync();
        Task RemoveAppointmentAsync(Guid id);
    }
}