using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Content.Mvc.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Content.Mvc.Services
{
    public class DataService : IDataService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _apiClient;
        private readonly string _apiUrl;

        public DataService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _apiClient = httpClient;
            _settings = settings;

            _apiUrl = $"{_settings.Value.DataUrl}/api";
        }

        public async Task<IEnumerable<TeamViewModel>> GetAllTeams()
        {
            var responseString = await _apiClient.GetStringAsync($"{_apiUrl}/teams");

            var response = JsonConvert.DeserializeObject<List<TeamViewModel>>(responseString);

            return response;
        }

        public async Task<int> CreateNewTeam(TeamViewModel model)
        {
            //var dataModel = new { TeamName = model.TeamName, MemberCount = model.MemberCount, Description = model.Description };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync($"{_apiUrl}/teams", jsonContent);


            response.EnsureSuccessStatusCode();

            var newTeam = JObject.Parse(await response.Content.ReadAsStringAsync());
            var newId = newTeam.Value<int>("Id");

            return newId;
        }

        public async Task<TeamViewModel> GetTeamDetail(int teamId)
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/teams/detail?id={1}", _apiUrl, teamId));

            var response = JsonConvert.DeserializeObject<TeamViewModel>(responseString);

            return response;

            //var team = JObject.Parse(responseString);

            //return new TeamViewModel
            //{
            //    Id = team.Value<int>("Id"),
            //    TeamName = team.Value<string>("TeamName"),
            //    Description = team.Value<string>("Description"),
            //    MemberCount = team.Value<int>("MemberCount"),
            //    Status = (TeamStatus)team.Value<int>("Status"),
            //    CreatedDate = team.Value<DateTime>("CreatedDate")
            //};
        }

        public async Task<List<TeamAssignmentViewModel>> GetTeamAssignments(int teamId)
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/teams/assignment?id={1}", _apiUrl, teamId));
            var response = JsonConvert.DeserializeObject<List<TeamAssignmentViewModel>>(responseString);

            return response;
        }

        public async Task<IEnumerable<ProjectViewModel>> GetAllProjects()
        {
            var responseString = await _apiClient.GetStringAsync($"{_apiUrl}/projects");

            var response = JsonConvert.DeserializeObject<List<ProjectViewModel>>(responseString);

            return response;
        }

        public async Task<ProjectViewModel> GetProjectDetail(int projectId)
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/projects/{1}", _apiUrl, projectId));

            var response = JsonConvert.DeserializeObject<ProjectViewModel>(responseString);

            return response;

            //var team = JObject.Parse(responseString);

            //return new ProjectViewModel
            //{
            //    Id = team.Value<int>("Id"),
            //    TeamName = team.Value<string>("TeamName"),
            //    Description = team.Value<string>("Description"),
            //    MemberCount = team.Value<int>("MemberCount"),
            //    Status = (TeamStatus)team.Value<int>("Status"),
            //    CreatedDate = team.Value<DateTime>("CreatedDate")
            //};
        }

        public async Task AssignProjectItemsToTeam(int projectId, ProjectAssignmentViewModel model)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync($"{_apiUrl}/projects/{projectId}", jsonContent);

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<TaggieProjectListViewModel>> GetTaggieProjectList()
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/projects/taggie", _apiUrl));

            var response = JsonConvert.DeserializeObject<List<TaggieProjectListViewModel>>(responseString);

            return response;
        }

        public async Task<List<ProjectCategoryViewModel>> GetProjectCategories(int projectId)
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/projectcategories", _apiUrl));

            var response = JsonConvert.DeserializeObject<List<ProjectCategoryViewModel>>(responseString);

            return response;
        }

        public async Task<List<ProjectCategoryViewModel>> GetProjectSubcategories(int projectId)
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/projectsubcategories", _apiUrl));

            var response = JsonConvert.DeserializeObject<List<ProjectCategoryViewModel>>(responseString);

            return response;
        }

        public async Task<TaggieQueueViewModel> GetNextQueueItem(int projectId)
        {
            HttpResponseMessage response = await _apiClient.GetAsync(string.Format("{0}/projectitems/next?projectId={1}", _apiUrl, projectId));

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TaggieQueueViewModel>(responseString);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return null;
        }

        public async Task<string> GetQueueItemContent(int projectItemId)
        {
            var responseString = await _apiClient.GetStringAsync(string.Format("{0}/QueueItemContent/{1}", _apiUrl, projectItemId));
            return responseString;
        }

        public async Task SubmitQueueItem(TaggieQueueViewModel queueItem)
        {

        }


    }
}
