using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class MenuInput : MonoBehaviour
{
    private DefaultInputActions inputActions;
    private GameManager gameManager;

    public void Init(GameManager gameMan)
    {
        gameManager = gameMan;
        inputActions = new DefaultInputActions();
        inputActions.Menu.Start.performed += StartGame;
        inputActions.Menu.ChangeDifficulty.performed += ChangeDifficulty;
        inputActions.Menu.Exit.performed += Exit;
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

    private void ChangeDifficulty(CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        gameManager.ChangeDifficulty(input.x > 0);
    }

    private void StartGame(CallbackContext _)
    {
        gameManager.GameStateManager.ChangeState(ECommand.Begin);
    }

    private void Exit(CallbackContext _)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
