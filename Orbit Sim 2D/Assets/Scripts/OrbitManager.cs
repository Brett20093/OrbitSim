using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrbitManager : MonoBehaviour {
    #region Singleton
    public static OrbitManager instance;
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("More than one instance of OrbitManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField] private TMP_InputField rpInput;
    [SerializeField] private TMP_InputField raInput;
    [SerializeField] private TMP_InputField littleOmegaInput;

    public List<Planet> planets = new List<Planet>();
    public List<Orbit> orbits = new List<Orbit>();
    public List<GameObject> popups = new List<GameObject>();

    private Planet selectedPlanet;
    private Orbit selectedOrbit;

    void Start() {
        Orbit[] findOrbits = FindObjectsOfType<Orbit>();
        for (int i = 0; i < findOrbits.Length; i++) {
            if(!orbits.Contains(findOrbits[i])) {
                AddOrbit(findOrbits[i]);
            }
        }
        Planet[] findPlanets = FindObjectsOfType<Planet>();
        for (int i = 0; i < findPlanets.Length; i++) {
            if (!planets.Contains(findPlanets[i])) {
                AddPlanet(findPlanets[i]);
            }
        }
    }

    public void AddPlanet(Planet planet) {
        if (!planets.Contains(planet))
            planets.Add(planet);
    }
    public void RemovePlanet(Planet planet) {
        planets.Remove(planet);
    }

    public void AddOrbit(Orbit orbit) {
        if (!orbits.Contains(orbit))
            orbits.Add(orbit);
    }
    public void RemoveOrbit(Orbit orbit) {
        orbits.Remove(orbit);
    }

    public void AddPopup(GameObject popup) {
        if (!popups.Contains(popup))
            popups.Add(popup);
    }
    public void RemoveOrbit(GameObject popup) {
        popups.Remove(popup);
    }

    public void SetOrbitLineWidths(float width) {
        for (int i = 0; i < orbits.Count; i++) {
            orbits[i].orbitLine.startWidth = width;
            orbits[i].orbitLine.endWidth = width;
        }
    }

    public void SelectOrbit(Orbit orbit) {
        selectedOrbit = orbit;
    }

    public void SelectPlanet(Planet planet) {
        selectedPlanet = planet;
    }

    public void UpdateOrbit() {
        try {
            selectedOrbit.ra = float.Parse(raInput.text) * Globals.KM_TO_SCALE;
            selectedOrbit.rp = float.Parse(rpInput.text) * Globals.KM_TO_SCALE;
            selectedOrbit.SetLittleOmega(float.Parse(littleOmegaInput.text) * Globals.DEG_TO_RAD);
        } catch (FormatException exception) {
            Debug.Log(exception.ToString());
            return;
        }
        selectedOrbit.SolveRaRp();
        selectedOrbit.UpdateOrbitLine();
    }
}
