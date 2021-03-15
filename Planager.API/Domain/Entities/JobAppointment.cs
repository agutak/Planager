using System;

namespace Planager.API.Domain.Entities
{
    public class JobAppointment
    {
        private JobAppointment()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Name = "Default name";
        }

        public JobAppointment(
            string name,
            string? description,
            DateTime shouldRunAt,
            TimeSpan timezone,
            RepeatType repeatType,
            int? repeatInterval,
            string? externalUrl) : this()
        {
            Name = name;
            Description = description;
            ShouldRunAt = shouldRunAt;
            Timezone = timezone;
            RepeatType = repeatType;
            RepeatInterval = repeatInterval;
            ExternalUrl = externalUrl;
        }

        public Guid Id { get; init; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ModifiedAt { get; private set; }
        public string? ExternalUrl { get; private set; }
        public RepeatType RepeatType { get; private set; }
        public int? RepeatInterval { get; private set; }
        public DateTime ShouldRunAt { get; private set; }
        public TimeSpan Timezone { get; private set; }
    }
}
