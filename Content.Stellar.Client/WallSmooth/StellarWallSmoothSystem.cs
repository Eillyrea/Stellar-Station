// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Client.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Content.Stellar.Shared.WallSmooth;

namespace Content.Stellar.Client.WallSmooth;

public sealed class StellarWallSmoothSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private int _generation;

    private EntityQuery<MapGridComponent> _mapGridQuery = default!;
    private EntityQuery<SpriteComponent> _spriteQuery = default!;
    private EntityQuery<StellarWallSmoothComponent> _wallSmoothQuery = default!;
    private EntityQuery<TransformComponent> _xformQuery = default!;

    private readonly Queue<EntityUid> _dirtyEntities = new();
    private readonly Queue<EntityUid> _anchorChangedEntities = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarWallSmoothComponent, AnchorStateChangedEvent>(OnAnchorChanged);
        SubscribeLocalEvent<StellarWallSmoothComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<StellarWallSmoothComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<StellarWallSmoothComponent, AppearanceChangeEvent>(OnAppearanceChange);

        _mapGridQuery = GetEntityQuery<MapGridComponent>();
        _spriteQuery = GetEntityQuery<SpriteComponent>();
        _wallSmoothQuery = GetEntityQuery<StellarWallSmoothComponent>();
        _xformQuery = GetEntityQuery<TransformComponent>();
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        while (_anchorChangedEntities.TryDequeue(out var entity))
        {
            if (!_xformQuery.TryGetComponent(entity, out var xform) || !_wallSmoothQuery.TryGetComponent(entity, out var wallSmooth))
                continue;

            if (xform.MapID == MapId.Nullspace)
                continue;

            DirtyNeighbours((entity, wallSmooth, xform));
        }

        if (_dirtyEntities.Count == 0)
            return;

        _generation++;

        while (_dirtyEntities.TryDequeue(out var entity))
        {
            if (!_wallSmoothQuery.TryGetComponent(entity, out var wallSmooth))
                continue;

            Recompute((entity, wallSmooth));
        }
    }

    private void OnAnchorChanged(Entity<StellarWallSmoothComponent> ent, ref AnchorStateChangedEvent args)
    {
        if (!args.Detaching)
            _anchorChangedEntities.Enqueue(ent);
    }

    private void OnStartup(Entity<StellarWallSmoothComponent> ent, ref ComponentStartup args)
    {
        var xform = Transform(ent);
        if (xform.Anchored)
        {
            DirtyNeighbours((ent, ent, xform));
        }

        InitializeLayers((ent, ent));
    }

    private void OnShutdown(Entity<StellarWallSmoothComponent> ent, ref ComponentShutdown args)
    {
        DirtyNeighbours((ent, ent, Transform(ent)));
    }

    private void DirtyNeighbours(Entity<StellarWallSmoothComponent, TransformComponent> ent)
    {
        if (!ent.Comp1.Running)
            return;

        Entity<MapGridComponent>? gridEnt = null;
        Vector2i pos;

        _dirtyEntities.Enqueue(ent);

        if (ent.Comp2.Anchored && _mapGridQuery.TryGetComponent(ent.Comp2.GridUid, out var gridComp))
        {
            gridEnt = (ent.Comp2.GridUid.Value, gridComp);
            pos = _map.CoordinatesToTile(gridEnt.Value, gridEnt.Value, ent.Comp2.Coordinates);
        }
        else
        {
            return;
            // last known position
        }

        if (gridEnt is not { } grid)
            return;

        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(1, 0)));
        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(-1, 0)));
        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(0, 1)));
        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(0, -1)));

        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(1, 1)));
        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(-1, -1)));
        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(-1, 1)));
        DirtyEntities(_map.GetAnchoredEntitiesEnumerator(grid, grid, pos + new Vector2i(1, -1)));
    }

    private void DirtyEntities(AnchoredEntitiesEnumerator entities)
    {
        while (entities.MoveNext(out var entity))
        {
            _dirtyEntities.Enqueue(entity.Value);
        }
    }

    private void InitializeLayers(Entity<StellarWallSmoothComponent, SpriteComponent?> ent)
    {
        if (!_spriteQuery.Resolve(ent, ref ent.Comp2, true))
            return;

        _sprite.LayerMapRemove((ent, ent), CornerLayers.SEBase);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.NEBase);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.NWBase);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.SWBase);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.SEOther);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.NEOther);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.NWOther);
        _sprite.LayerMapRemove((ent, ent), CornerLayers.SWOther);

        var state0 = $"{ent.Comp1.FullState}0";
        _sprite.LayerMapSet((ent, ent), CornerLayers.SEBase, _sprite.AddRsiLayer((ent, ent), state0));
        _sprite.LayerSetDirOffset((ent, ent), CornerLayers.SEBase, SpriteComponent.DirectionOffset.None);
        _sprite.LayerMapSet((ent, ent), CornerLayers.NEBase, _sprite.AddRsiLayer((ent, ent), state0));
        _sprite.LayerSetDirOffset((ent, ent), CornerLayers.NEBase, SpriteComponent.DirectionOffset.CounterClockwise);
        _sprite.LayerMapSet((ent, ent), CornerLayers.NWBase, _sprite.AddRsiLayer((ent, ent), state0));
        _sprite.LayerSetDirOffset((ent, ent), CornerLayers.NWBase, SpriteComponent.DirectionOffset.Flip);
        _sprite.LayerMapSet((ent, ent), CornerLayers.SWBase, _sprite.AddRsiLayer((ent, ent), state0));
        _sprite.LayerSetDirOffset((ent, ent), CornerLayers.SWBase, SpriteComponent.DirectionOffset.Clockwise);

        if (ent.Comp1.FullOtherState is { } otherState)
        {
            state0 = $"{otherState}0";

            _sprite.LayerMapSet((ent, ent), CornerLayers.SEOther, _sprite.AddRsiLayer((ent, ent), state0));
            _sprite.LayerSetDirOffset((ent, ent), CornerLayers.SEOther, SpriteComponent.DirectionOffset.None);
            _sprite.LayerMapSet((ent, ent), CornerLayers.NEOther, _sprite.AddRsiLayer((ent, ent), state0));
            _sprite.LayerSetDirOffset((ent, ent), CornerLayers.NEOther, SpriteComponent.DirectionOffset.CounterClockwise);
            _sprite.LayerMapSet((ent, ent), CornerLayers.NWOther, _sprite.AddRsiLayer((ent, ent), state0));
            _sprite.LayerSetDirOffset((ent, ent), CornerLayers.NWOther, SpriteComponent.DirectionOffset.Flip);
            _sprite.LayerMapSet((ent, ent), CornerLayers.SWOther, _sprite.AddRsiLayer((ent, ent), state0));
            _sprite.LayerSetDirOffset((ent, ent), CornerLayers.SWOther, SpriteComponent.DirectionOffset.Clockwise);
        }
    }

    private void Recompute(Entity<StellarWallSmoothComponent> ent)
    {
        if (ent.Comp.UpdateGeneration == _generation)
            return;

        ent.Comp.UpdateGeneration = _generation;

        Entity<MapGridComponent>? grid = null;

        var xform = Transform(ent);

        if (!_spriteQuery.TryGetComponent(ent, out var sprite))
            return;

        if (xform.Anchored)
        {
            if (_mapGridQuery.TryGetComponent(xform.GridUid, out var gridComp))
                grid = (xform.GridUid.Value, gridComp);
        }

        RecomputeCorners(grid, (ent, ent, sprite, xform));
    }

    private void RecomputeCorners(Entity<MapGridComponent>? grid, Entity<StellarWallSmoothComponent, SpriteComponent, TransformComponent> ent)
    {
        ent.Comp1.SameCorners = grid == null
            ? (CornerFill.None, CornerFill.None, CornerFill.None, CornerFill.None)
            : CalculateCornerFill(grid.Value, ent, false);

        ent.Comp1.OtherCorners = grid == null
            ? (CornerFill.None, CornerFill.None, CornerFill.None, CornerFill.None)
            : CalculateCornerFill(grid.Value, ent, true);

        ApplyCorners(ent, false);
    }

    private void OnAppearanceChange(Entity<StellarWallSmoothComponent> ent, ref AppearanceChangeEvent args)
    {
        if (!_appearance.TryGetData<string>(ent, RandomWallSmoothState.State, out var state, args.Component))
            return;

        SetFullState(ent, state);
    }

    private void SetFullState(Entity<StellarWallSmoothComponent> ent, string state)
    {
        if (!_spriteQuery.TryGetComponent(ent, out var spriteComp))
            return;

        ent.Comp.FullState = state;
        ApplyCorners((ent, ent, spriteComp), false);
    }

    private void ApplyCorners(Entity<StellarWallSmoothComponent, SpriteComponent> ent, bool down)
    {
        var fullState = down && ent.Comp1.DownState is not null ? ent.Comp1.DownState : ent.Comp1.FullState;
        var otherState = down && ent.Comp1.DownOtherState is not null ? ent.Comp1.DownOtherState : ent.Comp1.DownState;

        _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.NEBase, $"{fullState}{(int)ent.Comp1.SameCorners.ne}");
        _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.SEBase, $"{fullState}{(int)ent.Comp1.SameCorners.se}");
        _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.SWBase, $"{fullState}{(int)ent.Comp1.SameCorners.sw}");
        _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.NWBase, $"{fullState}{(int)ent.Comp1.SameCorners.nw}");

        if (otherState is not null)
        {
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.NEOther, ent.Comp1.OtherCorners.ne != CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.SEOther, ent.Comp1.OtherCorners.se != CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.SWOther, ent.Comp1.OtherCorners.sw != CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.NWOther, ent.Comp1.OtherCorners.nw != CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.NEBase, ent.Comp1.OtherCorners.ne == CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.SEBase, ent.Comp1.OtherCorners.se == CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.SWBase, ent.Comp1.OtherCorners.sw == CornerFill.None);
            _sprite.LayerSetVisible((ent, ent.Comp2), CornerLayers.NWBase, ent.Comp1.OtherCorners.nw == CornerFill.None);

            _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.NEOther, $"{otherState}{(int)ent.Comp1.OtherCorners.ne}");
            _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.SEOther, $"{otherState}{(int)ent.Comp1.OtherCorners.se}");
            _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.SWOther, $"{otherState}{(int)ent.Comp1.OtherCorners.sw}");
            _sprite.LayerSetRsiState((ent, ent.Comp2), CornerLayers.NWOther, $"{otherState}{(int)ent.Comp1.OtherCorners.nw}");
        }
    }

    private bool MatchingEntity(Entity<StellarWallSmoothComponent, SpriteComponent, TransformComponent> ent, AnchoredEntitiesEnumerator candidates, bool other)
    {
        while (candidates.MoveNext(out var entity))
        {
            if (!_wallSmoothQuery.TryGetComponent(entity, out var otherSmooth) || otherSmooth.Key is not { } otherKey)
                continue;

            if (!other && ent.Comp1.Key == otherKey)
                return true;

            if (other && ent.Comp1.OtherKeys.Contains(otherKey))
                return true;
        }

        return false;
    }

    private (CornerFill ne, CornerFill nw, CornerFill sw, CornerFill se) CalculateCornerFill(Entity<MapGridComponent> grid, Entity<StellarWallSmoothComponent, SpriteComponent, TransformComponent> ent, bool other)
    {
        var pos = _map.TileIndicesFor(grid, grid, ent.Comp3.Coordinates);
        var n = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.North)), other);
        var ne = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.NorthEast)), other);
        var e = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.East)), other);
        var se = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.SouthEast)), other);
        var s = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.South)), other);
        var sw = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.SouthWest)), other);
        var w = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.West)), other);
        var nw = MatchingEntity(ent, _map.GetAnchoredEntitiesEnumerator(grid, grid, pos.Offset(Direction.NorthWest)), other);

        // ReSharper disable InconsistentNaming
        var cornerNE = CornerFill.None;
        var cornerSE = CornerFill.None;
        var cornerSW = CornerFill.None;
        var cornerNW = CornerFill.None;
        // ReSharper restore InconsistentNaming

        if (n)
        {
            cornerNE |= CornerFill.CounterClockwise;
            cornerNW |= CornerFill.Clockwise;
        }

        if (ne)
        {
            cornerNE |= CornerFill.Diagonal;
        }

        if (e)
        {
            cornerNE |= CornerFill.Clockwise;
            cornerSE |= CornerFill.CounterClockwise;
        }

        if (se)
        {
            cornerSE |= CornerFill.Diagonal;
        }

        if (s)
        {
            cornerSE |= CornerFill.Clockwise;
            cornerSW |= CornerFill.CounterClockwise;
        }

        if (sw)
        {
            cornerSW |= CornerFill.Diagonal;
        }

        if (w)
        {
            cornerSW |= CornerFill.Clockwise;
            cornerNW |= CornerFill.CounterClockwise;
        }

        if (nw)
        {
            cornerNW |= CornerFill.Diagonal;
        }

        // Local is fine as we already know it's parented to the grid (due to the way anchoring works).
        switch (ent.Comp3.LocalRotation.GetCardinalDir())
        {
            case Direction.North:
                return (cornerSW, cornerSE, cornerNE, cornerNW);
            case Direction.West:
                return (cornerSE, cornerNE, cornerNW, cornerSW);
            case Direction.South:
                return (cornerNE, cornerNW, cornerSW, cornerSE);
            default:
                return (cornerNW, cornerSW, cornerSE, cornerNE);
        }
    }
}

[Flags]
internal enum CornerFill : byte
{
    // These values are pulled from Baystation12.
    // I'm too lazy to convert the state names.
    None = 0,

    // The cardinal tile counter-clockwise of this corner is filled.
    CounterClockwise = 1,

    // The diagonal tile in the direction of this corner.
    Diagonal = 2,

    // The cardinal tile clockwise of this corner is filled.
    Clockwise = 4,
}

internal enum CornerLayers : byte
{
    SEBase,
    NEBase,
    NWBase,
    SWBase,

    SEOther,
    NEOther,
    NWOther,
    SWOther,
}
