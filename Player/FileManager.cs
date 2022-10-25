using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//A savefile struct will contain all necessary information
//to resume a previous playthrough
[System.Serializable]
public struct SaveFile
{
	public bool[] puffsCollected;
	public bool[] staminaCollected;
	public bool[] healthCollected;
	public bool[] puzzleCompleted;

	public int[] levelUpgrades;
    public List<string> unlockedAbilities;

	public Vector3 position;
    public string equippedHead;
    public string equippedEyes;
    public string equippedBrows;
    public string equippedMouth;
    public string equippedTorso;
    public string equippedCover;
    public string equippedLegs;
    public string equippedFeet;

	public List<string> equippedRings;
    public List<string> inventoryCollection;
   
    public int puffCount;
	public int staminaCount;
	public int healthCount;

	public float health;
}

//The file manager class will manage saving and
//loading of a game playthrough
public class FileManager : MonoBehaviour
{
	public PuffPickup[] puffs;
	public Puzzle[] puzzles;
	public StaminaPickup[] staminaPotions;
	public HealthPickup[] healthPotions;
	public AbilityPickup[] abilityPickups;

	public InventoryManager inventoryManager;
	public PlayerController player;
	public ProgressionManager progressionManager;
	public SkillsManager skillsManager;

	private void Start() 
	{
		//Finding all instances of active items, 
		puffs = GameObject.FindObjectsOfType<PuffPickup>(true);
		puzzles = GameObject.FindObjectsOfType<Puzzle>(true);
		staminaPotions = GameObject.FindObjectsOfType<StaminaPickup>(true);
		healthPotions = GameObject.FindObjectsOfType<HealthPickup>(true);
		abilityPickups = GameObject.FindObjectsOfType<AbilityPickup>(true);

		//If the player chooses to load a game when starting
		if(PlayerOptions.load)
		{
			loadData();
		}
	}

	//Function will be called to save the player's information
	public void saveInfo()
	{
		SaveFile save = new SaveFile();
		
		save.unlockedAbilities = new List<string>();

		//Boolean array denotes collected items and completed
		//puzzles
		bool[] puffsCollected = new bool[puffs.Length];
		bool[] staminaCollected = new bool[staminaPotions.Length];
		bool[] healthCollected = new bool[healthPotions.Length];
		bool[] puzzlesCompleted = new bool[puzzles.Length];

		int index = 0;

		//Determining completed puzzles
		foreach(Puzzle puzzle in puzzles)
		{
			if(puzzle.cleared)
			{
				puzzlesCompleted[index] = true;
			}
			else
			{
				puzzlesCompleted[index] = false;
			}

			index++;
		}

		//Determining picked up puffs
		index = 0;
		foreach(PuffPickup puff in puffs)
		{
			if(puff.pickedUp)
			{
				puffsCollected[index] = true;
			}
			else
			{
				puffsCollected[index] = false;
			}

			index++;
		}

		index = 0;

		//Determining picked up health potions
		foreach(HealthPickup health in healthPotions)
		{
			if(health.pickedUp)
			{
				healthCollected[index] = true;
			}
			else
			{
				healthCollected[index] = false;
			}

			index++;
		}

		index = 0;
		//Determining picked up stamina potions
		foreach(StaminaPickup stamina in staminaPotions)
		{
			if(stamina.pickedUp)
			{
				staminaCollected[index] = true;
			}
			else
			{
				staminaCollected[index] = false;
			}

			index++;
		}

		//Saving any other playthrough-relevant information
		save.puffsCollected = puffsCollected;
		save.healthCollected = healthCollected;
		save.staminaCollected = healthCollected;
		save.puzzleCompleted = puzzlesCompleted;
		save.levelUpgrades = new int[4];
		save.levelUpgrades = skillsManager.upgradeLevels;

		//Determining current ability loadout
		foreach(Ability ability in progressionManager.abilities.Values)
		{
			if (ability.unlocked)
			{
				save.unlockedAbilities.Add(ability.name);
			}
		}

		save.position = player.transform.position;
		save.health = player.hp;
		save.equippedHead = inventoryManager.equippedHead;
		save.equippedEyes = inventoryManager.equippedEyes;
		save.equippedBrows = inventoryManager.equippedBrows;
		save.equippedMouth = inventoryManager.equippedMouth;
		save.equippedTorso = inventoryManager.equippedTorso;
		save.equippedCover = inventoryManager.equippedCover;
		save.equippedLegs = inventoryManager.equippedLegs;
		save.equippedFeet = inventoryManager.equippedFeet;
		save.equippedRings = inventoryManager.equippedRings;
		save.inventoryCollection = inventoryManager.collectionItems;
		save.puffCount = inventoryManager.puffCount;
		save.staminaCount = inventoryManager.staminaPotionCount;
		save.healthCount = inventoryManager.healthPotionCount;

		//Save the struct as a json, and write json to save file
		string saveJson = JsonUtility.ToJson(save);

		System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", saveJson);
		Debug.Log(Application.persistentDataPath);
	}

