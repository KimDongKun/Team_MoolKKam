using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCModel
{
    public Texture2D NPCImage { get; set; }
    public string Name { get; set; }
    public List<string> NPCTalk { get; set; }
    public bool isTrigger { get; set; }

    public NPCModel(string name, Texture2D npcImage,List<string> talks = null)
    {
        Name = name;
        NPCImage = npcImage;
        NPCTalk = talks;
    }

}
