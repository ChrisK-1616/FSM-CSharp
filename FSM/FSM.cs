// Author: Chris Knowles
// Date: Jan 2023
// Copyright: Copperhead Labs, (c)2023
// File: FSM.cs
// Version: 1.0.0
// Notes: 

using System.Reflection;

namespace FSM
{
    /// <summary>
    /// Class <c>FSM</c> that represents a complete Finite State Machine (FSM)
    /// </summary>
    public class FSM
    {
        /// <summary>
        /// Property <c>Name</c>: name associated with this FSM, read only property initialised through
        /// constructor
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Property <c>ActiveState</c>: currently active <see cref="FSMState"/> within the FSM which can be
        /// <c>null</c> if there is no currently active <see cref="FSMState"/>, read/write property initialised
        /// through constructor
        /// </summary>
        public FSMState? ActiveState { get; set; }
        /// <summary>
        /// Property <c>PreviousState</c>: previously active <see cref="FSMState"/> within the FSM which will
        /// be <c>null</c> if there was no previous active <see cref="FSMState"/>, read/write property initialised
        /// through constructor
        /// </summary>
        public FSMState? PreviousState { get; set; }
        /// <summary>
        /// Property <c>States</c>: set of all <see cref="FSMState"/> instances managed by this <c>FSM</c> instance,
        /// this is a fully protected property and should not be accessed directly to ensure integrity, initialised
        /// through constructor
        /// </summary>
        public Dictionary<string, FSMState> States { get; }

        /// <summary>
        /// <c>Constructor</c> method
        /// </summary>
        /// <param name=""></param>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSM(string name)
        {
            Name = name;
            ActiveState = null;
            PreviousState = null;
            States = new Dictionary<string, FSMState>();
        }

        /// <summary>
        /// Method <c>ToString</c> overrides object class <c>ToString</c> method
        /// </summary>
        /// <param name=""></param>
        /// <returns>
        /// String representation of this <c>FSM</c> object instance
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public override string ToString() => $"FSM [{Name}]";

        /// <summary>
        /// Method <c>AddState</c> instantiates and adds a new <see cref="FSMState"/> derived object (as declared
        /// by the <c>T</c> generic type) to the <c>States</c> dictionary, but only if there is no existing
        /// state in the dictionary that has the supplied <c>name</c> parameter, if there is an existing state
        /// then the method aborts and throws a <see cref="DuplicateStateError"/> exception - if adding the new
        /// is successful, and this is the first <see cref="FSMState"/> added, this <see cref="FSMState"/>
        /// instance is assigned as the <c>ActiveState</c> property on the FSM
        /// </summary>
        /// <param name="name">
        /// Unique name for the new <see cref="FSMState"/> derived object instance 
        /// </param>
        /// <returns>
        /// <c>true</c> if new state of generic type <c>T</c> is instantiated and found to be not a duplicate, it
        /// will then be added to the <c>States</c> dictionary keyed by the <c>Name</c> of the state, otherwise
        /// return <c>false</c>
        /// </returns>
        /// <exception cref="DuplicateStateError">
        /// Thrown if the new state to be added is already in the <c>States</c> dictionary
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool AddState<T>(string name, object? data = null) where T : FSMState
        {
            if (!HasState(name))
            {
                var method = typeof(T).GetMethod("ObtainInstance",
                                                 BindingFlags.Public | BindingFlags.Static,
                                                 new Type[] { typeof(FSM), typeof(string), typeof(object) });

                if (method != null)
                {
                    object[] parameters;
                    
                    if (data == null)
                    {
                        parameters = new object[] { this, name };
                    }
                    else
                    {
                        parameters = new object[] { this, name, data };
                    }

                    if (method.Invoke(null, parameters) is T state)
                    {
                        States[name] = state;

                        if (States.Count == 1)
                        {
                            ActiveState = state;
                        }

                        return true;
                    }
                }

                return false;
            }

            throw new DuplicateStateError() { Name = name, Fsm = this };
        }

