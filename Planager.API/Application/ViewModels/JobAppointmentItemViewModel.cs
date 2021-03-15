using System;

namespace Planager.API.Application.ViewModels
{
    public class JobAppointmentItemViewModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string RepeatType { get; init; }
        public int? RepeatInterval { get; init; }
        public DateTimeOffset ShouldRunAt { get; init; }
    }
}
