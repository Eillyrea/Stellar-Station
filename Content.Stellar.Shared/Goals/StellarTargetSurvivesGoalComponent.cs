// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Stellar.Shared.Goals;

[RegisterComponent, NetworkedComponent]
[Access(typeof(StellarTargetSurvivesGoalSystem))]
public sealed partial class StellarTargetSurvivesGoalComponent : Component;

[RegisterComponent]
[Access(typeof(StellarTargetSurvivesGoalSystem))]
public sealed partial class StellarTargetedSurvivesComponent : StellarTargetedComponent;
