using UnityEngine;

public class GroundStationProbe : MonoBehaviour
{
    [Header("Constellation Settings")]
    public float maskAngle = 10f; //
    public float txPower_dBm = 40f;

    [Header("Live Data")]
    public bool isCovered;
    public float bestSnR;
    
    private Vector3 meshWorldPos;
    private MeshRenderer cubeRenderer;

    void Start()
    {
        Transform meshTransform = transform.Find("Mesh");
        meshWorldPos = meshTransform != null ? meshTransform.position : transform.position;
        
        meshWorldPos *= 1000.0f;

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
            Vector3 satPos = sat.transform.position * 1000.0f;
            float elev = OrbitUtils.GetElevationAngle(meshWorldPos, satPos);

            // Visibility check based on Mask Angle
            if (elev >= maskAngle)
            {
                hasSignal = true;
                count++;

                float dist = Vector3.Distance(meshWorldPos, satPos);
                float snr = CalculateSnR(dist, elev);
                if (snr > maxSnR) maxSnR = snr;
            }
        }

        isCovered = hasSignal;
        bestSnR = hasSignal ? maxSnR : 0f;

        cubeRenderer.sharedMaterial = isCovered ? SimulationManager.greenMaterial : SimulationManager.redMaterial;
    }

    float CalculateSnR(float dist, float elev)
    {
        // Logarithmic path loss calculation
        float pathLoss = 20 * Mathf.Log10(dist + 1.0f);
        
        // Atmospheric penalty interpolation based on elevation
        float atmPenalty = Mathf.Lerp(10f, 0f, elev / 90f);
        
        return txPower_dBm - pathLoss - atmPenalty;
    }
}