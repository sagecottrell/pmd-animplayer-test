using Godot;
using Godot.Collections;

namespace breakout;

[GlobalClass]
public partial class StateMachine : Resource
{
    private Variant _currentState = "";
    private Dictionary<Variant, Callable> _states = [];

    public static StateMachine Create(Dictionary<Variant, Callable>? init = null)
    {
        var sm = new StateMachine();
        if (init is not null)
            sm._states = init;
        return sm;
    }

    public void On(Variant eventName, Callable callback)
    {
        // Implementation for registering an event listener
        _states[eventName] = callback;
    }

    public void Emit(Variant eventName)
    {
        if (eventName.Equals(_currentState))
            return;
        // Implementation for emitting an event
        if (_states.TryGetValue(eventName, out Callable value))
        {
            _currentState = eventName;
            value.CallDeferred();
        }
    }
}
