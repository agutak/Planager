using Planager.API.Domain.Entities;
using Planager.API.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planager.API.Persistence.Repositories
{
    public class JobAppointmentsRepository : IJobAppointmentsRepository
    {
        private readonly List<JobAppointment> _jobAppointments;

        public JobAppointmentsRepository()
        {
            _jobAppointments = new()
            {
                new JobAppointment("Job 1", "My job #1", DateTime.UtcNow, TimeSpan.Zero, RepeatType.NoRepeat, null, "https://localhost:4444"),
                new JobAppointment("Job 2", "My job #2", DateTime.UtcNow, TimeSpan.Zero, RepeatType.Hourly, 1, "https://localhost:4444")
            };
        }

        public Task AddAsync(JobAppointment job)
        {
            _jobAppointments.Add(job);
            return Task.CompletedTask;
        }

        public Task<List<JobAppointment>> GetAllAsync()
        {
            return Task.FromResult(_jobAppointments);
        }

        public Task<JobAppointment?> GetAsync(Guid id)
        {
            return Task.FromResult(_jobAppointments.SingleOrDefault(x => x.Id == id));
        }

        public Task RemoveAsync(Guid id)
        {
            var appointment = _jobAppointments.SingleOrDefault(x => x.Id == id);

            if (appointment is null)
                return Task.CompletedTask;

            _jobAppointments.Remove(appointment);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(JobAppointment job)
        {
            throw new NotImplementedException();
        }
    }
}
