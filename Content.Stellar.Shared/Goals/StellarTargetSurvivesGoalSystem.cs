// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Mind;
using Content.Shared.Mobs;

namespace Content.Stellar.Shared.Goals;

public sealed class StellarTargetSurvivesGoalSystem : StellarBaseTargetedGoalSystem<StellarTargetSurvivesGoalComponent, StellarTargetedSurvivesComponent>
{
    [Dependency] private readonly SharedMindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarTargetSurvivesGoalComponent, StellarGetGoalProgressEvent>(OnGetProgress);
        RefreshOnEvent<MindRelayedEvent<MobStateChangedEvent>>();
    }

    private void OnGetProgress(Entity<StellarTargetSurvivesGoalComponent> ent, ref StellarGetGoalProgressEvent args)
    {
        if (GetTarget(ent.Owner) is not { } target || !TryComp<MindComponent>(target, out var mindComp))
            return;

        args.Progress = _mind.IsCharacterDeadIc(mindComp) ? 0f : 1f;
    }
}
