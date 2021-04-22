using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickPan : MonoBehaviour {

    Vector3 mousePos;
    Vector3 mousePosDiff;
    public float scrollSensitivity = 15f;
    public float minZoom = 10f;
    public float maxZoom = 100f;

    Vector3 mousePosOffsetFromCenter;
    public float directionalScrollMultiplier;

    // Start is called before the first frame update
    void Start() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Update is called once per frame
    void Update() {
        //get mouse position delta
        mousePosDiff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;

        //if rmb or mmb are held down, move the camera in the opposite direction to the mouse movement
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1)) {
            transform.position -= mousePosDiff;
        }

        //zoom with scroll
        float fov = Camera.main.orthographicSize;
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        fov -= scrollInput * scrollSensitivity;
        fov = Mathf.Clamp(fov, minZoom, maxZoom);
        Camera.main.orthographicSize = fov;

        //displace the camera on zoom based on position
        mousePosOffsetFromCenter = new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0f);

        if(mousePosOffsetFromCenter.magnitude > 100f && Mathf.Abs(scrollInput) > 0f) {

            if (scrollInput > 0f) {
                Camera.main.transform.localPosition = Camera.main.transform.localPosition + (mousePosOffsetFromCenter * (directionalScrollMultiplier));
            }
            else if(scrollInput < 0f) {
                Camera.main.transform.localPosition = Camera.main.transform.localPosition + (mousePosOffsetFromCenter * -(directionalScrollMultiplier));
            }
        }

    }
}
