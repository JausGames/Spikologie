using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerMovement motor = null;
    [SerializeField] PlayerCombat combat = null;
    [SerializeField] Player player = null;
    [SerializeField] private Vector2 move;
    [SerializeField] private float index;
    [SerializeField] bool inputBlock = true;

    private Controls controls;
    private Controls Controls
    {
    get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        motor = GetComponent<PlayerMovement>();
        index = playerInput.playerIndex;
        var motors = FindObjectsOfType<PlayerMovement>();
        motor = motors.FirstOrDefault(m => m.GetPlayerIndex() == index);
        var combats = FindObjectsOfType<PlayerCombat>();
        combat = combats.FirstOrDefault(m => m.GetPlayerIndex() == index);
        var players = FindObjectsOfType<Player>();
        player = players.FirstOrDefault(m => m.GetPlayerIndex() == index);
        player.SetInputHandler(this);
    }

    // Update is called once per frame
    private void Start()
    {


    }
    public void OnMove(CallbackContext context)
    {
        if (motor == null || inputBlock ) return;
        move = context.ReadValue<Vector2>();
        motor.SetMoveDirection(move);
    }
    public void OnJump(CallbackContext context)
    {
        if (motor == null || inputBlock || !context.performed) return;
        motor.Jump();
    }
    public void OnAttack(CallbackContext context)
    {
        if (combat == null || inputBlock || !context.performed) return;
        combat.StartAttack();
    }
    public void OnDodge(CallbackContext context)
    {
        Debug.Log("OnDodge ok");
        if (combat == null || inputBlock || !context.performed) return;
        combat.Dodge(context.ReadValue<float>());
    }
    public void SetInputBlock(bool value)
    {
        inputBlock = value;
    }
}
