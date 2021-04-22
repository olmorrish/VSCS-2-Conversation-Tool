using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.IO;
using UnityEngine.UI;

public class NodeController : MonoBehaviour {

    [Header("References")]
    public GameObject chatNodePrefab;
    public TMPro.TMP_InputField headIDInputField;
    public TMPro.TMP_InputField exportNameInputField;
    public OutputText outputText;
    public Toggle autoGenerateNodeIDs;

    [Header("Export Parameters")]
    public bool useRealGamePath;
    private const string vscsPath = "C:\\Users\\olive\\Documents\\Unity Projects\\VSCS-2\\Assets\\Resources";

    //[Header("Data")]
    private string headNodeID;

    //support data for topological sort; reset each time
    private Dictionary<string, bool> temporaryMarks;
    private Dictionary<string, bool> permanentMarks;
    private List<ChatNode> sortedNodes;

    #region Button Fuctions

    /// <summary>
    /// Spawn a new blank ChatNode in the center of the view.
    /// </summary>
    public void SpawnNewChatNode() {
        GameObject newChatNode = Instantiate(chatNodePrefab, this.transform);
        newChatNode.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
    }

    /// <summary>
    /// Copy spawner, called by copy button on ChatNode. Tells new ChatnNode to copy the passed one.
    /// </summary>
    /// <param name="toCopy">The GameObject of the ChatNode to copy. Param is calling object.</param>
    public void SpawnNewChatNodeFromCopy(GameObject toCopy) {

        GameObject newChatNodeObject = Instantiate(chatNodePrefab, this.transform);
        newChatNodeObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        ChatNode newChatNode = newChatNodeObject.GetComponent<ChatNode>();
        newChatNode.CopyOtherNodeData(toCopy.GetComponent<ChatNode>());
    }

    /// <summary>
    /// Export all nodes to JSON.
    /// </summary>
    public void Export() {

        //autoID the nodes; doesn't change head node or overwrite it
        if (autoGenerateNodeIDs.isOn) {
            AutoGenerateAllNodeIDs();
        }

        //find the head node
        string headNodeID = headIDInputField.text;
        string exportName = exportNameInputField.text;

        if (headNodeID.Equals("")) {
            outputText.AddLine("ERROR EXPORTING: No head node ID was specified.");
            Debug.LogWarning("ERROR EXPORTING: No head node ID was specified.");
            return;
        }
        else if (exportName.Equals("")) {
            outputText.AddLine("ERROR EXPORTING: No export name was specified.");
            Debug.LogWarning("ERROR EXPORTING: No export name was specified.");
            return;
        }

        GameObject[] allChatNodesObjects = GameObject.FindGameObjectsWithTag("Node");
        ChatNode[] allChatNodes = new ChatNode[allChatNodesObjects.Length];

        //iterate over chatnode objects to find the head and get all ChatNode components
        ChatNode headNode = null;
        for (int i = 0; i<allChatNodesObjects.Length; i++) {
            allChatNodes[i] = allChatNodesObjects[i].GetComponent<ChatNode>();
            if (allChatNodes[i].GetID().Equals(headNodeID)) {
                headNode = allChatNodes[i];
            }
        }

        if(headNode == null) {
            outputText.AddLine("ERROR EXPORTING: Head node with ID \"" + headNodeID + "\" could not be found.");
            Debug.LogWarning("ERROR EXPORTING: Head node with ID \"" + headNodeID + "\" could not be found.");
            return;
        }

        //create a convodata then recursively go over each node and get it's data
        ConversationData allData = new ConversationData();
        allData.AddNodeEntry(headNode.GetChatNodeData());

        //topologically sort all nodes in the scene; this stores them in sortedNodes
        TopologicalSortNodes(allChatNodes, headNode);

        //Check for duplicate IDs
        if (CheckForDuplicateIDs()) {
            return;
        }

        //get a dictionary of entries for each node
        List<Dictionary<string, string>> nodeEntries = new List<Dictionary<string, string>>();
        foreach(ChatNode node in sortedNodes) {
            nodeEntries.Add(node.GetChatNodeData());
        }

        //convert dictionaries to a JSON array
        JSONArray allNodes = new JSONArray();

        foreach(Dictionary<string, string> nodeEntry in nodeEntries) {

            //position is saved in dictionary; save as we go
            Vector2 positionToJSONArray = new Vector2();

            //turn each ChatNode into a JSON obj...
            JSONObject singleNodeJSONObj = new JSONObject();
            foreach (KeyValuePair<string, string> pair in nodeEntry) {

                //some fields require array post processing
                if (pair.Key == "posx") { positionToJSONArray.x = float.Parse(pair.Value); }
                else if (pair.Key == "posy") { positionToJSONArray.y = float.Parse(pair.Value); }

                else if (pair.Key == "contents") {
                    string rawDialogue = pair.Value;
                    string[] splitDialogue = rawDialogue.Split(new string[] { "\n\n" }, StringSplitOptions.None);

                    JSONArray allDialogueArray = new JSONArray();
                    foreach(string lineOfDialogue in splitDialogue)
                        allDialogueArray.Add(lineOfDialogue);

                    singleNodeJSONObj.Add("contents", allDialogueArray);
                }

                else {
                    singleNodeJSONObj.Add(pair.Key, pair.Value); //used for most fields if not array'ed
                }
            }

            //...store coordinates of the node as array (we stored them as we looped through)...
            JSONArray position = new JSONArray();
            position.Add(positionToJSONArray.x);
            position.Add(positionToJSONArray.y);
            singleNodeJSONObj.Add("nodeposition", position);

            //... then add it to the full node array
            allNodes.Add(singleNodeJSONObj);
        }

        Debug.Log(allNodes.ToString());

        //write the JSON
        string saveFilePath = Application.persistentDataPath + "\\" +  exportNameInputField.text + ".json";
        if (useRealGamePath) {
            saveFilePath = vscsPath + "\\" + exportNameInputField.text + ".json"; //the resources folder in the full game
        }

        File.WriteAllText(saveFilePath, allNodes.ToString());

        outputText.AddLine("SUCCESS! Exported file as \"" + exportNameInputField.text + ".json\" (in AppData).");
    }

