using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

namespace Ball
{
    public enum ControlMode
    {
        Smooth,
        Tracks
    }
    public class BallController : MonoBehaviour
    {
        #region options
        public ControlMode ControlMode;
        #endregion
        
        #region Resources
        public Transform ball;
        public Transform forwardIndicator;
        public Transform tracks;
        public BallMovementParam movementParam;
        public float initialSpeed;
        public float accelForward;
        public Transform spawnPoint;
        public CinemachineCamera cam;
        #endregion
        
        #region MovementParam
        Vector3 forwardDirection;
        float reAccelRate;
        float maxSpeed = 5000;
        float maxHorizontalSpeed;
        float accelHorizontal;
        float accelHorizontal_Air;
        float frictionHorizontal;
        float frictionHorizontal_Air;
        float jumpHeight;
        float jumpCooldown;
        float gravity;
        float coyoteTimeDuration;
        Vector3 groundCheckCenter;
        float groundCheckRange;
        float trackSwitchingSpeed;
        #endregion

        #region Status
        bool isOnGround;
        bool isInJumpWindow;
        bool isJumpReady;
        float targetSpeed;
        string currentPhase;
        //For Tracks
        int currentTrack;
        List<SplineContainer> trackList =  new List<SplineContainer>();
        Vector3 targetPosition;
        #endregion

        #region Input
        Vector2 inputDirection;
        bool isJumping;
        bool isMoveReset;
        #endregion

        #region Components
        Rigidbody rb;
        BallInputHandler inputHandler;
        #endregion

        void ReadMovementParam(BallMovementParam param)
        {
            maxHorizontalSpeed = param.maxHorizontalSpeed;
            accelHorizontal = param.accelHorizontal;
            accelHorizontal_Air = param.accelHorizontalInAir;
            frictionHorizontal =  param.frictionHorizontal;
            frictionHorizontal_Air = param.frictionHorizontalInAir;
            jumpHeight = param.jumpHeight;
            jumpCooldown = param.jumpCooldown;
            gravity = param.gravity;
            coyoteTimeDuration = param.coyoteTimeDuration;
            groundCheckCenter = param.groundCheckCenter;
            groundCheckRange = param.groundCheckRange;
            trackSwitchingSpeed = param.trackSwitchingSpeed;
            reAccelRate = param.reAccelRate;
        }

        void Awake()
        {
            Application.targetFrameRate = 60;
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<BallInputHandler>();
            inputHandler.enabled = false;
            inputHandler.MoveHandler = val => { inputDirection = val; };
            inputHandler.JumpHandler = () => { isJumping = isInJumpWindow & isJumpReady; };
            inputHandler.enabled = true;
            if (ControlMode == ControlMode.Tracks)
            {
                for (int i = 0; i < tracks.childCount; i++)
                    trackList.Add(tracks.GetChild(i).GetComponent<SplineContainer>());
                currentTrack = (trackList.Count - 1) / 2;
            }
        }

        void Start()
        {
            forwardDirection = forwardIndicator.forward;
            ReadMovementParam(movementParam);
            transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            targetSpeed = initialSpeed;
            rb.linearVelocity = transform.forward * initialSpeed;
            isJumpReady = true;
            isMoveReset = true;
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
            if (isJumping)
            {
                Y = Mathf.Sqrt(2 * gravity * jumpHeight);
                isJumping = false;
                isJumpReady = false;
                StartCoroutine(JumpCooldown());
            }
            Y -= gravity * Time.fixedDeltaTime;

            if (ControlMode == ControlMode.Smooth)
            {
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
            }
            else if (ControlMode == ControlMode.Tracks)
            {
                if (Mathf.Abs(inputDirection.x) > 0.1f)
                {
                    if (isMoveReset)
                    {
                        currentTrack = Math.Clamp(currentTrack + Math.Sign(inputDirection.x), 0, trackList.Count - 1);
                        isMoveReset = false;
                        Debug.Log($"SetTrack: {currentTrack}");
                    }
                }
                else
                    isMoveReset = true;
                targetPosition = CalcPosition(trackList[currentTrack], transform.position, forwardDirection);
                X = CalcSpeed();
            }
            targetSpeed = Mathf.Min(targetSpeed + accelForward * Time.fixedDeltaTime, maxSpeed);
            Z = Mathf.Lerp(Z, targetSpeed, Time.fixedDeltaTime * reAccelRate);
            rb.linearVelocity = X * transform.right+ Y * transform.up + Z * forwardDirection;

            ball.rotation *= Quaternion.Euler(Z / (2*Mathf.PI) * 360 * Time.fixedDeltaTime, 0, 0);
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
            yield return new WaitForSeconds(jumpCooldown);
            isJumpReady = true;
        }

        bool GroundCheck()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + groundCheckCenter, groundCheckRange, LayerMask.GetMask("Ground"));
            return colliders.Length > 0;
        }

        Vector3 CalcPosition(SplineContainer spline, Vector3 currentPosition, Vector3 direction)
        {
            float l = 0, r = 1f, mid = 0.5f;
            Vector3 p = spline.EvaluatePosition(mid);
            float eps = 0.001f;
            while (r - l > eps)
            {
                if (Vector3.Dot(p - currentPosition, direction) < 0)
                    l = mid;
                else
                    r = mid;
                mid = (l + r) / 2;
                p =  spline.EvaluatePosition(mid);
            }
            return p;
        }

        float CalcSpeed()
        {
            float targetX = Vector3.Dot(targetPosition - transform.position, transform.right);
            float speed = Mathf.Abs(targetX) < 0.01f? 0: Mathf.Max(Mathf.Abs(targetX * trackSwitchingSpeed), 0.1f) * Mathf.Sign(targetX);
            return speed;
        }

        public void Reset()
        {
            targetSpeed = initialSpeed;
            rb.linearVelocity = new Vector3(0, 0, 0);
            transform.position = spawnPoint.position;
            // cam.GetComponent<CinemachinePositionComposer>()
        }

        public void ResetSpeed()
        {
            targetSpeed = initialSpeed;
            accelForward = 0.1f;
        }

        #endregion

        public void NotifySwitchPhase(PhaseData data)
        {
            if (data.phaseID == currentPhase)
                return;
            currentPhase = data.phaseID;
            if (ControlMode == ControlMode.Tracks)
            {
                trackList.Clear();
                for (int i = 0; i < data.TrackNumber + 2; i++)
                    trackList.Add(tracks.GetChild(i).GetComponent<SplineContainer>());
                currentTrack = (trackList.Count - 1) / 2;
            }

            maxSpeed = data.MaxSpeed;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + groundCheckCenter, groundCheckRange);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + forwardDirection);
        }
    }
}
