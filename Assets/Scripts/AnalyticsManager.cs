using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AnalyticsManager : MonoBehaviour
{
    private GroundStationProbe[] manualProbes;
    private bool probesFound = false;

    [Header("Update Frequency")]
    [Tooltip("How often to log the summary (in frames)")]
    public int logFrequency = 120;

    void Start()
    {
        // Small delay to let probes initialize
        Invoke("CacheManualProbes", 0.5f);
    }

    void CacheManualProbes()
    {
        // Find all ground stations once
        manualProbes = Object.FindObjectsByType<GroundStationProbe>(FindObjectsSortMode.None);
        
        if (manualProbes.Length > 0)
        {
            probesFound = true;
            Debug.Log($"<color=cyan>AnalyticsManager:</color> Successfully cached {manualProbes.Length} ground stations.");
        }
        else
        {
            Debug.LogWarning("AnalyticsManager: No GroundStationProbe objects found in the scene.");
        }
    }

    void Update()
    {
        if (!probesFound) return;

        int coveredCount = 0;
        float totalSnR = 0f;
        int activeLinkCount = 0;

        for (int i = 0; i < manualProbes.Length; i++)
        {
            if (manualProbes[i].isCovered)
            {
                coveredCount++;
                totalSnR += manualProbes[i].bestSnR;
                activeLinkCount++;
            }
        }

        float coverageRate = ((float)coveredCount / manualProbes.Length) * 100f;
        float avgSnR = activeLinkCount > 0 ? totalSnR / activeLinkCount : 0f;

        // Log periodically
        if (Time.frameCount % logFrequency == 0)
        {
            DisplaySummary(coverageRate, avgSnR, manualProbes.Length, coveredCount);
        }
    }

    void DisplaySummary(float rate, float snr, int total, int covered)
    {
        Debug.Log($"<b>--- CONSTELLATION PERFORMANCE ---</b>\n");
        Debug.Log($"Total Test Points: {total}\n");
        Debug.Log($"Points Covered: {covered}\n");
        Debug.Log($"Coverage Rate: {rate:F1}%\n");
        Debug.Log($"Average SnR: {snr:F2} dB\n");
        Debug.Log($"Active Satellites: {SimulationManager.ActiveSatellites.Count}");
    }
}