using Microsoft.VisualStudio.TestTools.UnitTesting;
using Planager.API.Application.Services;
using Planager.API.Domain.Entities;
using Planager.UnitTests.Fakes;
using System;

namespace Planager.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly FakeLogger _logger;

        public UnitTest1()
        {
            _logger = new();
        }

        [TestMethod]
        public void SucceedsToStartRunningJob()
        {
            //Arrange

            var jobApp = new JobAppointment(
                "Job 1",
                "My job #1",
                DateTime.UtcNow,
                TimeSpan.Zero,
                RepeatType.NoRepeat,
                null,
                "https://localhost:4444");

            var job = new RunningJob(jobApp, _logger);

            //Act
            job.Start(default);

            //Assert

            Assert.AreEqual(JobStatus.Running, job.Status);
        }
    }
}
