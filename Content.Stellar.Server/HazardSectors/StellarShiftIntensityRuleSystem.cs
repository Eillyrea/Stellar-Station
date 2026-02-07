// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Shared.GameTicking.Components;
using Content.Shared.Random.Helpers;

namespace Content.Stellar.Server.HazardSectors;

public sealed class StellarShiftIntensityRuleSystem : StellarGameRuleSystem<StellarShiftIntensityRuleComponent>
{
    protected override void Started(EntityUid uid, StellarShiftIntensityRuleComponent comp, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, comp, gameRule, args);

        comp.ShiftIntensityPreset = Random.Pick(comp.AvailableShiftIntensities);

        Log.Info($"Shift Intensity: {comp.ShiftIntensityPreset}");
    }
}
