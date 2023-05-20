// Author: Chris Knowles
// Date: Jan 2023
// Copyright: Copperhead Labs, (c)2023
// File: FSMTransition.cs
// Version: 1.0.0
// Notes: 

namespace FSM
{
    public class FSMTransition
    {
        /// <summary>
        /// Enumeration <c>InvokeMethods</c>: this provides the approach for the invocation of the <see cref="Enter"/>
        /// and/or <see cref="Exit"/> methods of the <c>SourceState</c> and <c>TargetState</c> when the transition
        /// fires where each value means:-
        /// 
        ///     Both      - both the <see cref="Exit"/> method of the <c>SourceState</c> and the <see cref="Enter"/>
        ///                 method of the <c>TargetState</c> are invoked - this is the default for 'non-short-circuit'
        ///                 transitions, i.e. those that have <c>SourceState</c> and <c>TargetState</c> different
        ///     EnterOnly - only the <see cref="Enter"/> method of the <c>TargetState</c> is invoked
        ///     ExitOnly  - only the <see cref="Exit"/> method of the <c>SourceState</c> is invoked
        ///     Neither   - neither the <see cref="Exit"/> method of the <c>SourceState</c> nor the <see cref="Enter"/>
        ///                 method of the <c>TargetState</c> are invoked - this is the default for 'short-circuit'
        ///                 transitions, i.e. those that have the same <c>SourceState</c> and <c>TargetState</c>
        /// </summary>
        public enum InvokeMethods
        {
            Both,
            ExitOnly,
            EnterOnly,
            Neither
        }
        /// <summary>
        /// Delegate <c>GuardFunc</c>: defines the type of method that can be provided as a 'guard' which will be invoked
        /// at the time the transition fires, this guard method must either return <c>true</c> if the transition is to
        /// fire successfully and move the FSM to the transition's target state or it must throw a 
        /// <see cref="GuardFailedException"/> with the transition firing being aborted
        /// </summary>
        public delegate bool GuardFunc(FSMState sourceState, FSMState targetState, object? data);

        /// <summary>
        /// Property <c>Id</c>: unique GUID associated with this <c>FSMTransition</c>, read only property initialised at
        /// instantiation in constructor
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Property <c>SourceState</c>: reference to <see cref="FSMState"/> instance at the source 'end' of this
        /// <c>FSMTransition</c>, read only property initialised through constructor
        /// </summary>
        public FSMState SourceState { get; }
        /// Property <c>TargetState</c>: reference to <see cref="FSMState"/> instance at the target 'end' of this
        /// <c>FSMTransition</c>, read only property initialised through constructor
        public FSMState TargetState { get; }
        /// <summary>
        /// Property <c>InvokeExitEnter</c>: this property provides the default approach for the invocation of the
        /// <see cref="Enter"/> and/or <see cref="Exit"/> methods of the <c>SourceState</c> and <c>TargetState</c> 
        /// when the transition fires - the type of this is the enumeration <c>InvokeMethods</c> - initialised through
        /// constructor
        /// </summary>
        public InvokeMethods InvokeExitEnter { get; set; }
        /// <summary>
        /// Property <c>Guard</c>: holds the reference to the guard method delegate, or is <c>null</c> if no guard is
        /// required, see <see cref="GuardFunc"/> for more details - initialised through constructor
        /// </summary>
        public GuardFunc? Guard { get; set; }

        /// <summary>
        /// <c>Constructor</c> method
        /// </summary>
        /// <param name=""></param>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMTransition(FSMState sourceState, FSMState targetState, GuardFunc? guard) 
        {
            Id = Guid.NewGuid();
            SourceState = sourceState;
            TargetState = targetState;
            Guard = guard;
            
            if(IsShortCircuited())
            {
                InvokeExitEnter = InvokeMethods.Neither;
            }
            else
            { 
                InvokeExitEnter = InvokeMethods.Both;
            }
        }

