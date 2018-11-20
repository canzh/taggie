using Content.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Services
{
    public interface IDataService
    {
        Task<IEnumerable<TeamViewModel>> GetAllTeams();
        Task<int> CreateNewTeam(TeamViewModel model);
        Task<TeamViewModel> GetTeamDetail(int teamId);
        Task<List<TeamAssignmentViewModel>> GetTeamAssignments(int teamId);

        Task<IEnumerable<ProjectViewModel>> GetAllProjects();
        Task<ProjectViewModel> GetProjectDetail(int projectId);
        Task AssignProjectItemsToTeam(int projectId, ProjectAssignmentViewModel model);

        Task<List<TaggieProjectListViewModel>> GetTaggieProjectList();

        Task<List<ProjectCategoryViewModel>> GetProjectCategories(int projectId);
        Task<List<ProjectCategoryViewModel>> GetProjectSubcategories(int projectId);

        Task<TaggieQueueViewModel> GetNextQueueItem(int projectId);
        Task<string> GetQueueItemContent(int projectItemId);
        Task SubmitQueueItem(TaggieQueueViewModel queueItem);
    }
}
