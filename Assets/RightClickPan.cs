using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickPan : MonoBehaviour {

    Vector3 mousePos;
    Vector3 mousePosDiff;
    public float scrollSensitivity;
    public float minZoom;
    public float maxZoom;

    // Start is called before the first frame update
    void Start() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Update is called once per frame
    void Update() {
        //get mouse position delta
        mousePosDiff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;

        //if rmb is held down, move the camera in the opposite direction to the mouse movement
        if (Input.GetMouseButtonDown(1)) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1)) {
            transform.position -= mousePosDiff;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0) {
            Debug.Log("yes");
        }

        //zoom with scroll
        float fov = Camera.main.orthographicSize;
        fov -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        fov = Mathf.Clamp(fov, minZoom, maxZoom);
        Camera.main.orthographicSize = fov;
    }
}
