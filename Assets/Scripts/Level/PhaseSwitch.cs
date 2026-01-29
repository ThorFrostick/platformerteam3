using System;
using Ball;
using UnityEngine;

public class PhaseSwitch : MonoBehaviour
{
    public PhaseData data;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<BallController>().NotifySwitchPhase(data);
        }
    }
    
}

[Serializable]
public struct PhaseData
{
    public string phaseID;
    public int TrackNumber;
    public float MaxSpeed;
}
