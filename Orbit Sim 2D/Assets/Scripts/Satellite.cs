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

    private void Start() {
        base.Start();
    }

    private void Update() {
        base.Update();
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
