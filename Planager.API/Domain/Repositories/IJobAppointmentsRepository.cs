using Planager.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planager.API.Domain.Repositories
{
    public interface IJobAppointmentsRepository
    {
        Task<JobAppointment?> GetAsync(Guid id);
        Task<List<JobAppointment>> GetAllAsync();
        Task AddAsync(JobAppointment job);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(JobAppointment job);
    }
}
