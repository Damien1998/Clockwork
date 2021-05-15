using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


//Refactoring is done! You may enter safely
public class GameManager : MonoBehaviour
{
    //God help us
    //The only game manager allowed to stay in this world of flesh
    public static GameManager instance;
    public ItemStateDisplay itemStates;
    public List<Recipe> RecipesList = new List<Recipe>();
    public List<Recipe> RandomWatchRecipesList = new List<Recipe>();
    [SerializeField]private int _levelID;

    public bool localQuestDone;
    
    private int points;

    //For the random watch generator. After I rework this to use the new sprites the code will have to be changed slightly.
    [SerializeField] private Item[] watchBases, decorBases, beltBases, boxBases, glassBases, mechComponentBases;
    [SerializeField] private Item mechanismBase, casingBase;

    public List<Item> randomWatches = new List<Item>();

    /*
    //PS
    //<summary>
    //A Variable that Hold Current Level Watch List and Time 
    //</summary>
    //It will only be changed when levelID is changed ,until then it is null
    //Which is changed when a level is selected
    */
    public LevelParams currentLevelParams;
    public SoundAudioClip[] soundAudioClipArray;
    public AudioMixerGroup SFX;
    public AudioSource BGM;

    public int levelID 
    { 
        get => _levelID;
        set
        {
            _levelID = value;

            currentLevelParams = Resources.Load<LevelParams>("LevelParams/Level " + _levelID);
        } 
    }
    private void Awake()
    {
        //Keeping the population of game managers in check
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        SoundManager.Initialize();
        if (!SaveController._initialized)
        {
            SaveController.InitializeSaveController(12, 12, 0, 12);
        }
    }
    

