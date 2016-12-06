
using Newtonsoft.Json;
using Octopus.Client.Serialization;
using System;

namespace Octopus.Client.Model
{
    public enum DeleteMachinesBehavior
    {
        DoNotDelete,
        DeleteUnavailableMachines
    }

    public class MachineCleanupPolicy
    {
        public DeleteMachinesBehavior DeleteMachinesBehavior { get; set; }

        [JsonConverter(typeof(TotalHoursTimeSpanConverter))]
        public TimeSpan DeleteMachinesElapsedTimeSpan { get; set; }
    }
}
