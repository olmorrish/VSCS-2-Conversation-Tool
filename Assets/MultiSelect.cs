using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelect : MonoBehaviour {

    //GameObject[]
    LineRenderer[] lines;

    bool mouseDownDrawingRect = false;
    Vector2 mouseDownPos;

    // Start is called before the first frame update
    void Start() {
        mouseDownDrawingRect = false;

        lines = GetComponentsInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if(mouseDownDrawingRect){
            DrawRect(mouseDownPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }


        if (Input.GetMouseButtonDown(0)) {
            if (MouseNotOnCollider()) {
                mouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseDownDrawingRect = true;

            }

        }

        else if (Input.GetMouseButtonUp(0)) {
            mouseDownDrawingRect = false;
            UndrawRect();

            Vector2 mouseUpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            List<GameObject> selectedNodes = CollectNodesWithin(mouseDownPos, mouseUpPos);

            //TODO

        }
    }

    private List<GameObject> CollectNodesWithin(Vector2 pointA, Vector2 pointB) {

        GameObject[] allChatNodeObjs = GameObject.FindGameObjectsWithTag("Node");
        List<GameObject> nodesWithinBounds = new List<GameObject>();

        foreach (GameObject nodeObj in allChatNodeObjs) {

            Vector2 nodePos = nodeObj.transform.position; 

            //TODO

        }

        Debug.Log("Collected " + nodesWithinBounds.Count + " nodes within those lines.");

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
            Debug.Log("Click hit something; not drawing a multi rect");
            return false;
        }

        return true;
    }
}