    /// <summary>
    /// Imports a file, given the entered file name. This clears the scene then populates it with the saved ChatNodes.
    /// </summary>
    public void Import() {

        ClearScreen();

        if (exportNameInputField.text.Equals("")) {
            outputText.AddLine("ERROR IMPORTING: No import name was specified.");
            Debug.LogWarning("ERROR IMPORTING: No import name was specified.");
            return;
        }

        //load in the data
        string loadFilePath = Application.persistentDataPath + "\\" +  exportNameInputField.text + ".json";
        if (useRealGamePath) {
            loadFilePath = vscsPath + "\\" + exportNameInputField.text + ".json"; //the resources folder in the full game
        }

        if (!File.Exists(loadFilePath)) {
            outputText.AddLine("ERROR IMPORTING: Could not find file: \"" + exportNameInputField.text + ".json \"");
            Debug.LogWarning("ERROR IMPORTING: Could not find file: \"" + exportNameInputField.text + ".json \"");
            return;
        }

        JSONArray loadedChatNodes = (JSONArray) JSON.Parse(File.ReadAllText(loadFilePath));

        //flag and string for post-processed contents
        bool addContents = false;
        string processedContents = "";

        //set up dictionary for nexts
        Dictionary<string, List<string>> nextDictionary = new Dictionary<string, List<string>>();

        //iterate over all the chatnode data and spwawn them in
        int i = 0;
        while (loadedChatNodes[i] != null) {

            //get object and spawn a blank chatnode
            JSONObject chatNodeJSONData = (JSONObject)loadedChatNodes[i];
            GameObject newChatNodeObject = Instantiate(chatNodePrefab, this.transform);
            newChatNodeObject.name = "ChatNode_" + i;

            //if this is the first node, populate the head ID input field 
            if (i == 0) {
                headIDInputField.text = loadedChatNodes[0]["id"].Value.ToString();
            }

            //obtain and then set the position
            JSONArray nodePosition = (JSONArray)chatNodeJSONData["nodeposition"];
            float xPos = nodePosition[0];
            float yPos = nodePosition[1];
            newChatNodeObject.transform.position = new Vector3(xPos, yPos, 0f);

            //if this is the first node, focus the camera on it
            if (i == 0) {
                Camera.main.transform.position = new Vector3(xPos, yPos, -10f);
            }

            //add all the fields in the json to a dictionary by iterating over the keys
            Dictionary<string, string> nodeDataAsDictionary = new Dictionary<string, string>();
            JSONNode.KeyEnumerator keys = chatNodeJSONData.Keys;
            foreach(string key in keys) {
                if (key.Equals("contents")) {

                    JSONArray contentAsJSON = (JSONArray) chatNodeJSONData["contents"];

                    foreach (string ckey in contentAsJSON.Values) {
                        processedContents += ckey + "\n\n";
                    }
                    processedContents = processedContents.Substring(0, processedContents.Length - 2);

                    addContents = true;
                }

                else if (key.Equals("nodeposition")) {
                    //do nothing, we already applied this data to the transform and don't need to pass it to the node
                }
                else {
                    nodeDataAsDictionary.Add(key, chatNodeJSONData[key]); //no post-processing needed
                }
            }

            //add and remove contents
            if (addContents) {
                nodeDataAsDictionary.Add("processedcontents", processedContents);
            }
            nodeDataAsDictionary.Remove("nodeposition"); //we don't need position anymore, already applied it

            //pass the JSON data to the node so it can populate itself
            newChatNodeObject.GetComponent<ChatNode>().PopulateChatNodeData(nodeDataAsDictionary);

            //collect nub connections based on "nexts", then remove ones that aren't there (null)
            List<string> nextIDs = new List<string>();
            nextIDs.Add(chatNodeJSONData["next"]);
            nextIDs.Add(chatNodeJSONData["nextT"]);
            nextIDs.Add(chatNodeJSONData["nextF"]);
            nextIDs.Add(chatNodeJSONData["nextA"]);
            nextIDs.Add(chatNodeJSONData["nextB"]);
            nextIDs.Add(chatNodeJSONData["nextC"]);

            while (nextIDs.Contains(null))
                nextIDs.Remove(null);
            while (nextIDs.Contains("TERMINATE"))
                nextIDs.Remove("TERMINATE");

            //map the id of the new node to the list of it's nexts; once all nodes are spawned in, we make the connections
            nextDictionary.Add(newChatNodeObject.GetComponent<ChatNode>().GetID(), nextIDs);

            i++;
        }

        //iterate over all the spawned chatnodes in the scene and connect them
        GameObject[] allChatNodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(GameObject n in allChatNodes) {

            //Debug.Log("Attempting to reconcile connections for node + " + n.GetComponent<ChatNode>().GetID() + "...");

            //get the ids of the next node
            ChatNode node = n.GetComponent<ChatNode>();
            string nodeID = node.GetID();

            List<string> nodeNextIDs = nextDictionary[nodeID];
            List<ConnectionNub> outgoingNubs = node.GetOutgoingNubs();

            //connect jth outgoing nub for the ChatNode to the jth element of the next list
            for (int j = 0; j < nodeNextIDs.Count; j++) {
                //find reference to the other "next" node
                ChatNode descendantNode = GetChatNodeWithID(allChatNodes, nodeNextIDs[j]);

                //call ConnectIncomingNub to connect the nub
                descendantNode.ConnectIncomingNub(outgoingNubs[j]);
            }
        }

        outputText.AddLine("SUCCESS! Imported file as \"" + exportNameInputField.text + ".json\".");
    }

