// Author: Chris Knowles
// Date: Jan 2023
// Copyright: Copperhead Labs, (c)2023
// File: FSMState.cs
// Version: 1.0.0
// Notes: 

namespace FSM
{
    /// <summary>
    /// Abstract class <c>FSMState</c> that represents a state within the FSM - must be inherited from
    /// </summary>
    public abstract class FSMState
    {
        /// <summary>
        /// Property <c>Id</c>: unique GUID associated with this state, read only property initialised at
        /// instantiation in constructor
        /// </summary>
        public Guid Id { get; protected set; }
        /// <summary>
        /// Property <c>Name</c>: unique name associated with this state, read only property initialised through
        /// initialiser
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Property <c>Fsm</c>: the managing <see cref="FSM"/> instance to which this <c>FSMState</c> instance belongs,
        /// read only property initialised through initialiser
        /// </summary>
        public FSM Fsm { get; protected set; }
        /// <summary>
        /// Property <c>Transitions</c>: set of all <see cref="FSMTransition"/> instances from this (source) <c>FSMState</c>
        /// to other (target) <c>FSMState</c>, this is a fully protected property and should not be accessed directly to
        /// ensure integrity, initialised through constructor
        /// </summary>
        public Dictionary<FSMState, FSMTransition> Transitions { get; protected set; }

        /// <summary>
        /// <c>Constructor</c> method
        /// </summary>
        /// <param name=""></param>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        protected FSMState(FSM fsm, string name)
        {
            Id = Guid.NewGuid();
            Fsm = fsm;
            Name = name;
            Transitions = new Dictionary<FSMState, FSMTransition>();
        }

        /// <summary>
        /// Method <c>ObtainInstance</c> 
        /// </summary>
        /// <param name="fsm">
        /// </param>
        /// <param name="name">
        /// </param>
        /// <param name="config">
        /// </param>
        /// <returns>
        /// Reference to a new instance of this <c>FSMState</c> class, note - because the constructor is declared
        /// as <c>protected</c> this is the only way a new instance of the <c>FSMState</c> class can be
        /// instantiated
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        protected static void ObtainInstance(FSM fsm, string name, object config)
        {
            throw new IllegalStateInstantiationError() { Fsm = fsm, Name = name };
        }

        /// <summary>
        /// Method <c>ToString</c> overrides object class <c>ToString</c> method
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        /// String representation of this <c>FSMState</c> object instance
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override string ToString() => $"State [{Fsm.Name}]:[{Id}]:[{Name}]";

        /// <summary>
        /// Method <c>Equals</c> overrides object class <c>Equals</c> method
        /// </summary>
        /// <param name="obj">
        /// Instance of <c>FSMState</c> object to check for equality to this <c>FSMState</c> object
        /// </param>
        /// <returns>
        /// <c>true</c> if <c>Name</c> property of the supplied <c>FSMState</c> object is equal to the
        /// <c>Name</c> property of this <c>FSMState</c> object, otherwise returns <c>false</c>
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override bool Equals(object? obj) => obj is FSMState state && Name.Equals(state.Name);

        /// <summary>
        /// Method <c>GetHashCode</c> overrides object class <c>GetHashCode</c> method
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        /// Integer value of the <c>Name</c> property's hashcode
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override int GetHashCode() => Name.GetHashCode();

        /// <summary>
        /// Method <c>AddTransition</c> instantiates and adds a new <see cref="FSMTransition"/> transition
        /// object to the <c>Transitions</c> dictionary, but only if there is no existing transition in the
        /// dictionary that has the supplied target state, if there is an existing transition then the method
        /// aborts and throws a <see cref="DuplicateTransitionError"/> exception
        /// </summary>
        /// <param name="targetState">
        /// Target <c>FSMState</c> instance for the new transition that is being added
        /// </param>
        /// <param name="guard">
        /// <see cref="FSMTransition.GuardFunc"/> optional parameter for this transition
        /// </param>
        /// <returns>
        /// <c>true</c> if new transition is added successfully 
        /// </returns>
        /// <exception cref="DuplicateTransitionError">
        /// Thrown if the new transition to be added is already in the <c>Transitions</c> dictionary
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool AddTransition(FSMState targetState, FSMTransition.GuardFunc? guard = null)
        {
            if (!Transitions.ContainsKey(targetState))
            {
                Transitions[targetState] = new FSMTransition(this, targetState, guard);

                return true;
            }

            throw new DuplicateTransitionError() { SourceState= this, TargetState = targetState };
        }

