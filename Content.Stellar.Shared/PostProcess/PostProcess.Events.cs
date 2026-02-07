// SPDX-FileCopyrightText: 2026 AftrLite
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Robust.Shared.Serialization;

namespace Content.Stellar.Shared.PostProcess;

[Serializable, NetSerializable]
public sealed class StellarPostProcessUpdateEvent : EntityEventArgs
{
    public readonly NetEntity Target;
    public readonly string Lut;

    public StellarPostProcessUpdateEvent(NetEntity target, string lut)
    {
        Target = target;
        Lut = lut;
    }
}
