using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChatNodeType {
    AddBCPFile,             //makes a BCP reserve file available
    AddContact,             //adds a contact for player access
    AddFeedArticle,         //makes a Feed reserve article available
    AddLogEntry,
    Audio,
    BranchOnPlayerInput,    //waits for player to select a choice, then changes path based on choice
    BranchOnStoredBool,     //queries a boolean in backend and changes path based on it
    Delay,                  //waits for a set amount of time specified by delayLength
    Dialogue,               //presents text to the player
    NewSpeaker,
    PlayerResponse,         //waits for a player text input and loops until it is correct
    StoreBool               //writes a boolean in the backend
}

public class ChatNode : MonoBehaviour {

    //public VariantPanel currentVariantPanel;

    // Start is called before the first frame update
    void Start() {
        

    }

    // Update is called once per frame
    void Update() {
        
    }
}
