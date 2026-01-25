// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.Random;

namespace Content.Stellar.Shared.WallSmooth;

public sealed partial class StellarRandomWallSmoothSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StellarRandomWallSmoothComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<StellarRandomWallSmoothComponent> ent, ref MapInitEvent args)
    {
        var state = _random.Pick(ent.Comp.RandomFullStates);
        _appearance.SetData(ent, RandomWallSmoothState.State, state);
    }
}