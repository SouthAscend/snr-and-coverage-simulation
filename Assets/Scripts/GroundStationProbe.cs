using UnityEngine;

public class GroundStationProbe : MonoBehaviour
{
    public float maskAngle = 10f;
    public float txPower_dBm = 40f;

    public bool isCovered;
    public float bestSnR;
    
    private Vector3 meshWorldPos;

    void Start()
    {
        Transform mesh = transform.Find("Mesh");
        meshWorldPos = mesh != null ? mesh.position : transform.position;
        meshWorldPos.x *= 1000.0f;
        meshWorldPos.y *= 1000.0f;
        meshWorldPos.z *= 1000.0f;
    }

    void Update()
    {
        int count = 0;
        float maxSnR = -999f;
        bool hasSignal = false;

        foreach (var sat in SimulationManager.ActiveSatellites)
        {
            Vector3 satPos = new Vector3(sat.transform.position.x * 1000.0f, sat.transform.position.y * 1000.0f, sat.transform.position.z * 1000.0f);
            float elev = OrbitUtils.GetElevationAngle(meshWorldPos, satPos);

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

        Vector3 drawPos = new Vector3(meshWorldPos.x / 1000.0f, meshWorldPos.y / 1000.0f, meshWorldPos.z / 1000.0f);
        Debug.DrawRay(drawPos, drawPos.normalized * 0.5f, isCovered ? Color.green : Color.red);
    }

    float CalculateSnR(float dist, float elev)
    {
        float pathLoss = 20 * Mathf.Log10(dist + 1.0f);
        float atmPenalty = Mathf.Lerp(10f, 0f, elev / 90f);
        return txPower_dBm - pathLoss - atmPenalty;
    }
}