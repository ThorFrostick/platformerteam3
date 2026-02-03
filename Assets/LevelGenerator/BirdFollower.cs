using UnityEngine;

public class BirdPhaseFollower : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public TimePhaseTileManager phaseManager;

    [Header("Phase Trigger")]
    public int startPhaseIndex = 2;

    [Header("Follow Offset (relative to player location)")]
    public float forwardDistance = 6f;
    public float height = 4f;
    public float sideAmplitude = 3f;

    [Header("Motion")]
    public float sideFrequencyHz = 0.6f; // left right
    public float bobAmplitude = 0.3f;    // bob top down
    public float bobFrequencyHz = 1.2f;

    [Header("Smoothing")]
    public float followSmooth = 8f;  
    public float rotateSmooth = 10f;

    private bool active = false;
    private float t0;

    void OnEnable()
    {
        if (phaseManager != null)
            phaseManager.OnPhaseChanged += HandlePhaseChanged;
    }

    void OnDisable()
    {
        if (phaseManager != null)
            phaseManager.OnPhaseChanged -= HandlePhaseChanged;
    }

    void Start()
    {
        if (phaseManager != null && phaseManager.CurrentPhaseIndex >= startPhaseIndex)
            Activate();
    }

    void HandlePhaseChanged(int idx)
    {
        if (!active && idx >= startPhaseIndex)
            Activate();
    }

    void Activate()
    {
        active = true;
        t0 = Time.time;
    }

    void LateUpdate()
    {
        if (!active) return;
        if (player == null) return;

        float t = Time.time - t0;

        // Set to the same direction as the player
        Vector3 forward = player.forward;
        Vector3 right = player.right;

        float side = Mathf.Sin(t * Mathf.PI * 2f * sideFrequencyHz) * sideAmplitude;
        float bob = Mathf.Sin(t * Mathf.PI * 2f * bobFrequencyHz) * bobAmplitude;

        Vector3 targetPos =
            player.position
            + forward * forwardDistance
            + Vector3.up * (height + bob)
            + right * side;

        // Make moving more smooth
        transform.position = Vector3.Lerp(transform.position, targetPos, 1f - Mathf.Exp(-followSmooth * Time.deltaTime));

        Vector3 lookDir = (targetPos - transform.position);
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-rotateSmooth * Time.deltaTime));
        }
    }
}
