using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelDialogue : VariantPanel {

    [Header("Internal References")]
    TMPro.TMP_InputField speakerInputField;
    TMPro.TMP_InputField dialogueInputField;

    public override Dictionary<string, string> GetNodeData() {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("speaker", speakerInputField.text);
        ret.Add("dialogue", dialogueInputField.text);   //TODO post processing on the dialogue for quotes and segments and such, can be done when serializing to JSON
        return ret;
    }

    public override void PopulateNodeData(Dictionary<string, string> savedData) {
        throw new System.NotImplementedException();
    }
}
