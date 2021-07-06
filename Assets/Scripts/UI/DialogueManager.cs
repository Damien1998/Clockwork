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

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialogueBox,optionsBox;
    public Text  dialogueText,dialogueHistoryText , optionText1, optionText2;
    public Image portrait;
    public Button option1, option2,progressButton;
    public Scrollbar dialogueScrollBar,textDialogueScrollBar;
    public Slider SkipBar;

    private string[] dialogue;
    private string name;
    private int currentLine;
    [SerializeField] 
    private float dialogueTextSpeed = .025f;
    private bool _isTyping = false, _skipping = false, options;

    public AudioSource audioSource;

    [SerializeField] private ParticleSystem extraSnow;
    
    public Sprite[] portraits;
    public string[] characters;
    
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        audioSource = GetComponent<AudioSource>();
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
    public void StartDialogue(TextAsset textFile)
    {
        StopAllCoroutines();
        ClearText();
        dialogue = textFile.text.Split('\n');
        SoundManager.PlaySound(SoundManager.Sound.PoiInteraction);
        dialogueBox.SetActive(true);
        currentLine = 0;
        ProgressDialogue();
    }
    public void ResetDialogue()
    {
        ClearText();
        StopAllCoroutines();
        dialogueText.rectTransform.sizeDelta = new Vector2(dialogueText.rectTransform.sizeDelta.x,40);
            
        currentLine = 0;
        
        ProgressDialogue();
    }
    void ExitDialogue()
    {
        _isTyping = false;
        dialogueText.rectTransform.sizeDelta = new Vector2(dialogueText.rectTransform.sizeDelta.x,40);
        dialogueBox.SetActive(false);
        Time.timeScale = 1;
    }
    public void ProgressDialogue()
    {
        if (currentLine >= dialogue.Length)
        {
            ExitDialogue();
        }
        else
        {
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

    private Sprite FindPortrait(string characterName)
    {
        var cName = characterName.Trim();
        int id = FindCharacterID(cName);
        if(id != -1)
        {
            return portraits[id];
        }
        else
        {
            return null;
        } 
    }
    private Sprite FindPortrait(string characterName,string[] _nameToDisplay)
    {
        var cName = characterName.Trim();
        StringBuilder _name = new StringBuilder();
        foreach (var nameWord in _nameToDisplay)
        {
            _name.Append(nameWord);
            if (nameWord != _nameToDisplay[_nameToDisplay.Length-1])
            {
                _name.Append(" ");
            }
        }
        name = _name.ToString() + ": ";
        int id = FindCharacterID(cName);
        if(id != -1)
        {
            return portraits[id];
        }
        else
        {
            return null;
        }
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
            switch (firstWord)
            {
                case "--end":
                    ExitDialogue();
                    goto While_Break;
                case "--quest_start":
                    SaveController.hasQuest = true;
                    break;
                case "--level_end":
                    UIManager.instance.ShowLevelEnd();
                    ExitDialogue();
                    break;
                case "--portrait":
                    portrait.gameObject.SetActive(true);
                    portrait.sprite = FindPortrait(words[1],GetName(2));
                    break;
                case "--options":
                    textDialogueScrollBar.value = 0;
                    _skipping = false;
                    portrait.sprite = FindPortrait(words[3]);
                    OpenOptions(words[1],words[2]);
                    goto While_Break;
                case "--level_start":
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
                    portrait.gameObject.SetActive(false);
                    name = "";
                    break;
                default:
                    portrait.gameObject.SetActive(false);
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (SkipBar.value<.95f)
            {
                SkipBar.value += .5f*Time.deltaTime;
            }
            else
            {
                Skip();
                SkipBar.value = 0;
            }
        }
        else if (SkipBar.value != 0)
        {
            SkipBar.value = 0;
        }
    }
    
    public void StartDialogueByName(string fileName)
    {

        fileName = fileName.Remove(fileName.Length-1);
        var data = Resources.Load<TextAsset>($"Dialogue/{fileName}");
        if (data != null)
        {
            StartDialogue(data);
        }
        else
        {
            Debug.Log("No Dialogue file!");
        }
    }

    public void RainSnow()
    {
        Instantiate(extraSnow, new Vector3(-2, 6.3f, 0), Quaternion.Euler(75,90,-90));
    }
    
    
}

