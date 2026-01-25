// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.GameStates;

namespace Content.Stellar.Shared.Cutaway;

[RegisterComponent, NetworkedComponent]
public sealed partial class StellarCutawayViewerComponent : Component
{
    public static readonly Angle Angle = new(Math.PI / 2f);
    public const double AngleDistance = Math.PI / 3f;
}
