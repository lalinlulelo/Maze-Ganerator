using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject northWall; //1
        public GameObject eastWall; //2
        public GameObject westWall; //3
        public GameObject southWall; //4
    }

    public GameObject floor;//
    public GameObject wall;//

    private float wallLength = 1.0f;

    private int width;//
    private int height;//
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    Regex numberRegex = new Regex("[0-9]");

    private Vector3 initialPos;
    private GameObject wallHolder;

    public Cell[] cells;//

    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;

    public GameObject MyPlayer;
    private bool startGame = false;
    public NavMeshSurface surface;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetValues()
    {
        width = int.Parse(widthInput.text); 
        height = int.Parse(heightInput.text);

        currentCell = 0;
        visitedCells = 0;
        startedBuilding = false;
        currentNeighbour = 0;
        backingUp = 0;
        wallToBreak = 0;
        //Make sure that not creating more than one maze at the same time
        if (wallHolder)
        {
            Destroy(wallHolder);
        }
        widthInput.text = "";
        heightInput.text = "";
    }

    public void CreateWalls()
    {
        GameObject tempFloor;
        
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        initialPos = new Vector3((-width / 2) + wallLength / 2, 0.0f, (-height / 2) + wallLength / 2);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        //For Width
        for(int i=0; i<height; i++)
        {
            for(int j=0; j<=width; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i*wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //For Height
        for (int i = 0; i <= height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //For floor
        for (int i = 0; i <= height - 1; i++)
        {
            for (int j = 0; j < width; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), -0.5f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempFloor = Instantiate(floor, myPos, Quaternion.identity) as GameObject;
                tempFloor.transform.parent = wallHolder.transform;
            }
        }

        CreateCells();
    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = width * height;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[width * height];
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        //Gets all the children
        for(int i=0; i<children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }
        //Assign walls to the cells
        for(int cellprocess=0; cellprocess<cells.Length; cellprocess++)
        {
            if (termCount == width)
            {
                eastWestProcess ++;
                termCount = 0;
            }
            cells[cellprocess] = new Cell();
            cells[cellprocess].eastWall = allWalls[eastWestProcess];
            cells[cellprocess].southWall = allWalls[childProcess + (width+1) * height];

            eastWestProcess++;

            termCount++;
            childProcess++;
            cells[cellprocess].westWall = allWalls[eastWestProcess];
            cells[cellprocess].northWall = allWalls[(childProcess + (width+1) * height) + width-1];
        }
        CreateMaze();
    }

    void CreateMaze()
    {
        while(visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();
                if(cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if(lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }
        }
    }

    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1: Destroy(cells[currentCell].northWall); break;
            case 2: Destroy(cells[currentCell].eastWall); break;
            case 3: Destroy(cells[currentCell].westWall); break;
            case 4: Destroy(cells[currentCell].southWall); break;
        }
    }

    void GiveMeNeighbour()
    {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = ((currentCell + 1) / width);
        check -= 1;
        check *= width;
        check += width;
        
        //Make sure that we are not in the corner or last one in x or y

        //West
        if(currentCell+1 < totalCells && (currentCell+1) != check)
        {
            if(cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //East
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //North
        if (currentCell + width < totalCells)
        {
            if (cells[currentCell + width].visited == false)
            {
                neighbours[length] = currentCell + width;
                connectingWall[length] = 1;
                length++;
            }
        }

        //South
        if (currentCell - width >= 0)
        {
            if (cells[currentCell - width].visited == false)
            {
                neighbours[length] = currentCell - width;
                connectingWall[length] = 4;
                length++;
            }
        }

        if(length != 0)
        {
            int theChosenOne = Random.Range(0, length);
            currentNeighbour = neighbours[theChosenOne];
            wallToBreak = connectingWall[theChosenOne];
        }
        else
        {
            if(backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (numberRegex.IsMatch(widthInput.text))
        {

        }
        else
        {
            widthInput.text = "";
        }

        if (numberRegex.IsMatch(heightInput.text))
        {

        }
        else
        {
            heightInput.text = "";
        }
    }

    public void StartGame()
    {
        if (!startGame)
        {
            MyPlayer = Instantiate(MyPlayer, new Vector3(initialPos.x,initialPos.y,initialPos.z-0.3f), Quaternion.identity) as GameObject;
            startGame = true;
        }
        else
        {
            MyPlayer.transform.SetPositionAndRotation(new Vector3(initialPos.x, initialPos.y, initialPos.z - 0.3f), Quaternion.identity);
        }
        surface.BuildNavMesh();
    }
}
