// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Stellar.Shared.Goals;

public sealed class StellarTargetSelfMindGoalSystem : EntitySystem
{
    [Dependency] private readonly StellarTargetedGoalSystem _targetedGoal = default!;
    [Dependency] private readonly StellarGoalsSystem _goals = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarTargetSelfMindGoalComponent, StellarGoalInsertedEvent>(OnInserted);
    }

    private void OnInserted(Entity<StellarTargetSelfMindGoalComponent> ent, ref StellarGoalInsertedEvent args)
    {
        if (!_goals.TryGetIndividualGoalOwner(args.Container.AsNullable(), out var observer))
            return;

        _targetedGoal.SetTarget(ent.Owner, observer);
    }
}
