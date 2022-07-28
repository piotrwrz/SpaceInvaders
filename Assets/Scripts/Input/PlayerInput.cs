using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    private Player player = null;
    private DefaultInputActions inputActions;
    private GameManager gameManager;

    public void Init(Player player)
    {
        gameManager = GameManager.Instance;
        this.player = player;
        inputActions = new DefaultInputActions();
        inputActions.Player.Shot.performed += Shot;
        inputActions.Player.Pause.performed += Pause;
        enabled = true;
    }

    private void Update()
    {
        MovePlayer(inputActions.Player.Move.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        enabled = false;
    }

    private void MovePlayer(Vector2 input)
    {
        player.Move(input.x * player.Speed * Time.deltaTime);
    }

    private void Shot(CallbackContext _)
    {
        player.Shot();
    }

    private void Pause(CallbackContext _)
    {
        gameManager.GameStateManager.ChangeState(gameManager.GameStateManager.CurrentState == EGameState.Game ? ECommand.Pause : ECommand.Resume);
    }
}