        /// <summary>
        /// Method <c>HasState</c> checks to see if a name supplied in parameter <c>stateName</c>
        /// exists in the <c>States</c> dictionary
        /// </summary>
        /// <param name="stateName">
        /// Name of the <see cref="FSMState"/> instance to check for
        /// </param>
        /// <returns>
        /// <c>true</c> if a <see cref="FSMState"/> instance exists in the <c>States</c> dictionary with
        /// the name supplied in parameter <c>stateName</c>, otherwise returns <c>false</c> 
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool HasState(string stateName) => States.ContainsKey(stateName);

        /// <summary>
        /// Method <c>GetState</c> returns the <see cref="FSMState"/> instance from the <c>States</c>
        /// dictionary which has the name supplied in parameter <c>statename</c>, or throws a 
        /// <see cref="StateNotFoundError"/> exception if the <see cref="FSMState"/> instance is not present
        /// in the <c>States</c> dictionary
        /// </summary>
        /// <param name="stateName">
        /// Name of the <see cref="FSMState"/> instance to obtain from the <c>States</c> dictionary
        /// </param>
        /// <returns>
        /// <see cref="FSMState"/> obtained from the <c>States</c> dictionary
        /// </returns>
        /// <exception cref=StateNotFoundError">
        /// Thrown if relevant <see cref="FSMState"/> is not found
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public FSMState? GetState(string stateName)
        {
            if (States == null)
            {
                throw new StateNotFoundError() { Fsm = this, StateName = stateName };
            }

            try
            {
                return States[stateName];
            }
            catch (KeyNotFoundException)
            {
                throw new StateNotFoundError() { Fsm = this, StateName = stateName };
            }
        }

        /// <summary>
        /// Method <c>GetState</c> returns the <see cref="FSMState"/> instance from the <c>States</c>
        /// dictionary which has the name supplied in parameter <c>statename</c>, or throws a 
        /// <see cref="StateNotFoundError"/> exception if the <see cref="FSMState"/> instance is not present
        /// in the <c>States</c> dictionary
        /// </summary>
        /// <param name="stateName">
        /// Name of the <see cref="FSMState"/> instance to obtain from the <c>States</c> dictionary
        /// </param>
        /// <returns>
        /// <see cref="FSMState"/> obtained from the <c>States</c> dictionary
        /// </returns>
        /// <exception cref=StateNotFoundError">
        /// Thrown if relevant <see cref="FSMState"/> is not found
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public T? GetState<T>(string stateName) where T : FSMState
        {
            if (States == null)
            {
                throw new StateNotFoundError() { Fsm = this, StateName = stateName };
            }

            try
            {
                return States[stateName] as T;
            }
            catch (KeyNotFoundException)
            {
                throw new StateNotFoundError() { Fsm = this, StateName = stateName };
            }
        }

        /// <summary>
        /// Method <c>RemoveState</c> attempts to remove <see cref="FSMState"/> instance from the <c>States</c>
        /// dicitionary that has the name supplied in the <c>stateName</c> parameter and assigns <c>null</c> to
        /// the <c>ActiveState</c> property if this removed <see cref="FSMState"/> is the active state
        /// </summary>
        /// <param name="stateName">
        /// Name of the <see cref="FSMState"/> instance to remove from the <c>States</c> dictionary
        /// </param>
        /// <returns>
        /// <c>true</c> if relevant <see cref="FSMState"/> is removed, otherwise returns <c>false</c>
        /// </returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public bool RemoveState(string stateName)
        {
            if (ActiveState != null)
            {
                if (HasState(stateName) && ActiveState.Equals(States[stateName]))
                {
                    ActiveState = null;
                }
            }

            return States.Remove(stateName);
        }

        /// <summary>
        /// Method <c>ClearStates</c> clears down the <c>States</c> dictionary and assigns <c>null</c> to the
        /// <c>ActiveState</c> property
        /// </summary>
        /// <param name=""></param>
        /// <returns>Nothing</returns>
        /// <exception cref="None thrown"></exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public void ClearStates()
        {
            ActiveState = null;
            States.Clear();
        }

