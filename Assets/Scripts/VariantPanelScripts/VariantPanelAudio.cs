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

        foreach(KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "param":
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

    public override List<ConnectionNub> GetNubs() {
        return new List<ConnectionNub>() { nextNub };
    }
}
