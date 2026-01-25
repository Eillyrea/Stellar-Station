// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Stellar.Client.WallSmooth;
using Content.Stellar.Shared.Cutaway;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Stellar.Client.Cutaway;

public sealed class StellarCutawayPreOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entity = default!;
    private readonly SharedTransformSystem _xform;
    private readonly StellarCutawayTreeSystem _tree;
    private readonly StellarWallSmoothSystem _wallSmooth;
    private readonly StellarCutawaySystem _cutaway;

    private readonly EntityQuery<StellarWallSmoothComponent> _wallSmoothQuery;
    private readonly EntityQuery<SpriteComponent> _spriteQuery;

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowEntities;

    public StellarCutawayPreOverlay()
    {
        IoCManager.InjectDependencies(this);

        _xform = _entity.System<SharedTransformSystem>();
        _tree = _entity.System<StellarCutawayTreeSystem>();
        _wallSmooth = _entity.System<StellarWallSmoothSystem>();
        _cutaway = _entity.System<StellarCutawaySystem>();

        _wallSmoothQuery = _entity.GetEntityQuery<StellarWallSmoothComponent>();
        _spriteQuery = _entity.GetEntityQuery<SpriteComponent>();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (args.Viewport.Eye is not { } eye)
            return;

        var targets = _tree.QueryAabb(args.MapId, args.WorldBounds);

        foreach (var target in targets)
        {
            var targetPos = _xform.GetWorldPosition(target.Transform);

            var targetOffset = eye.Rotation.RotateVec(eye.Position.Position) - eye.Rotation.RotateVec(targetPos);
            var offsetAngle = targetOffset.ToAngle();
            var distance = Angle.ShortestDistance(offsetAngle, StellarCutawayViewerComponent.Angle);

            if (Math.Abs(distance.Theta) > StellarCutawayViewerComponent.AngleDistance)
                continue;

            if (!_wallSmoothQuery.TryComp(target.Uid, out var wallSmooth) ||
                !_spriteQuery.TryComp(target.Uid, out var sprite))
                continue;

            _wallSmooth.ApplyCorners((target.Uid, wallSmooth, sprite), true);
            _cutaway.CutawayTargets.Add((target.Uid, wallSmooth, sprite));
        }
    }
}
