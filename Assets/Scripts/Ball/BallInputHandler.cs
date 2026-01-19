using System;
using Input;
using UnityEngine;

namespace Ball
{
    public class BallInputHandler : MonoBehaviour
    {
        public Action<Vector2> MoveHandler;
        public Action JumpHandler;
        
        void OnEnable()
        {
            InputReceiver.OnMove += MoveHandler;
            InputReceiver.OnJump += JumpHandler;
        }

        void OnDisable()
        {
            MoveHandler?.Invoke(new Vector2(0,0));
            InputReceiver.OnMove -= MoveHandler;
            InputReceiver.OnJump -= JumpHandler;
        }
    }
}