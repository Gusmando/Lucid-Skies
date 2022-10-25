using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Class will be used to define draggable inventory ui items
//It employs the use of several interfaces, such as the drag
//and pointer handlers
public class InventoryItemDrag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //Several private members will be assigned on awake
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private InventoryManager inventoryManager;

    //The transform of the parent object as well as
    //the name and type of the draggable item
    public Transform parent;
    public int itemType;
    public string itemName;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
        canvas = GameObject.FindGameObjectWithTag("Inventory Canvas").GetComponent<Canvas>();
    }
    

    public void OnPointerDown(PointerEventData eventData)
    {
                
    }

    //Event function executes when beginning to drag an
    //inventory ui item
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Grabbing a refence to the current parent object
        GameObject parentObj = parent.gameObject;

        //Dragging the item from an equipable inventory slot
        if(parentObj.GetComponent<InventorySlot>().slotType != InventoryConstants.INV_GEN_TYPE)
        {
            //Item must be unequipped and the slot must be set to unoccupied
            inventoryManager.unEquipItem(itemName,itemType);
            transform.parent = canvas.gameObject.transform;
            parentObj.GetComponent<InventorySlot>().occupied = false;
        }
        else
        {
            
            inventoryManager.removeFromCollection(itemName);
            GameObject oldPar = transform.parent.gameObject;     
            transform.parent = canvas.gameObject.transform;
            Destroy(oldPar);
        }

        canvasGroup.blocksRaycasts = false;
    }

    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if(parent.GetComponent<InventorySlot>().slotType != InventoryConstants.INV_GEN_TYPE)
        {
            transform.parent = parent;
            inventoryManager.equipItem(itemName,itemType);
            rectTransform.anchoredPosition = new Vector2(0,0);
        }
        else
        {
            inventoryManager.addToCollection(itemName);
            Destroy(this.gameObject);
        }
        
        canvasGroup.blocksRaycasts = true;

    }
}
