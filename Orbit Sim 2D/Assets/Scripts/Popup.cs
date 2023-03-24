using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour {
    public enum POPUP_TYPE { PLANET, ORBIT}
    private TMP_Text selectedName;
    private TMP_Text orbitingPlanet;
    private TMP_Text info;
    public POPUP_TYPE popupType;
    public Orbit orbitScript;
    public Planet planetScript;
    public string popupID;

    void Start() {
        selectedName = transform.Find("SelectNamePanel").Find("SelectName").GetComponent<TMP_Text>();
        info = transform.Find("OrbitInfo").GetComponent<TMP_Text>();
        switch (popupType) {
            case POPUP_TYPE.PLANET:
                selectedName.text = planetScript.name + " Planet Info";
                break;
            case POPUP_TYPE.ORBIT:
                selectedName.text = orbitScript.name + " Orbit Info";
                break;
        }
    }

    void Update() {
        int sec = (int)TimeKeeper.instance.time;
        int days = sec / (int)planetScript.secInPlanetDay;
        sec = sec % (int)planetScript.secInPlanetDay;
        int hours = sec / 3600;
        sec = sec % 3600;
        int mins = sec / 60;
        sec = sec % 60;
        string time = "Time: " + days + "d : " + hours + "h : " + mins + "m : " + sec + "s\n";

        string infoText = "";
        if (popupType == POPUP_TYPE.ORBIT) {
            infoText = UpdateOrbitInfo(time);
        } else if(popupType == POPUP_TYPE.PLANET) {
            infoText = UpdatePlanetInfo(time);
        }
        info.text = infoText;
    }

    public void ClosePopup() {
        OrbitManager.instance.RemovePopup(this);
        Destroy(this.gameObject);
    }

    private string UpdateOrbitInfo(string time) {
        string infoText = "";
        infoText += "Orbiting " + planetScript.name + "\n";
        infoText += time;
        string rt = String.Format("{0:#,##0.00}", orbitScript.GetR() / Globals.KM_TO_SCALE);
        infoText += "r: " + rt + " km\n";
        float ta = orbitScript.GetTrueAnom();
        if (ta < 0) {
            ta += 2.0f * Mathf.PI;
        }
        ta *= Globals.RAD_TO_DEG;
        string tat = String.Format("{0:#,##0.00}", ta);
        infoText += "\u03B8: " + tat + "°\n";
        string vt = String.Format("{0:#,##0.00}", orbitScript.GetVelo() / Globals.KM_TO_SCALE);
        infoText += "V: " + vt + " km/s";
        return infoText;
    }

    private string UpdatePlanetInfo(string time) {
        string infoText = "";
        infoText += time;
        string m = String.Format("{0:#.#####E+00}", planetScript.GetMass());
        infoText += "Mass: " + m + " kg\n";
        float r = planetScript.radius / Globals.KM_TO_SCALE;
        string a = String.Format("{0:#,##0.000}", planetScript.gravParameter / Mathf.Pow(Globals.KM_TO_SCALE, 3) / Mathf.Pow(r, 2) * Globals.KM_TO_M);
        infoText += "Grav Accel: " + a + " m/s^2\n";
        string rt = String.Format("{0:#,##0.00}", planetScript.radius / Globals.KM_TO_SCALE);
        infoText += "Radius: " + rt + " km\n";
        return infoText;
    }
}