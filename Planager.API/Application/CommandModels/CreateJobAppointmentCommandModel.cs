using System;

namespace Planager.API.Application.CommandModels
{
    public class CreateJobAppointmentCommandModel
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string? ExternalUrl { get; init; }
        public string RepeatType { get; init; }
        public int? RepeatInterval { get; set; }
        public DateTime ShouldRunAt { get; }
        public TimeSpan Timezone { get; }
    }
}
