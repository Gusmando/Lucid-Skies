using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions;

//Class manages the AI and cosmetic equip set
//for dummy players in the world
public class DummyPlayer : MonoBehaviour
{
    public SkinnedMeshRenderer headObj;
    public SkinnedMeshRenderer eyesObj;
    public SkinnedMeshRenderer browsObj;
    public SkinnedMeshRenderer mouthObj;
    public SkinnedMeshRenderer topObj;
    public SkinnedMeshRenderer coverObj;
    public SkinnedMeshRenderer legsObj;
    public SkinnedMeshRenderer feetObj;

    public Text puffText;

    public InventoryConstants inventoryConstants;

    public string equippedHead;
    public string equippedEyes;
    public string equippedBrows;
    public string equippedMouth;
    public string equippedTorso;
    public string equippedCover;
    public string equippedLegs;
    public string equippedFeet;

    public List<string> equippedRings = new List<string>();

    public int puffCount;
    public string sessionKey;
    public Query changingEventQ;
    public Query deletionEventQ;


    private FirebaseManager firebaseManager;

    private void Awake() 
    {   
        firebaseManager = GameObject.FindGameObjectWithTag("Firebase Manager").GetComponent<FirebaseManager>();
        inventoryConstants = GameObject.FindGameObjectWithTag("Inventory Constants").GetComponent<InventoryConstants>();
    }

    //Update puff count above dummy's head
    private void Update() 
    {
        puffText.text = "Puffs				" + "X" + puffCount;
    }

    //Used to subscribe to the change and deletion firebase events
    public void firebaseSub()
    {
        changingEventQ = firebaseManager.reference.Child(sessionKey);
        deletionEventQ = firebaseManager.reference.Child(sessionKey).LimitToFirst(1);
        changingEventQ.ChildChanged += playerChanged;
        deletionEventQ.ChildRemoved += playerDeleted;
    }

    //When the player is changed on the database
    public void playerChanged(object sender, ChildChangedEventArgs eventArgs)
    {
        if(eventArgs.DatabaseError != null)
        {
			Debug.LogError(eventArgs.DatabaseError.Message);
        	return;
		}

		eventArgs.Snapshot.Reference.Parent.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            //Update player info, position, and equip loadout based on changed
            //db entry
            DBPlayer newPlayerInfo = JsonUtility.FromJson<DBPlayer>(task.Result.GetRawJsonValue());
            this.transform.position = newPlayerInfo.position;
            populateFromDB(newPlayerInfo);
            refreshEquips();
        });
    }

    //Delete the dummy object
    public void playerDeleted(object sender, ChildChangedEventArgs eventArgs)
    {

        Destroy(this.gameObject);
    }

    //When the dummy object is destroyed,
    //unsub events
    private void OnDestroy() 
    {
        changingEventQ.ChildChanged -= playerChanged;
        deletionEventQ.ChildRemoved -= playerDeleted;
    }

    //Change equips to passed in db player object
    public void populateFromDB(DBPlayer newPlayerInfo)
    {
        equippedHead = newPlayerInfo.equippedHead;
        equippedEyes = newPlayerInfo.equippedEyes;
        equippedBrows = newPlayerInfo.equippedBrows;
        equippedMouth = newPlayerInfo.equippedMouth;
        equippedTorso = newPlayerInfo.equippedTorso;
        equippedCover = newPlayerInfo.equippedCover;

        equippedLegs = newPlayerInfo.equippedLegs;
        equippedFeet = newPlayerInfo.equippedFeet;
        equippedRings = newPlayerInfo.equippedRings;
        puffCount = newPlayerInfo.puffCount;

        sessionKey = newPlayerInfo.sessionKey;
    }

    //Equip the actual items to the dummy
    public void refreshEquips()
    {
        equipItem(headObj,equippedHead);
        equipItem(eyesObj,equippedEyes);
        equipItem(browsObj,equippedBrows);
        equipItem(mouthObj,equippedMouth);
        equipItem(topObj,equippedTorso);
        equipItem(coverObj,equippedCover);
        equipItem(legsObj,equippedLegs);
        equipItem(feetObj,equippedFeet);
    }

    //Equipping the item based on passed in parameters
    public void equipItem(SkinnedMeshRenderer itemMeshRend, string itemName)
    {
        InventoryConstants.InventoryItem newItem = inventoryConstants.getItem(itemName);
        itemMeshRend.sharedMesh = newItem.mesh;
    }


}
