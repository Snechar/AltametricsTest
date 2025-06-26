using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Altametrics_Backend_C__.NET;
using System;
using System.Net; // Ensure this matches your Program.cs namespace

namespace Altametrics.Tests.Tests
{
    [TestClass]
    public class RSVPTests
    {
        public static HttpClient _client;

        [ClassInitialize]
        public static void Init(TestContext context)
        {

            var factory = new TestApplicationFactory();
            _client = factory.CreateClient();
        }

        [TestMethod]
        public async Task CreateRSVP_ShouldReturnSuccess()
        {
 
            var request = new
            {
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                GuestName = "Test Guest",
                Email = "test@example.com",
                GuestCount = 1,
                ResponseStatus = "Going",
                ReminderRequested = false
            };

            var response = await _client.PostAsJsonAsync("/api/RSVP", request);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Request failed: {await response.Content.ReadAsStringAsync()}");
        }
        [TestMethod]
        public async Task EditRSVP_ShouldReturnSuccess()
        {
            var request = new
            {
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                GuestName = "Updated Guest",
                Email = "diana@example.com",
                GuestCount = 2,
                ResponseStatus = "Maybe",
                ReminderRequested = true
            };

            var response = await _client.PutAsJsonAsync("/api/RSVP", request);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Edit failed: {await response.Content.ReadAsStringAsync()}");
        }
        [TestMethod]
        public async Task DeleteRSVP_ShouldReturnSuccess()
        {
            var eventCode = Guid.Parse("00000000-0000-0000-0000-000000000003");
            var email = "diana@example.com";

            var response = await _client.DeleteAsync($"/api/RSVP?eventCode={eventCode}&email={email}");

            Assert.IsTrue(response.IsSuccessStatusCode, $"Delete failed: {await response.Content.ReadAsStringAsync()}");
        }
        [TestMethod]
        public async Task EditRSVP_NonExistent_ShouldReturnNotFound()
        {
            var request = new
            {
                EventCode = Guid.NewGuid(), // Random GUID, not in DB
                GuestName = "Nonexistent Guest",
                Email = "nonexistent@example.com",
                GuestCount = 1,
                ResponseStatus = "Going",
                ReminderRequested = false
            };

            var response = await _client.PutAsJsonAsync("/api/RSVP", request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, $"Expected 404 NotFound, got: {await response.Content.ReadAsStringAsync()}");
        }
        [TestMethod]
        public async Task DeleteRSVP_NonExistent_ShouldReturnNotFound()
        {
            var eventCode = Guid.NewGuid(); // Random GUID
            var email = "ghost@example.com";

            var response = await _client.DeleteAsync($"/api/RSVP?eventCode={eventCode}&email={email}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, $"Expected 404 NotFound, got: {await response.Content.ReadAsStringAsync()}");
        }

    }
}
