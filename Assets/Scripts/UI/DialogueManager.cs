using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

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
    private bool _isTyping = false;

    public AudioSource audioSource;

    [SerializeField] private ParticleSystem extraSnow;

    public Sprite[] portraits;
    public string[] characters;

    private bool options,_typing;

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
        if (!options)
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
    }

    public void ResetDialogue()
    {
        dialogue = null;
        dialogueText.text = null;
        StopAllCoroutines();
        dialogueText.rectTransform.sizeDelta = new Vector2(dialogueText.rectTransform.sizeDelta.x,40);
            
        currentLine = 0;
        CheckIfCommand();

        DisplayText();
    }
    void ExitDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogue = null;
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
        dialogue = null;
        dialogue = textFile.text.Split('\n');
        
        dialogueBox.SetActive(true);
        currentLine = 0;
        CheckIfCommand();

        DisplayText();
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

    private Sprite FindPortrait(string characterName)
    {
        var cName = characterName.Trim();
        name = characterName + ": ";

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
            _name.Append(" ");
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
            switch (firstWord)
            {
                case "--quest_start":
                    GameManager.instance.StartQuest(dialogue[currentLine].Replace("--quest_start ", ""),null);
                    break;
                case "--poi":
                    GameManager.instance.CompleteQuest(dialogue[currentLine].Replace("--poi ", ""));
                    break;
                case "--end":
                    ExitDialogue();
                    goto While_Break;
                case "--level_end":
                    UIManager.instance.ShowLevelEnd();
                    ExitDialogue();
                    break;
                case "--portrait":
                    portrait.gameObject.SetActive(true);
                    if(words.Length >2)
                    {
                        string[] nameToDisplay = new string[words.Length-2];
                        for (int i = 2; i < words.Length; i++)
                        {
                            nameToDisplay[i - 2] = words[i];
                        }
                        portrait.sprite = FindPortrait(words[1], nameToDisplay);
                    }
                    else
                    {
                        portrait.sprite = FindPortrait(dialogue[currentLine].Replace("--portrait ", ""));
                    }
                    break;
                case "--options":
                    portrait.sprite = FindPortrait(dialogue[currentLine].Replace("--options", ""));
                    OpenOptions();
                    break;
                case "--level_start":
                    UIManager.instance.LevelStart();
                    FindObjectOfType<CheckoutTable>().InitLevel();
                    goto While_Break;
                case "--Invoke":
                    Type thisType = this.GetType();
                    MethodInfo myMethod = thisType.GetMethod(words[1].Trim());
                    myMethod.Invoke(this,null);
                    break;
                default:
                    portrait.gameObject.SetActive(false);
                    name = "";
                    break;

            }
            currentLine++;
        }
        While_Break: ;
    }
    void SwitchLine(int lineID)
    {
        dialogueText.gameObject.SetActive(true);
        optionsBox.SetActive(false);
        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        options = false;
        currentLine = lineID;
        progressButton.gameObject.SetActive(true);
        
        DisplayText();

        CheckIfCommand();
        
    }
    private void OpenOptions()
    {
            options = true;
            dialogueText.gameObject.SetActive(false);
            progressButton.gameObject.SetActive(false);
            optionsBox.SetActive(true);
            int o1 = 0, o2 = 0;
            int.TryParse(dialogue[currentLine + 2].Replace("--", ""), out o1);
            int.TryParse(dialogue[currentLine + 4].Replace("--", ""), out o2);
            optionText1.text = "1: " + dialogue[currentLine + 1];
            option1.onClick.AddListener(() => SwitchLine(o1));
            optionText2.text = "2: " + dialogue[currentLine + 3];
            option2.onClick.AddListener(() => SwitchLine(o2));
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
        else
        {
            SkipBar.value = 0;
        }
    }
    public void Skip()
    {
        while (currentLine < dialogue.Length )
        {
            if (dialogue[currentLine].StartsWith("--"))
            {
                string[] words = dialogue[currentLine].Split(' ');
                string firstWord = words[0];
                switch (firstWord)
                {
                    case "--options":
                        CheckIfCommand();
                        goto SkipAdding;
                    case "--level_start":
                        CheckIfCommand();
                        break;
                    case "--level_end":
                        CheckIfCommand();
                        break;
                    case "--end":
                        ExitDialogue();
                        goto Break_while;
                }
            }
            currentLine++;
            SkipAdding: ;
        }
        Break_while: ;
    }
    private void DisplayText()
    {
        if (currentLine < dialogue.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence(dialogue[currentLine]));
            dialogueScrollBar.value = 0;
        }
    }
    IEnumerator TypeSentence(string sentence)
    {
        StringBuilder myText = new StringBuilder();
        var delay = .025f;
        if (dialogueText.text.Length!=0)
        {
            myText.Append(dialogueHistoryText.text+"\n");
        }
        if (_isTyping)
        {
            dialogueText.text = dialogueHistoryText.text;
            textDialogueScrollBar.value = 0;
            _isTyping = false;
            currentLine++;
        }
        else
        {
            dialogueHistoryText.text = myText + name + sentence;
            _isTyping = true;
            dialogueText.text = myText + name;
            foreach (var letter in sentence.ToCharArray())
            {
                textDialogueScrollBar.value = 0;
                dialogueText.text += letter;
                yield return new WaitForSeconds(delay);
            }
            _isTyping = false;
            currentLine++;
        }
    }
    public void RainSnow()
    {
        Instantiate(extraSnow, new Vector3(-2, 6.3f, 0), Quaternion.Euler(75,90,-90));
    }
}
