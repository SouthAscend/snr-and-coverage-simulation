using UnityEngine;
using TMPro; // Required for UI Text
using System.Linq;

public class AnalyticsManager : MonoBehaviour
{
    private GroundStationProbe[] manualProbes;
    private bool probesFound = false;

    [Header("UI Reference")]
    public TMP_Text statsText;

    void Start()
    {
        // Small delay to let probes and SimulationManager initialize
        Invoke("CacheManualProbes", 0.5f);
    }

    void CacheManualProbes()
    {
        manualProbes = Object.FindObjectsByType<GroundStationProbe>(FindObjectsSortMode.None);
        
        if (manualProbes.Length > 0)
        {
            probesFound = true;
        }
        else
        {
            Debug.LogWarning("AnalyticsManager: No ground stations found to track.");
        }
    }

    void Update()
    {
        if (!probesFound || statsText == null) return;

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

        UpdateUI(coverageRate, avgSnR, manualProbes.Length, coveredCount);
    }

    void UpdateUI(float rate, float snr, int total, int covered)
    {
        statsText.text = $"<b>CONSTELLATION PERFORMANCE</b>\n" +
                         $"Coverage Rate: {rate:F1}%\n" +
                         $"Avg SnR: {snr:F2} dB\n" +
                         $"Points: {covered}/{total}\n" +
                         $"Satellites: {SimulationManager.ActiveSatellites.Count}";
    }
}