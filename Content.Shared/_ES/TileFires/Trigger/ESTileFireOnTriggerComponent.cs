// SPDX-FileCopyrightText: 2025 EmoGarbage404
// SPDX-FileCopyrightText: 2025 mirrorcult
//
// SPDX-License-Identifier: MIT

using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Shared._ES.TileFires.Trigger;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESTileFireOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    ///     Tile fire stage to spawn.
    /// </summary>
    [DataField]
    public int Stage = 1;
}
