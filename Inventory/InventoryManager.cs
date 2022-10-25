using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class is responsible for maintaining the
//player's inventory through consumption and addition
//of new items
public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public GameObject iconPrefab;

    public GameObject collectionParent;
	public PlayerController playerController;

    public InventorySlot headSlot;
    public InventorySlot eyesSlot;
    public InventorySlot browsSlot;
    public InventorySlot mouthSlot;
    public InventorySlot topSlot;
    public InventorySlot coverSlot;
    public InventorySlot legsSlot;
    public InventorySlot feetSlot;

    public SkinnedMeshRenderer headObj;
    public SkinnedMeshRenderer eyesObj;
    public SkinnedMeshRenderer browsObj;
    public SkinnedMeshRenderer mouthObj;
    public SkinnedMeshRenderer topObj;
    public SkinnedMeshRenderer coverObj;
    public SkinnedMeshRenderer legsObj;
    public SkinnedMeshRenderer feetObj;

    public InventoryConstants inventoryConstants;
    
    public List<string> collectionItems = new List<string>();

    public string equippedHead = "Default Hair";
    public string equippedEyes = "Default Eyes";
    public string equippedBrows = "Default Brows";
    public string equippedMouth = "Default Mouth";
    public string equippedTorso = "Default Top";
    public string equippedCover = "Default Cover";
    public string equippedLegs = "Default Legs";
    public string equippedFeet = "Default Feet";
    public List<string> equippedRings = new List<string>();

    public int healthPotionCount;
    public int staminaPotionCount;
    public int puffCount;

    public int healthPotionLimit;
    public int staminaPotionLimit;

    private FirebaseManager firebaseManager;

    private void Awake()
    {
        firebaseManager = GameObject.FindGameObjectWithTag("Firebase Manager").GetComponent<FirebaseManager>();
        equippedHead = "Default Hair";
        equippedEyes = "Default Eyes";
        equippedBrows = "Default Brows";
        equippedMouth = "Default Mouth";
        equippedTorso = "Default Top";
        equippedCover = "Default Cover";
        equippedLegs = "Default Legs";
        equippedFeet = "Default Feet";
    }

    //Function utilized for checking if the passed in item is
    //currently in the player's collection or equip set.
    public bool checkOwnership(string itemName)
    {
        if(!collectionItems.Contains(itemName) 
        && itemName != equippedHead && itemName != equippedEyes
        && itemName != equippedBrows && itemName != equippedMouth
        && itemName != equippedTorso && itemName != equippedCover
        && itemName != equippedLegs && itemName != equippedFeet
        && !equippedRings.Contains(itemName))
        {
            return false1;
        }
        else
        {
            return true;
        }
    }

    //This function will be used to add items to the player's
    //current collection
    public void addToCollection(string itemName)
    {
        //If the item is new
        if(!checkOwnership(itemName))
        {
            //Create collection icon and adjust visual
            //components accordingly
            GameObject newCollectionIcon = Instantiate(slotPrefab,collectionParent.transform).transform.GetChild(0).gameObject;
            InventoryConstants.InventoryItem newItem = inventoryConstants.getItem(itemName);
            
            InventoryItemDrag inventoryItem = newCollectionIcon.GetComponent<InventoryItemDrag>();

            Sprite newSprite = newItem.image;

            newCollectionIcon.GetComponent<Image>().sprite = newSprite;
            
            inventoryItem.itemName = newItem.name;
            inventoryItem.itemType = newItem.itemType;
            inventoryItem.parent = collectionParent.transform;
        }

        collectionItems.Add(itemName);
    }

    //Removing an item from the collection
    public void removeFromCollection(string itemName)
    {
        if(!checkOwnership(itemName))
        {
            return;
        }

        collectionItems.Remove(itemName);
    }

    //This function will equip the passed in item by checking
    //the lookup inventory constants table and changing the 
    //appropriate mesh
    public void equipItem(string itemName, int itemType)
    {
        //Different slot dependent on item type
        switch (itemType)
        {
        
            case InventoryConstants.INV_HEAD_TYPE:
                equippedHead = itemName;

                InventoryConstants.InventoryItem defHead = inventoryConstants.getItem(itemName);
                headObj.sharedMesh = defHead.mesh;

                break;

            case InventoryConstants.INV_EYES_TYPE:
                equippedEyes = itemName;

                InventoryConstants.InventoryItem defEyes = inventoryConstants.getItem(itemName);
                eyesObj.sharedMesh = defEyes.mesh;

                break;

            case InventoryConstants.INV_BROWS_TYPE:
                equippedBrows = itemName;

                InventoryConstants.InventoryItem defBrows = inventoryConstants.getItem(itemName);
                browsObj.sharedMesh = defBrows.mesh;

                break;

            case InventoryConstants.INV_MOUTH_TYPE:
                equippedMouth = itemName;

                InventoryConstants.InventoryItem defMouth = inventoryConstants.getItem(itemName);
                mouthObj.sharedMesh = defMouth.mesh;

                break;

            case InventoryConstants.INV_TORSO_TYPE:
                equippedTorso = itemName;

                InventoryConstants.InventoryItem defTop = inventoryConstants.getItem(itemName);
                topObj.sharedMesh = defTop.mesh;

                break;

            case InventoryConstants.INV_LEGS_TYPE:
                equippedLegs = itemName;

                InventoryConstants.InventoryItem defLegs = inventoryConstants.getItem(itemName);
                legsObj.sharedMesh = defLegs.mesh;

                break;

            case InventoryConstants.INV_COVER_TYPE:
                equippedCover = itemName;

                InventoryConstants.InventoryItem defCover = inventoryConstants.getItem(itemName);
                coverObj.sharedMesh = defCover.mesh;

                break;

            case InventoryConstants.INV_FEET_TYPE:
                equippedFeet = itemName;

                InventoryConstants.InventoryItem defFeet = inventoryConstants.getItem(itemName);
                feetObj.sharedMesh = defFeet.mesh;

                break;

            case InventoryConstants.INV_RING_TYPE:
                equippedRings.Add(itemName);
                
                break;
        }

        //Sync current player loadout to database
        firebaseManager.addPlayertoSession();
    }

    //When a new item is equipped to the player, a new icon must
    //be simultaneously created
    public void equipItemAndAddIcon(string itemName, int itemType)
    {
        bool defaultItem = itemName.Contains("Default");

        switch (itemType)
        {

            case InventoryConstants.INV_HEAD_TYPE:
                equippedHead = itemName;

                InventoryConstants.InventoryItem defHead = inventoryConstants.getItem(itemName);
                headObj.sharedMesh = defHead.mesh;
                
                if(!defaultItem)
                {
                    createIcon(headSlot,InventoryConstants.INV_HEAD_TYPE,defHead.name,defHead.image);
                }
                break;

            case InventoryConstants.INV_EYES_TYPE:
                equippedEyes = itemName;

                InventoryConstants.InventoryItem defEyes = inventoryConstants.getItem(itemName);
                eyesObj.sharedMesh = defEyes.mesh;
                
                if(!defaultItem)
                {
                    createIcon(eyesSlot,InventoryConstants.INV_EYES_TYPE,defEyes.name, defEyes.image);
                }
                break;

            case InventoryConstants.INV_BROWS_TYPE:
                equippedBrows = itemName;

                InventoryConstants.InventoryItem defBrows = inventoryConstants.getItem(itemName);
                browsObj.sharedMesh = defBrows.mesh;

                if(!defaultItem)
                {
                    createIcon(browsSlot,InventoryConstants.INV_BROWS_TYPE,defBrows.name, defBrows.image);
                }
                break;

            case InventoryConstants.INV_MOUTH_TYPE:
                equippedMouth = itemName;

                InventoryConstants.InventoryItem defMouth = inventoryConstants.getItem(itemName);
                mouthObj.sharedMesh = defMouth.mesh;
                
                if(!defaultItem)
                {
                    createIcon(mouthSlot,InventoryConstants.INV_MOUTH_TYPE,defMouth.name, defMouth.image);
                }
                break;

            case InventoryConstants.INV_TORSO_TYPE:
                equippedTorso = itemName;

                InventoryConstants.InventoryItem defTop = inventoryConstants.getItem(itemName);
                topObj.sharedMesh = defTop.mesh;

                if(!defaultItem)
                {
                    createIcon(topSlot,InventoryConstants.INV_TORSO_TYPE,defTop.name, defTop.image);
                }

                break;

            case InventoryConstants.INV_LEGS_TYPE:
                equippedLegs = itemName;

                InventoryConstants.InventoryItem defLegs = inventoryConstants.getItem(itemName);
                legsObj.sharedMesh = defLegs.mesh;
                
                if(!defaultItem)
                {
                    createIcon(legsSlot,InventoryConstants.INV_LEGS_TYPE,defLegs.name, defLegs.image);
                }

                break;

            case InventoryConstants.INV_COVER_TYPE:
                equippedCover = itemName;

                InventoryConstants.InventoryItem defCover = inventoryConstants.getItem(itemName);
                coverObj.sharedMesh = defCover.mesh;
               
                if(!defaultItem)
                {
                    createIcon(coverSlot,InventoryConstants.INV_COVER_TYPE,defCover.name, defCover.image);
                }
                break;

            case InventoryConstants.INV_FEET_TYPE:
                equippedFeet = itemName;

                InventoryConstants.InventoryItem defFeet = inventoryConstants.getItem(itemName);
                feetObj.sharedMesh = defFeet.mesh;
                
                if(!defaultItem)
                {
                    createIcon(feetSlot,InventoryConstants.INV_FEET_TYPE,defFeet.name, defFeet.image);
                }
                break;

            case InventoryConstants.INV_RING_TYPE:
                equippedRings.Add(itemName);
                
                break;
        }

        firebaseManager.addPlayertoSession();
    }

    //Function for unequipping itemd, depending on type,
    //setting to default
    public void unEquipItem(string itemName, int itemType)
    {
        switch (itemType)
        {
        
            case InventoryConstants.INV_HEAD_TYPE:
                equippedHead = "Default Hair";

                InventoryConstants.InventoryItem defHead = inventoryConstants.getItem("Default Hair");
                headObj.sharedMesh = defHead.mesh;

                break;

            case InventoryConstants.INV_EYES_TYPE:
                equippedEyes = "Default Eyes";

                InventoryConstants.InventoryItem defEyes = inventoryConstants.getItem("Default Eyes");
                eyesObj.sharedMesh = defEyes.mesh;
                
                break;

            case InventoryConstants.INV_BROWS_TYPE:
                equippedBrows = "Default Brows";

                InventoryConstants.InventoryItem defBrows = inventoryConstants.getItem("Default Brows");
                browsObj.sharedMesh = defBrows.mesh;

                break;

            case InventoryConstants.INV_MOUTH_TYPE:
                equippedMouth = "Default Mouth";

                InventoryConstants.InventoryItem defMouth = inventoryConstants.getItem("Default Mouth");
                mouthObj.sharedMesh = defMouth.mesh;

                break;

            case InventoryConstants.INV_TORSO_TYPE:
                equippedTorso = "Default Top";

                InventoryConstants.InventoryItem defTop = inventoryConstants.getItem("Default Top");
                topObj.sharedMesh = defTop.mesh;

                break;

            case InventoryConstants.INV_LEGS_TYPE:
                equippedLegs = "Default Legs";
                
                InventoryConstants.InventoryItem defLegs = inventoryConstants.getItem("Default Legs");
                legsObj.sharedMesh = defLegs.mesh;

                break;

            case InventoryConstants.INV_COVER_TYPE:
                equippedCover = "Default Cover";

                InventoryConstants.InventoryItem defCover = inventoryConstants.getItem("Default Cover");
                coverObj.sharedMesh = defCover.mesh;

                break;

            case InventoryConstants.INV_FEET_TYPE:
                equippedFeet = "Default Feet";

                InventoryConstants.InventoryItem defFeet = inventoryConstants.getItem("Default Feet");
                headObj.sharedMesh = defFeet.mesh;

                break;

            case InventoryConstants.INV_RING_TYPE:
                equippedRings.Remove(itemName);
                break;

            default:
                break;
        }
    }

    //Set all equipped items to their default settings
    public void unEquipAllItems()
    {
        equippedHead = "Default Hair";
        equippedEyes = "Default Eyes";
        equippedBrows = "Default Brows";
        equippedMouth = "Default Mouth";
        equippedTorso = "Default Top";
        equippedCover = "Default Cover";
        equippedLegs = "Default Legs";
        equippedFeet = "Default Feet";

        InventoryConstants.InventoryItem defHead = inventoryConstants.getItem("Default Hair");
        headObj.sharedMesh = defHead.mesh;

        InventoryConstants.InventoryItem defEyes = inventoryConstants.getItem("Default Eyes");
        eyesObj.sharedMesh = defEyes.mesh;

        InventoryConstants.InventoryItem defBrows = inventoryConstants.getItem("Default Brows");
        browsObj.sharedMesh = defBrows.mesh;

        InventoryConstants.InventoryItem defMouth = inventoryConstants.getItem("Default Mouth");
        mouthObj.sharedMesh = defMouth.mesh;

        InventoryConstants.InventoryItem defTop = inventoryConstants.getItem("Default Top");
        topObj.sharedMesh = defTop.mesh;

        InventoryConstants.InventoryItem defLegs = inventoryConstants.getItem("Default Legs");
        legsObj.sharedMesh = defLegs.mesh;

        InventoryConstants.InventoryItem defCover = inventoryConstants.getItem("Default Cover");
        coverObj.sharedMesh = defCover.mesh;

        InventoryConstants.InventoryItem defFeet = inventoryConstants.getItem("Default Feet");
        headObj.sharedMesh = defFeet.mesh;

        if(headSlot.occupied)
        {
            Destroy(headSlot.transform.GetChild(0).gameObject);
            headSlot.occupied = false;
        }

        if(eyesSlot.occupied)
        {
            Destroy(eyesSlot.transform.GetChild(0).gameObject);
            eyesSlot.occupied = false;
        }

        if(browsSlot.occupied)
        {
            Destroy(browsSlot.transform.GetChild(0).gameObject);
            browsSlot.occupied = false;
        }

        if(mouthSlot.occupied)
        {
            Destroy(mouthSlot.transform.GetChild(0).gameObject);
            mouthSlot.occupied = false;
        }

        if(topSlot.occupied)
        {
            Destroy(topSlot.transform.GetChild(0).gameObject);
            topSlot.occupied = false;
        }

        if(legsSlot.occupied)
        {
            Destroy(legsSlot.transform.GetChild(0).gameObject);
            legsSlot.occupied = false;
        }

        if(coverSlot.occupied)
        {
            Destroy(coverSlot.transform.GetChild(0).gameObject);
            coverSlot.occupied = false;
        }

        if(feetSlot.occupied)
        {
            Destroy(feetSlot.transform.GetChild(0).gameObject);
            feetSlot.occupied = false;
        }
    }

    //Helper function will be used to ceate an icon utilizing the passed
    //in parameters
    public void createIcon(InventorySlot slot, int itemType, string itemName, Sprite icon)
    {
        GameObject newHeadIcon = Instantiate(iconPrefab,slot.transform);
        newHeadIcon.GetComponent<Image>().sprite = icon;
        newHeadIcon.GetComponent<InventoryItemDrag>().itemName = itemName;
        newHeadIcon.GetComponent<InventoryItemDrag>().itemType = itemType;
        newHeadIcon.GetComponent<InventoryItemDrag>().parent = slot.transform;
        
        newHeadIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);

        slot.occupied = true;
    }

    //If below health potion limit, add to current stack
    public void addHealthPotion()
    {
        healthPotionCount ++;

        if(healthPotionCount > healthPotionLimit)
        {
            healthPotionCount = healthPotionLimit;
        }
    }

    //If below stamina potion limit, add to current stack
    public void addStaminaPotion()
    {
        staminaPotionCount ++;

        if(staminaPotionCount > staminaPotionLimit)
        {
            staminaPotionCount = staminaPotionLimit;
        }
    }

    //Add a puff to current count
    public void addPuff()
    {
        puffCount ++;
    }

    //Consume a health potion
    public bool useHealth()
    {
        if(healthPotionCount > 0)
        {
            healthPotionCount --;
            return true;
        }

        return false;
    }

    //Consume a stamina potion
    public bool useStamina()
    {
        if(staminaPotionCount > 0)
        {
            staminaPotionCount --;
            return true;
        }

        return false;
    }

    //Update potion limit according to passed in level
	public void updatePotions(int level){
		switch(level){
			case 1:
				healthPotionLimit = 6;
				staminaPotionLimit = 6;
				break;
			case 2:
				playerController.upgradePotionVal();
				break;
			case 3:
				healthPotionLimit = 8;
				staminaPotionLimit = 8;
				break;
		}
	}

}