// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using System.Numerics;
using Content.Stellar.Shared.Cutaway;
using Robust.Client.GameObjects;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Physics;

namespace Content.Stellar.Client.Cutaway;

public sealed class StellarCutawayTreeSystem : ComponentTreeSystem<StellarCutawayTreeComponent, StellarCutawayComponent>
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    protected override bool DoFrameUpdate => true;
    protected override bool DoTickUpdate => false;
    protected override bool Recursive => false;

    protected override Box2 ExtractAabb(in ComponentTreeEntry<StellarCutawayComponent> entry, Vector2 pos, Angle rot)
    {
        return _sprite.CalculateBounds((entry.Uid, Comp<SpriteComponent>(entry.Uid)), pos, rot, default).CalcBoundingBox();
    }
}
