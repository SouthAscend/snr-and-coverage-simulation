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

    void Start()
    {
        orbitalPeriod = OrbitUtils.GetOrbitalPeriod(altitude);
        startTime = Time.time;
    }

    void Update() {
        simulatedTime += Time.deltaTime * SimulationManager.simulationTimeScale;
        float currentAngle = initialAngle + (simulatedTime / orbitalPeriod) * 360f;

        Vector3 newPosition = OrbitUtils.GetPosition(altitude, inclination, raan, currentAngle);
        newPosition.x *= 0.001f;
        newPosition.y *= 0.001f;
        newPosition.z *= 0.001f;

        transform.position = newPosition;
        transform.LookAt(Vector3.zero);
    }
}