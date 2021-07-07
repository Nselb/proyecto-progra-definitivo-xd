using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get { return instance; }
    }
    private PlayerInput playerControls;

    private void Awake()
    {
        playerControls = new PlayerInput();
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }
    public Vector2 GetPlayerLook()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }
    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;

    }
    public bool PlayerAttackedThisFrame()
    {
        return playerControls.Player.Fire.triggered;
    }
}