// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.Prototypes;

namespace Content.Stellar.Server.HazardSectors;

[RegisterComponent]
public sealed partial class StellarRandomHazardSectorRuleComponent : Component
{
    [DataField]
    public EntProtoId<StellarHazardSectorRuleComponent>? ChosenSector;

    [DataField]
    public Dictionary<EntProtoId<StellarHazardSectorRuleComponent>, float> AvailableSectors = new()
    {
        {"HazardSectorApocryphalRule", 1f},
        {"HazardSectorAzureRule", 1f},
        {"HazardSectorCorporateRule", 1f},
        {"HazardSectorCrystallineRule", 1f},
        {"HazardSectorMagnetarRule", 1f},
        {"HazardSectorMalignRule", 1f},
        {"HazardSectorPelagicRule", 1f},
        {"HazardSectorSupernovaRule", 1f},
    };
}
