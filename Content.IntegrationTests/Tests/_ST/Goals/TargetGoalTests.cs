// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Diagnostics;
using Content.IntegrationTests.Tests._Citadel;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Players;
using Content.Stellar.Shared.Goals;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._ST.Goals;

[TestFixture]
public sealed class TargetGoalTests : GameTest
{
    [TestPrototypes]
    private const string Prototypes = @"
- type: entity
  id: TargetGoalTestsSurviveGoal
  components:
  - type: StellarGoal
  - type: StellarTargetedGoal
  - type: StellarTargetSelfMindGoal
  - type: StellarTargetSurvivesGoal

- type: entity
  id: TargetGoalTestsMobStateMob
  parent: InteractionTestMob
  components:
  - type: MobState
";

    private const string TargetGoalTestsSurviveGoal = "TargetGoalTestsSurviveGoal";
    private const string TargetGoalTestsMobStateMob = "TargetGoalTestsMobStateMob";

    [System(Side.Server)] private readonly StellarGoalsSystem _goals = default!;
    [System(Side.Server)] private readonly MobStateSystem _mobState = default!;

    [Test]
    public async Task SurviveGoal()
    {
        await Pair.CreateTestMap();

        _ = await AssignPlayerBody(Player!, TargetGoalTestsMobStateMob);
        var mind = Player!.ContentData()!.Mind!.Value;
        var body = Player!.AttachedEntity!.Value;

        await Server.WaitAssertion(() =>
        {
            var container = _goals.GetIndividualGoalContainer(mind);
            var ok = _goals.TryAddGoal(container, TargetGoalTestsSurviveGoal, out var goal);
            Assert.That(ok, Is.True);
            Assert.That(goal, Is.Not.Null);

            var it = goal.Value!;
            Assert.That(it.Comp.Progress, Is.GreaterThanOrEqualTo(1d));

            _mobState.ChangeMobState(body, MobState.Dead);
            Assert.That(it.Comp.Progress, Is.LessThanOrEqualTo(0d));

            _mobState.ChangeMobState(body, MobState.Alive);
            Assert.That(it.Comp.Progress, Is.GreaterThanOrEqualTo(1d));
        });
    }
}
