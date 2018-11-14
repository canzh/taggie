using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Services.DTO
{
    public class IdentityRequestModel
    {
        public int TeamId {get;set;}
        public string TeamName {get;set;}
        public int MemberCount {get;set;}
        public bool IsQA {get;set;}
    }
}
