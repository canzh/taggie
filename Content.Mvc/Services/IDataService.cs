﻿using Content.Mvc.Models;
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

        Task<IEnumerable<ProjectViewModel>> GetAllProjects();
        Task<ProjectViewModel> GetProjectDetail(int projectId);
    }
}