    //TODO making it cleaner
    public void CreateRandomWatches()
    {
        Debug.Log(currentLevelParams);

        randomWatches = new List<Item>();
        RandomWatchRecipesList = new List<Recipe>();

        for(int i = 0; i < currentLevelParams.watchAmount; i++)
        {
            List<Item> newWatchBasicItems = new List<Item>();
            Item newWatch = ScriptableObject.CreateInstance<Item>();

            int watchTypeModifier = 0;

            //For choosing the watch type
            var weights = currentLevelParams.pocketWatchWeight + currentLevelParams.wristWatchWeight;

            //Selects the watch type
            if (Random.Range(0, weights) < currentLevelParams.pocketWatchWeight)
            {
                newWatch.SetParameters(watchBases[0]);
            }
            else
            {
                newWatch.SetParameters(watchBases[1]);
                watchTypeModifier = 3;
            }
            
            newWatch.components = new List<Item>();
            newWatch.state = ItemState.ComplexBroken;
            newWatch.trueState = ItemState.ComplexBroken;

            Item newWatchCasing = null;
            Item newWatchMechanism = null;
            Item newWatchDecor = null;

            if(currentLevelParams.eitherOfMechOrCasing)
            {
                newWatchMechanism = ScriptableObject.CreateInstance<Item>();
                newWatchMechanism.SetParameters(mechanismBase);
                newWatchMechanism.components = new List<Item>();

                newWatchCasing = ScriptableObject.CreateInstance<Item>();
                newWatchCasing.SetParameters(casingBase);
                newWatchCasing.components = new List<Item>();

                if (Random.Range(0, 2) == 0)
                {
                    newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                    var randomNumber = Random.Range(0, 3) + watchTypeModifier;

                    newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(glassBases[randomNumber]);
                    newWatchCasing.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    newWatchCasing.itemImages[0] = glassBases[randomNumber].itemImages[0];

                    newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                    randomNumber = Random.Range(0, 3) + watchTypeModifier;

                    newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(boxBases[randomNumber]);                    
                    newWatchCasing.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    newWatchCasing.itemImages[2] = boxBases[randomNumber].itemImages[2];

                    newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                    randomNumber = Random.Range(0, 3) + watchTypeModifier;

                    newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(beltBases[randomNumber]);
                    newWatchCasing.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    newWatchCasing.itemImages[3] = beltBases[randomNumber].itemImages[3];

                    newWatchCasing.state = ItemState.ComplexBroken;
                    newWatchCasing.trueState = ItemState.ComplexBroken;

                    RandomWatchRecipesList.Add(ScriptableObject.CreateInstance<Recipe>());
                    RandomWatchRecipesList[RandomWatchRecipesList.Count - 1].SetParameters(newWatchCasing, newWatchCasing.components);

                    newWatchBasicItems.Add(newWatchMechanism);
                }
                else
                {
                    newWatchCasing.itemImages[0] = glassBases[Random.Range(0, 3) + watchTypeModifier].itemImages[0];
                    newWatchCasing.itemImages[2] = boxBases[Random.Range(0, 3) + watchTypeModifier].itemImages[2];
                    newWatchCasing.itemImages[3] = beltBases[Random.Range(0, 3) + watchTypeModifier].itemImages[3];

                    for (int j = 0; j < Random.Range(currentLevelParams.mechMinParts, currentLevelParams.mechMaxParts); j++)
                    {
                        newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                        newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(mechComponentBases[Random.Range(0, mechComponentBases.Length)]);
                        
                        newWatchMechanism.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    }
                    newWatchBasicItems.Add(newWatchCasing);
                    
                    newWatchMechanism.state = ItemState.ComplexBroken;
                    newWatchMechanism.trueState = ItemState.ComplexBroken;
                }               
            }
            else if (currentLevelParams.casingComponents || currentLevelParams.mechanismComponents)
            {
                newWatchCasing = ScriptableObject.CreateInstance<Item>();
                newWatchCasing.SetParameters(casingBase);
                newWatchCasing.components = new List<Item>();

                if (currentLevelParams.casingComponents)
                {
                    newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                    var randomNumber = Random.Range(0, 3) + watchTypeModifier;

                    newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(glassBases[randomNumber]);
                    newWatchCasing.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    newWatchCasing.itemImages[0] = glassBases[randomNumber].itemImages[0];

                    newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                    randomNumber = Random.Range(0, 3) + watchTypeModifier;

                    newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(boxBases[randomNumber]);
                    newWatchCasing.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    newWatchCasing.itemImages[2] = boxBases[randomNumber].itemImages[2];

                    newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                    randomNumber = Random.Range(0, 3) + watchTypeModifier;

                    newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(beltBases[randomNumber]);
                    newWatchCasing.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    newWatchCasing.itemImages[3] = beltBases[randomNumber].itemImages[3];

                    RandomWatchRecipesList.Add(ScriptableObject.CreateInstance<Recipe>());
                    RandomWatchRecipesList[RandomWatchRecipesList.Count - 1].SetParameters(newWatchCasing, newWatchCasing.components);

                    newWatchCasing.state = ItemState.ComplexBroken;
                    newWatchCasing.trueState = ItemState.ComplexBroken;
                }     
                else
                {
                    newWatchBasicItems.Add(newWatchCasing);
                    newWatchCasing.itemImages[0] = glassBases[Random.Range(0, 3) + watchTypeModifier].itemImages[0];
                    newWatchCasing.itemImages[2] = boxBases[Random.Range(0, 3) + watchTypeModifier].itemImages[2];
                    newWatchCasing.itemImages[3] = beltBases[Random.Range(0, 3) + watchTypeModifier].itemImages[3];
                }

                newWatchMechanism = ScriptableObject.CreateInstance<Item>();
                newWatchMechanism.SetParameters(mechanismBase);
                newWatchMechanism.components = new List<Item>();

                if (currentLevelParams.mechanismComponents)
                {
                    var rAmount = Random.Range(currentLevelParams.mechMinParts, currentLevelParams.mechMaxParts);
                    for (int j = 0; j < rAmount; j++)
                    {
                        newWatchBasicItems.Add(ScriptableObject.CreateInstance<Item>());
                        newWatchBasicItems[newWatchBasicItems.Count - 1].SetParameters(mechComponentBases[Random.Range(0, mechComponentBases.Length)]);
                        
                        newWatchMechanism.components.Add(newWatchBasicItems[newWatchBasicItems.Count - 1]);
                    }
                    newWatchMechanism.state = ItemState.ComplexBroken;
                    newWatchMechanism.trueState = ItemState.ComplexBroken;
                }  
                else
                {
                    newWatchBasicItems.Add(newWatchMechanism);
                }
            }            
            else
            {
                newWatchCasing = ScriptableObject.CreateInstance<Item>();
                newWatchCasing.SetParameters(casingBase);
                newWatchCasing.itemImages[0] = glassBases[Random.Range(0, 3) + watchTypeModifier].itemImages[0];
                newWatchCasing.itemImages[2] = boxBases[Random.Range(0, 3) + watchTypeModifier].itemImages[2];
                newWatchCasing.itemImages[3] = beltBases[Random.Range(0, 3) + watchTypeModifier].itemImages[3];

                newWatchCasing.components = new List<Item>();
                newWatchMechanism = ScriptableObject.CreateInstance<Item>();
                newWatchMechanism.SetParameters(mechanismBase);
                newWatchMechanism.components = new List<Item>();                

                newWatchBasicItems.Add(newWatchMechanism);
                newWatchBasicItems.Add(newWatchCasing);
            }

            newWatch.components.Add(newWatchCasing);
            newWatch.components.Add(newWatchMechanism);

            newWatch.itemImages[0] = newWatchCasing.itemImages[0];
            newWatch.itemImages[2] = newWatchCasing.itemImages[2];
            newWatch.itemImages[3] = newWatchCasing.itemImages[3];

            if (Random.Range(0, 100) < currentLevelParams.decorPercentChance)
            {
                newWatchDecor = ScriptableObject.CreateInstance<Item>();
                newWatchDecor.SetParameters(decorBases[Random.Range(0, decorBases.Length)]);
                
                newWatch.components.Add(newWatchDecor);
                //basicPartAmount++;
            }

            randomWatches.Add(newWatch);
            RandomWatchRecipesList.Add(ScriptableObject.CreateInstance<Recipe>());
            RandomWatchRecipesList[RandomWatchRecipesList.Count - 1].SetParameters(newWatch, newWatch.components);

            //Setting the component states

            foreach(Item basicItem in newWatchBasicItems)
            {
                basicItem.trueState = ItemState.Repaired;
                basicItem.state = ItemState.Repaired;              
            }

            float brokenComponentPercentage = (Random.Range(currentLevelParams.brokenPartMinPercentage, currentLevelParams.brokenPartMaxPercentage));
            Debug.Log(brokenComponentPercentage);
            var brokenComponentAmount = Mathf.RoundToInt((brokenComponentPercentage/100) * newWatchBasicItems.Count);
            Debug.LogWarning("BrokenComponents: " + brokenComponentAmount);
            var safeguard = 100;

            Debug.Log(brokenComponentAmount);

            while(brokenComponentAmount > 0 && safeguard > 0)
            {
                Debug.Log("WHAT");
                var temp = Random.Range(0, newWatchBasicItems.Count);
                if(newWatchBasicItems[temp].trueState != ItemState.Unfixable && newWatchBasicItems[temp].trueState != ItemState.Broken)
                {
                    if (currentLevelParams.brokenState && currentLevelParams.unfixableState)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            Debug.LogWarning("Unfixable Part!");
                            newWatchBasicItems[temp].trueState = ItemState.Unfixable;
                            newWatchBasicItems[temp].state = newWatchBasicItems[temp].trueState;
                        }
                        else
                        {
                            Debug.LogWarning("EHHH");
                            newWatchBasicItems[temp].trueState = ItemState.Broken;
                            newWatchBasicItems[temp].state = newWatchBasicItems[temp].trueState;
                        }

                        newWatchBasicItems[temp].state = newWatchBasicItems[temp].trueState;
                    }
                    else if (currentLevelParams.brokenState)
                    {
                        Debug.LogWarning("UUHH");
                        newWatchBasicItems[temp].trueState = ItemState.Broken;
                        newWatchBasicItems[temp].state = newWatchBasicItems[temp].trueState;
                    }
                    else if (currentLevelParams.unfixableState)
                    {
                        Debug.LogWarning("Unfixable Part");
                        newWatchBasicItems[temp].trueState = ItemState.Unfixable;
                        newWatchBasicItems[temp].state = newWatchBasicItems[temp].trueState;
                    }

                    brokenComponentAmount--;
                }
                safeguard--;   
            }

