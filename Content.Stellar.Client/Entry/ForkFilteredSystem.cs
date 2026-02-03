// SPDX-FileCopyrightText: 2026 TheShuEd <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Stellar.Client.Entry;

/// <summary>
/// On the client side, it automatically enables entity filtering to hide all vanilla ss14 entities
/// not marked with the ForkFiltered category from the spawn menu.
/// </summary>
public sealed class ForkFilteredSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public override void Initialize()
    {
        _cfg.SetCVar(CVars.EntitiesCategoryFilter, "ForkFiltered");
    }
}
