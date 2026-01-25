// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Stellar.Client.WallSmooth;
using Content.Stellar.Shared.Cutaway;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Player;

namespace Content.Stellar.Client.Cutaway;

public sealed class StellarCutawaySystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;

    private StellarCutawayPreOverlay _preOverlay = default!;
    private StellarCutawayPostOverlay _postOverlay = default!;

    [Access(typeof(StellarCutawayPreOverlay), typeof(StellarCutawayPostOverlay))]
    public List<Entity<StellarWallSmoothComponent, SpriteComponent>> CutawayTargets = new(64);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarCutawayViewerComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<StellarCutawayViewerComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<StellarCutawayViewerComponent, LocalPlayerAttachedEvent>(OnLocalPlayerAttached);
        SubscribeLocalEvent<StellarCutawayViewerComponent, LocalPlayerDetachedEvent>(OnLocalPlayerDetached);

        _preOverlay = new();
        _postOverlay = new();
    }

    private void OnInit(Entity<StellarCutawayViewerComponent> ent, ref ComponentInit args)
    {
        if (_player.LocalSession?.AttachedEntity == ent.Owner)
            AddOverlays();
    }

    private void OnShutdown(Entity<StellarCutawayViewerComponent> ent, ref ComponentShutdown args)
    {
        if (_player.LocalSession?.AttachedEntity == ent.Owner)
            RemoveOverlays();
    }

    private void OnLocalPlayerAttached(Entity<StellarCutawayViewerComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        AddOverlays();
    }

    private void OnLocalPlayerDetached(Entity<StellarCutawayViewerComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        RemoveOverlays();
    }

    private void AddOverlays()
    {
        _overlay.AddOverlay(_preOverlay);
        _overlay.AddOverlay(_postOverlay);
    }

    private void RemoveOverlays()
    {
        _overlay.RemoveOverlay(_preOverlay);
        _overlay.RemoveOverlay(_postOverlay);
    }
}
