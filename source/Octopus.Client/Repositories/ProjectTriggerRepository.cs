using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        ProjectTriggerResource FindByName(ProjectResource project, string name);
        ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action);
    }
    
    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusClient client)
            : base(client, "ProjectTriggers")
        {
        }

        public ProjectTriggerResource FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Triggers"));
        }

        public ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            return new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
        }
    }
}