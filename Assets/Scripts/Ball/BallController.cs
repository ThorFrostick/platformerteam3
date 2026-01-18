using System;
using System.Collections;
using Input;
using UnityEngine;

namespace Ball
{
    public class BallController : MonoBehaviour
    {
        #region Resources
        public Transform forwardIndicator;
        public BallMovementParam movementParam;
        public float initialSpeed;
        public float accelForward;
        #endregion
        
        #region MovementParam
        Vector3 forwardDirection;
        float maxHorizontalSpeed;
        float accelHorizontal;
        float accelHorizontal_Air;
        float frictionHorizontal;
        float frictionHorizontal_Air;
        float jumpImpulse;
        float jumpCooldown;
        float gravity;
        float coyoteTimeDuration;
        Vector3 groundCheckCenter;
        float groundCheckRange;
        #endregion

        #region Status
        bool isOnGround;
        bool isInJumpWindow;
        bool isJumpReady = true;
        #endregion

        #region Input
        Vector2 inputDirection;
        bool isJumping;
        #endregion

        #region Components
        Rigidbody rb;
        Camera cam;
        BallInputHandler inputHandler;
        #endregion

        void ReadMovementParam(BallMovementParam param)
        {
            maxHorizontalSpeed = param.maxHorizontalSpeed;
            accelHorizontal = param.accelHorizontal;
            accelHorizontal_Air = param.accelHorizontalInAir;
            frictionHorizontal =  param.frictionHorizontal;
            frictionHorizontal_Air = param.frictionHorizontalInAir;
            jumpImpulse = param.jumpImpulse;
            jumpCooldown = param.jumpCooldown;
            gravity = param.gravity;
            coyoteTimeDuration = param.coyoteTimeDuration;
            groundCheckCenter = param.groundCheckCenter;
            groundCheckRange = param.groundCheckRange;
        }

        void Awake()
        {
            cam = Camera.main;
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<BallInputHandler>();
            inputHandler.enabled = false;
            inputHandler.MoveHandler = val => { inputDirection = val; };
            inputHandler.JumpHandler = () => { isJumping = isInJumpWindow; };
            inputHandler.enabled = true;
        }

        void Start()
        {
            forwardDirection = forwardIndicator.forward;
            ReadMovementParam(movementParam);
            transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            rb.linearVelocity = transform.forward * initialSpeed;
            isJumpReady = true;
        }

        void OnValidate()
        {
            forwardDirection = forwardIndicator.forward;
            ReadMovementParam(movementParam);
            transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        }


        void FixedUpdate()
        {
            Move();
        }
        
        #region Movement

        void Move()
        {
            Vector3 velocity = rb.linearVelocity;
            float X = Vector3.Dot(velocity,transform.right);
            float Y = Vector3.Dot(velocity,transform.up);
            float Z = Vector3.Dot(velocity,transform.forward);
            
            isOnGround = GroundCheck();
            if (isOnGround)
            {
                if (coyoteTimerInProgress != null)
                    StopCoroutine(coyoteTimerInProgress);
                coyoteTimerInProgress = StartCoroutine(CoyoteTimer());
            }
            if (isJumping && isJumpReady)
            {
                Y = jumpImpulse;
                StartCoroutine(JumpCooldown());
                isJumping = false;
            }
            Y -= gravity * Time.fixedDeltaTime;
            
            float accel = isOnGround ? accelHorizontal : accelHorizontal_Air;
            float friction = isOnGround ? frictionHorizontal : frictionHorizontal_Air;
            
            if (Mathf.Abs(inputDirection.x) > 0.1f)
            {
                X += (inputDirection.x > 0.1f ? 1 : -1) * accel * Time.fixedDeltaTime;
                X = Mathf.Clamp(X, -maxHorizontalSpeed, maxHorizontalSpeed);
            }
            else
            {
                X -= Mathf.Clamp(Mathf.Sign(X) * friction * Time.fixedDeltaTime, -Mathf.Abs(X), Mathf.Abs(X));
            }

            Z += accelForward * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(X, Y, Z);
        }

        Coroutine coyoteTimerInProgress;
        
        IEnumerator CoyoteTimer()
        {
            isInJumpWindow = true;
            yield return new WaitForSeconds(coyoteTimeDuration);
            isInJumpWindow = false;
            coyoteTimerInProgress = null;
        }
        
        IEnumerator JumpCooldown()
        {
            isJumpReady = false;
            yield return new WaitForSeconds(jumpCooldown);
            isJumpReady = true;
        }

        bool GroundCheck()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + groundCheckCenter, groundCheckRange, LayerMask.GetMask("Ground"));
            return colliders.Length > 0;
        }
            
        #endregion

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + groundCheckCenter, groundCheckRange);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + forwardDirection);
        }
    }
}
