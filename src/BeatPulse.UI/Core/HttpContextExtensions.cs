using BeatPulse.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
    {
        public static async Task<bool> IsAuthorizedAsync(this HttpContext context)
        {
            var authorizationFilters = context.RequestServices
                .GetServices<IUIAuthorizationFilter>();

            var authorized = true;

            if (authorizationFilters.Any())
            {
                var filterTasks = new List<Task<bool>>();

                foreach (var item in authorizationFilters)
                {
                    filterTasks.Add(item.Accept(context));
                }

                await Task.WhenAll(filterTasks);

                authorized = filterTasks.Select(task => task.Result).All(result => result);
            }

            return authorized;
        }
    }
}
