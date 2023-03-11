using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CamInteract : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text rText;
    [SerializeField] private TMP_Text trueAnomText;
    [SerializeField] private TMP_Text veloText;

    [SerializeField] private GameObject startingSatellite;
    [SerializeField] private GameObject startingPlanet;
    private Planet planetScript;
    private Orbit orbitScript;

    // Start is called before the first frame update
    void Start()
    {
        orbitScript = startingSatellite.GetComponent<Orbit>();
        planetScript = startingPlanet.GetComponent<Planet>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (rayHit && rayHit.transform.GetComponent<Orbit>() != null) {
                orbitScript = rayHit.transform.GetComponent<Orbit>();
                planetScript = orbitScript.getPlanetScript();
            }
        }

        UpdateInfoText();
    }

    private void UpdateInfoText() {
        int sec = (int)TimeKeeper.instance.time;
        int days = sec / (int)planetScript.secInPlanetDay;
        sec = sec % (int)planetScript.secInPlanetDay;
        int hours = sec / 3600;
        sec = sec % 3600;
        int mins = sec / 60;
        sec = sec % 60;

        timeText.text = days + "d : " + hours + "h : " + mins + "m : " + sec + "s";
        string rt = String.Format("{0:#,###.##}", orbitScript.getR() / Globals.KM_TO_SCALE);
        rText.text = rt + " km";
        float ta = orbitScript.getTrueAnom();
        if (ta < 0) {
            ta += 2.0f * Mathf.PI;
        }
        ta *= Globals.RAD_TO_DEG;
        string tat = String.Format("{0:#,###.##}", ta);
        trueAnomText.text = tat + "°";
        string vt = String.Format("{0:#,###.##}", orbitScript.getVelo() / Globals.KM_TO_SCALE);
        veloText.text = vt + " km/s";
    }
}
