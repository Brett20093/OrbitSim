using System.Collections;
using System.Collections.Generic;
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

    public List<Planet> planets = new List<Planet>();
    public List<Orbit> orbits = new List<Orbit>();

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

    public void SetOrbitLineWidths(float width) {
        for (int i = 0; i < orbits.Count; i++) {
            orbits[i].orbitLine.startWidth = width;
            orbits[i].orbitLine.endWidth = width;
        }
    }
}