	//Function will be used to load data from the db
	public void loadData()
	{
		//No save file exists -> load game
		if(!System.IO.File.Exists(Application.persistentDataPath + "/PlayerData.json"))
		{
			SceneManager.LoadScene(1);
			return;
		}

		//Read json save file
		string playerDataJSON = System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerData.json");
		SaveFile loadedFile= JsonUtility.FromJson<SaveFile>(playerDataJSON);

		int index = 0;

		//Game is resumed by setting picked up items to inactive
		foreach(PuffPickup puff in puffs)
		{
			if(loadedFile.puffsCollected[index])
			{
				puff.gameObject.SetActive(false);
			}

			index ++;
		}

		index = 0;
		foreach(HealthPickup hPotion in healthPotions)
		{
			if(loadedFile.healthCollected[index])
			{
				hPotion.gameObject.SetActive(false);
			}

			index ++;
		}

		index = 0;
		foreach(StaminaPickup sPotion in staminaPotions)
		{
			if(loadedFile.staminaCollected[index])
			{
				sPotion.gameObject.SetActive(false);
			}

			index ++;
		}

		index = 0;

		//Cleared puzzles recleared
		foreach(Puzzle puzzle in puzzles)
		{
			if(loadedFile.puzzleCompleted[index])
			{
				puzzle.clearPuzzle();
			}

			index ++;
		}

		foreach(AbilityPickup abilityPickup in abilityPickups)
		{
			if(loadedFile.unlockedAbilities.Contains(abilityPickup.abilityName))
			{
				abilityPickup.gameObject.SetActive(false);
			}
		}

		//Reunlock unlocked abilities
		foreach(string ability in loadedFile.unlockedAbilities)
		{
			progressionManager.unlockAbility(ability);
		}

		//Return to saved level
		skillsManager.upgradeLevels = loadedFile.levelUpgrades;
		index = 0;

		foreach(int upgrade in loadedFile.levelUpgrades)
		{
			skillsManager.increaseLevel(index);
			index ++;
		}

		player.transform.position = loadedFile.position;
		player.hp = loadedFile.health;
		
		//Reinstate player inventory
		inventoryManager.unEquipAllItems();

		inventoryManager.equipItemAndAddIcon(loadedFile.equippedHead,InventoryConstants.INV_HEAD_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedBrows,InventoryConstants.INV_BROWS_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedEyes,InventoryConstants.INV_EYES_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedMouth,InventoryConstants.INV_MOUTH_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedTorso,InventoryConstants.INV_TORSO_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedCover,InventoryConstants.INV_COVER_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedLegs,InventoryConstants.INV_LEGS_TYPE);
		inventoryManager.equipItemAndAddIcon(loadedFile.equippedFeet,InventoryConstants.INV_FEET_TYPE);
		inventoryManager.collectionItems = new List<string>();

		foreach(string collectionItem in loadedFile.inventoryCollection)
		{
			inventoryManager.addToCollection(collectionItem);
		}

		inventoryManager.staminaPotionCount = loadedFile.staminaCount;
		inventoryManager.healthPotionCount = loadedFile.healthCount;
		inventoryManager.puffCount = loadedFile.puffCount;
	}

}
