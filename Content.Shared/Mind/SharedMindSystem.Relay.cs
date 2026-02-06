using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Mind.Components;
// Begin Stellar - we need these relayed
using Content.Shared.Mobs;
using Robust.Shared.Player;
// End Stellar - we need these relayed

namespace Content.Shared.Mind;

/// <summary>
///     Relays events raised on a mobs body to its mind and mind role entities.
///     Useful for events that should be raised both on the body and the mind.
/// </summary>
public abstract partial class SharedMindSystem : EntitySystem
{
    public void InitializeRelay()
    {
        // for name modifiers that depend on certain mind roles
        SubscribeLocalEvent<MindContainerComponent, RefreshNameModifiersEvent>(RelayRefToMind);
        // Begin Stellar - we need this relayed
        SubscribeLocalEvent<MindContainerComponent, PlayerAttachedEvent>(RelayToMind);
        SubscribeLocalEvent<MindContainerComponent, PlayerDetachedEvent>(RelayToMind);
        SubscribeLocalEvent<MindContainerComponent, MobStateChangedEvent>(RelayRefToMind);
        // End Stellar - we need this relayed
    }

    protected void RelayToMind<T>(EntityUid uid, MindContainerComponent component, T args) where T : class
    {
        var ev = new MindRelayedEvent<T>(args);

        if (TryGetMind(uid, out var mindId, out var mindComp, component))
        {
            RaiseLocalEvent(mindId, ref ev);

            foreach (var role in mindComp.MindRoleContainer.ContainedEntities)
                RaiseLocalEvent(role, ref ev);
        }
    }

    protected void RelayRefToMind<T>(EntityUid uid, MindContainerComponent component, ref T args) where T : notnull // Stellar - your types are a bit screwed here
    {
        var ev = new MindRelayedEvent<T>(args);

        if (TryGetMind(uid, out var mindId, out var mindComp, component))
        {
            RaiseLocalEvent(mindId, ref ev);

            foreach (var role in mindComp.MindRoleContainer.ContainedEntities)
                RaiseLocalEvent(role, ref ev);
        }

        args = ev.Args;
    }
}

[ByRefEvent]
public record struct MindRelayedEvent<TEvent>(TEvent Args);
