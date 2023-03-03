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

    public List<Planet> planets;
    public List<Orbit> orbits;

    void Start() {
        planets = new List<Planet>();
        orbits = new List<Orbit>();
    }

    public void AddPlanet(Planet planet) {
        planets.Add(planet);
    }
    public void RemovePlanet(Planet planet) {
        planets.Remove(planet);
    }

    public void AddOrbit(Orbit orbit) {
        orbits.Add(orbit);
    }
    public void RemoveOrbit(Orbit orbit) {
        orbits.Remove(orbit);
    }
}
