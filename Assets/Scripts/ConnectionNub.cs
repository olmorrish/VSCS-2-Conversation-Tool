using UnityEngine;

public class ConnectionNub : MonoBehaviour {

    public ConnectionNub connectedNub;
    private SpriteRenderer spriteRenderer;
    private LineRenderer line;
    private Color defaultColor;

    private bool mouseHeldDown;
    private float clickPosX;
    private float clickPosY;

    private const string TAG_NUB = "Nub";

    // Start is called before the first frame update
    void Start() {
        mouseHeldDown = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        defaultColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update() {

        // redraw the connection each frame if there is a connected node, in case one moves.
        if(connectedNub != null) {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, connectedNub.transform.position);
        }
        else {
            ResetLineRenderer();
        }

        // always draw a line from the clicked node to the mouse, while M0 is held down
        if (mouseHeldDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, mousePos);
        }

        // nubs change colour to indicate connection
        spriteRenderer.color = connectedNub == null ? defaultColor : Color.cyan;
    }

    /// <summary>
    /// Called on mouse 0 down; tracks click and drag.
    /// </summary>
    private void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosX = mousePos.x - transform.localPosition.x;
            clickPosY = mousePos.y - transform.localPosition.y;
            mouseHeldDown = true;

            // decouple nubs if one is clicked, and reset their line
            DisconnectNub();
        }
    }

    /// <summary>
    /// Called on mouse 1 up; releasing over another nub creates a connection, otherwise the connection is intentionally broken.
    /// </summary>
    private void OnMouseUp() {

        mouseHeldDown = false;

        // check if there is a nub below the mouse when it is released
        bool nubBelow = false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray = Physics2D.Raycast(mousePos, new Vector3(0,0,1), 10);
        if(ray.collider != null) {
            if (ray.collider.gameObject.CompareTag(TAG_NUB) && ray.collider.gameObject != this.gameObject) { //check if the collided obj is a non-self nub
                nubBelow = true;
            }
        }

        if(!nubBelow) {
            ResetLineRenderer(); // no nub
        }
        else {
            ConnectToNub(ray.collider.gameObject.GetComponent<ConnectionNub>());
        }
    }

    /// <summary>
    /// Connects to another nub. This breaks any connection that nub might have.
    /// </summary>
    public void ConnectToNub(ConnectionNub otherNub) {
        connectedNub = otherNub;

        // disconnect other nub then doubly link to us
        connectedNub.DisconnectNub();
        connectedNub.connectedNub = this;
    }

    /// <summary>
    /// Disconnects the nubs. Also tells other nub to disconnect.
    /// </summary>
    public void DisconnectNub() {
        if (connectedNub != null) {
            connectedNub.connectedNub = null;
            connectedNub.ResetLineRenderer();
        }
        connectedNub = null;
        ResetLineRenderer();
    }

    private void ResetLineRenderer() {
        if (line) {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
        }
    }

    /// <summary>
    /// Recursively goes up the chain until the ChatNode this nub is attached to is found.
    /// </summary>
    /// <returns></returns>
    public ChatNode GetParentChatNode() {

        GameObject obj = gameObject;

        while (obj.GetComponent<ChatNode>() == null) {
            obj = obj.transform.parent.gameObject;
        }

        return obj.GetComponent<ChatNode>();
    }
}
