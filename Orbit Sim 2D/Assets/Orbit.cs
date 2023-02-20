using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private GameObject planet;
    [SerializeField] private float tolerance = 0.00001f;
    [SerializeField] private int numTries = 10;
    [SerializeField] private int resolution = 10000;
    public float timeMultiplier = 100.0f;
    public float ra = 2;
    public float rp = 1;
    private Planet planetScript;

    private float a;
    private float b;
    private float energy;
    private float e;
    private float trueAnom;
    private float eccAnom;
    private float r;
    private float velo;
    private float n;
    private float period;
    private float time = 0.0f;
    private Vector2 posFromPlanet = new Vector2(0.0f, 0.0f);
    private Vector3[] positions;

    // Start is called before the first frame update
    void Start()
    {
        planetScript = planet.GetComponent<Planet>();
        trueAnom = 0.0f;
        SolveRaRp();

        positions = CreateEllipse(a, b, resolution);
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++) {
            lr.SetPosition(i, positions[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * timeMultiplier;
        SolveKepler();
        CalculateR();
        CalculateVelo();
        posFromPlanet.x = r * Mathf.Cos(trueAnom);
        posFromPlanet.y = r * Mathf.Sin(trueAnom);
        transform.position = new Vector3(posFromPlanet.x + planet.transform.position.x, posFromPlanet.y + planet.transform.position.y, 0.0f);
    }

    private void SolveRaRp() {
        a = (ra + rp) / 2.0f;
        energy = -1.0f * planetScript.gravParameter / (2.0f * a);
        e = (ra - rp) / (ra + rp);
        b = a * Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f));
        CalculateR();
        CalculateVelo();
        n = Mathf.Sqrt(planetScript.gravParameter/Mathf.Pow(a, 3.0f));
        period = 2.0f * Mathf.PI / Mathf.Sqrt(Mathf.Sqrt(planetScript.gravParameter))*Mathf.Sqrt(Mathf.Pow(r, 3.0f));
    }

    private float EccentricAnomToTrueAnom(float eAnom) {
        // Ambiguous quadrant, needs quad check
        // float ta = Mathf.Asin(Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Sin(tAnom) / (1.0f - e * Mathf.Cos(tAnom)));
        float ta = 2.0f * Mathf.Atan(Mathf.Sqrt((1.0f + e) / (1.0f - e)) * Mathf.Tan(eAnom / 2.0f));
        return ta;
    }

    private float TrueAnomToEccentricAnom(float tAnom) {
        float ea = Mathf.Asin(Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f)) * Mathf.Sin(tAnom) / (1.0f + e * Mathf.Cos(tAnom)));
        return ea;
    }

    private void SolveKepler() {
        // time = periods % time; // oops...
        time = time % period;
        float answer = n * time;
        float check = eccAnom - e * Mathf.Sin(eccAnom);
        int i = 0;
        while (Mathf.Abs(answer-check) > tolerance && i < numTries) {
            eccAnom = eccAnom - (eccAnom - e * Mathf.Sin(eccAnom) - n * time) / (1 - e * Mathf.Cos(eccAnom));
            check = eccAnom - e * Mathf.Sin(eccAnom);
            i++;
        }
        if (eccAnom > 2.0f * Mathf.PI)
            eccAnom -= 2.0f * Mathf.PI;
        trueAnom = EccentricAnomToTrueAnom(eccAnom);
    }

    private void CalculateR() {
        r = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(trueAnom));
    }

    private void CalculateVelo() {
        velo = Mathf.Sqrt(2.0f * energy + (2.0f * planetScript.gravParameter) / r);
    }

    Vector3[] CreateEllipse(float a, float b, int resolution) {

        Vector3[] positions = new Vector3[resolution + 1];
        Vector3 center = new Vector3(planet.transform.position.x, planet.transform.position.y, 0.0f);

        for (int i = 0; i <= resolution; i++) {
            float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
            positions[i] = new Vector3(a * Mathf.Cos(angle), b * Mathf.Sin(angle), 0.0f);
            positions[i] = positions[i] + center - new Vector3(a*e, 0, 0);
        }

        return positions;
    }
}
