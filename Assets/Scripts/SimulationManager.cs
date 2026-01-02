using UnityEngine;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    public enum OrbitType { LEO, MEO, GEO }

    public const float LEO_MIN = 200.0f;
    public const float LEO_MAX = 3000.0f;
    public const float MEO_MIN = 5000.0f;
    public const float MEO_MAX = 15000.0f;
    public const float GEO_ALT = 35786.0f;

    [Header("Settings")]
    public GameObject satellitePrefab;

    public static List<SatelliteController> ActiveSatellites = new List<SatelliteController>();
    public int leoCount = 32;
    public int meoCount = 15;
    public int geoCount = 4;

    [Header("Time Speed")]
    public float timeScale = 100f; // 1 second in real life = 100 seconds in sim
    public static float simulationTimeScale;

    void Start()
    {
        SpawnOptimalOrbit(leoCount, OrbitType.LEO);
        SpawnOptimalOrbit(meoCount, OrbitType.MEO);
        SpawnOptimalOrbit(geoCount, OrbitType.GEO);
    }

    void Update()
    {
        simulationTimeScale = timeScale;
    }

    public void SpawnOptimalOrbit(int totalCount, OrbitType type)
    {
        float minAlt, maxAlt, inclination;

        GameObject parent;

        switch (type)
        {
            case OrbitType.LEO:
                minAlt = LEO_MIN; maxAlt = LEO_MAX; inclination = 53f; 
                parent = new GameObject("LEOs");
                break;
            case OrbitType.MEO:
                minAlt = MEO_MIN; maxAlt = MEO_MAX; inclination = 55f; 
                parent = new GameObject("MEOs");
                break;
            case OrbitType.GEO:
                minAlt = GEO_ALT; maxAlt = GEO_ALT; inclination = 0f; 
                parent = new GameObject("GEOs");
                break;
            default: return;
        }

        // Walker constellation: divide into evenly spaced planes
        int numPlanes = Mathf.CeilToInt(Mathf.Sqrt(totalCount));
        if (type == OrbitType.GEO) numPlanes = 1;

        int satsPerPlane = Mathf.CeilToInt((float)totalCount / numPlanes);
        int spawnedSoFar = 0;

        for (int p = 0; p < numPlanes; p++)
        {
            float raan = (360f / numPlanes) * p;

            for (int s = 0; s < satsPerPlane; s++)
            {
                if (spawnedSoFar >= totalCount) break;

                float altitude = Random.Range(minAlt, maxAlt);
                float phaseShift = (360f / totalCount) * p; 
                float currentAngle = ((360f / satsPerPlane) * s) + phaseShift;

                Vector3 spawnPos = OrbitUtils.GetPosition(altitude, inclination, raan, currentAngle);
                spawnPos.x *= 0.001f;
                spawnPos.y *= 0.001f;
                spawnPos.z *= 0.001f;
                
                GameObject satObj = Instantiate(satellitePrefab, spawnPos, Quaternion.identity, parent.transform);
                SatelliteController controller = satObj.GetComponent<SatelliteController>();
                
                controller.altitude = altitude;
                controller.inclination = inclination;
                controller.raan = raan;
                controller.initialAngle = currentAngle;

                spawnedSoFar++;
                ActiveSatellites.Add(controller);
            }
        }
    }
}