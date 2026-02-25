using Godot;
using System.Diagnostics.CodeAnalysis;
namespace breakout.components.scripts;

[GlobalClass]
public partial class GetComponent : GodotObject
{
    public static bool TryGetComponent<T>(Node? node, [NotNullWhen(true)] out T component) 
        where T : class, INodeComponent
    {
        component = null!;
        if (node is null)
            return false;
        if (node.HasNode(typeof(T).Name) && node.GetNode(typeof(T).Name) is T found)
            component = found;
        return component != null;
    }

    public static bool TryGetAIComponent(Node? node, out AIComponent component) => TryGetComponent(node, out component);
    public static bool TryGetHealthComponent(Node? node, out HealthComponent component) => TryGetComponent(node, out component);
    public static bool TryGetHitboxComponent(Node? node, out HitboxComponent component) => TryGetComponent(node, out component);
    public static bool TryGetHurtboxComponent(Node? node, out HurtboxComponent component) => TryGetComponent(node, out component);
    public static bool TryGetPmdSprite(Node? node, out PMDSprite component) => TryGetComponent(node, out component);
    public static bool TryGetSelectableComponent(Node? node, out SelectableComponent component) => TryGetComponent(node, out component);
    public static bool TryGetTeamComponent(Node? node, out TeamComponent component) => TryGetComponent(node, out component);
    public static bool TryGetUnitClickAreaComponent(Node? node, out UnitClickAreaComponent component) => TryGetComponent(node, out component);
    public static bool TryGetUserInputComponent(Node? node, out UserInputComponent component) => TryGetComponent(node, out component);
    public static bool TryGetUIComponent(Node? node, out UIComponent component) => TryGetComponent(node, out component);
}
