using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ChatNodeType {
    AddBCPFile,             //makes a BCP reserve file available
    AddContact,             //adds a contact for player access
    AddFeedArticle,         //makes a Feed reserve article available
    AddLogEntry,
    Audio,
    BranchOnPlayerInput,    //waits for player to select a choice, then changes path based on choice
    BranchOnStoredBool,     //queries a boolean in backend and changes path based on it
    Delay,                  //waits for a set amount of time specified by delayLength
    Dialogue,               //presents text to the player
    NewSpeaker,
    PlayerResponse,         //waits for a player text input and loops until it is correct
    StoreBool               //writes a boolean in the backend
}

public class ChatNode : MonoBehaviour {

    [Header("Variant Panel Prefabs")]
    public GameObject VariantPanelDialogue;

    [Header("Internal References")]
    public GameObject myCanvas;
    public TMPro.TMP_Dropdown nodetypeDropdown;

    public GameObject currentVariantPanel;
    //public VariantPanel currentVariantPanel;

    // Start is called before the first frame update
    void Start() {

        nodetypeDropdown.ClearOptions();

        List<string> optionsToAdd = new List<string>();
        foreach (ChatNodeType type in Enum.GetValues(typeof(ChatNodeType))){
            optionsToAdd.Add(type.ToString());
        }
        nodetypeDropdown.AddOptions(optionsToAdd);

        nodetypeDropdown.onValueChanged.AddListener(delegate { NodeTypeSelected(nodetypeDropdown); });
    }

    // Update is called once per frame
    void Update() {
        
    }

    void NodeTypeSelected(TMPro.TMP_Dropdown dropdown) {
        string text = dropdown.options[dropdown.value].text;
        Debug.Log("Selected: " + text);
        ChatNodeType selected = (ChatNodeType)Enum.Parse(typeof(ChatNodeType), text);

        //spawn in the proper variant panel
        ClearVariantPanel();
        SpawnVariantPanel(selected);
    }

    private void ClearVariantPanel() {
        Destroy(currentVariantPanel);
    }

    private void SpawnVariantPanel(ChatNodeType selected) {

        switch (selected) {
            case ChatNodeType.Dialogue:
                Instantiate(VariantPanelDialogue, myCanvas.transform);
                break;
        }

    }

    public void DeleteNode() {
        Destroy(this.gameObject);
    }
}