    /// <summary>
    /// Gets a chat node with a specific ID.
    /// </summary>
    /// <param name="allNodes">All the nodes to search through.</param>
    /// <param name="id">The ID to look for.</param>
    /// <returns></returns>
    private ChatNode GetChatNodeWithID(GameObject[] allNodes, string id) {
        foreach(GameObject node in allNodes) {
            ChatNode cNode = node.GetComponent<ChatNode>();
            if (cNode.GetID().Equals(id)) {
                return cNode;
            }
        }

        return null;
    }

    /// <summary>
    /// Renames all the ChatNodes in the scene.
    /// This will not change the head node ID, nor will i rename another node to have the same ID as the head node.
    /// </summary>
    public void AutoGenerateAllNodeIDs() {

        string headNodeID = headIDInputField.text;

        GameObject[] allNodes = GameObject.FindGameObjectsWithTag("Node");

        int i = 0;
        foreach(GameObject chatNode in allNodes) {

            TMPro.TMP_InputField thisNodeIDField = chatNode.GetComponent<ChatNode>().idInputField;
            
            //if we're about to give a node the same ID as the head... skip that number
            if (i.ToString().Equals(headNodeID)) {
                i++;
            }
            
            //if this is the head node, skip renaming it
            if (thisNodeIDField.text.Equals(headNodeID)) {
                continue;
            }
            else {
                thisNodeIDField.text = i.ToString();
                i++;
            }

        }

        outputText.AddLine("New ChatNode IDs have been generated. The head node ID has not overwritten or duplicated.");
    }

