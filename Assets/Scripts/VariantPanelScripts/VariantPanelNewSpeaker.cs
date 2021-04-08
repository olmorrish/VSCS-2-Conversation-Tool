using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelNewSpeaker : VariantPanel {

    public TMPro.TMP_InputField newSpeakerNameInputField;
    public TMPro.TMP_InputField newSpeakerBNETIDInputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("param", newSpeakerNameInputField.text);
        ret.Add("bnetid", newSpeakerBNETIDInputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {
        throw new System.NotImplementedException();
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