        /// <summary>
        /// Method <c>FireActiveState</c> 'fires' the <see cref="FSMTransition"/> associated with the supplied <c>targetName</c>
        /// <c>string</c> parameter, fires this <see cref="FSMTransition"/> using the optionally supplied
        /// <see cref="FSMTransition.InvokeMethods"/> <c>temporaryInvokeExitEnter</c> parameter, the optionally supplied
        /// <see cref="FSMTransition.GuardFunc"/> <c>temporaryGuard</c> parameter and the optionally supplied <c>data</c>
        /// parameter - note, if there is no <c>ActiveState</c> reference (i.e. this property is <c>null</c>) then a
        /// <see cref="NoActiveStateError"/> is thrown, if there is no relevant <see cref="FSMTransition"/> then a
        /// <see cref="TransitionNotFoundError"/> is thrown, also if the guard method 'fails' as the <see cref="FSMTransition"/>
        /// is 'fired' then the resulting <see cref="GuardFailedException"/> is thrown from this method
        /// </summary>
        /// <param name="targetName">
        /// Name of the target state associated with the <see cref="FSMTransition"/> to be 'fired'
        /// </param>
        /// <param name="temporaryInvokeExitEnter">
        /// Temporary replacement (i.e. only for this invocation of the method) for the <see cref="FSMTransition.InvokeExitEnter"/>
        /// property of the associated <see cref="FSMTransition"/>
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <see cref="FSMTransition.Guard"/>
        /// property of the associated <see cref="FSMTransition"/>
        /// </param>
        /// <param name="data">
        /// Optional generic instance to be passed through to the <see cref="FSMState.Exit"/> and/or <see cref="FSMState.Enter"/>
        /// methods involved as the relevant <see cref="FSMTransition"/> 'fires'
        /// </param>
        /// <returns>Nothing</returns>
        /// <exception cref="NoActiveStateError">
        /// This exception is thrown if the <c>ActiveState</c> property is <c>null</c>
        /// </exception>
        /// <exception cref="TransitionNotFoundError">
        /// This exception is thrown if there is no <see cref="FSMTransition"/> held by the <c>ActiveState</c> property that has
        /// the value supplied in the <c>targetName</c> parameter
        /// </exception>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the relevant guard method prevents the relevant <see cref="FSMTransition"/> from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public void FireActiveState<T>(string? targetName,
                                       T? data,
                                       FSMTransition.InvokeMethods? temporaryInvokeExitEnter = null,
                                       FSMTransition.GuardFunc? temporaryGuard = null) where T : class
        {
            if (ActiveState == null)
            {
                throw new NoActiveStateError() { Fsm = this };
            }

            if (targetName == null)
            {
                if (data == null)
                {
                    ActiveState.FireTransition(temporaryInvokeExitEnter : temporaryInvokeExitEnter,
                                               temporaryGuard : temporaryGuard);
                }
                else
                {
                    ActiveState.FireTransition<T>(temporaryInvokeExitEnter : temporaryInvokeExitEnter,
                                                  temporaryGuard: temporaryGuard, 
                                                  data : data);
                }
            }
            else
            {
                if (data == null)
                {
                    ActiveState.FireTransition(targetState : States[targetName],
                                               temporaryInvokeExitEnter: temporaryInvokeExitEnter,
                                               temporaryGuard: temporaryGuard);
                }
                else
                {
                    ActiveState.FireTransition<T>(targetState: States[targetName], 
                                                  temporaryInvokeExitEnter: temporaryInvokeExitEnter,
                                                  temporaryGuard: temporaryGuard,
                                                  data: data);
                }
            }
        }

