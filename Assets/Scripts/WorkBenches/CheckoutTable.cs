using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SaveController;
using Random = UnityEngine.Random;

//Refactoring is done! You may enter safely
namespace Models
{
    public class CheckoutTable : Workbench
    {
        public List<Item> randomWatches = new List<Item>();

        private int watchIndex = 0;
        [SerializeField] private LevelParams workbenchLevelParams;

        [SerializeField] private TextAsset endOfLevelDialogue,questEndDialogue;

        [SerializeField] private Transform watchThrowPoint;
        [SerializeField] private ParticleSystem deliveryFX, retrievedFX;
        private Watch questWatch;

        public bool PGAmode;

        WatchSprites currentWatchSprites;
        private int watchesFixed;

        void Start()
        {
            //PS
            //<summary>
            //About the WatchList ScriptableObject script
            //</summary>
            //Since we will have more then 1 level expected at the moment of making this
            //It's more comfortable to have a quick and easy to use object that can specify what kind of watches will spawn in the order that is put in
            //This way we can make list of watches that will be used instantly
            //If you want to change the order which the watches spawn in go into "Resources/LevelParams directory
            workbenchLevelParams = Resources.Load<LevelParams>($"LevelParams/Level {GameManager.instance.levelID}");
        }
        /*
     * PS
     * <summary>
     * Function Checks for CorrectWatch and Spawns new One if it's given the repaired correct one
     * </summary>
     * Currently because of no easy way to access the and store the item that player holds i used the currentWatch variable as a storage
     * Possibly in the future it would be more efficient and comfortable if we had one method to handle putting watches in the workbenches
     */

        public void InitLevel()
        {
            GameManager.instance.levelID = GameManager.instance.levelID;
            CreateRandomWatches();
            StartCoroutine(CheckForQuests());
            StartCoroutine(DispenseWatches());
        }

        public void ThrowWatches()
        {
            watchIndex = 0;
            StartCoroutine(DispenseWatches());
        }

        public override void PlaceItem(Watch itemToPlace)
        {
            if(CheckQuestWatch(itemToPlace))
            {
                SoundManager.PlaySound(SoundManager.Sound.ClockCompleted);
                itemToPlace.transform.position = transform.position;
                Destroy(itemToPlace.gameObject);
                retrievedFX.Play();
                CompleteQuest(GameManager.instance.levelID);
            }
            if(CheckWatch(itemToPlace))
            {
                retrievedFX.Play();
                //I'm not sure whether we really need that there:
                itemToPlace.transform.position = transform.position;
                Destroy(itemToPlace.gameObject);
                if(workbenchLevelParams.watchAmount <= watchesFixed)
                {
                    //ThrowNewWatch(workbenchLevelParams.listOfWatches[watchIndex]);

                    //ThrowRandomWatch();
                    Debug.Log("Last watch");

                    if (ReturnLevel(GameManager.instance.levelID).HasValue && ReturnLevel(GameManager.instance.levelID).Value.levelSideQuest.completed)
                    {
                        AnalyticsController.SendAnalyticDictionary("CompletedQuestWithLevel", "Level", GameManager.instance.levelID);
                        SoundManager.PlaySound(SoundManager.Sound.WinPopup);
                        DialogueManager.instance.StartDialogue(questEndDialogue);
                        hasQuest = false;
                    }
                    else
                    {
                        AnalyticsController.SendAnalyticDictionary("CompletedLevel","Level",GameManager.instance.levelID);
                        UIManager.instance.ShowLevelEnd();
                        // DialogueManager.instance.StartDialogue(endOfLevelDialogue);
                    }
                    SaveGame();
                    SoundManager.PlaySound(SoundManager.Sound.WinPopup);
                    UIManager.instance.levelTimer.StopTimer();
                    watchesFixed = 0;
                }
            }
        }

