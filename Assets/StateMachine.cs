using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public class State
    {
        public string name;             // name of state
        public System.Action onEnter;   // handle things that might happen on entering this state
        public System.Action onExit;    // handle things that might happen on exitng this state
        public System.Action onStay;    // handle things that might happen on staying this state

        public override string ToString()
        {
            return name;
        }
    }

    Dictionary<string, State> states = new Dictionary<string, State>();

    public State currentState;
    public State initialState;

    public State CreateState(string name)
    {
        var newState = new State();
        newState.name = name;
        if (states.Count == 0)
        {
            initialState = newState;
        }
        states[name] = newState;
        return newState;
    }

    public void Update()
    {
        if (states.Count == 0 || initialState == null)
        {
            Debug.Log("*** No states !?!");
        }

        if(currentState == null)
        {
            TransitionTo(initialState);
        }

        if(currentState.onStay != null)
        {
            currentState.onStay();
        }
    }

    public void TransitionTo(State newState)
    {
        // check for null
        if (newState == null)
        {
            Debug.Log("*** new state is null !?!");
            return;
        }

        // check onExit of current state
        if (currentState != null && currentState.onExit != null)
        {
            currentState.onExit();
        }

        // Here is the transition itself
        Debug.LogFormat("Transition from state '{0}' to state '{1}'", currentState, newState);
        currentState = newState;

        // check onEnter of the newState (=currentState)
        if (newState.onEnter != null)
        {
            newState.onEnter();
        }
    }

    public void TransitionTo(string newStateName)
    {
        if (!states.ContainsKey(newStateName))
        {
            Debug.Log("StateMachine doesn't contain the state " + newStateName);
            return;
        }

        var state = states[newStateName];
        TransitionTo(state);
    }
}
