using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float gravParameter = 398600.0f; // km^3 * s^-2
    public float radius = 6378.1f; // km
    [SerializeField] public float secInPlanetDay = 86400.0f;

    private float timeOfPlanetDay = 0.0f;

    private void Awake() {
        gravParameter *= Mathf.Pow(Globals.KM_TO_SCALE, 3);
        radius *= Globals.KM_TO_SCALE;
    }

    // Start is called before the first frame update

    void Start()
    {
        transform.localScale = new Vector2(radius * 2.0f, radius * 2.0f);
    }

    void Update() {
        timeOfPlanetDay = TimeKeeper.instance.time % secInPlanetDay;
        float rotationRad = (timeOfPlanetDay * 360.0f) / secInPlanetDay;
        var rotation = Quaternion.AngleAxis(rotationRad, Vector3.forward);
        transform.rotation = rotation;
    }
}
