using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planager.API.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Planager.API.Application.CommandModels;
using System.Threading;
using Planager.API.Domain.Entities;

namespace Planager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IJobsManagementService _jobsManagementService;

        public JobsController(
            ILogger<JobsController> logger,
            IJobsManagementService jobsManagementService)
        {
            _logger = logger;
            _jobsManagementService = jobsManagementService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllJobAppointmentsAsync()
        {
            var jobAppointment = await _jobsManagementService.GetJobAppointmentsAsync();
            return Ok(jobAppointment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobAppointmentAsync([FromRoute] Guid id)
        {
            var jobAppointment = await _jobsManagementService.GetJobAppointmentAsync(id);
            return Ok(jobAppointment);
        }

        [HttpGet("{id}/status")]
        public IActionResult GetStatus([FromRoute] Guid id)
        {
            return Ok(_jobsManagementService.GetJobStatus(id));
        }

        [HttpGet("status")]
        public IActionResult GetStatuses()
        {
            return Ok(_jobsManagementService.GetJobStatuses());
        }

        [HttpPost]
        public async Task<IActionResult> AddJobAppointmentAsync([FromBody] CreateJobAppointmentCommandModel model)
        {
            var jobId = await _jobsManagementService.AddJobAsync(model);

            return Ok(jobId);
        }

        [HttpPut("{id}/stop")]
        public IActionResult StopJob([FromRoute] Guid id)
        {
            _jobsManagementService.StopJob(id);

            return Ok();
        }

        [HttpPut("{id}/start")]
        public IActionResult StartJob([FromRoute] Guid id)
        {
            _jobsManagementService.StartJob(id);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobAppointmentAsync([FromRoute] Guid id)
        {
            await _jobsManagementService.RemoveAppointmentAsync(id);
            return NoContent();
        }
    }
}
