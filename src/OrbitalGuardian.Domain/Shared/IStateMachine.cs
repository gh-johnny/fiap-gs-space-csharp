namespace OrbitalGuardian.Domain.Shared;

public interface IStateMachine<TState> where TState : Enum
{
    /// <summary>Returns the current state of the state machine.</summary>
    TState CurrentState { get; }

    /// <summary>Returns whether a transition to the given state is valid from the current state.</summary>
    bool CanTransitionTo(TState state);

    /// <summary>Transitions to the given state. Throws if the transition is invalid.</summary>
    void TransitionTo(TState state);
}
