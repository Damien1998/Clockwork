using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Models;
using Polyglot;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialogueBox, optionsBox;
    public TextMeshProUGUI dialogueText, dialogueHistoryText, optionText1, optionText2;
    public DialogueActor[] actors;

    [SerializeField]
    private Color idleColor, talkColor;

    public Button option1, option2, progressButton;
    public Scrollbar dialogueScrollBar, textDialogueScrollBar;
    public Slider SkipBar;

    private string[] dialogue;
    private string name;
    private int currentLine;
    [SerializeField]
    private float dialogueTextSpeed = .025f;
    private bool _isTyping = false, _skipping = false, options;
    private bool isHoldingSkip, isInDialogue;

    [SerializeField] private ParticleSystem extraSnow;

    public Sprite[] portraits;
    public string[] characters;

    private List<DialogueSpriteData> dialoguePortraits;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            Initialize();
        }
    }

    private void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ProgressDialogue();
            }
            SkipBarProgress();
        }
    }
    private void Initialize()
    {
        var dPortraits = Resources.LoadAll("DialoguePortraits", typeof(DialogueSpriteData));

        List<DialogueSpriteData> portraitList = new List<DialogueSpriteData>();

        foreach (var portrait in dPortraits)
        {
            portraitList.Add((DialogueSpriteData)portrait);
        }

        dialoguePortraits = new List<DialogueSpriteData>();
        foreach (var portrait in portraitList)
        {
            dialoguePortraits.Add(portrait);
        }
    }

    public void StartDialogue(TextAsset textFile)
    {
        if (!isInDialogue)
        {
            StopAllCoroutines();
            ClearText();
            dialogue = textFile.text.Split('\n');
            SoundManager.PlaySound(SoundManager.Sound.PoiInteraction);
            dialogueBox.SetActive(true);
            dialogueBox.GetComponent<AnimatedPanel>().Appear();
            currentLine = 0;
            isInDialogue = true;
            ProgressDialogue();
            UIManager.instance.PauseGameNoBlur(true);
        }
    }
    public void StartDialogue(string textFile)
    {
        SoundManager.PlaySound(SoundManager.Sound.PoiInteraction);

        ClearText();
        StopAllCoroutines();
        dialogueText.rectTransform.sizeDelta = new Vector2(dialogueText.rectTransform.sizeDelta.x, 40);
        currentLine = 0;

        dialogue = Resources.Load<TextAsset>($"Dialogue/{Localization.Instance.SelectedLanguage}/{textFile}").text
            .Split('\n');
        dialogueBox.SetActive(true);
        dialogueBox.GetComponent<AnimatedPanel>().Appear();
        ProgressDialogue();
        isInDialogue = true;

        UIManager.instance.PauseGameNoBlur(true);
}
    public void ResetDialogue()
    {
        ClearText();
        StopAllCoroutines();
        dialogueText.rectTransform.sizeDelta = new Vector2(dialogueText.rectTransform.sizeDelta.x, 40);

        currentLine = 0;

        ProgressDialogue();
    }
    public void ExitDialogue()
    {
        foreach(DialogueActor actor in actors)
        {
            actor.gameObject.SetActive(false);
        }
        _isTyping = false;
        isHoldingSkip = false;
        dialogueText.rectTransform.sizeDelta = new Vector2(dialogueText.rectTransform.sizeDelta.x, 40);
        dialogueBox.GetComponent<AnimatedPanel>().Disappear();
        Time.timeScale = 1;
        isInDialogue = false;
        UIManager.instance.PauseGameNoBlur(false);
    }

    public void ReturnToPreviousScene()
    {
        //TODO
    }

    public void ProgressDialogue()
    {
        if (currentLine >= dialogue.Length)
        {
            ExitDialogue();
        }
        else
        {
            SoundManager.PlaySound(SoundManager.Sound.ProgressDialogue);

            CheckIfCommand();

            DisplayText();
        }
    }

    private void ClearText()
    {
        dialogue = null;
        dialogueText.text = "";
        dialogueHistoryText.text = "";
    }
    private int FindCharacterID(string characterName)
    {
        for(int i = 0; i < characters.Length; i++)
        {
            if(characters[i] == characterName)
            {
                return i;
            }
        }
        return -1;
    }
    private string[] GetName(int _numberOfCommands)
    {
        string[] words = dialogue[currentLine].Split(' ');
        string[] nameToDisplay = new string[words.Length-1];
        for (int i = 0; i < words.Length - _numberOfCommands; i++)
        {
            nameToDisplay[i] = words[i + _numberOfCommands];
        }
        return nameToDisplay;
    }

    private Sprite FindPortrait(string characterName, int actorID)
    {
        var cName = characterName.Trim();
        for (int i = 0; i < dialoguePortraits.Count; i++)
        {
            if (dialoguePortraits[i].characterID == characterName)
            {
                actors[actorID].sound = dialoguePortraits[i].sound;
                actors[actorID].emoteFX = dialoguePortraits[i].particleFX;
                return dialoguePortraits[i].portrait;
            }
        }
        return null;
        //int id = FindCharacterID(cName);
        //if(id != -1)
        //{
        //    return portraits[id];
        //}
        //else
        //{
        //    return null;
        //}
    }
    private Sprite FindPortrait(string characterName,string[] _nameToDisplay, int actorID)
    {
        var cName = characterName.Trim();
        StringBuilder _name = new StringBuilder();
        foreach (var nameWord in _nameToDisplay)
        {
            _name.Append(nameWord);
        }
        // Very Weird Symbol?
        // Probably Fixed Now Was Due to Portrait thing
        if (_name.Length - 1 > 0)
        {
            _name.Remove(_name.Length - 1, 1);
        }

        _name.Append(": ");
        name = _name.ToString();
        for (int i = 0; i < dialoguePortraits.Count; i++)
        {
            if (dialoguePortraits[i].characterID == characterName)
            {
                actors[actorID].sound = dialoguePortraits[i].sound;
                actors[actorID].emoteFX = dialoguePortraits[i].particleFX;
                return dialoguePortraits[i].portrait;
            }
        }
        return null;
        //int id = FindCharacterID(cName);
        //if(id != -1)
        //{
        //    return portraits[id];
        //}
        //else
        //{
        //    return null;
        //}
    }

    private void SetName(string[] _nameToDisplay)
    {
        StringBuilder _name = new StringBuilder();
        foreach (var nameWord in _nameToDisplay)
        {
            _name.Append(nameWord);
        }
        // Very Weird Symbol?
        // Probably Fixed Now Was Due to Portrait thing
        if (_name.Length - 1 > 0)
        {
            _name.Remove(_name.Length - 1, 1);
        }

        _name.Append(": ");
        name = _name.ToString();
    }

    private void Skip()
    {
        _skipping = true;
        _isTyping = false;
        while (currentLine < dialogue.Length  && _skipping)
        {
            CompileText();
            CheckIfCommand();
        }
    }
    //PS
    //<summary>
    //CheckIf Method Explained
    //</summary>
    //Currently this is the main way Dialogue Manager checks for commands inside the dialogue
    //it singles out first word that starts with -- and then accesses the correct method with switch
    //while loop is there in case of more then 2 methods running after each other in that case the CheckIfCommand will continue
    //
    //However it can conflict with other method such as TypeSentence Coroutine because both of them contain currentLine++
    //Which manages which line the Dialogue should be at currently
    //
    private void CheckIfCommand()
    {
        while (currentLine < dialogue.Length && dialogue[currentLine].StartsWith("--"))
        {
            string[] words = dialogue[currentLine].Split(' ');
            string firstWord = words[0];
            Debug.Log(firstWord);
            Debug.Log(firstWord.Length);
            string result = firstWord.Remove(firstWord.Length-1);
            if (result == "--level_start")
            {
                firstWord = result;
            }
            int actorID = 0;
            switch (firstWord)
            {
                case "--end":
                    ExitDialogue();
                    goto While_Break;
                case "--quest_start":
                    AnalyticsController.SendAnalyticDictionary("StartedQuest","Level",GameManager.instance.levelID);
                    SaveController.hasQuest = true;
                    break;
                case "--level_end":
                    UIManager.instance.ShowLevelEnd();
                    ExitDialogue();
                    break;
                case "--portrait":                   
                    int.TryParse(words[1], out actorID);
                    actors[actorID].gameObject.SetActive(true);
                    actors[actorID].image.sprite = FindPortrait(words[2], GetName(3), actorID);
                    //actors.gameObject.SetActive(true);
                    //actors.sprite = FindPortrait(words[1],GetName(2));
                    break;
                case "--talk":
                    int.TryParse(words[1], out actorID);
                    actors[actorID].gameObject.SetActive(true);
                    for(int i = 0; i < actors.Length; i++)
                    {
                        actors[i].image.color = idleColor;
                        //actors[i].GetComponent<Canvas>().sortingLayerName = "UIBackground";
                        //actors[i].GetComponent<Animator>().Play("PanelActive");
                    }
                    actors[actorID].image.color = talkColor;
                    //actors[actorID].GetComponent<Canvas>().sortingLayerName = "UIFront";
                    actors[actorID].animator.Play("Talk");
                    if(actors[actorID].sound != SoundManager.Sound.NONE)
                    {
                        SoundManager.PlaySound(actors[actorID].sound);
                    }
                    
                    SetName(GetName(2));
                    break;
                case "--animate":
                    int.TryParse(words[1], out actorID);
                    actors[actorID].gameObject.SetActive(true);
                    actors[actorID].image.color = talkColor;
                    //actors[actorID].GetComponent<Canvas>().sortingLayerName = "UIFront";
                    actors[actorID].animator.Play(words[2].Trim());
                    break;
                case "--sound":
                        int sound = 0;
                        int.TryParse(words[1], out sound);

                        SoundManager.PlaySound((SoundManager.Sound)Enum.ToObject(typeof(SoundManager.Sound), sound));
                    break;
                case "--fx":
                        int fx = 0;
                        int.TryParse(words[1], out fx);
                        int.TryParse(words[2], out actorID);

                        FXManager.PlayFX((FXManager.ParticleFX)Enum.ToObject(typeof(FXManager.ParticleFX), fx), actors[actorID].transform.position);
                    break;
                case "--emote":
                    int.TryParse(words[1], out actorID);

                    FXManager.PlayFX(actors[actorID].emoteFX, actors[actorID].transform.position);
                    break;
                case "--exit":
                    int.TryParse(words[1], out actorID);

                    actors[actorID].gameObject.SetActive(false);

                    break;
                case "--options":
                    textDialogueScrollBar.value = 0;
                    _skipping = false;
                    //int.TryParse(words[3], out actorID);
                    //actors[actorID].image.sprite = FindPortrait(words[3], actorID);
                    OpenOptions(words[1],words[2]);
                    goto While_Break;
                case "--level_start":
                    Debug.Log("Started Level");
                    UIManager.instance.LevelStart();
                    FindObjectOfType<CheckoutTable>().InitLevel();
                    ExitDialogue();
                    goto While_Break;
                case "--Invoke":
                    Type thisType = this.GetType();
                    if (words.Length > 2)
                    {
                        MethodInfo myMethod = thisType.GetMethod(words[1].Trim());
                        myMethod.Invoke(this,new object[]{words[2]});
                    }
                    else
                    {
                        MethodInfo myMethod = thisType.GetMethod(words[1].Trim());
                        myMethod.Invoke(this,null);
                    }
                    break;
                case "--description":
                    //actors.gameObject.SetActive(false);
                    for (int i = 0; i < actors.Length; i++)
                    {
                        actors[i].image.color = idleColor;
                        //actors[i].GetComponent<Canvas>().sortingLayerName = "UIBackground";
                        actors[i].GetComponent<Animator>().Play("PanelActive");
                    }
                    name = "";
                    break;
                default:
                    //actors.gameObject.SetActive(false);
                    name = "";
                    break;
            }
            // Ok so this is the weirdest bug i have ever encountered for SOME REASON it only works on --end when it isn't on the last line
            // it adds a weird symbol that i can't even copy or remove or trim so i am just gonna check it manually for now gonna have to ask others about it
            if (firstWord.StartsWith("--end"))
            {
                ExitDialogue();
                goto While_Break;
            }
            currentLine++;
        }
        While_Break: ;
    }

    void SwitchLine(int lineID)
    {
        optionsBox.SetActive(false);
        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        options = false;

        currentLine = lineID-1; // Since the Lines in Text editor and Arrays differ we subtract 1 to lineID
        dialogueText.gameObject.SetActive(true);
        progressButton.gameObject.SetActive(true);

        ProgressDialogue();
    }
    private void OpenOptions(string option1Line,string option2Line)
    {
            options = true;
            dialogueText.gameObject.SetActive(false);
            progressButton.gameObject.SetActive(false);
            optionsBox.SetActive(true);
            int o1 = 0, o2 = 0;
            int.TryParse(option1Line, out o1);
            int.TryParse(option2Line, out o2);
            optionText1.text = "1: " + dialogue[currentLine + 1];
            optionText2.text = "2: " + dialogue[currentLine + 2];
            option1.onClick.AddListener(() => SwitchLine(o1));
            option2.onClick.AddListener(() => SwitchLine(o2));
    }
    private void DisplayText()
    {
        if (currentLine < dialogue.Length&&!dialogue[currentLine].StartsWith("--"))
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence(dialogue[currentLine]));
            textDialogueScrollBar.value = 0;
        }
    }
    private void CompileText()
    {
        if (!dialogue[currentLine].StartsWith("--"))
        {
            StringBuilder myText = new StringBuilder();
            myText.Append(dialogueHistoryText.text + "\n");
            dialogueHistoryText.text = myText + name + dialogue[currentLine];
            dialogueText.text = dialogueHistoryText.text;
            textDialogueScrollBar.value = 0;
        }
        currentLine++;
    }
    IEnumerator TypeSentence(string sentence)
    {
        StringBuilder myText = new StringBuilder();
        myText.Append(dialogueHistoryText.text);
        if (_isTyping)
        {
            dialogueText.text = dialogueHistoryText.text;
            textDialogueScrollBar.value = 0;
        }
        else
        {
            _isTyping = true;
            dialogueHistoryText.text = myText + name + sentence +"\n";
            dialogueText.text = myText + name;
            foreach (char letter in sentence.ToCharArray())
            {
                SoundManager.PlaySound(SoundManager.Sound.Typing);
                textDialogueScrollBar.value = 0;
                dialogueText.text += letter;
                yield return new WaitForSeconds(dialogueTextSpeed);
            }
        }
        _isTyping = false;
        currentLine++;
    }
    private void SkipBarProgress()
    {
        if (Input.GetKey(KeyCode.LeftShift) || isHoldingSkip)
        {
            if (SkipBar.value<.95f)
            {
                SkipBar.value += 1.75f * Time.deltaTime;
            }
            else
            {
                Skip();
                SoundManager.PlaySound(SoundManager.Sound.Skip);
                SkipBar.value = 0;
            }
        }
        else if (SkipBar.value != 0)
        {
            SkipBar.value = 0;
        }
    }

    public void SetIsHoldingSkip(bool val)
    {
        isHoldingSkip = val;
    }

    public void StartDialogueByName(string fileName)
    {
        fileName = fileName.Remove(fileName.Length-1);
        StartDialogue(fileName);

    }

    public void RainSnow()
    {
        Instantiate(extraSnow, new Vector3(-2, 6.3f, 0), Quaternion.Euler(75,90,-90));
    }

    public void SendAnalytics(string poiName)
    {
        AnalyticsController.SendAnalyticResult($"Clicked Poi {poiName}");
    }

    public void SendFinishedAnalytics(string poiName)
    {
        AnalyticsController.SendAnalyticResult($"Finished Reading {poiName}");
    }



}