        IEnumerator DispenseWatches()
        {
            while(watchIndex < workbenchLevelParams.watchAmount)
            {
                deliveryFX.Play();
                ThrowRandomWatch();
                yield return new WaitForSeconds(workbenchLevelParams.watchDispensingTime);
                watchIndex++;
                if(PGAmode)
                {
                    break;
                }
            }
        }
        IEnumerator CheckForQuests()
        {
            yield return new WaitForSeconds(1f);
            if (hasQuest)
            {
                ThrowQuestWatch(GameManager.instance.currentLevelParams.questWatch);
            }
        }

        /*
    PS
    <summary>
    This is a simple method to make new watch if there is none at the moment of creating
    </summary>
    Sadly as of now it does not pool the watch due to the nature of watch components and fact that the watch object constantly changes in scene
    But it can be noted that this might be changed to pooling later on in the production
    */
        private void ThrowQuestWatch(QuestWatch _questWatch)
        {
            var pos = transform.position;
            if (watchThrowPoint != null)
            {
                pos = watchThrowPoint.position;
            }
            var newWatch = Instantiate(WatchTemplate, pos, Quaternion.identity);
            var watchItem = new Item();
            var WatchComponent = newWatch.GetComponent<Watch>();
            WatchComponent.itemRenderer[0].gameObject.SetActive(true);
            LoadQuestWatch(_questWatch,watchItem);
            var newItem = new Item();
            newItem.SetParameters(watchItem);
            WatchComponent.WatchItem = newItem;
            WatchComponent.WatchItem.SetParameters(watchItem);

            WatchComponent.SetItemType(ItemType.QuestWatch);

            WatchComponent.itemRenderer[0].sprite = _questWatch.QuestWatchSprites[0];
            questWatch = WatchComponent;
        }

        private void LoadQuestWatch(QuestWatch _questWatch,Item myItem)
        {
            myItem.itemImages = new Sprite[_questWatch.QuestWatchSprites.Length];
            for (int i = 0; i < _questWatch.QuestWatchSprites.Length; i++)
            {
                myItem.itemImages[i] = _questWatch.QuestWatchSprites[i];
            }
            myItem.components = new List<Item>();
            for (int i = 0; i <  _questWatch.Parts.Length; i++)
            {
             myItem.components.Add(new Item());
             myItem.components[i].parentItem = myItem;
             LoadQuestWatch(_questWatch.Parts[i],myItem.components[i]);
            }
            myItem.State = _questWatch.myState;
            myItem.trueState = _questWatch.trueState;
            myItem.itemType = _questWatch.myType;
        }
        private void ThrowRandomWatch()
        {
            var pos = transform.position;
            if(watchThrowPoint != null)
            {
                pos = watchThrowPoint.position;
            }

            var newWatch = Instantiate(WatchTemplate, pos, Quaternion.identity);
            var newItem = new Item();
            newItem.SetParameters(randomWatches[watchIndex]);
            newWatch.GetComponent<Watch>().WatchItem = newItem;
            newWatch.GetComponent<Watch>().WatchItem.SetParameters(randomWatches[watchIndex]);
            newWatch.GetComponent<Watch>().SetItemType(ItemType.FullWatch);
            newWatch.GetComponent<Watch>().TrueState = ItemState.Broken;
        }
        /*
     PS
     <summary>
     The bool checks if the Item we placed matches the requirements of the CheckoutTable
     </summary>
     */
        private bool CheckWatch(Watch currentWatch)
        {
            if(currentWatch.WatchItem.State == ItemState.Repaired&&currentWatch.WatchItem.itemType == ItemType.FullWatch&&currentWatch.WatchItem.itemType != ItemType.QuestWatch)
            {
                for (int i = 0; i < randomWatches.Count; i++)
                {
                    if (currentWatch.WatchItem.itemID == randomWatches[i].itemID)
                    {
                        SoundManager.PlaySound(SoundManager.Sound.ClockCompleted);
                        watchesFixed++;
                        if(PGAmode && watchIndex < workbenchLevelParams.watchAmount)
                        {
                            deliveryFX.Play();
                            ThrowRandomWatch();
                            watchIndex++;
                        }
                        return true;
                    }
                }
            }

            return false;

        }