            for(int j = 0; j < newWatchBasicItems.Count; j++)
            {
                if (Random.Range(0, 100) < currentLevelParams.unknownStatePercentChance)
                {
                    newWatchBasicItems[j].state = ItemState.UnknownState;
                }
            }

            CheckItemComponents(newWatch);
        }
    }

    void CheckItemComponents(Item _newWatch)
    {
        for(int i = 0; i < _newWatch.components.Count; i++)
        {
            ClearItemImages(_newWatch.components[i]);
            if(_newWatch.components[i].components.Count>1)
            {
                CheckItemComponents(_newWatch.components[i]);
            }
        }
    }
    void ClearItemImages(Item _itemToClear)
    {
        for (int i = _itemToClear.itemImages.Count-1; i >= 0; i--)
        {
            if (_itemToClear.itemImages[i] == null)
            {
                _itemToClear.itemImages.Remove(_itemToClear.itemImages[i]);
            }
        }
    }
    public void CompleteLevel()
    {
        // if(levelTimer < levels[levelID].completionTime || levels[levelID].completionTime == 0)
        // {
        //     levels[levelID] = new SaveData.Level(levels[levelID].name, true, true, levelTimer, levels[levelID].completionTimeSideQuest);
        // }
        SaveController.UnlockLevel(levelID);
    }
    public void StartQuest(string questName,string questItemName)
    {
        if (SaveController._sideQuest == false)
        {
            var sideQuest = SaveController.sideQuests[levelID-1];
            sideQuest.questItemName = questItemName;
            SaveController.sideQuests[levelID-1] = sideQuest;
            SaveController._sideQuest = true;
        }
    }
    public void CompleteQuest(string questName)
    {
        SaveController.CompletePOI(questName);
    }
    public void Save(int saveID)
    {
        SaveController.CreateSaveGame(saveID);
    }
    // Update is called once per frame
    // void Update()
    // {
    //     levelTimer += Time.deltaTime;
    //     float temp = levelTimer / levelTimerBase;
    //     timerDisplay.value = temp;
    //     if(points >= pointsToComplete && !levelCompletionCalled)
    //     {
    //         endDisplay.gameObject.SetActive(true);
    //         UIManager.instance.pointDisplayEnd.text = "Czas: " + levelTimer;
    //         CompleteLevel();
    //         SaveGame();
    //         levelCompletionCalled = true;
    //         Time.timeScale = 0;
    //     }
    // }
    
    // public void AddPoints(int pointAmount)
    // {
    //     points += pointAmount;
    //     UIManager.instance.pointDisplay.text = "Punkty: " + points;
    // }
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip[] audioClip;
    }
}


