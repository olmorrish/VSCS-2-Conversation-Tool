using UnityEngine;

public class RightClickPan : MonoBehaviour {

    Vector3 mousePos;
    Vector3 mousePosDiff;
    public float scrollSensitivity = 15f;
    public float minZoom = 10f;
    public float maxZoom = 100f;

    Vector3 mousePosOffsetFromCenter;
    public float directionalScrollMultiplier;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Update is called once per frame
    void Update() {
        // get mouse position delta
        mousePosDiff = mainCamera.ScreenToWorldPoint(Input.mousePosition) - mousePos;

        // if RMB or MMB are held down, move the camera in the opposite direction to the mouse movement
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(1)) {
            transform.position -= mousePosDiff;
        }

        // zoom with scroll
        float fov = mainCamera.orthographicSize;
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        fov -= scrollInput * scrollSensitivity;
        fov = Mathf.Clamp(fov, minZoom, maxZoom);
        mainCamera.orthographicSize = fov;

        // displace the camera on zoom based on position
        mousePosOffsetFromCenter = new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0f);

        if(mousePosOffsetFromCenter.magnitude > 100f && Mathf.Abs(scrollInput) > 0f) {

            if (scrollInput > 0f) {
                mainCamera.transform.localPosition += (mousePosOffsetFromCenter * (directionalScrollMultiplier));
            }
            else if(scrollInput < 0f) {
                mainCamera.transform.localPosition += (mousePosOffsetFromCenter * -(directionalScrollMultiplier));
            }
        }

    }
}
