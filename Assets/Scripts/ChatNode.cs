using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ChatNodeType {
    AddBCPApp,              //adds an app download to the BCP
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
    Note,
    PlayerResponse,         //waits for a player text input and loops until it is correct
    StoreBool               //writes a boolean in the backend
}

public class ChatNode : MonoBehaviour {

    [Header("Variant Panel Prefabs")]
    public GameObject variantPanelAddBCPApp;
    public GameObject variantPanelAddBCPFile; //done
    public GameObject variantPanelAddContact; //done
    public GameObject variantPanelAddFeedArticle;
    public GameObject variantPanelAddLogEntry;
    public GameObject variantPanelAudio;
    public GameObject variantPanelBranchOnPlayerInput;
    public GameObject variantPanelBranchOnStoredBool; //done
    public GameObject variantPanelDelay;
    public GameObject variantPanelDialogue; //done
    public GameObject variantPanelNewSpeaker;
    public GameObject variantPanelNote;
    public GameObject variantPanelPlayerResponse;
    public GameObject variantPanelStoreBool;

    [Header("Internal References")]
    public GameObject myCanvas;
    public TMPro.TMP_InputField idInputField;
    public TMPro.TMP_Dropdown nodetypeDropdown;
    public ConnectionNub[] incomingNubs; 

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

    /// <summary>
    /// Copies over all data from another node, used when duplicating a node. Nub connections are not carried over.
    /// </summary>
    /// <param name="otherNode"></param>
    public void CopyOtherNodeData(ChatNode otherNode) {

        //header properties
        idInputField.text = otherNode.idInputField.text + "c";
        nodetypeDropdown.value = otherNode.nodetypeDropdown.value;
        selectedType = otherNode.selectedType;

        NodeTypeSelected(nodetypeDropdown);

        //package up the variant panel data and populate self using it
        Dictionary<string, string> otherNodeVariantData = otherNode.GetChatNodeData();
        currentVariantPanel.PopulateVariantPanelData(otherNodeVariantData);
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
            case ChatNodeType.AddBCPApp:
                currentVariantPanelObject = Instantiate(variantPanelAddBCPApp, myCanvas.transform);
                break;
            case ChatNodeType.AddBCPFile:
                currentVariantPanelObject = Instantiate(variantPanelAddBCPFile, myCanvas.transform);
                break;
            case ChatNodeType.AddContact:
                currentVariantPanelObject = Instantiate(variantPanelAddContact, myCanvas.transform);
                break;
            case ChatNodeType.AddFeedArticle:
                currentVariantPanelObject = Instantiate(variantPanelAddFeedArticle, myCanvas.transform);
                break;
            case ChatNodeType.AddLogEntry:
                currentVariantPanelObject = Instantiate(variantPanelAddLogEntry, myCanvas.transform);
                break;
            case ChatNodeType.Audio:
                currentVariantPanelObject = Instantiate(variantPanelAudio, myCanvas.transform);
                break;
            case ChatNodeType.BranchOnPlayerInput:
                currentVariantPanelObject = Instantiate(variantPanelBranchOnPlayerInput, myCanvas.transform);
                break;
            case ChatNodeType.BranchOnStoredBool:
                currentVariantPanelObject = Instantiate(variantPanelBranchOnStoredBool, myCanvas.transform);
                break;
            case ChatNodeType.Delay:
                currentVariantPanelObject = Instantiate(variantPanelDelay, myCanvas.transform);
                break;
            case ChatNodeType.Dialogue:
                currentVariantPanelObject = Instantiate(variantPanelDialogue, myCanvas.transform);
                break;
            case ChatNodeType.NewSpeaker:
                currentVariantPanelObject = Instantiate(variantPanelNewSpeaker, myCanvas.transform);
                break;
            case ChatNodeType.Note:
                currentVariantPanelObject = Instantiate(variantPanelNote, myCanvas.transform);
                break;
            case ChatNodeType.PlayerResponse:
                currentVariantPanelObject = Instantiate(variantPanelPlayerResponse, myCanvas.transform);
                break;
            case ChatNodeType.StoreBool:
                currentVariantPanelObject = Instantiate(variantPanelStoreBool, myCanvas.transform);
                break;
        }

