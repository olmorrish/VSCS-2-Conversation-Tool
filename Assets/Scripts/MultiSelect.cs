using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelect : MonoBehaviour {


    public float distUntilSelect; // distance the mouse has to move while held down to start drawing the selection
    [HideInInspector] public List<GameObject> selectedNodes;
    LineRenderer[] lines;
    Vector2 mouseDownPos;
    bool mouseDownDrawingRect = false; //

    // Start is called before the first frame update
    void Start() {
        mouseDownDrawingRect = false;

        lines = GetComponentsInChildren<LineRenderer>();
        selectedNodes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        if(mouseDownDrawingRect){
            DrawRect(mouseDownPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetMouseButtonDown(0) && MouseNotOnCollider()) {
            mouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDownDrawingRect = true;

            //TODO clear selected nodes
            if (selectedNodes.Count > 0)
                DeselectNodes();

        }
        else if (Input.GetMouseButtonUp(0) && MouseNotOnCollider()) {
            mouseDownDrawingRect = false;
            UndrawRect();

            Vector2 mouseUpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if((mouseDownPos - mouseUpPos).magnitude > distUntilSelect) {

                List<GameObject> nodesWithinBounds = CollectNodesWithin(mouseDownPos, mouseUpPos);

                SelectNodes(nodesWithinBounds);
            }
        }
    }

    private void SelectNodes(List<GameObject> nodesToSelect) {
        selectedNodes = new List<GameObject>();
        selectedNodes.AddRange(nodesToSelect);

        foreach (GameObject node in selectedNodes)
            node.GetComponent<SpriteRenderer>().color = Color.black;

        //Debug.Log("Selected " + selectedNodes.Count + " nodes within those lines.");
    }

    private void DeselectNodes() {

        foreach (GameObject node in selectedNodes)
            node.GetComponent<SpriteRenderer>().color = Color.white;

        //Debug.Log("Deselected " + selectedNodes.Count + " nodes.");
        selectedNodes.Clear();
    }

    private List<GameObject> CollectNodesWithin(Vector2 pointA, Vector2 pointB) {

        GameObject[] allChatNodeObjs = GameObject.FindGameObjectsWithTag(Constants.TAG_NODE);
        List<GameObject> nodesWithinBounds = new List<GameObject>();

        foreach (GameObject nodeObj in allChatNodeObjs) {

            Vector2 nodePos = nodeObj.transform.position;

            // bottom right of A
            if (pointB.x > pointA.x && pointB.y < pointA.y) {
                if (nodePos.x > pointA.x && nodePos.x < pointB.x
                    && nodePos.y < pointA.y && nodePos.y > pointB.y) {

                    nodesWithinBounds.Add(nodeObj);
                }
            }

            // top right of A
            else if (pointB.x > pointA.x && pointB.y > pointA.y) {
                if (nodePos.x > pointA.x && nodePos.x < pointB.x
                    && nodePos.y > pointA.y && nodePos.y < pointB.y) {

                    nodesWithinBounds.Add(nodeObj);
                }


            }

            // bottom left of A
            else if (pointB.x < pointA.x && pointB.y < pointA.y) {
                if (nodePos.x < pointA.x && nodePos.x > pointB.x
                    && nodePos.y < pointA.y && nodePos.y > pointB.y) {

                    nodesWithinBounds.Add(nodeObj);
                }
            }

            // top left of A
            else if (pointB.x < pointA.x && pointB.y > pointA.y) {
                if (nodePos.x < pointA.x && nodePos.x > pointB.x
                    && nodePos.y > pointA.y && nodePos.y < pointB.y) {

                    nodesWithinBounds.Add(nodeObj);
                }
            }

        }


        return nodesWithinBounds;
    }

    void DrawRect(Vector2 pointA, Vector2 pointB) {

        Vector2 pointC = new Vector2(pointA.x, pointB.y);
        Vector2 pointD = new Vector2(pointB.x, pointA.y);

        lines[0].SetPosition(0, pointA);
        lines[0].SetPosition(1, pointD);

        lines[1].SetPosition(0, pointA);
        lines[1].SetPosition(1, pointC);

        lines[2].SetPosition(0, pointB);
        lines[2].SetPosition(1, pointD);

        lines[3].SetPosition(0, pointB);
        lines[3].SetPosition(1, pointC);

    }

    private void UndrawRect() {
        foreach (LineRenderer rend in lines) {
            rend.SetPosition(0, Vector2.zero);
            rend.SetPosition(1, Vector2.zero);
        }
    }

    private bool MouseNotOnCollider() {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray = Physics2D.Raycast(mousePos, new Vector3(0, 0, 1), 10);

        if(ray.collider != null) {
            //Debug.Log("Click hit something; not drawing a multi rect");
            return false;
        }

        return true;
    }

    public void MoveAllSelectedNodesBy(Vector2 movement, GameObject exceptionNode) {

        foreach (GameObject node in selectedNodes) {

            if (!node.Equals(exceptionNode)) {
                node.transform.localPosition = new Vector3(node.transform.position.x + movement.x,
                    node.transform.position.y + movement.y,
                    0f);
            }

        }

    }
}
