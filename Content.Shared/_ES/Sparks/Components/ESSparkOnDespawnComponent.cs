// SPDX-FileCopyrightText: 2025 EmoGarbage404
// SPDX-FileCopyrightText: 2025 mirrorcult
//
// SPDX-License-Identifier: MIT

using Content.Shared._ES.Core.Timer.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Spawners;

namespace Content.Shared._ES.Sparks.Components;

/// <summary>
/// An entity with <see cref="TimedDespawnComponent"/> or <see cref="ESTimedDespawnComponent"/> that sparks when it despawns.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSparksSystem))]
public sealed partial class ESSparkOnDespawnComponent : ESBaseSparkConfigurationComponent;
