using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public event Action<EGameState> StateChanged = delegate { };

    public EGameState CurrentState
    {
        get;
        private set;
    }

    private Dictionary<EGameState, Dictionary<ECommand, EGameState>> transitions = new Dictionary<EGameState, Dictionary<ECommand, EGameState>>();

    public void Init()
    {
        CurrentState = EGameState.Intro;
        transitions = new Dictionary<EGameState, Dictionary<ECommand, EGameState>>
        {
            {EGameState.Intro, new Dictionary<ECommand, EGameState>{{ECommand.Begin, EGameState.Preparation} } },
            {EGameState.Preparation, new Dictionary<ECommand, EGameState>{{ECommand.Begin, EGameState.Game} } },
            {EGameState.Game, new Dictionary<ECommand, EGameState>{{ECommand.Pause, EGameState.Paused}, { ECommand.End, EGameState.End }, { ECommand.Begin, EGameState.Preparation } } },
            {EGameState.Paused, new Dictionary<ECommand, EGameState>{{ECommand.Resume, EGameState.Game} } },
            {EGameState.End, new Dictionary<ECommand, EGameState>{{ECommand.Begin, EGameState.Intro} } },
        };
    }

    public EGameState ChangeState(ECommand command)
    {
        CurrentState = GetNext(command);
        StateChanged(CurrentState);
        return CurrentState;
    }

    private EGameState GetNext(ECommand command)
    {
        EGameState nextState;
        if (!transitions[CurrentState].TryGetValue(command, out nextState))
        {
            throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
        }
        return nextState;
    }
}
