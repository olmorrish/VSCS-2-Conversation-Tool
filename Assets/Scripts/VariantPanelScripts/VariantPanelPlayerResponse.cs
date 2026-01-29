using System.Collections.Generic;

public class VariantPanelPlayerResponse : VariantPanel {

    public TMPro.TMP_InputField correctResponseInputField;
    public TMPro.TMP_InputField speakerInputField;
    public TMPro.TMP_InputField wrongReaction1InputField;
    public TMPro.TMP_InputField wrongReaction2InputField;
    public TMPro.TMP_InputField wrongReaction3InputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add(Constants.KEY_EXPECTED_RESPONSE, correctResponseInputField.text); // TODO rename to "expected"
        ret.Add(Constants.KEY_SPEAKER, speakerInputField.text);
        ret.Add(Constants.KEY_WRONG_REACTION_1, wrongReaction1InputField.text);
        ret.Add(Constants.KEY_WRONG_REACTION_2, wrongReaction2InputField.text);
        ret.Add(Constants.KEY_WRONG_REACTION_3, wrongReaction3InputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add(Constants.KEY_NEXT_NODE, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case Constants.KEY_EXPECTED_RESPONSE:
                    correctResponseInputField.text = pair.Value;
                    break;
                case Constants.KEY_SPEAKER:
                    speakerInputField.text = pair.Value;
                    break;
                case Constants.KEY_WRONG_REACTION_1:
                    wrongReaction1InputField.text = pair.Value;
                    break;
                case Constants.KEY_WRONG_REACTION_2:
                    wrongReaction2InputField.text = pair.Value;
                    break;
                case Constants.KEY_WRONG_REACTION_3:
                    wrongReaction3InputField.text = pair.Value;
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {
        return nextNub.connectedNub == null ? new List<ChatNode> { } : // no connection => no descendants
            new List<ChatNode> { nextNub.connectedNub.GetParentChatNode() };
    }

    public override List<ConnectionNub> GetNubs() {
        return new List<ConnectionNub>() { nextNub };
    }
}
