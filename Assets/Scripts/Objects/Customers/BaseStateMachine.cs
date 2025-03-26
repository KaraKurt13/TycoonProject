using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Objects.Customers
{
    public class BaseStateMachine<TState> where TState : IState
    {
        private TState _currentState;

        public TState CurrentState => _currentState;

        private readonly Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

        public void AddState(Type type, IState state)
        {
            if (!_states.ContainsKey(type))
            {
                _states.Add(type, state);
            }
        }

        public void Enter<T>() where T : IState
        {
            _currentState?.Exit();

            var stateType = typeof(T);
            if (_states.TryGetValue(stateType, out var state))
            {
                _currentState = (TState)state;
                _currentState.Enter();
            }
            else
            {
                Debug.LogError($"State of type {stateType.Name} is not registered.");
            }

        }

        public void UpdateState()
        {
            _currentState.Update();
        }

        public void UpdateStatePhysics()
        {
            _currentState.PhysicsUpdate();
        }
    }
}