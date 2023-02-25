using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Satellite : Orbit
{
    [SerializeField] private TMP_InputField rpInput;
    [SerializeField] private TMP_InputField raInput;
    [SerializeField] private TMP_InputField littleOmegaInput;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text rText;
    [SerializeField] private TMP_Text trueAnomText;
    [SerializeField] private TMP_Text veloText;

    private void Start() {
        base.Start();
    }

    private void Update() {
        base.Update();
        UpdateSatInfoText();
    }

    private void UpdateSatInfoText() {
        int sec = (int)TimeKeeper.instance.time;
        int days = sec / (int)planetScript.secInPlanetDay;
        sec = sec % (int)planetScript.secInPlanetDay;
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
        trueAnomText.text = tat + "°";
        string vt = String.Format("{0:#,###.##}", velo / Globals.KM_TO_SCALE);
        veloText.text = vt + " km/s";
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
