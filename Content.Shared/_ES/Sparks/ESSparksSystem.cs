// SPDX-FileCopyrightText: 2025 EmoGarbage404
// SPDX-FileCopyrightText: 2025 mirrorcult
//
// SPDX-License-Identifier: MIT

using Content.Shared._ES.Physics.PreventCollide;
using Content.Shared._ES.Sparks.Components;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Throwing;
using Content.Shared._ES.TileFires;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._ES.Sparks;

public sealed partial class ESSparksSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPowerReceiverSystem _powerReceiver = default!;
    [Dependency] private readonly ESPreventCollideSystem _preventCollide = default!;
    [Dependency] private readonly ESSharedTileFireSystem _tileFire = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public static readonly EntProtoId DefaultSparks = "StellarEffectSparks";

    /// <summary>
    /// Variant of <see cref="DoSparks(EntityUid, int, Nullable{EntProtoId}, Nullable{EntityUid}, float, bool, bool)"/> that takes
    /// the configuration from a base component rather than being passed in as args
    /// </summary>
    /// <param name="ent">Entity that the sparks are originating from. Additionally, holds YAML configuration for spark effect</param>
    /// <param name="user">A "user" who triggered the sparks</param>
    /// <param name="cooldown">If true, will check the cooldown on <see cref="ESSparkCooldownComponent"/> before spawning sparks</param>
    /// <param name="randomize">If true, will randomize the amount of sparks based on the previously provided input amount.</param>
    public void DoSparks<T>(
        Entity<T> ent,
        EntityUid? user = null,
        bool cooldown = true,
        bool randomize = false)
        where T : ESBaseSparkConfigurationComponent
    {
        if (!_random.Prob(ent.Comp.Prob))
            return;

        SharedApcPowerReceiverComponent? powerReceiver = null;
        if (_powerReceiver.ResolveApc(ent, ref powerReceiver) &&
            (!_powerReceiver.IsPowered((ent, powerReceiver)) || powerReceiver.Load <= 0))
            return;

        DoSparks(ent,
            amount: ent.Comp.Count,
            ent.Comp.SparkPrototype,
            user: user,
            tileFireChance: ent.Comp.TileFireChance,
            cooldown: cooldown,
            randomize: randomize);
    }

    /// <summary>
    /// Spawns sparks originating from a target entity
    /// </summary>
    /// <param name="source">Entity that the sparks are originating from</param>
    /// <param name="amount">Number of sparks to spawn</param>
    /// <param name="sparksPrototype">Spark prototype to use. Defaults to <see cref="DefaultSparks"/></param>
    /// <param name="user">A "user" who triggered the sparks</param>
    /// <param name="tileFireChance">Chance that sparks will cause a fire to start</param>
    /// <param name="cooldown">If true, will check the cooldown on <see cref="ESSparkCooldownComponent"/> before spawning sparks</param>
    /// <param name="randomize">If true, will randomize the amount of sparks based on the previously provided input amount.</param>
    [PublicAPI]
    public void DoSparks(
        EntityUid source,
        int amount = 3,
        EntProtoId? sparksPrototype = null,
        EntityUid? user = null,
        float tileFireChance = 0f,
        bool cooldown = true,
        bool randomize = false)
    {
        // track last spark time
        var comp = EnsureComp<ESSparkCooldownComponent>(source);
        if (cooldown && _timing.CurTime - comp.LastSparkTime < comp.SparkDelay)
            return;
        comp.LastSparkTime = _timing.CurTime;

        var coords = Transform(source).Coordinates;
        DoSparks(coords, amount, sparksPrototype, user, source, tileFireChance, randomize);
    }

    /// <summary>
    /// Spawns sparks at a given set of coordinates
    /// </summary>
    /// <param name="coordinates">Where the sparks should spawn</param>
    /// <param name="amount">Number of sparks to spawn</param>
    /// <param name="sparksPrototype">Spark prototype to use. Defaults to <see cref="DefaultSparks"/></param>
    /// <param name="user">A "user" who triggered the sparks</param>
    /// <param name="ignored">An entity whose collision will be ignored by the sparks</param>
    /// <param name="tileFireChance">Chance that sparks will cause a fire to start</param>
    /// <param name="randomize">If true, will randomize the amount of sparks based on the previously provided input amount.</param>
    [PublicAPI]
    public void DoSparks(
        EntityCoordinates coordinates,
        int amount = 3,
        EntProtoId? sparksPrototype = null,
        EntityUid? user = null,
        EntityUid? ignored = null,
        float tileFireChance = 0f,
        bool randomize = false)
    {
        if (_net.IsClient)
            return;

        sparksPrototype ??= DefaultSparks;
        var number = amount;
        if (randomize)
            number = _random.Next(1, amount+1);

        var angleDelta = (Angle) (MathF.Tau / number * _random.NextFloat(0f, number));
        var angle = _random.NextAngle();
        for (var i = 0; i < number; i++)
        {
            var sparks = Spawn(sparksPrototype, _transform.ToMapCoordinates(coordinates), rotation: angle);
            angle += angleDelta;
            _throwing.TryThrow(sparks, angle.ToVec(), 0.25f * _random.NextFloat(0.5f, number+1), animated: false);
            _preventCollide.PreventCollide(sparks, ignored);
        }

        if (_random.Prob(tileFireChance))
            _tileFire.TryDoTileFire(coordinates, user, _random.Next(1, 4));
    }
}
