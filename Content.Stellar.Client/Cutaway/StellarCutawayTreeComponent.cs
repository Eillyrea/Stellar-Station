// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Stellar.Shared.Cutaway;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Physics;

namespace Content.Stellar.Client.Cutaway;

[RegisterComponent]
public sealed partial class StellarCutawayTreeComponent : Component, IComponentTreeComponent<StellarCutawayComponent>
{
    public DynamicTree<ComponentTreeEntry<StellarCutawayComponent>> Tree { get; set; }
}
