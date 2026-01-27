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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="color"></param>
    /// <param name="length"></param>
    /// <param name="width"></param>
    /// <param name="origin">if set, draw a line from this point to the arrow</param>
    /// <param name="line_width">width of the line to the arrow, if origin is set</param>
    public static void DrawArrow(this Node2D node, 
        Vector2 pos, 
        Vector2 dir, 
        Color color, 
        float length, 
        float width, 
        float weight = 2, 
        Vector2? origin = null, 
        float line_width = 2, 
        Color? line_color = null, 
        int line_dashed = 0)
    {
        var perp = new Vector2(-dir.Y, dir.X);
        node.DrawPolyline([
            pos,
            pos - dir * length / 2 + perp * width / 2,
            pos - dir * length / 2 - perp * width / 2,
            pos,
            ], color, width: weight);
        if (origin is not null)
            if (line_dashed > 0)
                node.DrawDashedLine(origin.Value, pos - dir * length / 2, line_color ?? color, width: line_width, dash: line_dashed);
            else
                node.DrawLine(origin.Value, pos - dir * length / 2, line_color ?? color, width: line_width);
    }
}
