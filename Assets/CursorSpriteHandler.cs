using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSpriteHandler : MonoBehaviour {

    public GameObject screenCamera;
    public float cursorYCoordinate;
    public Texture2D cursorArrow;

    // Start is called before the first frame update
    void Start() {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
        }
        Vector2 position = screenCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        position.x = (Mathf.Round(position.x * 16f))/16f;
        position.y = (Mathf.Round(position.y * 16f))/16f;
        transform.position = new Vector3(position.x, position.y, cursorYCoordinate);
    }
}
