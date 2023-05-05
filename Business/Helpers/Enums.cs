using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.Helpers
{
    public static class ClaimsConstant
    {
        public const string FirstNameClaim = "firstName";
        public const string LastNameClaim = "lastName";
        public const string DesingnationClaim = "desingnation";

    }
    public enum ProfileType
    {
        Admin,
        CEO,
        CTO,
        GroupLead,
        SolutionAnalyst,
        ProgramAnalyst,
        Probationer,
        Intern,
        User,
    }


}
