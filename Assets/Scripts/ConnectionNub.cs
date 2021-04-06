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

        //redraw the connection each frame if there is a connected node, in case one moves.
        if(connectedNub != null) {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, connectedNub.transform.position);
        }
        else {
            ResetLineRenderer();
        }

        if (mouseHeldDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, mousePos);
        }

        spriteRenderer.color = connectedNub == null ? Color.white : Color.cyan;
    }

    private void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosX = mousePos.x - transform.localPosition.x;
            clickPosY = mousePos.y - transform.localPosition.y;
            mouseHeldDown = true;

            //decouple nubs if one is clicked, and reset their line
            DisconnectNub();
        }
    }

    private void OnMouseUp() {

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
            ResetLineRenderer(); //no nub
        }
        else {
            ConnectToNub(ray.collider.gameObject);


        }
    }

    /// <summary>
    /// Connects to another nub. This breaks any connection that nub might have.
    /// </summary>
    public void ConnectToNub(GameObject connectingTo) {
        connectedNub = connectingTo.GetComponent<ConnectionNub>();
        line.SetPosition(1, connectingTo.transform.position);

        //Disconnect other nub then doubly link to us
        connectedNub.DisconnectNub();
        connectedNub.connectedNub = this;
    }

    /// <summary>
    /// Disconnects the nubs. Also tells other nub to reset.
    /// </summary>
    public void DisconnectNub() {
        if (connectedNub != null) {
            connectedNub.connectedNub = null;
            connectedNub.ResetLineRenderer();
        }
        connectedNub = null;
        ResetLineRenderer();
    }

    /// <summary>
    /// Disconnects this nub only; this is required when this nub is on a copy of a node. Called on copyNode.
    /// </summary>
    public void DisconnectThisNubOnly() {
        connectedNub = null;
        ResetLineRenderer();
    }

    public void ResetLineRenderer() {
        if (line) {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
        }
    }

    /*
     * Recursively goes up the chain until the ChatNode this nub is attached to is found
     */
    public ChatNode GetParentChatNode() {

        GameObject obj = this.gameObject;

        while (obj.GetComponent<ChatNode>() == null) {
            obj = obj.transform.parent.gameObject;
        }

        return obj.GetComponent<ChatNode>();
    }
}
