using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NPCViewModel
{
    private NPCModel npcModel;
    public bool isTrigger => npcModel.isTrigger;

    public event Action<string> OnDialogueUpdate;

    public int dialogueIndex = 0;

    public string Name
    {
        get { return npcModel.Name; }
        set
        {
            if (npcModel.Name != value)
            {
                npcModel.Name = value;
            }
        }
    }
    public Texture2D Image
    {
        get { return npcModel.NPCImage; }
        set
        {
            if (npcModel.NPCImage)
            {
                npcModel.NPCImage = value;
                Debug.Log("NPC :" + npcModel.Name);
            }
        }
    }

    public List<string> Talks => npcModel.NPCTalk;

    
    public void ShowNextDialogue()
    {
        if (dialogueIndex < Talks.Count && npcModel.isTrigger)
        {
            OnDialogueUpdate?.Invoke(Talks[dialogueIndex++]);
        }
        else
        {
            ResetDialoque();
        }
    }

    public void ResetDialoque()
    {
        npcModel.isTrigger = false;
        dialogueIndex = 0;
    }

    public NPCViewModel(NPCModel model)
    {
        npcModel = model;
    }

}
