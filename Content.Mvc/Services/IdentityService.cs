using Content.Mvc.Models;
using Content.Mvc.Services.DTO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Content.Mvc.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _apiClient;
        private readonly string _identityUrl;

        public IdentityService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _apiClient = httpClient;
            _settings = settings;

            _identityUrl = $"{_settings.Value.IdentityUrl}/api/teamuser";
        }

        public async Task CreateTeamUsers(IdentityRequestModel teamModel)
        {
            var response = await _apiClient.PostAsJsonAsync(_identityUrl, teamModel);

            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<IdentityResponseModel>> GetTeamMembers(int teamId)
        {
            var responseString = await _apiClient.GetStringAsync($"{_identityUrl}/{teamId}");

            return JsonConvert.DeserializeObject<IEnumerable<IdentityResponseModel>>(responseString);
        }
    }
}
