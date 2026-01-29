// SPDX-FileCopyrightText: 2026 TheShuEd
//
// SPDX-License-Identifier: LicenseRef-CosmicCult

using Robust.Shared.Configuration;

namespace Content.Stellar.Shared.CCVars;

public sealed partial class STCCVars
{
    /// <summary>
    /// Toggle for non-gameplay-affecting or otherwise status indicative post-process effects, such additive lighting.
    /// </summary>
    public static readonly CVarDef<bool>
        PostProcess = CVarDef.Create("shaders.post_process", true, CVar.CLIENTONLY | CVar.ARCHIVE);
}
