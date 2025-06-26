using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Altametrics_Backend_C__.NET.Models.DTOs.Event;
using System.Net;
using System.Text;
using System.Text.Json;
using System;

namespace Altametrics.Tests.Tests
{
    [TestClass]
    public class EventTests
    {
        private static HttpClient _client;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            var factory = new TestApplicationFactory();
            _client = factory.CreateClient();
        }

        private static void AddAuthHeader()
        {
            // Simulate a JWT by manually setting Authorization header
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "mock_token_for_owner@example.com");
        }

        [TestMethod]
        public async Task CreateEvent_ShouldReturnSuccess()
        {
            AuthTestHelper.AddJwtAuthorizationHeader(_client, "1");

            var request = new CreateEventModel
            {
                Name = "Test Event",
                Description = "Test Description",
                Location = "Test Location",
                EventDate = DateTime.UtcNow.AddDays(1)
            };

            var response = await _client.PostAsJsonAsync("/api/Event", request);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to create event: {await response.Content.ReadAsStringAsync()}");
        }

        [TestMethod]
        public async Task GetEventByCode_ShouldReturnEvent()
        {
            var eventCode = "00000000-0000-0000-0000-000000000000";
            var response = await _client.GetAsync($"/api/Event/{eventCode}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetEventDetails_ShouldReturnEventWithRSVPs()
        {
            AuthTestHelper.AddJwtAuthorizationHeader(_client, "1");
            var eventCode = "00000000-0000-0000-0000-000000000000";

            var response = await _client.GetAsync($"/api/Event/{eventCode}/details");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetMyEvents_ShouldReturnPaginatedList()
        {
            AuthTestHelper.AddJwtAuthorizationHeader(_client, "1");

            var response = await _client.GetAsync("/api/Event/my?page=1&pageSize=10");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateEvent_ShouldReturnSuccess()
        {
            AuthTestHelper.AddJwtAuthorizationHeader(_client, "1"   );

            var updateModel = new EventUpdateModel
            {
                Name = "Updated Event",
                Description = "Updated Description",
                EventDate = DateTime.UtcNow.AddDays(3),
                Location = "Updated Location"
            };

            var response = await _client.PutAsJsonAsync("/api/Event/1", updateModel);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteEvent_ShouldReturnSuccess()
        {
            AuthTestHelper.AddJwtAuthorizationHeader(_client, "1");

            var response = await _client.DeleteAsync("/api/Event/4");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task UnauthorizedCreateEvent_ShouldReturn401()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var request = new CreateEventModel
            {
                Name = "Unauthorized Event",
                EventDate = DateTime.UtcNow.AddDays(2)
            };

            var response = await _client.PostAsJsonAsync("/api/Event", request);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task GetEventByInvalidCode_ShouldReturn404()
        {
            var invalidCode = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/Event/{invalidCode}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}