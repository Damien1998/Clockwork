using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Refactoring is done! You may enter safely
public class GameManager : MonoBehaviour
{
    //List of ALL avaliable items
    //God help us
    public Activator[] items;

    //The only game manager allowed to stay in this world of flesh
    public static GameManager instance;

    //All state sprites
    public Sprite repairedImage;
    public Sprite brokenImage;
    public Sprite unfixableImage;
    public Sprite unknownImage;
    public Sprite complexBrokenImage;

    //Min and max broken individual pieces per watch (the most basic ones)
    //Currently not used
    //Will probably be needed for difficulty settings and scaling
    public int minBrokenPieces = 1;
    public int maxBrokenPieces = 8;

    //A data type for holding workbench recipies
    public struct Recipe
    {
        public Recipe(int result, int part0, int part1, int part2)
        {
            resultID = result;
            partID = new int[] { part0, part1, part2 };           
        }

        //Recipe result item ID
        public int resultID;
        //Required part IDs
        /// <summary>
        /// It's an array for less clunky management, but it has to have 3 items inside
        /// Input '-1' as IDs for blank items
        /// I.e. when a recipe only takes two or one item to make
        /// PLEASE sort the values and put blank items at the end
        /// </summary>       
        public int[] partID;
    }

    //ALL recipies for the "basic" workbench
    //They work both ways, according to the workbench's functionality
    public Recipe[] basicRecipes;

    // Start is called before the first frame update
    void Start()
    {   
        //Keeping the population of game managers in check
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        LoadRecipes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadRecipes()
    {
        basicRecipes = new Recipe[10];
        //Five watches
        basicRecipes[0] = new Recipe(0, 5, 10, -1);
        basicRecipes[1] = new Recipe(1, 6, 10, 11);
        basicRecipes[2] = new Recipe(2, 7, 10, -1);
        basicRecipes[3] = new Recipe(3, 8, 10, 12);
        basicRecipes[4] = new Recipe(4, 9, 10, -1);
        //Watch casings
        basicRecipes[5] = new Recipe(5, 13, 14, 17);
        basicRecipes[6] = new Recipe(6, 15, 16, 17);
        basicRecipes[7] = new Recipe(7, 18, 19, 20);
        basicRecipes[8] = new Recipe(8, 21, 22, 24);
        basicRecipes[9] = new Recipe(9, 23, 24, 25);
    }
}
