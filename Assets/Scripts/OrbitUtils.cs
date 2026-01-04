using UnityEngine;

public static class OrbitUtils {
    public const float EARTH_RADIUS = 6371.0f; 
    public const float MU = 398600.44f; 

    // Orbital period using Kepler's 3rd law
    public static float GetOrbitalPeriod(float altitude) {
        float a = EARTH_RADIUS + altitude;
        return 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3) / MU);
    }

    // Get satellite position from orbital parameters
    public static Vector3 GetPosition(float altitude, float inclination, float raan, float currentAngle) {
        float r = EARTH_RADIUS + altitude;
        
        // Position in orbital plane
        float x = r * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float z = r * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector3 flatPosition = new Vector3(x, 0, z);

        // Apply inclination and RAAN rotations
        Quaternion tilt = Quaternion.Euler(inclination, 0, 0);
        Quaternion spin = Quaternion.Euler(0, raan, 0);

        return spin * (tilt * flatPosition);
    }

    // Calculate elevation angle for signal strength
    public static float GetElevationAngle(Vector3 groundPoint, Vector3 satellitePos) {
        Vector3 upAtGroundPoint = groundPoint.normalized;
        Vector3 vectorToSatellite = (satellitePos - groundPoint).normalized;
        
        float angleFromZenith = Vector3.Angle(upAtGroundPoint, vectorToSatellite);
        return 90f - angleFromZenith;
    }
}