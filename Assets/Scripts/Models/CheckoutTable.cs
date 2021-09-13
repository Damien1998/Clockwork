using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SaveController;

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

        public override void PlaceItem(Watch itemToPlace)
        {
            if(CheckQuestWatch(itemToPlace))
            {
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
                        DialogueManager.instance.StartDialogue(endOfLevelDialogue);
                    }
                    SaveGame();
                    UIManager.instance.levelTimer.StopTimer();
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
            LoadQuestWatch(_questWatch,watchItem);
            WatchComponent.WatchItem = new Item();
            WatchComponent.WatchItem.SetParameters(watchItem);
            WatchComponent.WatchItem.trueState = _questWatch.trueState;
            WatchComponent.WatchItem.State = _questWatch.myState;

            WatchComponent.isCompleteWatch = true;
            WatchComponent.questWatch = true;
            WatchComponent.itemRenderer[0].sprite = _questWatch.QuestWatchSprites[0];
            WatchComponent.itemRenderer[0].gameObject.SetActive(true);
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
            newWatch.GetComponent<Watch>().isCompleteWatch = true;
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
            if(currentWatch.WatchItem.State == ItemState.Repaired&&currentWatch.isCompleteWatch&&!currentWatch.questWatch)
            {
                for (int i = 0; i < randomWatches.Count; i++)
                {
                    if (currentWatch.WatchItem.itemID == randomWatches[i].itemID)
                    {
                        watchesFixed++;
                        return true;
                    }
                }
            }

            return false;

        }

        private bool CheckQuestWatch(Watch currentWatch)
        {
            if(hasQuest)
            {
                if (currentWatch.WatchItem.State == ItemState.Repaired && currentWatch.questWatch)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                return true;
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
        }

        void GenerateCasing(WatchSprites casingSprites,Item newWatchCasing)
        {
            MakeQuickParts(casingSprites.Glass,newWatchCasing,0);

            MakeQuickParts(casingSprites.Box ,newWatchCasing,1);

            MakeQuickParts(casingSprites.Belt ,newWatchCasing,2);
        }
        void GenerateMechanism(WatchSprites watchSprites,Item mechanism)
        {
            for (int j = 0; j < Random.Range(workbenchLevelParams.mechMinParts, workbenchLevelParams.mechMaxParts); j++)
            {
                var newItem = new Item
                {
                    itemImages = {[0] = watchSprites.Mechanical[Random.Range(0, watchSprites.Mechanical.Length-1)]}
                };
                newItem.parentItem = mechanism;
                mechanism.components.Add(newItem);
            }
        }
        void SimpleCasingWatch(WatchSprites watchSprites,Item item)
        {
            item.itemImages[0] = watchSprites.Glass[Random.Range(0, watchSprites.Glass.Length)];
            item.itemImages[1] = watchSprites.Box[Random.Range(0, watchSprites.Box.Length)];
            item.itemImages[2] = watchSprites.Belt[Random.Range(0,  watchSprites.Belt.Length)];
        }
        //TODO making it cleaner
        private void CreateRandomWatches()
        {
            var watchTypes = GameManager.instance.watchTypes;
            var currentLevelParams = workbenchLevelParams;

            for(int i = 0; i < currentLevelParams.watchAmount; i++)
            {

                Item newWatch = new Item {itemImages = new Sprite[5]};
                newWatch.itemID = Random.Range(0, 99999);

                //For choosing the watch type
                var weights = currentLevelParams.pocketWatchWeight + currentLevelParams.wristWatchWeight;

                WatchType watchType;
                WatchSprites watchSprites;

                //Selects the watch type
                if (Random.Range(0, weights) < currentLevelParams.pocketWatchWeight)
                {
                    watchType = WatchType.HandWatch;
                    watchSprites = watchTypes[(int) watchType];
                    //newWatch.itemImages[0] = watchSprites.Housing[0];
                    newWatch.itemImages[1] = watchSprites.Housing[0];
                }
                else
                {
                    watchType = WatchType.PocketWatch;
                    watchSprites = watchTypes[(int)watchType];
                    //newWatch.itemImages[0] = watchSprites.Housing[0];
                    newWatch.itemImages[1] = watchSprites.Housing[0];
                }

                newWatch.components = new List<Item>();
                newWatch.State = ItemState.ComplexBroken;
                newWatch.trueState = ItemState.ComplexBroken;

                Item newWatchCasing = new Item {itemImages = new Sprite[3]};
                Item newWatchMechanism = new Item();
                newWatchMechanism.itemID = Random.Range(99999, 199999);

                if(currentLevelParams.eitherOfMechOrCasing)
                {

                    if (Random.Range(0, 2) == 0)
                    {
                        GenerateCasing(watchSprites,newWatchCasing);

                        newWatchCasing.State = ItemState.ComplexBroken;
                        newWatchCasing.trueState = ItemState.ComplexBroken;

                    }
                    else
                    {
                        SimpleCasingWatch(watchSprites, newWatchCasing);

                        GenerateMechanism(watchSprites,newWatchMechanism);

                        newWatchMechanism.State = ItemState.ComplexBroken;
                        newWatchMechanism.trueState = ItemState.ComplexBroken;
                    }
                }
                else if (currentLevelParams.casingComponents || currentLevelParams.mechanismComponents)
                {

                    if (currentLevelParams.casingComponents)
                    {
                        GenerateCasing(watchSprites,newWatchCasing);

                        newWatchCasing.State = ItemState.ComplexBroken;
                        newWatchCasing.trueState = ItemState.ComplexBroken;
                    }
                    else
                    {
                        SimpleCasingWatch(watchSprites, newWatchCasing);
                    }

                    newWatchMechanism.components = new List<Item>();

                    if (currentLevelParams.mechanismComponents)
                    {
                        var rAmount = Random.Range(currentLevelParams.mechMinParts, currentLevelParams.mechMaxParts);
                        for (int j = 0; j < rAmount; j++)
                        {
                            var newItem = new Item();
                            newItem.itemImages[0] =  watchSprites.Mechanical[Random.Range(0, watchSprites.Mechanical.Length)];
                            newItem.parentItem = newWatchMechanism;

                            newWatchMechanism.components.Add(newItem);
                        }
                        newWatchMechanism.State = ItemState.ComplexBroken;
                        newWatchMechanism.trueState = ItemState.ComplexBroken;
                    }
                }
                else
                {
                    SimpleCasingWatch(watchSprites, newWatchCasing);

                    newWatchCasing.components = new List<Item>();

                    newWatchMechanism.components = new List<Item>();
                }
                newWatchMechanism.itemImages[0] = watchSprites.Mechanical[watchSprites.Mechanical.Length-1];
                newWatchCasing.parentItem = newWatch;
                newWatchMechanism.parentItem = newWatch;

                newWatch.components.Add(newWatchCasing);
                newWatch.components.Add(newWatchMechanism);

                newWatch.itemImages[1] = newWatchCasing.itemImages[0];
                newWatch.itemImages[2] = watchSprites.Face[Random.Range(0, watchSprites.Face.Length)];
                newWatch.itemImages[3] = newWatchCasing.itemImages[1];
                newWatch.itemImages[4] = newWatchCasing.itemImages[2];

                if (Random.Range(0, 100) < currentLevelParams.decorPercentChance)
                {
                    var decorID = Random.Range(0, watchSprites.Decoration.Length - 1);

                    var newWatchDecor = new Item
                    {
                        itemImages = {[0] = watchSprites.Decoration[decorID]},
                        parentItem = newWatch
                    };

                    newWatch.components.Add(newWatchDecor);

                    newWatch.itemImages[0] = newWatchDecor.itemImages[0];
                    if(decorID > 0)
                    {
                        newWatch.itemImages[0] = watchSprites.Decoration[2];
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
                    basicItem.trueState = ItemState.Repaired;
                    basicItem.State = ItemState.Repaired;
                }

                float brokenComponentPercentage = (Random.Range(currentLevelParams.brokenPartMinPercentage, currentLevelParams.brokenPartMaxPercentage));
                Debug.Log(brokenComponentPercentage);
                var brokenComponentAmount = Mathf.RoundToInt((brokenComponentPercentage/100) * newWatchBasicItems.Count);
                Debug.LogWarning("BasicItems: " + newWatchBasicItems.Count);
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
                                newWatchBasicItems[temp].State = newWatchBasicItems[temp].trueState;
                            }
                            else
                            {
                                Debug.LogWarning("EHHH");
                                newWatchBasicItems[temp].trueState = ItemState.Broken;
                                newWatchBasicItems[temp].State = newWatchBasicItems[temp].trueState;
                            }

                            newWatchBasicItems[temp].State = newWatchBasicItems[temp].trueState;
                        }
                        else if (currentLevelParams.brokenState)
                        {
                            Debug.LogWarning("UUHH");
                            newWatchBasicItems[temp].trueState = ItemState.Broken;
                            newWatchBasicItems[temp].State = newWatchBasicItems[temp].trueState;
                        }
                        else if (currentLevelParams.unfixableState)
                        {
                            Debug.LogWarning("Unfixable Part");
                            newWatchBasicItems[temp].trueState = ItemState.Unfixable;
                            newWatchBasicItems[temp].State = newWatchBasicItems[temp].trueState;
                        }

                        brokenComponentAmount--;
                    }
                    safeguard--;
                }

                for(int j = 0; j < newWatchBasicItems.Count; j++)
                {
                    if (Random.Range(0, 100) < currentLevelParams.unknownStatePercentChance)
                    {
                        newWatchBasicItems[j].State = ItemState.UnknownState;
                    }
                }
                randomWatches.Add(newWatch);
            }
        }
        List<Item> GetBasicItems(Item myItem)
        {
            var myList = new List<Item>();
            if (myItem.components.Count > 0)
            {
                for (int i = 0; i < myItem.components.Count; i++)
                {
                    var tmpList = GetBasicItems(myItem.components[i]);
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
