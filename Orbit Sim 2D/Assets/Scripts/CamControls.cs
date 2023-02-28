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
    private const int FREE = 0;
    private const int SAT = 1;
    private const int PLANET = 2;
    private int camState = FREE;
    private float camZoomFactor = 0.01f;
    private const float MAX_CAM_DISTANCE = 100000.0f;
    private float timeMultFactor = 0.01f;
    private const float MAX_TIME_FACTOR = 100000.0f;
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
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -1.0f * Mathf.Pow(camZoomFactor, 3.0f) * MAX_CAM_DISTANCE);
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (camZoomFactor > 0.0001f) {
                camZoomFactor -= Time.deltaTime;
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, - 1.0f * Mathf.Pow(camZoomFactor, 3.0f) * MAX_CAM_DISTANCE);
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (camState == PLANET) {
                camState = FREE;
            } else {
                camState++;
            }
        }

        switch (camState) {
            case FREE:
                FreeCamControls();
                break;
            case SAT:
                transform.position = new Vector3(satellite.transform.position.x, satellite.transform.position.y, transform.position.z);
                break;
            case PLANET:
                transform.position = new Vector3(planet.transform.position.x, planet.transform.position.y, transform.position.z);
                break;
            default:
                break;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            if (timeMultFactor < 1.0f) {
                timeMultFactor += Time.deltaTime;
                Globals.timeMultiplier = Mathf.Pow(timeMultFactor, 3.0f) * MAX_TIME_FACTOR + 1.0f;
            }
            Debug.Log("Right mult: " + Globals.timeMultiplier);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (timeMultFactor > 0) {
                timeMultFactor -= Time.deltaTime;
                Globals.timeMultiplier = Mathf.Pow(timeMultFactor, 3.0f) * MAX_TIME_FACTOR + 1.0f;
            }
            Debug.Log("Left mult: " + Globals.timeMultiplier);
        }
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