    /// <summary>
    /// Checks all nodes for duplicate IDs. Called when exporting, but after any autorenaming.
    /// </summary>
    /// <returns>True if there is a duplicate node ID.</returns>
    private bool CheckForDuplicateIDs() {

        GameObject[] allNodes = GameObject.FindGameObjectsWithTag("Node");

        List<string> allIDs = new List<string>();
        foreach (GameObject nodeObj in allNodes) {

            Debug.Log("Checking for duplicate nodes...");

            string id = nodeObj.GetComponent<ChatNode>().GetID();
            if (allIDs.Contains(id)) {
                outputText.AddLine("ERROR EXPORTING: The ID \"" + id + "\" appears more than once in the exported nodes. This will likely cause an issue upon ChatSystem interpretation.");
                Debug.LogWarning("The ID \"" + id + "\" appears more than once in the exported nodes. This will likely cause an issue upon ChatSystem interpretation.");
                return true;
            }
            else {
                allIDs.Add(id);
            }

        }

        return false;
    }

    /// <summary>
    /// Deletes all ChatNodes in the scene. Called before importing a file.
    /// </summary>
    private void ClearScreen() {
        outputText.AddLine("Clearing all nodes...");
        GameObject[] allNodes = GameObject.FindGameObjectsWithTag("Node");

        foreach (GameObject node in allNodes) {
            node.SetActive(false);
            Destroy(node);
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitApplication() {
        Application.Quit();
    }

    #endregion

    #region Topological Sort Functions

    /* 
     * Utilizes DFS to topologically sort all chatnodes, such that all predecessors of a given node appear before it in the order.
     * This is required due to the way the VSCS-II chat system build the chattree from the JSON; they must be ordered topologically before saving them to JSON.
     * ALG REF: https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search
     * Returns: Nothing; stores nodes in local variable sortedNodes. Cleaner signatures. 
     */
    private void TopologicalSortNodes(ChatNode[] nodesToSort, ChatNode headNode) {

        //set up the marks, all false by default 
        temporaryMarks = new Dictionary<string, bool>();
        permanentMarks = new Dictionary<string, bool>();
        foreach(ChatNode node in nodesToSort) {
            temporaryMarks[node.GetID()] = false;
            permanentMarks[node.GetID()] = false;
        }

        //DFS topological sort, loop until all a nodes permanently marked
        sortedNodes = new List<ChatNode>();

        VisitNode(headNode);
    }

    /// <summary>
    /// Support method. Resursively calls Visit() on direct descendants
    /// </summary>
    /// <param name="n">The node to visit.</param>
    private void VisitNode(ChatNode n) {

        if (permanentMarks[n.GetID()])
            return;
        if (temporaryMarks[n.GetID()]) {
            outputText.AddLine("ERROR EXPORTING: A cycle in the graph was detected. Conversations must be directed acyclic graphs (DAGs).");
            Debug.LogError("ERROR EXPORTING: A cycle in the graph was detected. Conversations must be directed acyclic graphs (DAGs).");
            return;
        }

        temporaryMarks[n.GetID()] = true;

        foreach(ChatNode m in n.GetDescendantNodes()) {
            VisitNode(m);
        }

        temporaryMarks[n.GetID()] = false;
        permanentMarks[n.GetID()] = true;

        sortedNodes.Insert(0, n);

    }

    /// <summary>
    /// Support method. Returns false if any node is not marked permanently, true if all are marked.
    /// </summary>
    /// <param name="nodes">A list of all nodes to check.</param>
    /// <returns>True if all nodes have been permanently marked.</returns>
    private bool AllNodesMarkedPermanent(ChatNode[] nodes) {
        foreach (ChatNode node in nodes) {
            if (permanentMarks[node.GetID()] == false)
                return false;
        }

        return true;
    }

    #endregion
}
