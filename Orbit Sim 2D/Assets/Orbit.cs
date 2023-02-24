using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private GameObject planet;

    [SerializeField] private TMP_InputField rpInput;
    [SerializeField] private TMP_InputField raInput;
    [SerializeField] private TMP_InputField littleOmegaInput;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text rText;
    [SerializeField] private TMP_Text trueAnomText;
    [SerializeField] private TMP_Text veloText;

    [SerializeField] private float tolerance = 0.00001f;
    [SerializeField] private int numTries = 10;
    [SerializeField] private int resolution = 10000;

    [SerializeField] private float littleOmega = 0.0f;

    public float ra = 2;
    public float rp = 1;
    private Planet planetScript;

    private float a;
    private float b;
    private float energy;
    private float e;
    private float trueAnom;
    private float theta;
    private float eccAnom;
    private float r;
    private float velo;
    private float n;
    private float period;
    private float calcTime = 0.0f;
    private Vector2 posFromPlanet = new Vector2(0.0f, 0.0f);
    private Vector3[] ellipsePositions;

    private LineRenderer orbitLine;

    // Start is called before the first frame update
    void Start()
    {
        planetScript = planet.GetComponent<Planet>();
        trueAnom = 0.0f;
        littleOmega *= Globals.DEG_TO_RAD;
        theta = littleOmega + trueAnom;
        SolveRaRp();
        orbitLine = transform.Find("OrbitLineRenderer").GetComponent<LineRenderer>();
        UpdateOrbitLine();
    }

    private void UpdateOrbitLine() {
        ellipsePositions = null;
        ellipsePositions = CreateEllipse(resolution);
        orbitLine.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++) {
            orbitLine.SetPosition(i, ellipsePositions[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Globals.time += Time.deltaTime * Globals.timeMultiplier;
        SolveKepler();
        CalculateR();
        CalculateVelo();
        UpdateSatInfoText();
        posFromPlanet.x = r * Mathf.Cos(theta);
        posFromPlanet.y = r * Mathf.Sin(theta);
        transform.position = new Vector3(posFromPlanet.x + planet.transform.position.x, posFromPlanet.y + planet.transform.position.y, 0.0f);
    }

    private void UpdateSatInfoText() {
        int sec = (int)Globals.time;
        int days = sec / (int)Globals.SEC_IN_DAY;
        sec = sec % (int)Globals.SEC_IN_DAY;
        int hours = sec / 3600;
        sec = sec % 3600;
        int mins = sec / 60;
        sec = sec % 60;

        timeText.text = days + "d : " + hours + "h : " + mins + "m : " + sec + "s";
        string rt = String.Format("{0:#,###.##}", r / Globals.KM_TO_SCALE);
        rText.text = rt + " km";
        float ta = trueAnom;
        if (ta < 0) {
            ta += 2.0f * Mathf.PI;
        }
        ta *= Globals.RAD_TO_DEG;
        string tat = String.Format("{0:#,###.##}", ta);
        trueAnomText.text =  tat + "°";
        string vt = String.Format("{0:#,###.##}", velo / Globals.KM_TO_SCALE);
        veloText.text = vt + " km/s";
    }

    private void SolveRaRp() {
        a = (ra + rp) / 2.0f;
        energy = -1.0f * planetScript.gravParameter / (2.0f * a);
        e = (ra - rp) / (ra + rp);
        b = a * Mathf.Sqrt(1.0f - Mathf.Pow(e, 2.0f));
        CalculateR();
        CalculateVelo();
        n = Mathf.Sqrt(planetScript.gravParameter/Mathf.Pow(a, 3.0f));
        period = 2.0f * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3.0f) / planetScript.gravParameter);
        // period = 2.0f * Mathf.PI / Mathf.Sqrt(Mathf.Sqrt(planetScript.gravParameter))*Mathf.Sqrt(Mathf.Pow(r, 3.0f)); // big oops
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
        calcTime = Globals.time % period;
        float answer = n * calcTime;
        float check = eccAnom - e * Mathf.Sin(eccAnom);
        int i = 0;
        while (Mathf.Abs(answer-check) > tolerance && i < numTries) {
            eccAnom = eccAnom - (eccAnom - e * Mathf.Sin(eccAnom) - n * calcTime) / (1 - e * Mathf.Cos(eccAnom));
            check = eccAnom - e * Mathf.Sin(eccAnom);
            i++;
        }
        if (eccAnom > 2.0f * Mathf.PI) {
            eccAnom -= 2.0f * Mathf.PI;
        }
        trueAnom = EccentricAnomToTrueAnom(eccAnom);
        theta = littleOmega + trueAnom;
    }

    private void CalculateR() {
        r = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(trueAnom));
    }

    private void CalculateVelo() {
        velo = Mathf.Sqrt(2.0f * energy + (2.0f * planetScript.gravParameter) / r);
    }

    Vector3[] CreateEllipse(int resolution) {

        Vector3[] positions = new Vector3[resolution + 1];
        Vector3 center = new Vector3(planet.transform.position.x, planet.transform.position.y, 0.0f);

        for (int i = 0; i <= resolution; i++) {
            float angle = (float)i / resolution * 2.0f * Mathf.PI;
            float rEllipse = a * (1.0f - Mathf.Pow(e, 2.0f)) / (1.0f + e * Mathf.Cos(angle));
            float xEllipse = rEllipse * Mathf.Cos(angle + littleOmega);
            float yEllipse = rEllipse * Mathf.Sin(angle + littleOmega);
            positions[i] = new Vector3(xEllipse + planet.transform.position.x, yEllipse + planet.transform.position.y, 0.0f);
        }

        return positions;
    }

    public void UpdateOrbit() {
        try {
            ra = float.Parse(raInput.text) * Globals.KM_TO_SCALE;
            rp = float.Parse(rpInput.text) * Globals.KM_TO_SCALE;
            littleOmega = float.Parse(littleOmegaInput.text) * Globals.DEG_TO_RAD;
        } catch (FormatException exception) {
            Debug.Log(exception.ToString());
            return;
        }
        SolveRaRp();
        UpdateOrbitLine();
    }
}
