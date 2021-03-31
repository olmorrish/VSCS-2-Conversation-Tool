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

            //decouple nubs if one is clicked, and reset their line
            if(connectedNub != null) {
                connectedNub.connectedNub = null;
                connectedNub.gameObject.GetComponent<LineRenderer>().SetPosition(1, connectedNub.gameObject.transform.position);
            }
            connectedNub = null;
        }
    }

    private void OnMouseUp() {
        Debug.Log("let go");

        mouseHeldDown = false;

        //check if there is a nub below the mouse when it is released
        bool nubBelow = false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray = Physics2D.Raycast(mousePos, new Vector3(0,0,1), 10);
        if(ray.collider != null) {
            if (ray.collider.gameObject.CompareTag("Nub") && ray.collider.gameObject != this.gameObject) { //check if the collided obj is a non-self nub
                nubBelow = true;
            }
        }

        if(!nubBelow) {
            line.SetPosition(1, transform.position); //reset, no nub
        }
        else {
            Debug.Log("CONNECTION: " + ray.collider.gameObject.name);

            //connect to the other nub
            GameObject connectingTo = ray.collider.gameObject;
            connectedNub = connectingTo.GetComponent<ConnectionNub>();
            line.SetPosition(1, connectingTo.transform.position);

            //make sure they are doubly linked
            connectedNub.connectedNub = this;
        }
    }

}
