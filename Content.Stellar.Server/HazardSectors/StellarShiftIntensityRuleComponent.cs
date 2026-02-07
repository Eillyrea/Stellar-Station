// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Stellar.Shared.HazardSectors;
using Robust.Shared.Prototypes;

namespace Content.Stellar.Server.HazardSectors;

[RegisterComponent]
public sealed partial class StellarShiftIntensityRuleComponent : Component
{
    /// <summary>
    /// What intensity preset are we using? These tie to hardcoded stuff inside the event scheduler.
    /// </summary>
    public ProtoId<StellarShiftIntensityPrototype> ShiftIntensityPreset;

    [DataField]
    public Dictionary<ProtoId<StellarShiftIntensityPrototype>, float> AvailableShiftIntensities = new()
    {
        {"StellarIntensityGreen", 7.5f},
        {"StellarIntensityYellow", 32.5f},
        {"StellarIntensityOrange", 37.5f},
        {"StellarIntensityRed", 17.5f},
        {"StellarIntensityBlack", 5f},
    };
}
