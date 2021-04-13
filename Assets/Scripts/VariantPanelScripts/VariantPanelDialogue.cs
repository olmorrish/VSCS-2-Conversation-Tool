using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelDialogue : VariantPanel {

    [Header("Internal References")]
    public TMPro.TMP_InputField speakerInputField;
    public TMPro.TMP_InputField dialogueInputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("speaker", speakerInputField.text);
        ret.Add("contents", dialogueInputField.text);   //TODO post processing on the dialogue for quotes and segments and such, can be done when serializing to JSON

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {
        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "speaker":
                    speakerInputField.text = pair.Value.ToString();
                    break;
                case "contents":
                    dialogueInputField.text = pair.Value.ToString();
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        if(nextNub.connectedNub == null) {
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
