using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITaskRepository : IPaginate<TaskResource>, IGet<TaskResource>, ICreate<TaskResource>
    {
        TaskResource ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null);
        TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null);
        TaskResource ExecuteBackup(string description = null);
        TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null);
        TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell");
        TaskResource ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null);
        TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null);
        TaskDetailsResource GetDetails(TaskResource resource);
        string GetRawOutputLog(TaskResource resource);
        void Rerun(TaskResource resource);
        void Cancel(TaskResource resource);
        IReadOnlyList<TaskResource> GetQueuedBehindTasks(TaskResource resource);
        void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
    }
    
    class TaskRepository : BasicRepository<TaskResource>, ITaskRepository
    {
        public TaskRepository(IOctopusClient client)
            : base(client, "Tasks")
        {
        }

        public TaskResource ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.Health.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual health check" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.Health.Arguments.Timeout, TimeSpan.FromMinutes(timeoutAfterMinutes)},
                {BuiltInTasks.Health.Arguments.MachineTimeout, TimeSpan.FromMinutes(machineTimeoutAfterMinutes)},
                {BuiltInTasks.Health.Arguments.EnvironmentId, environmentId},
                {BuiltInTasks.Health.Arguments.MachineIds, machineIds}
            };
            return Create(resource);
        }

        public TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.UpdateCalamari.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual Calamari update" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.UpdateCalamari.Arguments.MachineIds, machineIds }
            };
            return Create(resource);
        }

        public TaskResource ExecuteBackup(string description = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.Backup.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description;
            return Create(resource);
        }

        public TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.Upgrade.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual upgrade" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.Upgrade.Arguments.EnvironmentId, environmentId},
                {BuiltInTasks.Upgrade.Arguments.MachineIds, machineIds}
            };
            return Create(resource);
        }

        public TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell")
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.AdHocScript.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Run ad-hoc PowerShell script" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                {BuiltInTasks.AdHocScript.Arguments.ScriptBody, scriptBody},
                {BuiltInTasks.AdHocScript.Arguments.Syntax, syntax}
            };
            return Create(resource);
        }

        public TaskResource ExecuteActionTemplate(ActionTemplateResource template, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null,
            string[] environmentIds = null, string[] targetRoles = null, string description = null)
        {
            if (string.IsNullOrEmpty(template?.Id)) throw new ArgumentException("The step template was either null, or has no ID");

            var resource = new TaskResource();
            resource.Name = BuiltInTasks.AdHocScript.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Run step template: " + template.Name : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                {BuiltInTasks.AdHocScript.Arguments.ActionTemplateId, template.Id},
                {BuiltInTasks.AdHocScript.Arguments.Properties, properties}
            };
            return Create(resource);
        }

        public TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.SyncCommunityActionTemplates.Name;
            resource.Description = description ?? "Run " + BuiltInTasks.SyncCommunityActionTemplates.Name;

            return Create(resource);
        }

        public TaskDetailsResource GetDetails(TaskResource resource)
        {
            return Client.Get<TaskDetailsResource>(resource.Link("Details"));
        }

        public string GetRawOutputLog(TaskResource resource)
        {
            return Client.Get<string>(resource.Link("Raw"));
        }

        public void Rerun(TaskResource resource)
        {
            Client.Post(resource.Link("Rerun"), (TaskResource)null);
        }

        public void Cancel(TaskResource resource)
        {
            Client.Post(resource.Link("Cancel"), (TaskResource)null);
        }

        public IReadOnlyList<TaskResource> GetQueuedBehindTasks(TaskResource resource)
        {
            return Client.ListAll<TaskResource>(resource.Link("QueuedBehind"));
        }

        public void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
        {
            WaitForCompletion(new[] { task }, pollIntervalSeconds, timeoutAfterMinutes, interval);
        }

        public void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
        {
            var start = Stopwatch.StartNew();
            if (tasks == null || tasks.Length == 0)
                return;

            while (true)
            {
                var stillRunning =
                (from task in tasks
                    let currentStatus = Client.Get<TaskResource>(task.Link("Self"))
                    select currentStatus).ToArray();

                if (interval != null)
                {
                    interval(stillRunning);
                }

                if (stillRunning.All(t => t.IsCompleted))
                    return;

                if (timeoutAfterMinutes > 0 && start.Elapsed.TotalMinutes > timeoutAfterMinutes)
                {
                    throw new TimeoutException(string.Format("One or more tasks did not complete before the timeout was reached. We waited {0:n1} minutes for the tasks to complete.", start.Elapsed.TotalMinutes));
                }

                Thread.Sleep(pollIntervalSeconds * 1000);
            }
        }
    }
}