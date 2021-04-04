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
        ret.Add("dialogue", dialogueInputField.text);   //TODO post processing on the dialogue for quotes and segments and such, can be done when serializing to JSON

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {
        throw new System.NotImplementedException();
    }

    public override ChatNode[] GetDescendantChatNodes() {

        if(nextNub.connectedNub == null) {
            return new ChatNode[] { }; //no connection => no descendants
        }
        else {
            return new ChatNode[] { nextNub.connectedNub.GetParentChatNode() };
        }

    }

}