        private static bool CheckQuestWatch(Watch currentWatch)
        {
            if(hasQuest)
            {
                return currentWatch.WatchItem.itemType == ItemType.QuestWatch;
            }
            else
            {
                return false;
            }

        }
        private void MakeQuickParts(Sprite[] bases,Item parentItem,int itemSlots)
        {
            var newItem = new Item();
            var randomNumber = Random.Range(0, bases.Length);

            newItem.parentItem = parentItem;
            parentItem.components.Add(newItem);
            newItem.itemImages[0] = bases[randomNumber];
            parentItem.itemImages[itemSlots] = bases[randomNumber];
            if (parentItem.itemType == ItemType.FullCasing)
            {
                newItem.itemType = ItemType.Casing;
            }
        }

        void GenerateCasing(Item newWatchCasing)
        {
            MakeQuickParts(currentWatchSprites.Glass,newWatchCasing,0);

            MakeQuickParts(currentWatchSprites.Box ,newWatchCasing,1);

            MakeQuickParts(currentWatchSprites.Belt ,newWatchCasing,2);

            newWatchCasing.SetAllStates(ItemState.ComplexBroken);
        }

        private void GenerateMechanism(Item mechanism)
        {
            for (int j = 0; j < Random.Range(workbenchLevelParams.mechMinParts, workbenchLevelParams.mechMaxParts); j++)
            {
                var newItem = new Item
                {
                    itemImages = { [0] = currentWatchSprites.Mechanical[Random.Range(0, currentWatchSprites.Mechanical.Length - 1)] },
                    itemType = ItemType.Mechanism,
                    parentItem = mechanism
                };

                mechanism.components.Add(newItem);
            }

            mechanism.SetAllStates(ItemState.ComplexBroken);
        }

        private void SimpleCasingWatch(Item item)
        {
            item.itemImages[0] = currentWatchSprites.Glass[Random.Range(0, currentWatchSprites.Glass.Length)];
            item.itemImages[1] = currentWatchSprites.Box[Random.Range(0, currentWatchSprites.Box.Length)];
            item.itemImages[2] = currentWatchSprites.Belt[Random.Range(0,  currentWatchSprites.Belt.Length)];
        }

        //TODO making it cleaner
        private void CreateRandomWatches()
        {
            var watchTypes = GameManager.instance.watchTypes;
            var currentLevelParams = workbenchLevelParams;

            for(int i = 0; i < currentLevelParams.watchAmount; i++)
            {

                Item newWatch = new Item {itemImages = new Sprite[5], itemType = ItemType.FullWatch};
                newWatch.SetAllStates(ItemState.ComplexBroken);
                newWatch.itemID = Random.Range(0, 99999);

                //For choosing the watch type
                var weights = currentLevelParams.pocketWatchWeight + currentLevelParams.wristWatchWeight;

                //Selects the watch type And Grants it the Correct Sprite
                if (Random.Range(0, weights) < currentLevelParams.pocketWatchWeight)
                {
                    currentWatchSprites = watchTypes[(int) WatchType.HandWatch];
                    newWatch.itemImages[1] = currentWatchSprites.Housing[0];
                }
                else
                {
                    currentWatchSprites = watchTypes[(int)WatchType.PocketWatch];
                    newWatch.itemImages[1] = currentWatchSprites.Housing[0];
                }

                // Preparing Casing
                Item newWatchCasing = new Item { itemImages = new Sprite[3], parentItem = newWatch, itemType = ItemType.FullCasing};

                // Preparing Mechanism
                Item newWatchMechanism = new Item { itemImages = new Sprite[1], parentItem = newWatch, itemType = ItemType.FullMechanism};
                newWatchMechanism.itemImages[0] = currentWatchSprites.Mechanical[currentWatchSprites.Mechanical.Length-1];
                newWatchMechanism.itemID = Random.Range(99999, 199999);


                switch (currentLevelParams.SpawningType)
                {
                    case WatchSpawningTypes.Casing:
                    {
                        GenerateCasing(newWatchCasing);

                        break;
                    }
                    case WatchSpawningTypes.Mechanism:
                    {
                        SimpleCasingWatch(newWatchCasing);

                        GenerateMechanism(newWatchMechanism);
                        break;
                    }
                    case WatchSpawningTypes.MechanismOrCasing:
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            GenerateCasing(newWatchCasing);
                        }
                        else
                        {
                            SimpleCasingWatch(newWatchCasing);

                            GenerateMechanism(newWatchMechanism);
                        }
                        break;
                    }
                    case WatchSpawningTypes.MechanismAndCasing:
                    {
                        GenerateCasing(newWatchCasing);

                        GenerateMechanism(newWatchMechanism);
                        break;
                    }
                    case WatchSpawningTypes.None:
                    {
                        SimpleCasingWatch(newWatchCasing);
                        break;
                    }
                }

