using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces
{
    public interface IPermissionAssignmentsService
    {
        public List<string> Assignments { get; set; }
        void Configure(string topResource, int topLevel, string wildCard, string separator);
        void Insert(string resource);
        bool Search(string resource, List<string> roles);
        bool SearchPrefix(string resource);
    }
}
