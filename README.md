# FSM (Finite State Machine)

Basic finite state machine implmentation that enables states to be transitioned between other states depending on guard functions associated with these transitions. Also, each state can have an entry and/or exit function associated with it that can be configured to be invoked when this state is either entered and/or exited respectively. The managing finite state machine object contains the states and transitions betwen these states along with a reference to the one currently active state.

This FSM system is provided as a .Net 7.0 assembly and is written in C#.