                newWatch.components.Add(newWatchCasing);
                newWatch.components.Add(newWatchMechanism);

                newWatch.itemImages[1] = newWatchCasing.itemImages[0];
                newWatch.itemImages[2] = currentWatchSprites.Face[Random.Range(0, currentWatchSprites.Face.Length)];
                newWatch.itemImages[3] = newWatchCasing.itemImages[1];
                newWatch.itemImages[4] = newWatchCasing.itemImages[2];

                if (Random.Range(0, 100) < currentLevelParams.decorPercentChance)
                {
                    var decorID = Random.Range(0, currentWatchSprites.Decoration.Length - 1);

                    var newWatchDecor = new Item
                    {
                        itemImages = {[0] = currentWatchSprites.Decoration[decorID]},
                        parentItem = newWatch
                    };

                    newWatch.components.Add(newWatchDecor);

                    newWatch.itemImages[0] = newWatchDecor.itemImages[0];
                    if(decorID > 0)
                    {
                        newWatch.itemImages[0] = currentWatchSprites.Decoration[2];
                    }

                }
                else
                {
                    newWatch.itemImages[0] = null;
                }

                //Setting the component states
                List<Item> newWatchBasicItems = GetBasicItems(newWatch);

                foreach(Item basicItem in newWatchBasicItems)
                {
                    basicItem.SetAllStates(ItemState.Repaired);
                }

                float brokenComponentPercentage = (Random.Range(currentLevelParams.brokenPartMinPercentage, currentLevelParams.brokenPartMaxPercentage));

                var brokenComponentAmount = Mathf.RoundToInt((brokenComponentPercentage/100) * newWatchBasicItems.Count);

                var safeguard = 100;

                while(brokenComponentAmount > 0 && safeguard > 0)
                {
                    var temp = Random.Range(0, newWatchBasicItems.Count);
                    if(newWatchBasicItems[temp].trueState != ItemState.Unfixable && newWatchBasicItems[temp].trueState != ItemState.Broken)
                    {
                        switch (currentLevelParams.brokenState)
                        {
                            case true when currentLevelParams.unfixableState:
                            {
                                newWatchBasicItems[temp].SetAllStates(Random.Range(0, 2) == 0 ? ItemState.Unfixable : ItemState.Broken);
                                break;
                            }
                            case true:
                                newWatchBasicItems[temp].SetAllStates(ItemState.Broken);
                                break;
                            default:
                            {
                                if (currentLevelParams.unfixableState)
                                {
                                    newWatchBasicItems[temp].SetAllStates(ItemState.Unfixable);
                                }
                                break;
                            }
                        }

                        brokenComponentAmount--;
                    }
                    safeguard--;
                }

                foreach (var item in newWatchBasicItems.Where(item => Random.Range(0, 100) < currentLevelParams.unknownStatePercentChance))
                {
                    item.State = ItemState.UnknownState;
                }
                randomWatches.Add(newWatch);
            }
        }

        List<Item> GetBasicItems(Item myItem)
        {
            var myList = new List<Item>();
            if (myItem.components.Count > 0)
            {
                foreach (var tmpList in myItem.components.Select(GetBasicItems))
                {
                    myList.AddRange(tmpList);
                }
            }
            else
            {
                myList.Add(myItem);
            }
            return myList;
        }
    }
}
