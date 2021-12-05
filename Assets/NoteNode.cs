using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteNode : MonoBehaviour {

    public TMPro.TMP_InputField noteTextInputField;
    public Toggle toggle;
    public Image bodyPanelImage;

    // Update is called once per frame
    void Update() {
        bodyPanelImage.color = toggle.isOn ? Color.yellow : Color.white;
    }

    /// <summary>
    /// Returns the data as a dictionary indexed by value names. 
    /// This is similar to the VariantPanel exports and is saved in the same fashion, but without the interface. 
    /// </summary>
    /// <returns>The note data as a dictionary.</returns>
    public Dictionary<string, string> GetNoteData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();

        ret.Add("text", noteTextInputField.text);
        ret.Add("todobool", toggle.isOn ? "true" : "false");

        return ret;
    }

    /// <summary>
    /// Sets the data from a dictionary that was serialized. 
    /// This is similar to the VariantPanel imports and is loaded in the same fashion, but without the interface. 
    /// </summary>
    /// <param name="savedData">Dictionary containing the saved note data.</param>
    public void PopulateNoteData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "text":
                    noteTextInputField.text = pair.Value.ToString();
                    break;
                case "todobool":
                    toggle.isOn = pair.Value.Equals("true");
                    break;
            }
        }
    }
}
