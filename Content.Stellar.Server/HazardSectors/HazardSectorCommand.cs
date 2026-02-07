// SPDX-FileCopyrightText: 2026 Janet Blackquill
//
// SPDX-License-Identifier: LicenseRef-Wallening

using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed;

namespace Content.Stellar.Server.HazardSectors;

[ToolshedCommand, AdminCommand(AdminFlags.Debug)]
public sealed class HazardSectorCommand : ToolshedCommand
{
    [CommandImplementation("force_next_round")]
    public void ForceNextRound(EntProtoId sector)
    {
        Sys<StellarRandomHazardSectorRuleSystem>().ForceSector(sector.Id);
    }
}
