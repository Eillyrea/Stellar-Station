// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Shared.GameTicking.Components;
using Robust.Shared.Random;

namespace Content.Stellar.Server.HazardSectors;

public sealed class StellarMajorThreatRuleSystem : StellarGameRuleSystem<StellarMajorThreatRuleComponent>
{
    protected override void Started(EntityUid uid,
        StellarMajorThreatRuleComponent comp,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        if (comp.ThreatChance is { } chance && Random.Prob(chance))
        {
            var randomMins = Random.NextGaussian(comp.ThreatStartTimeAvg.TotalMinutes, comp.ThreatStartTimeStdDev.TotalMinutes);
            randomMins = Math.Round(randomMins);
            comp.ExpectedRoundEndTime = Timing.CurTime + TimeSpan.FromMinutes(randomMins);
            comp.ThreatActive = true;
            Log.Info($"Major threat is ACTIVE.");
            Log.Info($"Round ends in {randomMins} minutes!");
        }
        else
        {
            comp.ExpectedRoundEndTime = Timing.CurTime + TimeSpan.FromMinutes(90);
            Log.Info($"No major threat.");
            Log.Info($"Round ends in 90 minutes.");
        }
    }
}
