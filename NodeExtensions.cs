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
}
