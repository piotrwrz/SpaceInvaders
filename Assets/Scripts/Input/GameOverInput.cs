using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GameOverInput : MonoBehaviour
{
    private DefaultInputActions inputActions;
    private GameManager gameManager;

    public void Init(GameManager gameMan)
    {
        gameManager = gameMan;
        inputActions = new DefaultInputActions();
        inputActions.GameOver.Continue.performed += Continue;
        enabled = true;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Continue(CallbackContext _)
    {
        gameManager.GameStateManager.ChangeState(ECommand.Begin);
    }
}
