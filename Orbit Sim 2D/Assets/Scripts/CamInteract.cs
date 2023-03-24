using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CamInteract : MonoBehaviour {
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Canvas canvas;

    private Planet planetScript;
    private Orbit orbitScript;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (rayHit && rayHit.transform.GetComponent<Orbit>() != null) {
                orbitScript = rayHit.transform.GetComponent<Orbit>();
                planetScript = orbitScript.GetPlanetScript();
                string id = orbitScript.name + planetScript.name;

                if (OrbitManager.instance.CheckDuplicatePopup(id)) {
                    Debug.Log("Duplicate orbit popup, " + id);
                    goto SkipOrbitAdd;
                }

                AddPopupPrefab(id, Popup.POPUP_TYPE.ORBIT);
            }
            SkipOrbitAdd:
            if (rayHit && rayHit.transform.GetComponent<Planet>() != null) {
                planetScript = rayHit.transform.GetComponent<Planet>();
                string id = planetScript.name;

                if (OrbitManager.instance.CheckDuplicatePopup(id) || planetScript.isSatellite) {
                    Debug.Log("Duplicate planet popup, " + id);
                    return;
                }

                AddPopupPrefab(id, Popup.POPUP_TYPE.PLANET);
            }
        }
    }

    private void AddPopupPrefab(string id, Popup.POPUP_TYPE popupType) {
        GameObject newPrefab = Instantiate(popupPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newPrefab.transform.SetParent(canvas.transform);
        newPrefab.transform.localPosition = new Vector3(0, 0, 0);
        newPrefab.transform.localScale = new Vector3(0.4326068f, 0.4326068f, 0.4326068f);
        Popup popup = newPrefab.GetComponent<Popup>();
        popup.planetScript = planetScript;
        popup.orbitScript = orbitScript;
        popup.popupType = popupType;
        popup.popupID = id;
        popup.transform.Find("SelectNamePanel").GetComponent<DraggableWindow>().canvas = canvas;
        OrbitManager.instance.AddPopup(newPrefab.GetComponent<Popup>());
    }
}
