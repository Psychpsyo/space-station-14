using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Server.Horny.Components;
using Content.Shared.Horny;
using Content.Shared.Chemistry.Components;
using Content.Server.Fluids.EntitySystems;

namespace Content.Server.Horny.EntitySystems;

public sealed class CumSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CumComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<CumComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<CumComponent, EmoteEvent>(OnEmote);
        SubscribeLocalEvent<CumComponent, CumActionEvent>(OnCumAction);
    }

    private void OnMapInit(EntityUid uid, CumComponent component, MapInitEvent args)
    {
        // try to add cum action when vocal comp added
        _actions.AddAction(uid, ref component.CumActionEntity, component.CumAction);
    }

    private void OnShutdown(EntityUid uid, CumComponent component, ComponentShutdown args)
    {
        // remove cum action when component removed
        if (component.CumActionEntity != null)
        {
            _actions.RemoveAction(uid, component.CumActionEntity);
        }
    }

    private void OnEmote(EntityUid uid, CumComponent component, ref EmoteEvent args)
    {
        if (args.Handled || args.Emote.ID != "Cum")
            return;

        args.Handled = TryCum(uid, component);
    }

    private void OnCumAction(EntityUid uid, CumComponent component, CumActionEvent args)
    {
        if (args.Handled)
            return;

        if (TryCum(uid, component))
        {
            _chat.TryEmoteWithChat(uid, "Cum");
            args.Handled = true;
        }
    }

    private bool TryCum(EntityUid uid, CumComponent component)
    {
        var cumSolution = new Solution(component.CumReagent, component.Amount);
        var xform = Transform(uid);
        _puddle.TrySpillAt(xform.Coordinates, cumSolution, out _);
        return true;
    }
}
