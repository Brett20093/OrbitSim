using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour {
    public enum POPUP_TYPE { PLANET, ORBIT}
    [SerializeField] private TMP_Text selectedName;
    [SerializeField] private TMP_Text info;
    [SerializeField] private TMP_InputField rpInput;
    [SerializeField] private TMP_InputField raInput;
    [SerializeField] private TMP_InputField omegaInput;
    [SerializeField] private TMP_InputField gravParamInput;
    [SerializeField] private TMP_InputField radiusInput;
    [SerializeField] private GameObject editOrbitPanel;
    [SerializeField] private GameObject editPlanetPanel;
    public POPUP_TYPE popupType;
    public Orbit orbitScript;
    public Planet planetScript;
    public string popupID;

    void Start() {
        switch (popupType) {
            case POPUP_TYPE.PLANET:
                selectedName.text = planetScript.name + " Planet Info";
                editPlanetPanel.SetActive(true);
                editOrbitPanel.SetActive(false);
                break;
            case POPUP_TYPE.ORBIT:
                selectedName.text = orbitScript.name + " Orbit Info";
                editPlanetPanel.SetActive(false);
                editOrbitPanel.SetActive(true);
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

    public void UpdateOrbit() {
        try {
            orbitScript.ra = float.Parse(raInput.text) * Globals.KM_TO_SCALE;
            orbitScript.rp = float.Parse(rpInput.text) * Globals.KM_TO_SCALE;
            orbitScript.littleOmega = float.Parse(rpInput.text) * Globals.DEG_TO_RAD;
        } catch (FormatException exception) {
            Debug.Log(exception.ToString());
            return;
        }
        orbitScript.SolveRaRp();
        orbitScript.UpdateOrbitLine();
    }

    public void UpdatePlanet() {
        try {
            planetScript.gravParameter = float.Parse(gravParamInput.text) * Mathf.Pow(Globals.KM_TO_SCALE, 3);
            planetScript.radius = float.Parse(radiusInput.text) * Globals.KM_TO_SCALE;
        } catch (FormatException exception) {
            Debug.Log(exception.ToString());
            return;
        }
        planetScript.UpdatePlanet();
        foreach(Orbit orbit in OrbitManager.instance.orbits) {
            orbit.SolveRaRp();
            orbit.UpdateOrbitLine();
        }
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
        string a = String.Format("{0:#,##0.000}", planetScript.gravAccel);
        infoText += "Grav Accel: " + a + " m/s^2\n";
        string rt = String.Format("{0:#,##0.00}", planetScript.radius / Globals.KM_TO_SCALE);
        infoText += "Radius: " + rt + " km\n";
        return infoText;
    }
}