using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelAudio : VariantPanel {

    public TMPro.TMP_InputField audioClipNameInputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("param", audioClipNameInputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        Debug.Log("trying to populate an audio node. How many keys: " + savedData.Values.Count);

        foreach(KeyValuePair<string, string> pair in savedData) {
            Debug.Log("Name: " + pair.Key);
            switch (pair.Key) {
                case "param":
                    Debug.Log("Trying to populate audio name field...");
                    audioClipNameInputField.text = pair.Value.ToString();
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        if (nextNub.connectedNub == null) {
            return new List<ChatNode> { }; //no connection => no descendants
        }
        else {
            return new List<ChatNode> { nextNub.connectedNub.GetParentChatNode() };
        }
    }
}
