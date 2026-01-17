using System;
using UnityEngine;

namespace Input
{
    public class InputReceiver
    {
        static InputActions inputActions;
        public static Action<Vector2> OnMove;
        public static Action OnJump;
        static InputReceiver()
        {
            inputActions = new InputActions();
            inputActions.Enable();
            inputActions.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            inputActions.Player.Move.canceled += ctx => OnMove?.Invoke(new Vector2(0,0));
            inputActions.Player.Jump.performed += ctx => OnJump?.Invoke();
        }
    }
}