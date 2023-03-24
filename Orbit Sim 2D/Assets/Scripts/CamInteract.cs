using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CamInteract : MonoBehaviour {
    [SerializeField] private GameObject popup;
    [SerializeField] private TMP_Text selectedName;
    [SerializeField] private TMP_Text orbitingPlanet;
    [SerializeField] private TMP_Text orbitInfo;
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject startingSatellite;
    [SerializeField] private GameObject startingPlanet;
    private Planet planetScript;
    private Orbit orbitScript;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        orbitScript = startingSatellite.GetComponent<Orbit>();
        planetScript = startingPlanet.GetComponent<Planet>();
        orbitingPlanet.text = "Orbiting " + planetScript.name;
        selectedName.text = orbitScript.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (rayHit && rayHit.transform.GetComponent<Orbit>() != null) {
                orbitScript = rayHit.transform.GetComponent<Orbit>();
                planetScript = orbitScript.GetPlanetScript();
                OrbitManager.instance.SelectOrbit(orbitScript);
                OrbitManager.instance.SelectPlanet(planetScript);
                orbitingPlanet.text = "Orbiting " + planetScript.name;
                selectedName.text = orbitScript.name;

                GameObject newPrefab = Instantiate(popupPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                newPrefab.transform.parent = canvas.transform;
                newPrefab.transform.localPosition = new Vector3(0, 0, 0);
                newPrefab.transform.localScale = new Vector3(0.4326068f, 0.4326068f, 0.4326068f);
                OrbitManager.instance.AddPopup(newPrefab);
            } else if(rayHit/* && rayHit.transform.CompareTag("PopupDragger")*/) {
                Debug.Log("Here");
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

        timeText.text = "Time: " + days + "d : " + hours + "h : " + mins + "m : " + sec + "s";
        string orbitInfoText = "";
        string rt = String.Format("{0:#,##0.00}", orbitScript.GetR() / Globals.KM_TO_SCALE);
        orbitInfoText += "r: " + rt + " km\n";
        float ta = orbitScript.GetTrueAnom();
        if (ta < 0) {
            ta += 2.0f * Mathf.PI;
        }
        ta *= Globals.RAD_TO_DEG;
        string tat = String.Format("{0:#,##0.00}", ta);
        orbitInfoText += "\u03B8: " + tat + "°\n";
        string vt = String.Format("{0:#,##0.00}", orbitScript.GetVelo() / Globals.KM_TO_SCALE);
        orbitInfoText += "V: " + vt + " km/s";
        orbitInfo.text = orbitInfoText;
    }
}
