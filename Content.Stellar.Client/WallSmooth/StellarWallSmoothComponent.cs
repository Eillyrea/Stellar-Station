// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Stellar.Shared.WallSmooth;
using Robust.Shared.Prototypes;

namespace Content.Stellar.Client.WallSmooth;

[RegisterComponent]
public sealed partial class StellarWallSmoothComponent : Component
{
    [DataField]
    public ProtoId<StellarWallSmoothKeyPrototype>? Key;

    [DataField]
    public HashSet<ProtoId<StellarWallSmoothKeyPrototype>> OtherKeys = new();

    [DataField]
    public string FullState;

    [DataField]
    public string? FullOtherState;

    [DataField]
    public string? DownState;

    [DataField]
    public string? DownOtherState;

    [ViewVariables]
    internal (CornerFill ne, CornerFill nw, CornerFill sw, CornerFill se) SameCorners;

    [ViewVariables]
    internal (CornerFill ne, CornerFill nw, CornerFill sw, CornerFill se) OtherCorners;

    /// <summary>
    ///     Used by to reduce redundant updates.
    /// </summary>
    [ViewVariables]
    internal int UpdateGeneration;
}