        /// <summary>
        /// Method <c>AddTransition</c> instantiates and adds a new <see cref="FSMTransition"/> transition
        /// object to the <c>Transitions</c> dictionary, but only if there is no existing transition in the
        /// dictionary that has the supplied target state, if there is an existing transition then the method
        /// aborts and throws a <see cref="DuplicateTransitionError"/> exception
        /// </summary>
        /// <param name="targetState">
        /// Target <c>FSMState</c> instance for the new transition that is being added
        /// </param>
        /// <param name="invokeExitEnter">
        /// <see cref="FSMTransition.InvokeMethods"/> parameter for this transition
        /// </param>
        /// <param name="guard">
        /// <see cref="FSMTransition.GuardFunc"/> optional parameter for this transition
        /// </param>
        /// <returns>
        /// <c>true</c> if new transition is added successfully 
        /// </returns>
        /// <exception cref="DuplicateTransitionError">
        /// Thrown if the new transition to be added is already in the <c>Transitions</c> dictionary
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool AddTransition(FSMState targetState, FSMTransition.InvokeMethods invokeExitEnter,
                                  FSMTransition.GuardFunc? guard = null)
        {
            if (!Transitions.ContainsKey(targetState))
            {
                Transitions[targetState] = new FSMTransition(this, targetState, invokeExitEnter, guard);

                return true;
            }

            throw new DuplicateTransitionError() { SourceState = this, TargetState = targetState };
        }

        /// <summary>
        /// Method <c>HasTransition</c> checks to see if this <c>FSMState</c> contains the <see cref="FSMTransition"/>
        /// which has the supplied target <c>FSMState</c> parameter associated with it
        /// </summary>
        /// <param name="targetState">
        /// Target <c>FSMState</c> instance associated with the <see cref="FSMTransition"/> instance to find
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>Transitions</c> dictionary contains a <see cref="FSMTransition"/> associated with the
        /// supplied target state parameter, otherwise returns <c>false</c>
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool HasTransition(FSMState targetState) => Transitions.ContainsKey(targetState);

        /// <summary>
        /// Method <c>GetTransition</c> returns the <see cref="FSMTransition"/> from the <c>Transitions</c>
        /// dictionary which has the supplied target <c>FSMState</c> parameter reference associated with
        /// it, or throws a <see cref="TransitionNotFoundError"/> exception if the transition is not present
        /// in the <c>Transitions</c> dictionary
        /// </summary>
        /// <param name="targetState">
        /// Target <c>FSMState</c> instance associated with the <see cref="FSMTransition"/> instance to find
        /// </param>
        /// <returns>
        /// <see cref="FSMTransition"/> associated with the supplied target parameter if it is found in the
        /// <c>Transitions</c> dictionary
        /// </returns>
        /// <exception cref=TransitionNotFoundError">
        /// Thrown if relevant <see cref="FSMTransition"/> is not found
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMTransition GetTransition(FSMState? targetState)
        {
            if (targetState == null)
            {
                throw new TargetStateIsNullError() { SourceState = this };
            }

            try
            {
                return Transitions[targetState];
            }
            catch (KeyNotFoundException)
            {
                throw new TransitionNotFoundError() { SourceState = this, TargetState = targetState };
            }
        }

        /// <summary>
        /// Method <c>RemoveTransition</c> attempt to remove <see cref="FSMTransition"/> instance that is
        /// associated with the supplied target <c>FSMState</c> parameter reference
        /// </summary>
        /// <param name="targetState">
        /// Target <c>FSMState</c> instance associated with the <see cref="FSMTransition"/> instance to remove
        /// </param>
        /// <returns>
        /// <c>true</c> if relevant <see cref="FSMTransition"/> is removed, otherwise returns <c>false</c>
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool RemoveTransition(FSMState targetState) => Transitions.Remove(targetState);

