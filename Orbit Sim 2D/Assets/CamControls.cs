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
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            cam.fieldOfView--;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            cam.fieldOfView++;
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
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Globals.timeMultiplier *= 4.0f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Globals.timeMultiplier /= 4.0f;
        }
    }
}
