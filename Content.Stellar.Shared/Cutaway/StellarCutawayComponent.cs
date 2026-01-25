// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Physics;

namespace Content.Stellar.Shared.Cutaway;

[RegisterComponent, NetworkedComponent]
public sealed partial class StellarCutawayComponent : Component, IComponentTreeEntry<StellarCutawayComponent>
{
    public EntityUid? TreeUid { get; set; }
    public DynamicTree<ComponentTreeEntry<StellarCutawayComponent>>? Tree { get; set; }
    public bool AddToTree => true;
    public bool TreeUpdateQueued { get; set; }
}
