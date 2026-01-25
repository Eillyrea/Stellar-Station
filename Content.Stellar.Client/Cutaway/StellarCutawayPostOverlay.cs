// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Stellar.Client.WallSmooth;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Stellar.Client.Cutaway;

public sealed class StellarCutawayPostOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entity = default!;
    private readonly StellarCutawaySystem _cutaway;
    private readonly StellarWallSmoothSystem _wallSmooth;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    public StellarCutawayPostOverlay()
    {
        IoCManager.InjectDependencies(this);

        _cutaway = _entity.System<StellarCutawaySystem>();
        _wallSmooth = _entity.System<StellarWallSmoothSystem>();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        foreach (var ent in _cutaway.CutawayTargets)
        {
            _wallSmooth.ApplyCorners(ent, false);
        }

        _cutaway.CutawayTargets.Clear();
    }
}
