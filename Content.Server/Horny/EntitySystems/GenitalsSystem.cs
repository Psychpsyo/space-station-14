using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Server.Horny.Components;
using Content.Shared.Horny;
using Content.Shared.Chemistry.Components;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Humanoid;

namespace Content.Server.Horny.EntitySystems;

public sealed class GenitalsSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GenitalsComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<GenitalsComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<GenitalsComponent, SexChangedEvent>(OnSexChanged);
        SubscribeLocalEvent<GenitalsComponent, EmoteEvent>(OnEmote);
        SubscribeLocalEvent<GenitalsComponent, CumActionEvent>(OnCumAction);
    }

    private void OnMapInit(EntityUid uid, GenitalsComponent component, MapInitEvent args)
    {
        // try to add cum action when genitals component is added
        if (IsMale(uid))
            _actions.AddAction(uid, ref component.CumActionEntity, component.CumAction);
    }

    private void OnShutdown(EntityUid uid, GenitalsComponent component, ComponentShutdown args)
    {
        // remove cum action when genitals component is removed
        if (IsMale(uid))
            _actions.RemoveAction(uid, component.CumActionEntity);
    }

    private void OnSexChanged(EntityUid uid, GenitalsComponent component, SexChangedEvent args)
    {
        if (args.OldSex != Sex.Male && args.NewSex == Sex.Male)
            _actions.AddAction(uid, ref component.CumActionEntity, component.CumAction);

        if (args.OldSex == Sex.Male && args.NewSex != Sex.Male)
            _actions.RemoveAction(uid, component.CumActionEntity);
    }

    private void OnEmote(EntityUid uid, GenitalsComponent component, ref EmoteEvent args)
    {
        if (args.Handled || args.Emote.ID != "Cum")
            return;

        args.Handled = TryCum(uid, component);
    }

    private void OnCumAction(EntityUid uid, GenitalsComponent component, CumActionEvent args)
    {
        if (args.Handled)
            return;

        if (TryCum(uid, component))
        {
            _chat.TryEmoteWithChat(uid, "Cum");
            args.Handled = true;
        }
    }

    private bool TryCum(EntityUid uid, GenitalsComponent component)
    {
        if (!IsMale(uid))
            return false;

        var cumSolution = new Solution(component.CumReagent, component.Amount);
        var xform = Transform(uid);
        _puddle.TrySpillAt(xform.Coordinates, cumSolution, out _);
        return true;
    }

    private bool IsMale(EntityUid uid)
    {
        Sex? sex = CompOrNull<HumanoidAppearanceComponent>(uid)?.Sex ?? Sex.Unsexed;
        return sex == Sex.Male;
    }
}
