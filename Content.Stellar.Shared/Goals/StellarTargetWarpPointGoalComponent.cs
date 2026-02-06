// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Whitelist;

namespace Content.Stellar.Shared.Goals;

/// <summary>
/// Targets a random warp point
/// </summary>
[RegisterComponent]
public sealed partial class StellarTargetWarpPointGoalComponent : Component
{
    /// <summary>
    /// Warp points matching this blacklist will be excluded
    /// </summary>
    [DataField(required: true)]
    public EntityWhitelist? Blacklist;
}
