// SPDX-FileCopyrightText: 2025 EmoGarbage404
// SPDX-FileCopyrightText: 2025 mirrorcult
//
// SPDX-License-Identifier: MIT

using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._ES.Sparks.Components;

/// <summary>
/// An entity that sparks when damaged by something
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSparksSystem))]
public sealed partial class ESSparkOnHitComponent : ESBaseSparkConfigurationComponent
{
    /// <summary>
    /// Amount of damage that needs to be dealt to cause sparks
    /// </summary>
    [DataField]
    public FixedPoint2 Threshold = 1;
}
