using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IEventRepository : IGet<EventResource>
    {
        Task<ResourceCollection<EventResource>> List(int skip = 0,
                string from = null,
                string to = null,
                string regarding = null,
                string regardingAny = null,
                string users = null,
                string projects = null,
                string environments = null,
                string eventGroups = null,
                string eventCategories = null,
                string tenants = null,
                string tags = null);
    }

    class EventRepository : BasicRepository<EventResource>, IEventRepository
    {
        public EventRepository(IOctopusAsyncClient client)
            : base(client, "Events")
        {
        }

        public Task<ResourceCollection<EventResource>> List(int skip = 0,
                string from = null,
                string to = null,
                string regarding = null,
                string regardingAny = null,
                string users = null,
                string projects = null,
                string environments = null,
                string eventGroups = null,
                string eventCategories = null,
                string tenants = null,
                string tags = null)
        {
            return Client.List<EventResource>(Client.RootDocument.Link("Events"), new
            {
                skip,
                from = from,
                to = to,
                regarding = regarding,
                regardingAny = regardingAny,
                users = users,
                projects = projects,
                environments = environments,
                eventGroups = eventGroups,
                eventCategories = eventCategories,
                tenants = tenants,
                tags = tags
            });
        }
    }
}