using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDragNode : MonoBehaviour {

    //private ViewController viewController;

    private bool mouseHeldDown;
    private float clickPosX;
    private float clickPosY;

    //private Vector2 topLeftPositionLimit;
    //private Vector2 bottomRightPositionLimit;
    //private float initialZPos;

    public bool isPopupSized;

    // Start is called before the first frame update
    void Start() {
        //viewController = GameObject.Find("ViewControllerObject").GetComponent<ViewController>();
        mouseHeldDown = false;
        //topLeftPositionLimit = isPopupSized ? new Vector2(-16f, 5.85f) : new Vector2(-16f, 9.33f); //popups are not as tall
        //bottomRightPositionLimit = isPopupSized ? new Vector2(16f, -9f) : new Vector2(16f, -8.33f);
        //initialZPos = transform.position.z;
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = new Vector3(mousePos.x - clickPosX, mousePos.y - clickPosY, transform.localPosition.z);
        }

        //Restrict position horizontal
        //if (transform.position.x < topLeftPositionLimit.x) {
        //    transform.position = new Vector3(topLeftPositionLimit.x, transform.position.y, initialZPos);
        //}
        //else if (transform.position.x > bottomRightPositionLimit.x) {
        //    transform.position = new Vector3(bottomRightPositionLimit.x, transform.position.y, initialZPos);
        //}

        ////Restrict position vertical
        //if (transform.position.y > topLeftPositionLimit.y) {
        //    transform.position = new Vector3(transform.position.x, topLeftPositionLimit.y, initialZPos);
        //}
        //else if (transform.position.y < bottomRightPositionLimit.y) {
        //    transform.position = new Vector3(transform.position.x, bottomRightPositionLimit.y, initialZPos);
        //}
    }

    private void OnMouseDown() {
        //viewController.SetActiveWindow(gameObject);
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosX = mousePos.x - transform.localPosition.x;
            clickPosY = mousePos.y - transform.localPosition.y;
            mouseHeldDown = true;
        }
    }

    private void OnMouseUp() {
        mouseHeldDown = false;
    }
}
