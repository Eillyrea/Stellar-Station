// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Shared.EntityTable.EntitySelectors;


namespace Content.Stellar.Server.HazardSectors;

[RegisterComponent, AutoGenerateComponentPause, Access(typeof(StellarHazardSectorEventSchedulerSystem))]
public sealed partial class StellarHazardSectorEventSchedulerComponent : Component
{
    /// <summary>
    /// The gamerules that the scheduler can choose from
    /// </summary>
    [DataField]
    public EntityTableSelector ScheduledEvents;

    /// <summary>
    /// How long the scheduler waits to begin running events.
    /// </summary>
    [DataField, AutoPausedField]
    public TimeSpan TimeUntilNextEvent;

    /// <summary>
    ///     Average time for the scheduler to pick an event.
    /// </summary>
    [DataField]
    public TimeSpan EventStartTimeMin = TimeSpan.FromMinutes(2);

    /// <summary>
    ///     Standard deviation for the scheduler's time to pick an event.
    /// </summary>
    [DataField]
    public TimeSpan EventStartTimeMax = TimeSpan.FromMinutes(10);

}

