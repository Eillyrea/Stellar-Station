// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Server.GameTicking;
using Content.Server.StationEvents;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Stellar.Server.HazardSectors;

public sealed class StellarHazardSectorEventSchedulerSystem : StellarGameRuleSystem<StellarHazardSectorEventSchedulerComponent>
{
    [Dependency] private readonly EventManagerSystem _event = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void Started(EntityUid uid, StellarHazardSectorEventSchedulerComponent comp, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, comp, gameRule, args);

        comp.TimeUntilNextEvent = _timing.CurTime + _random.Next(TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(8)); // First event of the round spawns early to kick things off.
    }

    protected override void ActiveTick(EntityUid uid, StellarHazardSectorEventSchedulerComponent comp, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, comp, gameRule, frameTime);

        // can this even happen?
        if (_ticker.RunLevel != GameRunLevel.InRound)
            return;

        if (comp.TimeUntilNextEvent > _timing.CurTime)
            return;

        ScheduleNextEvent((uid, comp));
        _event.RunRandomEvent(comp.ScheduledEvents);
    }

    /// <summary>
    /// Sector intensity acts as a scalar for linearly interpolating between the Max event Start Time and Min event Start Time.
    /// Are these magic numbers? Sort of. Do i care? Absolutely not. The math works with any given maxRoundTime, and lerping is straightforward.
    /// </summary>
    private void ScheduleNextEvent(Entity<StellarHazardSectorEventSchedulerComponent> ent)
    {
        var intensity = GetShiftIntensity();
        var roundTime = ExpectedRoundDuration();
        var currentRoundTime = _ticker.RoundDuration();
        if (currentRoundTime > roundTime)
            currentRoundTime = roundTime;

        var lerpedIntensity = (currentRoundTime/roundTime) * intensity.Intensity;
        var lerpedTime = MathHelper.Lerp(ent.Comp.EventStartTimeMax.TotalSeconds, ent.Comp.EventStartTimeMin.TotalSeconds, lerpedIntensity);

        ent.Comp.TimeUntilNextEvent = _timing.CurTime + TimeSpan.FromSeconds(lerpedTime);
    }
}
