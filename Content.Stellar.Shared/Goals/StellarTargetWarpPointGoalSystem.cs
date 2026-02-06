// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Warps;
using Content.Shared.Whitelist;
using Robust.Shared.Random;

namespace Content.Stellar.Shared.Goals;

public sealed class StellarTargetWarpPointGoalSystem : EntitySystem
{
    [Dependency] private readonly StellarTargetedGoalSystem _targetedGoal = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly EntityWhitelistSystem _entityWhitelist = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarTargetWarpPointGoalComponent, StellarGoalInsertedEvent>(OnInserted);
        SubscribeLocalEvent<StellarTargetWarpPointGoalComponent, StellarTargetNameEvent>(OnName);
    }

    private void OnInserted(Entity<StellarTargetWarpPointGoalComponent> ent, ref StellarGoalInsertedEvent args)
    {
        var warps = new List<EntityUid>();
        var query = EntityQueryEnumerator<WarpPointComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            if (_entityWhitelist.IsWhitelistPass(comp.Blacklist, uid))
                continue;

            if (comp.Location == null)
                continue;

            warps.Add(uid);
        }

        if (warps.Count <= 0)
            return;

        _targetedGoal.SetTarget(ent.Owner, _random.Pick(warps));
    }

    private void OnName(Entity<StellarTargetWarpPointGoalComponent> ent, ref StellarTargetNameEvent args)
    {
        if (!TryComp<WarpPointComponent>(args.Target, out var warpPoint) || warpPoint.Location is not { } name)
            return;

        args.Name = name;
    }
}
