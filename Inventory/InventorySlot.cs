using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Class manages behaviors and events related to
//inventory slots, it employs the i drop handler
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotType;
    public bool occupied = false;
    private InventoryManager inventoryManager;

    private void Awake() 
    {
        inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
    }

    //Function fires when dropping item onto slot
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            //Referencing dropped item as well as its previous slot
            GameObject itemGameObj = eventData.pointerDrag;
            InventoryItemDrag invItem = itemGameObj.GetComponent<InventoryItemDrag>(); 
            InventorySlot invItemPrevSlot = invItem.parent.gameObject.GetComponent<InventorySlot>(); 
            
            //If the current slot is occupied and the dropped item
            //is of the same slot
            if(occupied && slotType != InventoryConstants.INV_GEN_TYPE && invItem.itemType == slotType)
            {
                GameObject equippedItem = transform.GetChild(0).gameObject;
                string equippedItemName = equippedItem.GetComponent<InventoryItemDrag>().itemName;
                int equippedItemType = equippedItem.GetComponent<InventoryItemDrag>().itemType;
                
                //Different types
                if(equippedItemType != invItem.itemType)
                {
                    return;
                }

                //Ensuring updated parent references
                equippedItem.GetComponent<InventoryItemDrag>().parent = invItem.parent;
                equippedItem.transform.parent = invItem.parent;
                equippedItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
                
                //Swapping the items from collection to equp slot based on item origin
                if(slotType != InventoryConstants.INV_GEN_TYPE)
                {
                    inventoryManager.unEquipItem(equippedItemName,equippedItemType);
                    inventoryManager.addToCollection(equippedItemName);
                }
                else if(slotType == InventoryConstants.INV_GEN_TYPE && invItemPrevSlot.slotType != InventoryConstants.INV_GEN_TYPE)
                {
                    inventoryManager.equipItem(equippedItemName,equippedItemType);
                    inventoryManager.removeFromCollection(equippedItemName);
                }

                invItemPrevSlot.occupied = true;
            }

            //Empty slot
            if(slotType != InventoryConstants.INV_GEN_TYPE && invItem.itemType == slotType || slotType == InventoryConstants.INV_GEN_TYPE)
            {
                invItem.parent = this.transform;
                
                if(slotType != InventoryConstants.INV_GEN_TYPE)
                {
                    occupied = true;
                }
            }            
        }
    }
}