        /// <summary>
        /// Method <c>ClearTransitions</c> clears down the <c>Transitions</c> dictionary
        /// </summary>
        /// <param name=""></param>
        /// <returns>Nothing</returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public void ClearTransitions() => Transitions.Clear();

        /// <summary>
        /// Method <c>FireTransition</c> 'fires' the transition that has the supplied <c>FSMState</c> as its
        /// <see cref="FSMTransition.TargetState"/> (note - if no <c>FSMState</c> is supplied in parameter 
        /// <c>targetState</c> then the transition that is fired is the first value in the dictionary 
        /// <c>Transitions</c>, this is useful when this state only has one transition) - if there are no transitions associated
        /// with this state then a <see cref="NoTransitionsError"/> exception is thrown - if there is not a transition that
        /// has the supplied <c>targetState</c> then a <see cref="TransitionNotFoundError"/> is thrown - if the 
        /// <see cref="FSMTransition.Guard"/> delegate method (or if supplied the optional <c>temporaryGuard</c> parameter)
        /// resolves to <c>false</c> then the firing of the transition is aborted and a <see cref="GuardFailedException"/> is
        /// thrown - when firing the transition this causes the <c>Exit</c> and/or <c>Enter</c> (or neither)to be invoked
        /// depending upon the <see cref="FSMTransition.InvokeExitEnter"/> (or the optional <c>temporaryInvokeExitEnter</c>
        /// parameter) property of the transition 
        /// </summary>
        /// <param name="targetState">
        /// Optional <c>FSMState</c> instance of the transition's <see cref="FSMTransition.TargetState"/> that identifies the
        /// <c>FSMTransition</c> that is being fired - if this is not supplied then the first transition in the dictionary
        /// <c>Transitions</c> will be fired, this is useful if this state only has a single transition
        /// </param>
        /// <param name="temporaryInvokeExitEnter">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>InvokeExitEnter</c>
        /// property of this transition
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>Guard</c>
        /// property of this transition
        /// </param>
        /// <param name="data">
        /// Optional generic type instance to be passed through to the <see cref="FSMState.Exit"/>
        /// and/or <see cref="FSMState.Enter"/> methods involved as the transition fires
        /// </param>
        /// <returns>
        /// <see cref="FSMTransition.TargetState"/> taken from the fired <c>FSMTransition</c>
        /// </returns>
        /// <exception cref="NoTransitionsError">
        /// This exception is thrown if there are no transitions associated with this <c>FSMState</c> instance
        /// </exception>
        /// <exception cref="TransitionNotFoundError">
        /// This exception is thrown if there is no transition that has <see cref="FSMTransition.TargetState"/>
        /// equal to the supplied <c>FSMState</c> target state
        /// </exception>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState? FireTransition<T>(FSMState? targetState = null,
                                           FSMTransition.InvokeMethods? temporaryInvokeExitEnter = null,
                                           FSMTransition.GuardFunc? temporaryGuard = null,
                                           T? data = null) where T : class
        {
            if (Transitions.Count == 0)
            {
                throw new NoTransitionsError() { State = this };
            }

            if (targetState != null)
            {
                if (temporaryInvokeExitEnter != null)
                {
                    return GetTransition(targetState).Fire<T>(temporaryInvokeExitEnter, temporaryGuard, data);
                }

                return GetTransition(targetState).Fire<T>(temporaryGuard, data);
            }

            if (temporaryInvokeExitEnter != null)
            {
                return Transitions.Values.ElementAt<FSMTransition>(0).Fire<T>(temporaryInvokeExitEnter,
                                                                              temporaryGuard, data);
            }

            return Transitions.Values.ElementAt<FSMTransition>(0).Fire<T>(temporaryGuard, data);
        }

