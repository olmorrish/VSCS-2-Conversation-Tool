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
    public GameObject variantPanelBranchOnStoredBool;
    public GameObject variantPanelDialogue;

    [Header("Internal References")]
    public GameObject myCanvas;
    public TMPro.TMP_InputField idInputField;
    public TMPro.TMP_Dropdown nodetypeDropdown;

    [Header("State")]
    public GameObject currentVariantPanelObject;
    public VariantPanel currentVariantPanel;
    ChatNodeType selectedType;

    private void Awake() {        
        nodetypeDropdown.ClearOptions();

        List<string> optionsToAdd = new List<string>();
        foreach (ChatNodeType type in Enum.GetValues(typeof(ChatNodeType))){
            optionsToAdd.Add(type.ToString());
        }
        nodetypeDropdown.AddOptions(optionsToAdd);

    }

    // Start is called before the first frame update
    void Start() {
        nodetypeDropdown.onValueChanged.AddListener(delegate { NodeTypeSelected(nodetypeDropdown); });

    }

    // Update is called once per frame
    void Update() {
        
    }

    void NodeTypeSelected(TMPro.TMP_Dropdown dropdown) {
        string text = dropdown.options[dropdown.value].text;
        selectedType = (ChatNodeType)Enum.Parse(typeof(ChatNodeType), text);

        //spawn in the proper variant panel, remove old one
        SpawnVariantPanel(selectedType);
    }

    private void SpawnVariantPanel(ChatNodeType selected) {
        currentVariantPanel = null;
        Destroy(currentVariantPanelObject);

        switch (selected) {
            case ChatNodeType.BranchOnStoredBool:
                currentVariantPanelObject = Instantiate(variantPanelBranchOnStoredBool, myCanvas.transform);
                break;
            case ChatNodeType.Dialogue:
                currentVariantPanelObject = Instantiate(variantPanelDialogue, myCanvas.transform);
                break;
        }

        currentVariantPanel = currentVariantPanelObject.GetComponent<VariantPanel>();
    }

    /*
     * Used by other nodes to get ID for connections
     */
    public string GetID() {
        return idInputField.text;
    }

    public void DuplicateNode() {
        GameObject.Find("NodeController").GetComponent<NodeController>().SpawnNewChatNode(this.gameObject);
    }

    public void DeleteNode() {
        Destroy(this.gameObject);
    }

    public Dictionary<string, string> GetChatNodeData() {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("id", idInputField.text);
        data.Add("nodetype", selectedType.ToString());

        //add all the data that the Variant Panel gives, give warning if there isn't one
        if(currentVariantPanel == null) {
            Debug.LogError("ERROR EXPORTING: A node in the graph does not have a variant panel specified: ID of node is: \"" + GetID() + "\".");
        }
        else {
            foreach(KeyValuePair<string, string> entry in currentVariantPanel.GetVariantPanelData()) {
                data.Add(entry.Key, entry.Value);
            }
        }

        //TODO Apply post-processing to some fields

        return data;
    }

    public void PopulateChatNodeData(Dictionary<string, string> data) {
        throw new NotImplementedException();
        //populate id and nodetype
        //spawn variant panel
        //tell variant panel to populate
    }

    public ChatNode[] GetDescendantNodes() {
        //descentants are connected via the variant panel; there are sometimes multiple outgoing nubs
        if (currentVariantPanel == null) {
             Debug.LogError("ERROR EXPORTING: Node \"" + GetID() + "\" does not have a nodetype selected; variant panel is not set and descendants cannot be obtained.");
            return null;
        }
        else {
            return currentVariantPanel.GetDescendantChatNodes();
        }
    }
}