        /// <summary>
        /// <c>Constructor</c> method
        /// </summary>
        /// <param name="sourceState">
        /// </param>
        /// <param name="targetState">
        /// </param>
        /// <param name="guard">
        /// </param>
        /// <param name="invokeExitEnter">
        /// </param>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMTransition(FSMState sourceState, FSMState targetState, InvokeMethods invokeExitEnter,
                             GuardFunc? guard)
        {
            Id = Guid.NewGuid();
            SourceState = sourceState;
            TargetState = targetState;
            InvokeExitEnter = invokeExitEnter;
            Guard = guard;
        }

        /// <summary>
        /// Method <c>ToString</c> overrides object class <c>ToString</c> method
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        /// String representation of this <c>FSMTransition</c> object instance
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override string ToString() => $"Transition [{Id}]:{SourceState}->{TargetState}";

        /// <summary>
        /// Method <c>Equals</c> overrides object class <c>Equals</c> method
        /// </summary>
        /// <param name="obj">
        /// Instance of <c>FSMTransition</c> object to check for equality to this <c>FSMTransition</c> object
        /// </param>
        /// <returns>
        /// <c>true</c> if <c>Id</c> property of the supplied <c>FSMTransition</c> object is equal to the
        /// <c>Id</c> property of this <c>FSMTransition</c> object, otherwise returns <c>false</c>
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override bool Equals(object? obj) => obj is FSMTransition transition && Id.Equals(transition.Id);

        /// <summary>
        /// Method <c>GetHashCode</c> overrides object class <c>GetHashCode</c> method
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        /// Integer value of the <c>Id</c> property's hashcode
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Method <c>IsShortCircuited</c> checks if this transition is a 'short-circuit' - for more detail on the
        /// concept of 'fire' see summary of method <see cref="Fire"/>
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        /// <c>true</c> if this transition is a 'short-circuit', otherwise returns <c>false</c>
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool IsShortCircuited() => SourceState.Equals(TargetState);

        /// <summary>
        /// Method <c>FireOK</c> private method invoked by <see cref="Fire(GuardFunc?)"/> and
        /// <see cref="Fire(InvokeMethods, GuardFunc?)"/>
        /// </summary>
        /// <param name="activeInvokeExitEnter">
        /// <c>InvokeMethods</c> value to be used in the method
        /// </param>
        /// <param name="data">
        /// Optional generic instance to be passed through to the <see cref="FSMState.Exit"/> and/or <see cref="FSMState.Enter"/>
        /// methods involved as the transition fires
        /// </param>
        /// <returns>
        /// <c>TargetState</c> of this transition
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        private FSMState FireOK<T>(InvokeMethods? activeInvokeExitEnter, T? data)
        {
            switch (activeInvokeExitEnter)
            {
                case InvokeMethods.Both:
                {
                    SourceState.Exit<T>(TargetState, data);
                    SourceState.Fsm.PreviousState= SourceState;
                    TargetState.Enter<T>(SourceState, data);
                    TargetState.Fsm.ActiveState= TargetState;
                    return TargetState;
                }

                case InvokeMethods.ExitOnly:
                {
                    SourceState.Exit<T>(TargetState, data);
                    SourceState.Fsm.PreviousState = SourceState;
                    TargetState.Fsm.ActiveState = TargetState;
                    return TargetState;
                }

                case InvokeMethods.EnterOnly:
                {
                    SourceState.Fsm.PreviousState = SourceState;
                    TargetState.Enter<T>(SourceState, data);
                    TargetState.Fsm.ActiveState = TargetState;
                    return TargetState;
                }

                default:
                {
                    TargetState.Fsm.ActiveState = TargetState;
                    SourceState.Fsm.PreviousState = SourceState;
                    return TargetState;
                }
            }
        }

