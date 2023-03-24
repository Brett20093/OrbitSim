using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float gravParameter = 398600.0f; // km^3 * s^-2
    public float radius = 6378.1f; // km
    private float mass;
    public bool isSatellite = false;
    [SerializeField] public float secInPlanetDay = 86400.0f;

    private float timeOfPlanetDay = 0.0f;

    private void Awake() {
        mass = gravParameter * Mathf.Pow(Globals.KM_TO_M, 3) / Globals.GRAV_CONST;
        gravParameter *= Mathf.Pow(Globals.KM_TO_SCALE, 3);
        radius *= Globals.KM_TO_SCALE;
    }

    // Start is called before the first frame update

    protected void Start()
    {
        transform.localScale = new Vector2(radius * 2.0f, radius * 2.0f);
        OrbitManager.instance.AddPlanet(this);
    }

    protected void Update() {
        if (!isSatellite) {
            timeOfPlanetDay = (float)TimeKeeper.instance.time % secInPlanetDay;
            double rotationRad = (timeOfPlanetDay * 360.0) / secInPlanetDay;
            var rotation = Quaternion.AngleAxis((float)rotationRad, Vector3.forward);
            transform.rotation = rotation;
        }
    }

    public float GetMass() {
        return mass;
    }
}
