using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileWeight
{
    public TileType type;
    public float weight;
}

[Serializable]
public class PhaseConfig
{
    [Tooltip("Phase Start Time")]
    public float startTime;

    [Tooltip("Tile Weights in the Pase")]
    public List<TileWeight> weights = new();
}

public class TimePhaseTileManager : MonoBehaviour
{
    [Header("Phases (sorted by startTime asc)")]
    public List<PhaseConfig> phases = new();

    [Header("Runtime")]
    public float elapsed;

    public event Action<int> OnPhaseChanged;
    private int lastPhaseIndex = -1;

    public int CurrentPhaseIndex { get; private set; } = 0;

    void Awake()
    {
        // Sort by time
        phases.Sort((a, b) => a.startTime.CompareTo(b.startTime));
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Check the current phase. If elapsed is bigger than the current phase time zone, move to next phase
        int phaseindex = 0;
        for (int i = 0; i < phases.Count; i++)
        {
            if (elapsed >= phases[i].startTime) phaseindex = i;
            else break;
        }
        CurrentPhaseIndex = phaseindex;

        if (CurrentPhaseIndex != lastPhaseIndex)
        {
            lastPhaseIndex = CurrentPhaseIndex;
            OnPhaseChanged?.Invoke(CurrentPhaseIndex);
        }
    }

    public PhaseConfig GetCurrentPhase()
    {
        if (phases == null || phases.Count == 0) return null;
        return phases[Mathf.Clamp(CurrentPhaseIndex, 0, phases.Count - 1)];
    }

    public bool TryGetWeight(TileType type, out float w)
    {
        w = 0f;
        var phase = GetCurrentPhase();
        if (phase == null) return false;

        for (int i = 0; i < phase.weights.Count; i++)
        {
            if (phase.weights[i].type == type)
            {
                w = phase.weights[i].weight;
                return true;
            }
        }
        return false;
    }
}
