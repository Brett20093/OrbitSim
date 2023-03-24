using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CamInteract : MonoBehaviour {
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Canvas canvas;

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
                planetScript = orbitScript.GetPlanetScript();

                if (OrbitManager.instance.CheckDuplicatePopup(orbitScript.name))
                    goto SkipAdd;

                GameObject newPrefab = Instantiate(popupPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                newPrefab.transform.parent = canvas.transform;
                newPrefab.transform.localPosition = new Vector3(0, 0, 0);
                newPrefab.transform.localScale = new Vector3(0.4326068f, 0.4326068f, 0.4326068f);
                Popup popup = newPrefab.GetComponent<Popup>();
                popup.planetScript = planetScript;
                popup.orbitScript = orbitScript;
                popup.transform.Find("SelectNamePanel").GetComponent<DraggableWindow>().canvas = canvas;
                OrbitManager.instance.AddPopup(newPrefab.GetComponent<Popup>());
            }
        }
    SkipAdd:
        UpdateTime();
    }

    private void UpdateTime() {
        int sec = (int)TimeKeeper.instance.time;
        int days = sec / (int)planetScript.secInPlanetDay;
        sec = sec % (int)planetScript.secInPlanetDay;
        int hours = sec / 3600;
        sec = sec % 3600;
        int mins = sec / 60;
        sec = sec % 60;

        timeText.text = "Time: " + days + "d : " + hours + "h : " + mins + "m : " + sec + "s";
    }
}
