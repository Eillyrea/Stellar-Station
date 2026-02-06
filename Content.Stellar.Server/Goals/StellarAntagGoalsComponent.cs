// SPDX-FileCopyrightText: 2026 Janet Blackquill <uhhadd@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Stellar.Server.Goals;

[RegisterComponent]
[Access(typeof(StellarAntagGoalsSystem))]
public sealed partial class StellarAntagGoalsComponent : Component
{
    [DataField(required: true)]
    public EntityTableSelector Goals;
}
