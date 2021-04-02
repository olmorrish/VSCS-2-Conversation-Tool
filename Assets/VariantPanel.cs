using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VariantPanel {

    public abstract Dictionary<string, string> GetVariantPanelData();
    public abstract void PopulateVariantPanelData(Dictionary<string, string> savedData);
}
