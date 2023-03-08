using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    [SerializeField] private GameObject planet;
    [SerializeField] private GameObject satellite;
    [SerializeField] private float camSpeed = 500.0f;
    private Camera cam;
    private float camZoomFactor = 0.2f;
    private const float MAX_CAM_DISTANCE = 100000.0f;
    private float timeMultFactor = 0.01f;
    private const float MAX_TIME_FACTOR = 100000.0f;
    private Vector3 focusPosition = new Vector3(0,0,0);
    private GameObject focusGO = null;
    private int focusIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (camZoomFactor < 1.0f) {
                camZoomFactor += Time.deltaTime;
                float multiplier = Mathf.Pow(camZoomFactor, 3.0f);
                OrbitManager.instance.SetOrbitLineWidths(1000.0f * multiplier);
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -1.0f * multiplier * MAX_CAM_DISTANCE);
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (camZoomFactor > 0.0001f) {
                camZoomFactor -= Time.deltaTime;
                float multiplier = Mathf.Pow(camZoomFactor, 3.0f);
                OrbitManager.instance.SetOrbitLineWidths(1000.0f * multiplier);
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, - 1.0f * multiplier * MAX_CAM_DISTANCE);
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (focusIndex >= OrbitManager.instance.planets.Count-1) {
                focusIndex = -1;
                focusGO = null;
            } else {
                focusIndex++;
                focusPosition = new Vector3(0, 0, 0);
                focusGO = OrbitManager.instance.planets[focusIndex].gameObject;
            }
        }

        if (focusGO != null) {
            CamControlsFocus(focusGO);
        } else {
            FreeCamControls();
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            if (timeMultFactor < 1.0f) {
                timeMultFactor += Time.deltaTime;
                Globals.timeMultiplier = Mathf.Pow(timeMultFactor, 3.0f) * MAX_TIME_FACTOR + 1.0f;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (timeMultFactor > 0) {
                timeMultFactor -= Time.deltaTime;
                Globals.timeMultiplier = Mathf.Pow(timeMultFactor, 3.0f) * MAX_TIME_FACTOR + 1.0f;
            }
        }
    }

    private void CamControlsFocus(GameObject focus) {
        if (Input.GetKey(KeyCode.D)) {
            focusPosition += new Vector3(1, 0, 0) * Time.deltaTime * camSpeed;
        }
        if (Input.GetKey(KeyCode.W)) {
            focusPosition += new Vector3(0, 1, 0) * Time.deltaTime * camSpeed;
        }
        if (Input.GetKey(KeyCode.A)) {
            focusPosition += new Vector3(-1, 0, 0) * Time.deltaTime * camSpeed;
        }
        if (Input.GetKey(KeyCode.S)) {
            focusPosition += new Vector3(0, -1, 0) * Time.deltaTime * camSpeed;
        }
        transform.position = new Vector3(focus.transform.position.x + focusPosition.x, focus.transform.position.y + focusPosition.y, transform.position.z);
    }

    private void FreeCamControls() {
        if (Input.GetKey(KeyCode.D)) {
            transform.position += new Vector3(1, 0, 0) * Time.deltaTime * camSpeed;
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.position += new Vector3(0, 1, 0) * Time.deltaTime * camSpeed;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position += new Vector3(-1,0,0) * Time.deltaTime * camSpeed;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += new Vector3(0, -1, 0) * Time.deltaTime * camSpeed;
        }
    }
}
