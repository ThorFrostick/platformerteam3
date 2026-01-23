using UnityEngine;

namespace Ball
{
    [CreateAssetMenu(fileName = "MovementParam", menuName = "ScriptableObjects/BallMovementParam")]
    public class BallMovementParam : ScriptableObject
    {
        public float maxHorizontalSpeed;
        public float accelHorizontal;
        public float accelHorizontalInAir;
        public float frictionHorizontal;
        public float frictionHorizontalInAir;
        public float jumpHeight;
        public float jumpCooldown;
        public float gravity;
        public float coyoteTimeDuration;
        public Vector3 groundCheckCenter;
        public float groundCheckRange;
        public float trackSwitchingSpeed;
        public float reAccelRate;
    }
}