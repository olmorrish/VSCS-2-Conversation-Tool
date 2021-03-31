using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionNub : MonoBehaviour {

    public ConnectionNub connectedNub;
    private SpriteRenderer spriteRenderer;
    private LineRenderer line;

    private bool mouseHeldDown;
    private float clickPosX;
    private float clickPosY;

    // Start is called before the first frame update
    void Start() {
        mouseHeldDown = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //gameObject.transform.localPosition = new Vector3(mousePos.x - clickPosX, mousePos.y - clickPosY, transform.localPosition.z);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, mousePos);
        }
        spriteRenderer.color = connectedNub == null ? Color.red : Color.cyan;
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

        //TODO check if another nub is below the mouse and connect it to this nub
    }

}
