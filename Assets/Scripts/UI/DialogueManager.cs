using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialogueBox,optionsBox;
    public Text nameText ,dialogueText , optionText1, optionText2;
    public Image portrait;
    public Button option1, option2,progressButton;

    private string[] dialogue;
    private int currentLine;

    public AudioSource audioSource;

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
                if (currentLine < dialogue.Length)
                {
                    dialogueText.text = dialogue[currentLine];
                    currentLine++;
                }
            }
        }
    }
    public void ExitDialogue()
    {
        Debug.Log("Hello");
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
            
            if (currentLine < dialogue.Length)
            {
                dialogueText.text = dialogue[currentLine];
                currentLine++;
            }
        }
        else
        {
            Debug.Log("No save file!");
        }

        Time.timeScale = 0;
    }

    public void StartDialogue(TextAsset textFile)
    {
        dialogue = textFile.text.Split('\n');
        
        dialogueBox.SetActive(true);
        currentLine = 0;
        CheckIfCommand();

        if (currentLine < dialogue.Length)
        {
            dialogueText.text = dialogue[currentLine];
            currentLine++;
        }
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

    public void SwitchLine(int lineID)
    {
        dialogueText.gameObject.SetActive(true);
        option1.gameObject.SetActive(false);
        option2.gameObject.SetActive(false);
        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        options = false;
        currentLine = lineID;
        progressButton.gameObject.SetActive(true);
        CheckIfCommand();

        if (currentLine < dialogue.Length)
        {
            dialogueText.text = dialogue[currentLine];
            currentLine++;
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
                case "--quest_start ":
                    GameManager.instance.StartQuest(dialogue[currentLine].Replace("--quest_start ", ""),null);
                    break;
                case "--poi":
                    GameManager.instance.CompleteQuest(dialogue[currentLine].Replace("--poi ", ""));
                    break;
                case "--end":
                    ExitDialogue();
                    break;
                case "--portrait":
                    portrait.sprite = FindPortrait(dialogue[currentLine].Replace("--portrait ", ""));
                    break;
                case "--options":
                    nameText.text = dialogue[currentLine].Replace("--", "");
                    portrait.sprite = FindPortrait(dialogue[currentLine].Replace("--", ""));
                    OpenOptions();
                    break;
            }
            currentLine++;
        }
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
            currentLine += 4;
    }
}