        /// <summary>
        /// Method <c>FireTransition</c> 'fires' the transition that has the supplied <c>FSMState</c> as its
        /// <see cref="FSMTransition.TargetState"/> (note - if no <c>FSMState</c> is supplied in parameter 
        /// <c>targetState</c> then the transition that is fired is the first value in the dictionary 
        /// <c>Transitions</c>, this is useful when this state only has one transition) - if there are no transitions associated
        /// with this state then a <see cref="NoTransitionsError"/> exception is thrown - if there is not a transition that
        /// has the supplied <c>targetState</c> then a <see cref="TransitionNotFoundError"/> is thrown - if the 
        /// <see cref="FSMTransition.Guard"/> delegate method (or if supplied the optional <c>temporaryGuard</c> parameter)
        /// resolves to <c>false</c> then the firing of the transition is aborted and a <see cref="GuardFailedException"/> is
        /// thrown - when firing the transition this causes the <c>Exit</c> and/or <c>Enter</c> (or neither)to be invoked
        /// depending upon the <see cref="FSMTransition.InvokeExitEnter"/> (or the optional <c>temporaryInvokeExitEnter</c>
        /// parameter) property of the transition 
        /// </summary>
        /// <param name="targetState">
        /// Optional <c>FSMState</c> instance of the transition's <see cref="FSMTransition.TargetState"/> that identifies the
        /// <c>FSMTransition</c> that is being fired - if this is not supplied then the first transition in the dictionary
        /// <c>Transitions</c> will be fired, this is useful if this state only has a single transition
        /// </param>
        /// <param name="temporaryInvokeExitEnter">
        /// Temporary replacement (i.e. only for this invocation of the method) for the <c>InvokeExitEnter</c>
        /// property of this transition
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>Guard</c>
        /// property of this transition
        /// </param>
        /// <returns>
        /// <see cref="FSMTransition.TargetState"/> taken from the fired <c>FSMTransition</c>
        /// </returns>
        /// <exception cref="NoTransitionsError">
        /// This exception is thrown if there are no transitions associated with this <c>FSMState</c> instance
        /// </exception>
        /// <exception cref="TransitionNotFoundError">
        /// This exception is thrown if there is no transition that has <see cref="FSMTransition.TargetState"/>
        /// equal to the supplied <c>FSMState</c> target state
        /// </exception>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState? FireTransition(FSMState? targetState = null,
                                        FSMTransition.InvokeMethods? temporaryInvokeExitEnter = null,
                                        FSMTransition.GuardFunc? temporaryGuard = null)
        {
            if (Transitions.Count == 0)
            {
                throw new NoTransitionsError() { State = this };
            }

            if (targetState != null)
            {
                if (temporaryInvokeExitEnter != null)
                {
                    return GetTransition(targetState).Fire(temporaryInvokeExitEnter, temporaryGuard);
                }

                return GetTransition(targetState).Fire(temporaryGuard);
            }

            if (temporaryInvokeExitEnter != null)
            {
                return Transitions.Values.ElementAt<FSMTransition>(0).Fire(temporaryInvokeExitEnter, temporaryGuard);
            }

            return Transitions.Values.ElementAt<FSMTransition>(0).Fire(temporaryGuard);
        }

        /// <summary>
        /// Method <c>Enter</c> is an abstract method that must be overridden in the derived class, it
        /// is used to invoke code when an instance of this state is first 'entered' and will only be
        /// invoked once per 'entry' - the concept of 'entry' means that this state is about to become the
        /// new active state of the FSM
        /// </summary>
        /// <param name="previousState">
        /// Reference to the <c>FSMState</c> instance that is about to be 'entered' into, see summary of
        /// method <see cref="Enter"/> for detail of the concept 'enter'
        /// </param>
        /// <param name="data">
        /// Generic type instance to be used in this method - may be <c>object</c> type <c>null</c> if
        /// not required
        /// </param>
        /// <returns>Nothing</returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public abstract void Enter<T>(FSMState? previousState, T? data);

        /// <summary>
        /// Method <c>Exit</c> is an abstract method that must be overridden in the derived class, it
        /// is used to invoke code when an instance of this state is 'exited' and will only be invoked
        /// once per 'exit' - the concept of 'exit' means that this state is about to lose its status as
        /// the current active state of the FSM
        /// </summary>
        /// <param name="nextState">
        /// Reference to the <c>FSMState</c> instance that has just been 'exited' from, see summary of
        /// method <see cref="Exit"/> for detail of the concept 'exit'
        /// </param>
        /// <param name="data">
        /// Generic type instance to be used in this method - may be <c>object</c> type <c>null</c> if
        /// not required
        /// </param>
        /// <returns>Nothing</returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public abstract void Exit<T>(FSMState? nextState, T? data);
    }
}
