using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class simply serves as structure for saving
//player data to a class, enables trivial 
public class DBPlayer
{
    public Vector3 position;
    public string equippedHead;
    public string equippedEyes;
    public string equippedBrows;
    public string equippedMouth;
    public string equippedTorso;
    public string equippedCover;
    public string equippedLegs;
    public string equippedFeet;
    public string sessionKey;
    public List<string> equippedRings = new List<string>();
    public int puffCount;
}
