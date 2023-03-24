using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour {
    private TMP_Text selectedName;
    private TMP_Text orbitingPlanet;
    private TMP_Text orbitInfo;
    public Orbit orbitScript;
    public Planet planetScript;

    void Start() {
        selectedName = transform.Find("SelectNamePanel").Find("SelectName").GetComponent<TMP_Text>();
        orbitingPlanet = transform.Find("OrbitingPlanet").GetComponent<TMP_Text>();
        orbitInfo = transform.Find("OrbitInfo").GetComponent<TMP_Text>();
        orbitingPlanet.text = "Orbiting " + planetScript.name;
        selectedName.text = orbitScript.name;
    }

    void Update() {
        int sec = (int)TimeKeeper.instance.time;
        int days = sec / (int)planetScript.secInPlanetDay;
        sec = sec % (int)planetScript.secInPlanetDay;
        int hours = sec / 3600;
        sec = sec % 3600;
        int mins = sec / 60;
        sec = sec % 60;

        string orbitInfoText = "Time: " + days + "d : " + hours + "h : " + mins + "m : " + sec + "s\n";
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

    public void ClosePopup() {
        OrbitManager.instance.RemovePopup(this);
        Destroy(this.gameObject);
    }
}