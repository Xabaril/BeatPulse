using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core.Helpers
{
    public class UIAuthorizationHelper
    {
        public static async Task<bool> IsAuthorizedAsync(HttpContext context)
        {
            var authorizationFilters = (IEnumerable<IUIAuthorizationFilter>)context.RequestServices.GetService(typeof(IEnumerable<IUIAuthorizationFilter>));
            bool authorized = true;
            if (authorizationFilters.Any())
            {
                var filterTasks = new List<Task<bool>>();
                
                foreach (var filter in authorizationFilters)
                {
                    filterTasks.Add(filter.Accept(context));
                }

                await Task.WhenAll(filterTasks);
                return filterTasks.Select(t => t.Result).All(r => r);
            }

            return authorized;
        }
    }
}