        /// <summary>
        /// Method <c>FireActiveState</c> 'fires' the <see cref="FSMTransition"/> associated with the supplied <c>targetName</c>
        /// <c>string</c> parameter, fires this <see cref="FSMTransition"/> using the optionally supplied
        /// <see cref="FSMTransition.InvokeMethods"/> <c>temporaryInvokeExitEnter</c> parameter, the optionally supplied
        /// <see cref="FSMTransition.GuardFunc"/> <c>temporaryGuard</c> parameter - note, if there is no <c>ActiveState</c>
        /// reference (i.e. this property is <c>null</c>) then a <see cref="NoActiveStateError"/> is thrown, if there is no
        /// relevant <see cref="FSMTransition"/> then a <see cref="TransitionNotFoundError"/> is thrown, also if the guard method
        /// 'fails' as the <see cref="FSMTransition"/> is 'fired' then the resulting <see cref="GuardFailedException"/> is thrown
        /// from this method
        /// </summary>
        /// <param name="targetName">
        /// Name of the target state associated with the <see cref="FSMTransition"/> to be 'fired'
        /// </param>
        /// <param name="temporaryInvokeExitEnter">
        /// Temporary replacement (i.e. only for this invocation of the method) for the <see cref="FSMTransition.InvokeExitEnter"/>
        /// property of the associated <see cref="FSMTransition"/>
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <see cref="FSMTransition.Guard"/>
        /// property of the associated <see cref="FSMTransition"/>
        /// </param>
        /// <returns>Nothing</returns>
        /// <exception cref="NoActiveStateError">
        /// This exception is thrown if the <c>ActiveState</c> property is <c>null</c>
        /// </exception>
        /// <exception cref="TransitionNotFoundError">
        /// This exception is thrown if there is no <see cref="FSMTransition"/> held by the <c>ActiveState</c> property that has
        /// the value supplied in the <c>targetName</c> parameter
        /// </exception>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the relevant guard method prevents the relevant <see cref="FSMTransition"/> from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public void FireActiveState(string? targetName,
                                    FSMTransition.InvokeMethods? temporaryInvokeExitEnter = null,
                                    FSMTransition.GuardFunc? temporaryGuard = null) => FireActiveState<object>(targetName, null,
                                                                                                               temporaryInvokeExitEnter,
                                                                                                               temporaryGuard);

        /// <summary>
        /// Method <c>FireActiveState</c> 'fires' the <see cref="FSMTransition"/> associated with the supplied <c>targetName</c>
        /// <c>string</c> parameter, fires this <see cref="FSMTransition"/> using the optionally supplied
        /// <see cref="FSMTransition.InvokeMethods"/> <c>temporaryInvokeExitEnter</c> parameter, the optionally supplied
        /// <see cref="FSMTransition.GuardFunc"/> <c>temporaryGuard</c> parameter - note, if there is no <c>ActiveState</c>
        /// reference (i.e. this property is <c>null</c>) then a <see cref="NoActiveStateError"/> is thrown, if there is no
        /// relevant <see cref="FSMTransition"/> then a <see cref="TransitionNotFoundError"/> is thrown, also if the guard method
        /// 'fails' as the <see cref="FSMTransition"/> is 'fired' then the resulting <see cref="GuardFailedException"/> is thrown
        /// from this method
        /// </summary>
        /// <param name="temporaryInvokeExitEnter">
        /// Temporary replacement (i.e. only for this invocation of the method) for the <see cref="FSMTransition.InvokeExitEnter"/>
        /// property of the associated <see cref="FSMTransition"/>
        /// </param>
        /// <param name="temporaryGuard">
        /// Optional temporary replacement (i.e. only for this invocation of the method) for the <see cref="FSMTransition.Guard"/>
        /// property of the associated <see cref="FSMTransition"/>
        /// </param>
        /// <returns>Nothing</returns>
        /// <exception cref="NoActiveStateError">
        /// This exception is thrown if the <c>ActiveState</c> property is <c>null</c>
        /// </exception>
        /// <exception cref="TransitionNotFoundError">
        /// This exception is thrown if there is no <see cref="FSMTransition"/> held by the <c>ActiveState</c> property that has
        /// the value supplied in the <c>targetName</c> parameter
        /// </exception>
        /// <exception cref="GuardFailedException">
        /// This exception is thrown if the relevant guard method prevents the relevant <see cref="FSMTransition"/> from 'firing'
        /// </exception>
        /// <example></example>
        /// <code></code>
        /// <remarks></remarks>
        public void FireActiveState(FSMTransition.InvokeMethods? temporaryInvokeExitEnter = null,
                                    FSMTransition.GuardFunc? temporaryGuard = null) => FireActiveState(null,
                                                                                                       temporaryInvokeExitEnter,
                                                                                                       temporaryGuard);
    }
}
