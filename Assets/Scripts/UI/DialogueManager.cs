﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;
using System.Text;

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
    private bool _isTyping = false,_skipping = false;

    public AudioSource audioSource;

    [SerializeField] private ParticleSystem extraSnow;
    
    public Sprite[] portraits;
    public string[] characters;

    private bool options;

    // Start is called before the first frame update
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

    public void ResetDialogue()
    {
        dialogue = null;
        dialogueText.text = null;
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

    public void AcceptQuest(Item WatchToMake)
    {
        if (GameManager.instance.sideQuestActive == false)
        {
         GameManager.instance.StartQuest("Epic Quest",WatchToMake);
        }
    }
    public void StartDialogue(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/dialogue/" + fileName + ".txt"))
        {
            //BinaryFormatter formatter = new BinaryFormatter();
            //FileStream file = File.Open(Application.persistentDataPath + "/dialogue/" + fileName + ".txt", FileMode.Open);
            //dialogue = (string[])formatter.Deserialize(file);
            //file.Close();
            dialogue = System.IO.File.ReadAllLines(Application.persistentDataPath + "/dialogue/" + fileName + ".txt");
            dialogueBox.SetActive(true);
            currentLine = 0;
            CheckIfCommand();
            
            DisplayText();
            
        }
        else
        {
            Debug.Log("No save file!");
        }

        Time.timeScale = 0;
    }

    public void StartDialogue(TextAsset textFile)
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogue = null;
        dialogue = textFile.text.Split('\n');
        
        dialogueBox.SetActive(true);
        currentLine = 0;
        ProgressDialogue();
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

    private void CheckIfCommand()
    {
        while (currentLine < dialogue.Length && dialogue[currentLine].StartsWith("--"))
        {
            string[] words = dialogue[currentLine].Split(' ');
            string firstWord = words[0];
            Debug.Log(firstWord);
            switch (firstWord)
            {
                case "--end":
                    ExitDialogue();
                    goto While_Break;
                case "--quest_start":
                    GameManager.instance.StartQuest(dialogue[currentLine].Replace("--quest_start ", ""),null);
                    break;
                case "--poi":
                    GameManager.instance.CompleteQuest(dialogue[currentLine].Replace("--poi ", ""));
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
                    MethodInfo myMethod = thisType.GetMethod(words[1].Trim());
                    myMethod.Invoke(this,null);
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
        
        currentLine = lineID-1;
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

    private void Skip()
    {
        _skipping = true;
        _isTyping = false;
        while (currentLine < dialogue.Length  && _skipping)
        {
            CheckIfCommand();
            CompileText();
        }
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
        if (dialogueText.text.Length!=0)
        {
            myText.Append(dialogueHistoryText.text+"\n");
        }
        if (_isTyping)
        {
            dialogueText.text = dialogueHistoryText.text;
            textDialogueScrollBar.value = 0;
        }
        else
        {
            _isTyping = true;
            dialogueHistoryText.text = myText + name + sentence;
            dialogueText.text = myText + name;
            foreach (char letter in sentence.ToCharArray())
            {
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
    public void RainSnow()
    {
        Instantiate(extraSnow, new Vector3(-2, 6.3f, 0), Quaternion.Euler(75,90,-90));
    }
    
}

