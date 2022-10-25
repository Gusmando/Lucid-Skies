using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class degines all inventory-related
//constants and it will contain all inventory items in an
//array of custon structs
public class InventoryConstants : MonoBehaviour 
{
    //Each inventory item contains the name of the item,
    //the item's type, an image of the item, and a mesh
    [System.Serializable]
    public struct InventoryItem
    {
        public string name;
        public int itemType;
        public Sprite image;
        public Mesh mesh;
    }

    [Header("Assignments")]
    public InventoryManager inventoryManager;
    public InventoryItem[] inventoryItems;

    //Constants define int codes for different
    //inventory types
    public const int INV_GEN_TYPE = 100;
    public const int  INV_HEAD_TYPE = 101;
    public const int  INV_EYES_TYPE = 102;
    public const int  INV_BROWS_TYPE = 103;
    public const int  INV_MOUTH_TYPE = 104;
    public const int  INV_TORSO_TYPE = 105;
    public const int  INV_COVER_TYPE = 106;
    public const int  INV_LEGS_TYPE = 107;
    public const int  INV_FEET_TYPE = 108;
    public const int  INV_RING_TYPE = 109;   

    //This dictionary will be used to hold the actual inventory item
    //structs, and because they are serializable they will be revealed
    //in the inspector
    private Dictionary <string,InventoryItem> inventoryDictionary;

    //Function instantiates the inventory item dictionary and assigns
    //the manager variable
    private void Awake() 
    {
        inventoryDictionary = new Dictionary<string,InventoryItem>();
        inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();

        foreach (InventoryItem item in inventoryItems)
        {
            inventoryDictionary.Add(item.name,item);
        }
    }

    //Used to fetch and return the correct item from the dictionary
    //based on its name
    public InventoryItem getItem(string toSearch)
    {
        return inventoryDictionary[toSearch];
    }
    
    //Function used to return a random item from the item dictionary
    public InventoryItem getRandItem()
    {
        //Generate the number
        System.Random random = new System.Random();

        //Creating a list with all of the item names
        List<string> itemNames = new List<string>(inventoryDictionary.Keys);
        
        //Generating a random name
        int randIdx = random.Next(itemNames.Count);
        string randName = itemNames[randIdx];

        while(randName.Contains("Default") || inventoryManager.checkOwnership(randName))
        {
            randIdx = random.Next(itemNames.Count);
            randName = itemNames[randIdx];
        }

        //Returning the inventory item
        return inventoryDictionary[randName];
    }
}
