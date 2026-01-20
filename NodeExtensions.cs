using breakout.components.scripts;
using Godot;

namespace breakout;

public static class NodeExtensions
{
    public static bool TryGetContext<T>(this Node node, out T ctx)
    {
        while (node != null)
        {
            node = node.GetParent();
            if (node is T t)
            {
                ctx = t;
                return true;
            }
        }
        ctx = default!;
        return false;
    }

    public static bool TryGetAncestorWithComponent<T>(this Node node, out Node ancestor, out T component) 
        where T : class, INodeComponent
    {
        while (node != null)
        {
            node = node.GetParent();
            if (GetComponent.TryGetComponent(node, out T found))
            {
                component = found;
                ancestor = node;
                return true;
            }
        }
        component = default!;
        ancestor = default!;
        return false;
    }

    public static bool TryGetComponent<T>(this Node node, out T component) 
        where T : class, INodeComponent 
        => GetComponent.TryGetComponent(node, out component);
}
