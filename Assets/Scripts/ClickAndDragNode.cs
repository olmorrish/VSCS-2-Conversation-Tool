using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDragNode : MonoBehaviour {

    private bool mouseHeldDown;
    private float clickPosX;
    private float clickPosY;

    // Start is called before the first frame update
    void Start() {
        mouseHeldDown = false;
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = new Vector3(mousePos.x - clickPosX, mousePos.y - clickPosY, transform.localPosition.z);
        }
    }

    private void OnMouseDown() {
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
