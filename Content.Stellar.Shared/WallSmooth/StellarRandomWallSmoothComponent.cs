// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Stellar.Shared.WallSmooth;

/// <summary>
/// Randomizes the FullState of StellarWallSmoothComponent
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class StellarRandomWallSmoothComponent : Component
{
    /// <summary>
    /// The set of full states to select from.
    /// </summary>
    [DataField(required: true)]
    public List<string> RandomFullStates = new();
}

[Serializable, NetSerializable]
public enum RandomWallSmoothState : byte
{
    State
}
