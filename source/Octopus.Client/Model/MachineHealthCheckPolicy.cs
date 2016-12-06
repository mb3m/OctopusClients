﻿using System;
using Newtonsoft.Json;
using Octopus.Client.Serialization;

namespace Octopus.Client.Model
{
    public class MachineHealthCheckPolicy
    {
        public MachineScriptPolicy TentacleEndpointHealthCheckPolicy { get; set; }
        public MachineScriptPolicy SshEndpointHealthCheckPolicy { get; set; }
        [JsonConverter(typeof(TotalHoursTimeSpanConverter))]
        public TimeSpan HealthCheckInterval { get; set; }

        public MachineHealthCheckPolicy()
        {
            TentacleEndpointHealthCheckPolicy = new MachineScriptPolicy();
            SshEndpointHealthCheckPolicy = new MachineScriptPolicy();
            HealthCheckInterval = TimeSpan.FromHours(1);
        }

        [JsonConstructor]
        public MachineHealthCheckPolicy(MachineScriptPolicy tentacleEndpointHealthCheckPolicy, MachineScriptPolicy sshEndpointHealthCheckPolicy)
        {
            TentacleEndpointHealthCheckPolicy = tentacleEndpointHealthCheckPolicy;
            SshEndpointHealthCheckPolicy = sshEndpointHealthCheckPolicy;
        }
    }
}