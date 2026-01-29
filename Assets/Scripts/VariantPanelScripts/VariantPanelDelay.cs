using System.Collections.Generic;

public class VariantPanelDelay : VariantPanel {

    public TMPro.TMP_InputField delayAmountInputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add(Constants.KEY_DELAY_LENGTH, delayAmountInputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add(Constants.KEY_NEXT_NODE, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {
        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case Constants.KEY_DELAY_LENGTH:
                    delayAmountInputField.text = pair.Value;
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
