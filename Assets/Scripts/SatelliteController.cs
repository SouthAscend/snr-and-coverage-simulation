using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    [Header("Orbital Parameters")]
    public float altitude;      // Height above Earth (km)
    public float inclination;   // Tilt of the orbit (degrees)
    public float raan;          // Rotation around Earth's axis (degrees)
    public float initialAngle;  // Where it starts in the circle (degrees)

    private float orbitalPeriod; // Time for one full orbit (seconds)
    private float startTime;
    private float simulatedTime = 0f;

    public bool isOnline = true;
    public float lambda = 0f; // failures/sec
    public float mu = 0f;     // repairs/sec

    private MeshRenderer meshRendererSphere;

    public void StepReliability(float dt)
    {
        meshRendererSphere.sharedMaterial = isOnline ? SimulationManager.greenMaterialSat : SimulationManager.redMaterialSat;
        
        if (isOnline)
        {
            float pFail = 1f - Mathf.Exp(-lambda * dt);
            if (Random.value < pFail) isOnline = false;
        }
        else
        {
            float pRepair = 1f - Mathf.Exp(-mu * dt);
            if (Random.value < pRepair) isOnline = true;
        }
    }


    void Start()
    {
        orbitalPeriod = OrbitUtils.GetOrbitalPeriod(altitude);
        startTime = Time.time;
        meshRendererSphere = transform.Find("Sphere").GetComponent<MeshRenderer>();
    }

    void Update()
    {
        simulatedTime += Time.deltaTime * SimulationManager.simulationTimeScale;
        float currentAngle = initialAngle + (simulatedTime / orbitalPeriod) * 360f;

        Vector3 newPosition_km = OrbitUtils.GetPosition(altitude, inclination, raan, currentAngle);

        transform.position = newPosition_km * 0.001f;
        transform.LookAt(Vector3.zero);

        StepReliability(Time.deltaTime);
        if (!isOnline) return;

    }
}