using UnityEngine;

public class GroundStationProbe : MonoBehaviour
{
    [Header("Constellation Settings")]
    public float maskAngle = 10f;
    public float eirp_dBm = 40f;

    [Header("Link Budget Settings")]
    public float frequency_MHz = 12000f;  
    public float bandwidth_Hz = 20e6f;     
    public float noiseFigure_dB = 3f;       

    [Header("Live Data")]
    public bool isCovered;
    public float bestSnR;
    
    private Vector3 meshWorldPos;
    private MeshRenderer cubeRenderer;

    void Start()
    {
        Transform meshTransform = transform.Find("Mesh");
        Vector3 worldPosUnity = meshTransform != null ? meshTransform.position : transform.position;

        Vector3 dir = (worldPosUnity - Vector3.zero).normalized;

        meshWorldPos = dir * OrbitUtils.EARTH_RADIUS;

        Transform cubeTransform = transform.Find("Mesh/Cube");
        if (cubeTransform != null)
        {
            cubeRenderer = cubeTransform.GetComponent<MeshRenderer>();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Could not find Mesh/Cube path!");
        }
    }

    void Update()
    {
        if (cubeRenderer == null) return;

        int count = 0;
        float maxSnR = -999f;
        bool hasSignal = false;

        // Iteration through the global registry
        foreach (var sat in SimulationManager.ActiveSatellites)
        {
            if (!sat.isOnline) continue;
            Vector3 satPos_km = sat.transform.position * 1000.0f;

            float elev = OrbitUtils.GetElevationAngle(meshWorldPos, satPos_km);

            if (elev >= maskAngle)
            {
                hasSignal = true;
                count++;

                float dist_km = Vector3.Distance(meshWorldPos, satPos_km);
                float snr = CalculateSnR(dist_km, elev);
                if (snr > maxSnR) maxSnR = snr;
            }

        }

        isCovered = hasSignal;
        bestSnR = hasSignal ? maxSnR : 0f;

        cubeRenderer.sharedMaterial = isCovered ? SimulationManager.greenMaterialProbe : SimulationManager.redMaterialProbe;
    }

    float CalculateSnR(float dist_km, float elev_deg)
    {
        dist_km = Mathf.Max(dist_km, 0.001f);

        float fspl_dB = 32.44f + 20f * Mathf.Log10(dist_km) + 20f * Mathf.Log10(Mathf.Max(frequency_MHz, 0.001f));

        float atmPenalty_dB = Mathf.Lerp(10f, 0f, Mathf.Clamp01(elev_deg / 90f));

        float pr_dBm = eirp_dBm - fspl_dB - atmPenalty_dB;

        float noise_dBm = -174f + 10f * Mathf.Log10(Mathf.Max(bandwidth_Hz, 1f)) + noiseFigure_dB;
    
        return pr_dBm - noise_dBm;
    }

}