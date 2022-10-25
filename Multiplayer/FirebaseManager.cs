using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions;

//Class manages the firebase instance, containing functions
//for readimg and writing to the database 
public class FirebaseManager : MonoBehaviour
{
    public DatabaseReference reference;
    public bool online;
    public string sessionKey;

    public GameObject dummyPrefab;
    public GameObject player;

    private InventoryManager inventoryManager;
    Query newestQuery;

    void Start()
    {
        //Initializing reference to the realtime database
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();
        addPlayertoSession();
    }

    //Function used for reading all player data from the database
    public void readPlayerData()
    {
        //Getting the root reference value, essentially grabbing
        //all player data at once
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsFaulted)
            {
				Debug.LogError("Failed to load player data.");
            }
            else if(task.IsCompleted)
            {
                
                //Saving the data from the db into snapshot
                //for conversion
                DataSnapshot snapshot = task.Result;
                IEnumerable<DataSnapshot> players  = snapshot.Children;

                //Moving through all of the players in a for loop
                foreach(DataSnapshot player in players)
                {
                    //Creating dummy players for each json
                    //entry in the database
                    string playerJson = player.GetRawJsonValue();
                    DBPlayer playerFromDB = JsonUtility.FromJson<DBPlayer>(playerJson);
                    
                    if(playerFromDB.sessionKey != sessionKey)
                    {
                        createDummyPlayer(playerFromDB);
                    }
                }
            }
        });
    }

    //Function adds the current player to the database
    public void addPlayertoSession()
    {
        //Player is not online
        if(!online)
        {
            //A new reference myst be made
            DatabaseReference dbRef = reference.Push();
            string newKey = reference.Push().Key;

            sessionKey = dbRef.Key;

            //Sunscribe to all changes to the database after the
            //addition of thid player
            newestQuery = reference.OrderByKey().StartAt(newKey);
            newestQuery.ChildAdded += playerAddedToSession;

            DBPlayer dBPlayer = createDBPlayer();
            dBPlayer.sessionKey = sessionKey;

            string playerAsJson = JsonUtility.ToJson(dBPlayer);

            dbRef.SetRawJsonValueAsync(playerAsJson);
            online = true;
        }
        else
        {
            //Rewrite the current player to the database
            DatabaseReference dbRef = reference.Child(sessionKey);

            DBPlayer dBPlayer = createDBPlayer();
            dBPlayer.sessionKey = sessionKey;

            string playerAsJson = JsonUtility.ToJson(dBPlayer);

            dbRef.SetRawJsonValueAsync(playerAsJson);
        }

    }
    
    //Delete the current player from the database, unsub from events
    public void removePlayerFromSession()
    {
        if(online)
        {
            DatabaseReference dbRef = reference.Child(sessionKey);
            dbRef.RemoveValueAsync();
            newestQuery.ChildAdded -= playerAddedToSession;
        }
    }

    //Create a dummy player from a passed in database player structure
    public void createDummyPlayer(DBPlayer playerFromDB)
    {
        //Create the object and ensure apropriate transformations
        DummyPlayer newDummy = Instantiate(dummyPrefab, playerFromDB.position, Quaternion.identity).GetComponent<DummyPlayer>();
        newDummy.populateFromDB(playerFromDB);
        newDummy.firebaseSub();
        newDummy.refreshEquips();
    }

    //Function used for returning the current player as a database
    //structured object, essentially saving the player's position
    //and equip set
    public DBPlayer createDBPlayer()
    {
        DBPlayer dbPlayer = new DBPlayer();

        dbPlayer.position = player.transform.position;

        dbPlayer.equippedHead = inventoryManager.equippedHead;
        dbPlayer.equippedBrows = inventoryManager.equippedBrows;
        dbPlayer.equippedEyes = inventoryManager.equippedEyes;
        dbPlayer.equippedMouth = inventoryManager.equippedMouth;
        dbPlayer.equippedTorso = inventoryManager.equippedTorso;
        dbPlayer.equippedCover = inventoryManager.equippedCover;
        dbPlayer.equippedLegs = inventoryManager.equippedLegs;
        dbPlayer.equippedFeet = inventoryManager.equippedFeet;
        dbPlayer.equippedRings = inventoryManager.equippedRings;

        dbPlayer.puffCount = inventoryManager.puffCount;

        return dbPlayer;
    }
    //When the game ends
    private void OnApplicationQuit() 
    {
        removePlayerFromSession();
    }

    //Event function fires when a new player is added to the database,
    //creating a dummy as a result
    private void playerAddedToSession(object sender, ChildChangedEventArgs eventArgs)
    {
        DataSnapshot playerAdded = eventArgs.Snapshot;

        if(sessionKey != playerAdded.Value.ToString())
        {
            string playerJson = playerAdded.GetRawJsonValue();
            DBPlayer playerFromDB = JsonUtility.FromJson<DBPlayer>(playerJson);

            createDummyPlayer(playerFromDB);
        }

    }

}
