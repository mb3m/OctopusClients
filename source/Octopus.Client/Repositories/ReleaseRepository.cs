using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IReleaseRepository : IGet<ReleaseResource>, ICreate<ReleaseResource>, IPaginate<ReleaseResource>, IModify<ReleaseResource>, IDelete<ReleaseResource>
    {
        ResourceCollection<DeploymentResource> GetDeployments(ReleaseResource release, int skip = 0);
        ResourceCollection<ArtifactResource> GetArtifacts(ReleaseResource release, int skip = 0);
        DeploymentTemplateResource GetTemplate(ReleaseResource release);
        DeploymentPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        ReleaseResource SnapshotVariables(ReleaseResource release);    
        ReleaseResource Create(ReleaseResource resource, bool ignoreChannelRules = false);
        ReleaseResource Modify(ReleaseResource resource, bool ignoreChannelRules = false);
    }
    
    class ReleaseRepository : BasicRepository<ReleaseResource>, IReleaseRepository
    {
        public ReleaseRepository(IOctopusClient client)
            : base(client, "Releases")
        {
        }

        public ResourceCollection<DeploymentResource> GetDeployments(ReleaseResource release, int skip = 0)
        {
            return Client.List<DeploymentResource>(release.Link("Deployments"), new { skip });
        }

        public ResourceCollection<ArtifactResource> GetArtifacts(ReleaseResource release, int skip = 0)
        {
            return Client.List<ArtifactResource>(release.Link("Artifacts"), new { skip });
        }

        public DeploymentTemplateResource GetTemplate(ReleaseResource release)
        {
            return Client.Get<DeploymentTemplateResource>(release.Link("DeploymentTemplate"));
        }

        public DeploymentPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<DeploymentPreviewResource>(promotionTarget.Link("Preview"));
        }

        public ReleaseResource SnapshotVariables(ReleaseResource release)
        {
            Client.Post(release.Link("SnapshotVariables"));
            return Get(release.Id);
        }

        public ReleaseResource Create(ReleaseResource resource, bool ignoreChannelRules = false)
        {
            return Client.Create(Client.RootDocument.Link(CollectionLinkName), resource, new { ignoreChannelRules });
        }

        public ReleaseResource Modify(ReleaseResource resource, bool ignoreChannelRules = false)
        {
            return Client.Update(resource.Links["Self"], resource, new { ignoreChannelRules });
        }
    }
}