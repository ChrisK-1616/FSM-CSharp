// Author: Chris Knowles
// Date: Jan 2023
// Copyright: Copperhead Labs, (c)2023
// File: FSMExceptions.cs
// Version: 1.0.0
// Notes: 

namespace FSM
{
    /// <summary>
    /// Class <c>IllegalStateInstantiationError</c> that represents the exception thrown when attempting to 
    /// create a new instance of the <see cref="FSMState"/> class which is an abstract class and cannot be
    /// instantiated
    /// </summary>
    public class IllegalStateInstantiationError : Exception
    {
        public required FSM Fsm { get; init; }
        public required string Name { get; init; }

        public override string Message => $"Cannot instantiate state object (to be named [{Name}]) " +
                                          $"in FSM named [{Fsm.Name}] as it is from an abstract class";
    }

    /// <summary>
    /// Class <c>DuplicateTransitionError</c> that represents the exception thrown when creating a new 
    /// <see cref="FSMState"/> instance and a duplicate of that <see cref="FSMState"/> already exists
    /// </summary>
    public class DuplicateStateError : Exception
    {
        public required FSM Fsm { get; init; }
        public required string Name { get; init; }

        public override string Message => $"New state to be named [{Name}] cannot be a duplicate of an existing " +
                                          $"state in FSM named [{Fsm.Name}]";
    }

    /// <summary>
    /// Class <c>StateNotFoundError</c> that represents the exception thrown when searching for a <see cref="FSMState"/>
    /// instance and it is not found
    /// </summary>
    public class StateNotFoundError : Exception
    {
        public required FSM Fsm { get; init; }
        public required string StateName { get; init; }

        public override string Message => $"State named [{StateName}] not found in FSM named [{Fsm.Name}]";
    }

    /// <summary>
    /// Class <c>NoActiveStateError</c> that represents the exception thrown when a <see cref="FSM"/> instance has no
    /// active state
    /// </summary>
    public class NoActiveStateError : Exception
    {
        public required FSM Fsm { get; init; }

        public override string Message => $"FSM named [{Fsm.Name}] has no active state";
    }

    /// <summary>
    /// Class <c>DuplicateTransitionError</c> that represents the exception thrown when creating a new
    /// <see cref="FSMTransition"/> instance and a duplicate of that <see cref="FSMTransition"/> already exists
    /// </summary>
    public class DuplicateTransitionError : Exception
    {
        public required FSMState SourceState { get; init; }
        public required FSMState TargetState { get; init; }

        public override string Message => $"New transition with source state " +
                                          $"[{SourceState.Id}]:[{SourceState.Name}] " +
                                          $"and target state " +
                                          $"[{TargetState.Id}]:[{TargetState.Name}] " +
                                          $"cannot be a duplicate of an existing transition";
    }

    /// <summary>
    /// Class <c>NoTransitionsError</c> that represents the exception thrown when a <see cref="FSMState"/> instance
    /// possesses no <see cref="FSMTransition"/> instances
    /// </summary>
    public class NoTransitionsError : Exception
    {
        public required FSMState State { get; init; }

        public override string Message => $"State [{State.Id}]:[{State.Name}] has no transitions";
    }

    /// <summary>
    /// Class <c>TransitionNotFoundError</c> that represents the exception thrown when searching for a
    /// <see cref="FSMTransition"/> instance and it is not found
    /// </summary>
    public class TransitionNotFoundError : Exception
    {
        public required FSMState SourceState { get; init; }
        public required FSMState TargetState { get; init; }

        public override string Message => $"Transition not found that has source state " +
                                          $"[{SourceState.Id}]:[{SourceState.Name}] " +
                                          $"and target state " +
                                          $"[{TargetState.Id}]:[{TargetState.Name}]";
    }

    /// <summary>
    /// Class <c>TargetStateIsNullError</c> that represents the exception thrown when a <c>null</c>
    /// value for a target <see cref="FSMState"/> instance is provided when searching for a specific
    /// <see cref="FSMTransition"/> instance
    /// </summary>
    public class TargetStateIsNullError : Exception
    {
        public required FSMState SourceState { get; init; }

        public override string Message => $"Transition not found that has source state " +
                                          $"[{SourceState.Id}]:[{SourceState.Name}] " +
                                          $"since target state is null value";
    }

    /// <summary>
    /// Class <c>GuardFailedException</c> that represents the exception thrown when a <see cref="FSMTransition.Guard"/>
    /// method of a <see cref="FSMTransition"/> instance 'fails', i.e. the <see cref="FSMTransition.Guard"/> method in
    /// question returns <c>false</c> after being invoked
    /// </summary>
    public class GuardFailedException : Exception
    {
        public required FSMState SourceState { get; init; }
        public required FSMState TargetState { get; init; }
        public required FSMTransition Transition { get; init; }

        public override string Message => $"Guard function failed during transition between source state " +
                                          $"[{SourceState.Id}]:[{SourceState.Name}] " +
                                          $"and target state " +
                                          $"[{TargetState.Id}]:[{TargetState.Name}] " +
                                          $"using transition " +
                                          $"[{Transition.Id}]";
    }
}
