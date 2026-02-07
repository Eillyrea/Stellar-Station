// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Shared.GameTicking.Components;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;

namespace Content.Stellar.Server.HazardSectors;

public sealed class StellarRandomHazardSectorRuleSystem : StellarGameRuleSystem<StellarRandomHazardSectorRuleComponent>
{
    [ViewVariables] private EntProtoId<StellarHazardSectorRuleComponent>? _forcedSector;

    protected override void Started(EntityUid uid,
        StellarRandomHazardSectorRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        var sector = _forcedSector ?? Random.Pick(component.AvailableSectors);
        _forcedSector = null;

        GameTicker.StartGameRule(sector);
    }

    public void ForceSector(EntProtoId<StellarHazardSectorRuleComponent> id)
    {
        _forcedSector = id;
    }
}