        currentVariantPanel = currentVariantPanelObject.GetComponent<VariantPanel>();
    }

    /// <summary>
    /// Gets the ID of the node. Used by other nodes to get ID for nub connections during serialization.
    /// </summary>
    /// <returns>ID of this node.</returns>
    public string GetID() {
        return idInputField.text;
    }

    /// <summary>
    /// Gets the ChatNodeType of this node. Used when trying to locate and count note nodes. 
    /// </summary>
    /// <returns></returns>
    public ChatNodeType GetChatNodeType() {
        string text = nodetypeDropdown.options[nodetypeDropdown.value].text;
        return (ChatNodeType)Enum.Parse(typeof(ChatNodeType), text);
    }

    /// <summary>
    /// Called by click of Copy button. Sends self over to NodeController, so that a new node can copy data from this one.
    /// </summary>
    public void DuplicateNode() {
        GameObject.Find("NodeController").GetComponent<NodeController>().SpawnNewChatNodeFromCopy(this.gameObject);
    }

    /// <summary>
    /// Destroys this node. Nub connections handle this situation themselves.
    /// </summary>
    public void DeleteNode() {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Used when loading in the scene
    /// </summary>
    public void ConnectIncomingNub(ConnectionNub incomingNubToConnect) {

        ConnectionNub myNubToConnect = GetNextFreeIncomingNub();

        if (myNubToConnect == null) {
            Debug.LogError("ERROR IMPORTING: ChatNode " + GetID() + " doesn't have enough free incoming nubs to deserialize the file properly.");
        }
        else {
            incomingNubToConnect.ConnectToNub(myNubToConnect); //connect the two
        }
    }

    /// <summary>
    /// Get a free incoming connection nub. Returns null if none exist, meaning something went wrong with serialization.
    /// </summary>
    /// <returns>Free connectionNub, null if there are none.</returns>
    private ConnectionNub GetNextFreeIncomingNub() {

        ConnectionNub freeNub = null;

        foreach (ConnectionNub incomingNub in incomingNubs) {
            if(incomingNub.connectedNub == null) {
                freeNub = incomingNub;
                break;
            }
        }

        return freeNub;
    }

    /// <summary>
    /// Packages up data from self (ID, NodeType, Position) and current VariantPanel into a dictionary.
    /// This is used for serialization or getting data to create a copy of this node.
    /// </summary>
    /// <returns>all data pertinent to this ChatNode.</returns>
    public Dictionary<string, string> GetChatNodeData() {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("id", idInputField.text);
        data.Add("nodetype", selectedType.ToString());

        data.Add("posx", transform.position.x.ToString());
        data.Add("posy", transform.position.y.ToString());

        //add all the data that the Variant Panel gives, give warning if there isn't one
        if(currentVariantPanel == null) {
            Debug.LogError("ERROR EXPORTING: A node in the graph does not have a variant panel specified: ID of node is: \"" + GetID() + "\".");
        }
        else {
            foreach(KeyValuePair<string, string> entry in currentVariantPanel.GetVariantPanelData()) {
                data.Add(entry.Key, entry.Value);
            }
        }

        return data;
    }

    /// <summary>
    /// The inverse of GetChatNodeData(); iterates over provided dictionary to find data to populate self and current VariantPanel.
    /// Called when deserializing while importing, and when this node is a copy of another and needs data from another node to populate.
    /// </summary>
    /// <param name="data"></param>
    public void PopulateChatNodeData(Dictionary<string, string> data) {

        //set the id
        idInputField.text = data["id"];
        data.Remove("id");

        //Set the node type, which spawns in the variant panel
        SetNodeTypeFromString(data["nodetype"]);
        data.Remove("nodetype");

        //pass on the remaining data to populate the variant panel that was set
        currentVariantPanel.PopulateVariantPanelData(data);

        //don't handle nub connections here; we have to spawn all desendants first
    }

    /// <summary>
    /// Given the string name of the node type, assign the type to the dropdown.
    /// </summary>
    /// <param name="typeAsString">The node type as a string.</param>
    public void SetNodeTypeFromString(string typeAsString) {

        //get all options as strings, find the index matching the nodetype value, then set that index
        List<TMPro.TMP_Dropdown.OptionData> menuOptions = nodetypeDropdown.GetComponent<TMPro.TMP_Dropdown>().options;
        List<string> menuOptionsAsStrings = new List<string>();
        foreach (TMPro.TMP_Dropdown.OptionData optionData in menuOptions) {
            menuOptionsAsStrings.Add(optionData.text);
        }
        nodetypeDropdown.value = menuOptionsAsStrings.IndexOf(typeAsString);

        //spawn in the variantpanel, then 
        NodeTypeSelected(nodetypeDropdown);
    }

    /// <summary>
    /// Gets all outgoing nubs, which can then be connected to other nodes. These are gathered when making connections on import.
    /// </summary>
    /// <returns>All outgoing nubs, 1-3 depending on the VariantPanel type.</returns>
    public List<ConnectionNub> GetOutgoingNubs() {
        return currentVariantPanel.GetNubs();
    }

    /// <summary>
    /// Gets the descendants of this ChatNode by querying the current VariantPanel.
    /// </summary>
    /// <returns>A list of descendant ChatNodes.</returns>
    public List<ChatNode> GetDescendantNodes() {
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
