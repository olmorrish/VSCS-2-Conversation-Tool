using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoubleClickButton : MonoBehaviour, IPointerDownHandler {

    public bool interactable = true;
    public Sprite pressedSprite;
    private Sprite defaultSprite;    //set in the image component; saved here for reference
    public UnityEvent onDoubleClick; //allows selection of a function to be invoked on a doubleclick

    float timesClicked = 0;
    float clickTime = 0;
    float lastClickTime = 0f;
    float clickDelay = 0.5f;

    private Image buttonImage;

    public void Start() {
        buttonImage = GetComponent<Image>();
        defaultSprite = buttonImage.sprite;
    }

    public void Update() {
        //mouse button must be released for sprite to return to normal
        if(Time.time > lastClickTime + 0.1f && !Input.GetMouseButton(0))
            buttonImage.sprite = defaultSprite;
    }

    public void OnPointerDown(PointerEventData data) {
        if (!interactable)
            return;

        //Sprite alterations
        buttonImage.sprite = pressedSprite;
        lastClickTime = Time.time;

        //Double-click timeout behavior
        timesClicked++;
        if (timesClicked == 1)
            clickTime = Time.time;

        if (timesClicked > 1 && Time.time - clickTime < clickDelay) {
            timesClicked = 0;
            clickTime = 0;

            onDoubleClick.Invoke(); //invoke the specified function
        }
        else if (timesClicked > 2 || Time.time - clickTime > 1)
            timesClicked = 0;
    }
}
