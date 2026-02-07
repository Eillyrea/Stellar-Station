// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

namespace Content.Stellar.Server.HazardSectors;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class StellarMajorThreatRuleComponent : Component
{
    /// <summary>
    /// Chance for the sector to have a Major Threat.
    /// </summary>
    [DataField]
    public float? ThreatChance;

    /// <summary>
    ///     Average time that the major threat starts at
    /// </summary>
    [DataField]
    public TimeSpan ThreatStartTimeAvg = TimeSpan.FromMinutes(60f);

    /// <summary>
    ///     Standard deviation for time that the major threat can start at
    /// </summary>
    [DataField]
    public TimeSpan ThreatStartTimeStdDev = TimeSpan.FromMinutes(5f);

    /// <summary>
    /// The upper bound on the time the threat can take
    /// </summary>
    [DataField]
    public TimeSpan ThreatMaximumTime = TimeSpan.FromMinutes(90f);

    /// <summary>
    ///     Picked randomly when the rule is added. Time into the round that the Hazard Sector's Major Threat activates, ending the round.
    ///     and time relative to which the phases should be announced. If no major threat is active, this value defaults to 1hr 30min.
    /// </summary>
    [DataField, AutoPausedField]
    public TimeSpan ExpectedRoundEndTime = TimeSpan.Zero;

    /// <summary>
    /// Whether the sector's Major Threat is enabled.
    /// </summary>
    public bool ThreatActive = false;
}
