using NationalParkWebApp.Models;
using NationalParkWebApp.Repository.IRepository;
using System.Net.Http.Json;

namespace NationalParkWebApp.Repository
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AppointmentRepository(IHttpClientFactory httpClientFactory):base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
public async Task<Appointment> CreateAppointmentAsync(
    string url, Appointment appointment)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync(url, appointment);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return await response.Content.ReadFromJsonAsync<Appointment>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(errorMessage)
                    ? "Booking failed. Please check the available capacity."
                    : errorMessage.Trim('"'));
        }
    }
}
