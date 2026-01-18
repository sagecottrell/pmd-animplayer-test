using Godot;
namespace breakout.components.scripts;

[GlobalClass]
public partial class GetComponent : GodotObject
{
    public static bool TryGetPmdSprite(Node node, out PMDSprite component)
    {
        component = node.GetNode<PMDSprite>(nameof(PMDSprite));
        return component != null;
    }

    public static bool TryGetAIComponent(Node node, out AIComponent component)
    {
        component = node.GetNode<AIComponent>(nameof(AIComponent));
        return component != null;
    }

    public static bool TryGetHealthComponent(Node node, out HealthComponent component)
    {
        component = node.GetNode<HealthComponent>(nameof(HealthComponent));
        return component != null;
    }

    public static bool TryGetHitboxComponent(Node node, out HitboxComponent component)
    {
        component = node.GetNode<HitboxComponent>(nameof(HitboxComponent));
        return component != null;
    }

    public static bool TryGetHurtboxComponent(Node node, out HurtboxComponent component)
    {
        component = node.GetNode<HurtboxComponent>(nameof(HurtboxComponent));
        return component != null;
    }

    public static bool TryGetTeamComponent(Node node, out TeamComponent component)
    {
        component = node.GetNode<TeamComponent>(nameof(TeamComponent));
        return component != null;
    }

    public static bool TryGetUserInputComponent(Node node, out UserInputComponent component)
    {
        component = node.GetNode<UserInputComponent>(nameof(UserInputComponent));
        return component != null;
    }

    public static bool TryGetSelectableComponent(Node node, out SelectableComponent component)
    {
        component = node.GetNode<SelectableComponent>(nameof(SelectableComponent));
        return component != null;
    }
}
