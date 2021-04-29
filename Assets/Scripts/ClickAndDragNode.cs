using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDragNode : MonoBehaviour {

    private bool mouseHeldDown;
    private float clickPosX;
    private float clickPosY;
    private MultiSelect multiSelect;
    private bool isPartOfMultiSelection;

    // Start is called before the first frame update
    void Start() {
        mouseHeldDown = false;
        multiSelect = GameObject.Find("MultiSelect").GetComponent<MultiSelect>();
        isPartOfMultiSelection = false;
    }

    // Update is called once per frame
    void Update() {
        if (mouseHeldDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 prevLocalPosition = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(mousePos.x - clickPosX, mousePos.y - clickPosY, transform.localPosition.z);

            if (isPartOfMultiSelection) {
                multiSelect.MoveAllSelectedNodesBy(gameObject.transform.position - prevLocalPosition, this.gameObject);
            }

        }
    }

    private void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosX = mousePos.x - transform.localPosition.x;
            clickPosY = mousePos.y - transform.localPosition.y;
            mouseHeldDown = true;

            //check if this node is in the current multiselection; if yes, set a flag
            isPartOfMultiSelection = multiSelect.selectedNodes.Contains(this.gameObject);
        }
    }

    private void OnMouseUp() {
        mouseHeldDown = false;
    }
}