        /// <summary>
        /// Method <c>Fire</c> is invoked to 'fire' the transition, this will attempt to move the FSM from the current active
        /// state of the FSM to the <c>TargetState</c> of this transition - parameter <c>data</c> is passed through as an
        /// <c>object</c> type <c>null</c> to the <see cref="FSMState.Exit"/> and/or <see cref="FSMState.Enter"/> methods
        /// involved as the transition fires
        /// </summary>
        /// <param name="temporaryInvokeExitEnter">
        /// Temporary replacement (i.e. only for this invocation of the method) for the <c>InvokeExitEnter</c> property of this
        /// transition
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>Guard</c> property of this
        /// transition
        /// </param>
        /// <returns>
        /// <c>TargetState</c> of this transition
        /// </returns>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState Fire(InvokeMethods? temporaryInvokeExitEnter, GuardFunc? temporaryGuard)
        {
            var activeGuard = temporaryGuard ?? Guard;

            if ((activeGuard != null) && !activeGuard(SourceState, TargetState, null))
            {
                throw new GuardFailedException() { SourceState = SourceState, TargetState = TargetState, Transition = this };
            }

            return Fire<object>(temporaryInvokeExitEnter, temporaryGuard, null);
        }

        /// <summary>
        /// Method <c>Fire</c> is invoked to 'fire' the transition, this will attempt to move the FSM from the current active
        /// state of the FSM to the <c>TargetState</c> of this transition
        /// </summary>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>Guard</c> property of this
        /// transition
        /// </param>
        /// <param name="data">
        /// Optional generic instance to be passed through to the <see cref="FSMState.Exit"/> and/or <see cref="FSMState.Enter"/>
        /// methods involved as the transition fires
        /// <see cref="FSMState.Enter"/> methods involved as the transition fires
        /// </param>
        /// <returns>
        /// <c>TargetState</c> of this transition
        /// </returns>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState Fire<T>(GuardFunc? temporaryGuard, T? data)
        {
            var activeGuard = temporaryGuard ?? Guard;

            if ((activeGuard != null) && !activeGuard(SourceState, TargetState, null))
            {
                throw new GuardFailedException() { SourceState = SourceState, TargetState = TargetState, Transition = this };
            }

            return FireOK<T>(InvokeExitEnter, data);
        }

        /// <summary>
        /// Method <c>Fire</c> is invoked to 'fire' the transition, this will attempt to move the FSM from the current active
        /// state of the FSM to the <c>TargetState</c> of this transition
        /// </summary>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>Guard</c> property of this
        /// transition
        /// </param>
        /// <returns>
        /// <c>TargetState</c> of this transition
        /// </returns>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState Fire(GuardFunc? temporaryGuard) => Fire<object>(temporaryGuard, null);

        /// <summary>
        /// Method <c>Fire</c> is invoked to 'fire' the transition, this will attempt to move the FSM from the current active
        /// state of the FSM to the <c>TargetState</c> of this transition
        /// </summary>
        /// <param name="temporaryInvokeExitEnter">
        /// Temporary replacement (i.e. only for this invocation of the method) for the <c>InvokeExitEnter</c> property of this
        /// transition
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <c>Guard</c> property of this
        /// transition
        /// </param>
        /// <param name="data">
        /// Optional generic instance to be passed through to the <see cref="FSMState.Exit"/> and/or <see cref="FSMState.Enter"/>
        /// methods involved as the transition fires
        /// </param>
        /// <returns>
        /// <c>TargetState</c> of this transition
        /// </returns>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the guard method prevents the transition from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState Fire<T>(InvokeMethods? temporaryInvokeExitEnter, GuardFunc? temporaryGuard, T? data)
        {
            var activeGuard = temporaryGuard ?? Guard;

            if ((activeGuard != null) && !activeGuard(SourceState, TargetState, null))
            {
                throw new GuardFailedException() { SourceState = SourceState, TargetState = TargetState, Transition = this };
            }

            return FireOK<T>(temporaryInvokeExitEnter ?? InvokeExitEnter, data);
        }
    }
}
