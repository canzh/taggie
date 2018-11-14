using Content.Mvc.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Services
{
    public interface IIdentityService
    {
        Task CreateTeamUsers(IdentityRequestModel teamModel);
        Task<IEnumerable<IdentityResponseModel>> GetTeamMembers(int teamId);
    }
}
