using Godot;
using Godot.Collections;

namespace breakout;

public static class GroupHelpers
{
    private const string AddedToGroup = nameof(AddedToGroup);
    private const string RemovedFromGroup = nameof(RemovedFromGroup);
    private static readonly Array AddedToGroupArgs = [
        new Dictionary() { { "name", "node"}, { "type", (int)Variant.Type.NodePath } },
        new Dictionary() { { "name", "groupName" }, { "type", (int)Variant.Type.Array } },
    ];

    public static void AddToGroupsAndSignal(this Node node, params StringName[] groups)
    {
        if (!node.HasUserSignal(AddedToGroup)) node.AddUserSignal(AddedToGroup, AddedToGroupArgs);

        foreach (var group in groups)
        {
            node.AddToGroup(group, true);
        }
        node.EmitSignal(AddedToGroup, new Array<StringName>(groups));
    }

    public static void RemoveFromGroupsAndSignal(this Node node, params StringName[] groups)
    {
        if (!node.HasUserSignal(RemovedFromGroup)) node.AddUserSignal(RemovedFromGroup, AddedToGroupArgs);

        foreach (var group in groups)
        {
            node.RemoveFromGroup(group);
        }
        node.EmitSignal(RemovedFromGroup, new Array<StringName>(groups));
    }

    public static void ConnectAddedToGroupSignal(this Node node, System.Action<Node, Array<StringName>> handler)
    {
        if (!node.HasUserSignal(AddedToGroup)) node.AddUserSignal(AddedToGroup, AddedToGroupArgs);
        node.Connect(AddedToGroup, Callable.From(handler));
    }

    public static void ConnectRemovedFromGroupSignal(this Node node, System.Action<Node, Array<StringName>> handler)
    {
        if (!node.HasUserSignal(RemovedFromGroup)) node.AddUserSignal(RemovedFromGroup, AddedToGroupArgs);
        node.Connect(RemovedFromGroup, Callable.From(handler));
    }

    public static void DisconnectAddedToGroupSignal(this Node node, System.Action<Node, Array<StringName>> handler)
    {
        if (!node.HasUserSignal(AddedToGroup)) node.AddUserSignal(AddedToGroup, AddedToGroupArgs);
        node.Disconnect(AddedToGroup, Callable.From(handler));
    }

    public static void DisconnectRemovedFromGroupSignal(this Node node, System.Action<Node, Array<StringName>> handler)
    {
        if (!node.HasUserSignal(RemovedFromGroup)) node.AddUserSignal(RemovedFromGroup, AddedToGroupArgs);
        node.Disconnect(RemovedFromGroup, Callable.From(handler));
    }
}
