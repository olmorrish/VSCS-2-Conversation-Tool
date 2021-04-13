using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelPlayerResponse : VariantPanel {

    public TMPro.TMP_InputField correctResponseInputField;
    public TMPro.TMP_InputField speakerInputField;
    public TMPro.TMP_InputField wrongReaction1InputField;
    public TMPro.TMP_InputField wrongReaction2InputField;
    public TMPro.TMP_InputField wrongReaction3InputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("param", correctResponseInputField.text);
        ret.Add("speaker", speakerInputField.text);
        ret.Add("wrongreaction1", wrongReaction1InputField.text);
        ret.Add("wrongreaction2", wrongReaction2InputField.text);
        ret.Add("wrongreaction3", wrongReaction3InputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "param":
                    correctResponseInputField.text = pair.Value.ToString();
                    break;
                case "speaker":
                    speakerInputField.text = pair.Value.ToString();
                    break;
                case "wrongreaction1":
                    wrongReaction1InputField.text = pair.Value.ToString();
                    break;
                case "wrongreaction2":
                    wrongReaction2InputField.text = pair.Value.ToString();
                    break;
                case "wrongreaction3":
                    wrongReaction3InputField.text = pair.Value.ToString();
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
