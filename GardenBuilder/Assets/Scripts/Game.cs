using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int width = 11;
    public int height = 11;
    public int waterCount = 10;
    public int stoneCount = 10;
    public int myceliumCount = 2;
    public int inactiveRootsCount = 2;

    private int energyMax = 20;
    private int energy;
    public TMP_Text energyUI;
    public GameOver gameOver;
    public GameObject efect1;
    public GameObject efect2;
    public GameObject efect3;
    public GameObject efect4;

    private bool isMycorrrhizal;
    private bool hasMineral;
    private bool updateMineralVision;
    private int waterSources = 0;
    private int waterBonusRounds = 3;
    private int activeRootsCounter = 1;
    private int mushroomsCount = 0;
    private int treesCount = 0;

    private Board board;
    private Cell[,] state;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        energyMax = 10;
        waterSources = 0;
        isMycorrrhizal = false;
        hasMineral = false;
        updateMineralVision = false;
        waterBonusRounds = 3;
        activeRootsCounter = 1;
        mushroomsCount = 0;
        treesCount = 0;
        NewGame();
    }

    private void NewGame() 
    {
        state = new Cell[width, height];
        GenerateCells();
        GenerateElements();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
        energy = energyMax;
        SetEnergyUI(energyMax);
        board.SetSeedling();
    }

    private void GenerateCells() 
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                if (x == 5 && y == height - 1)
                {
                    cell.type = Cell.Type.RootActive;
                    state[x, y] = cell;
                    state[x, y].revealed = true;
                }
                else
                {
                    cell.type = Cell.Type.Dirt;
                    state[x, y] = cell;
                }
            }
        }
    }

    private void GenerateElements() 
    {
        GenerateRootsInactive();
        GenerateMycelium();
        GenerateStones();
        GenerateWater();
        RevealStartingTiles();
    }

    private void GenerateWater() 
    {
        for (int i = 0; i < waterCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x,y].type != Cell.Type.Dirt || x >= 4 && x <= 6 && y >= height - 2)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }
            }

            state[x, y].type = Cell.Type.Water;
        }
    }

    private void GenerateStones()
    {
        for (int i = 0; i < stoneCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x, y].type != Cell.Type.Dirt || x >= 4 && x <= 6 && y >= height - 2)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }
            }

            state[x, y].type = Cell.Type.Stone;
        }
    }

    private void GenerateMycelium()
    {
        for (int i = 0; i < myceliumCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(height - 2, height);

            while (state[x, y].type != Cell.Type.Dirt || x >= 4 && x <= 6 && y >= height - 2)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = height - 2;
                    }
                }
            }

            state[x, y].type = Cell.Type.Mycelium;
        }
    }

    private void GenerateRootsInactive()
    {
        for (int i = 0; i < inactiveRootsCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(height - 1, height);

            while (state[x, y].type != Cell.Type.Dirt)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = height - 2;
                    }
                }
            }

            state[x, y].type = Cell.Type.RootInactive;
        }
    }

    private void RevealStartingTiles() 
    {
        state[4,10].revealed = true;
        state[6, 10].revealed = true;
        state[5, 9].revealed = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Grow();
        }
    }

    private void Grow() 
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || !cell.revealed || cell.type == Cell.Type.RootActive)
        {
            return;
        }

        Interact(cell);
    }

    private Cell GetCell(int x, int y) 
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        }
        else 
        {
            return new Cell();
        }
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private void Interact(Cell cell) 
    {
        if (energy > 0)
        {
            energy--;
            SetEnergyUI(energy);
        }
        else
        {
            gameOver.Setup();
        }

        WaterBonus();

        switch (cell.type)
        {
            case Cell.Type.Invalid:
                break;
            case Cell.Type.Dirt: InteractionDirt(cell);
                break;
            case Cell.Type.Water: InteractionWater(cell);
                break;
            case Cell.Type.Stone: InteractionStone(cell);
                break;
            case Cell.Type.Mycelium: InteractionMycelium(cell);
                break;
            case Cell.Type.RootActive:
                break;
            case Cell.Type.RootInactive: InteractionInactiveRoot(cell);
                break;
            default:
                break;
        }
    }

    private void SetEnergyUI(int energy) 
    {
        energyUI.text = "Energy " + energy.ToString();
    }

    private void WaterBonus() 
    {
        if (waterSources > 0)
        {
            if (waterBonusRounds > 0)
            {
                waterBonusRounds--;
            }
            else
            {
                waterBonusRounds = 3;
                AddEnergy(3);
            }
        }
    }

    private void AddEnergy(int bonus) 
    {
        int newEnergy = energy + bonus;
        if (newEnergy <= energyMax)
        {
            energy += bonus;
        }
        else
        {
            energy = energyMax;
        }
        SetEnergyUI(energy);
    }

    private void SetToActiveRoot(Cell cell) 
    {
        activeRootsCounter++;
        if (activeRootsCounter >= 15)
        {
            //Show trees in start
            SceneryValues.showTrees = true;
            board.SetTree();
        }
        cell.type = Cell.Type.RootActive;
        state[cell.position.x, cell.position.y] = cell;
        board.Draw(state);
        
        RevealTiles(cell);
    }

    private void InteractionDirt(Cell cell) 
    {
        SetToActiveRoot(cell);
    }
    private void InteractionWater(Cell cell)
    {
        waterSources++;
        AddEnergy(5);
        SetToActiveRoot(cell);
        efect1.SetActive(true);
    }
    private void InteractionMycelium(Cell cell)
    {
        mushroomsCount++;
        if (mushroomsCount >= 2)
        {
            //show mushrooms in start
            SceneryValues.showMushrooms = true;
            board.SetMushroom();
        }
        if (!isMycorrrhizal)
        {
            isMycorrrhizal = true;
        }
        AddEnergy(1);
        SetToActiveRoot(cell);
        efect2.SetActive(true);
    }
    private void InteractionInactiveRoot(Cell cell)
    {
        if (isMycorrrhizal)
        {
            treesCount++;
            if (treesCount >= 2)
            {
                //Show Ferns in start
                SceneryValues.showFerns = true;
                board.SetFern();
            }
            energyMax += 5;
            SetToActiveRoot(cell);
            efect3.SetActive(true);
        }
    }
    private void InteractionStone(Cell cell)
    {
        if (isMycorrrhizal)
        {
            if (!hasMineral)
            {
                hasMineral = true;
                updateMineralVision = true;
            }
            else
            {
                AddEnergy(4);
            }
            SetToActiveRoot(cell);
            efect4.SetActive(true);
        }
    }

    private void RevealTiles(Cell cell) 
    {
        if (hasMineral)
        {
            if (updateMineralVision)
            {
                ActivateMineralVision();
            }
            else
            {
                RevealMineralVision4ExtraTiles(cell);
            }
        }
        RevealStandard4Tiles(cell);

        board.Draw(state);
    }

    private void RevealMineralVision4ExtraTiles(Cell cell) 
    {
        Cell ajacentCell1 = GetCell(cell.position.x - 1, cell.position.y - 1);
        Cell ajacentCell2 = GetCell(cell.position.x + 1, cell.position.y - 1);
        Cell ajacentCell3 = GetCell(cell.position.x - 1, cell.position.y + 1);
        Cell ajacentCell4 = GetCell(cell.position.x + 1, cell.position.y + 1);

        if (ajacentCell1.type != Cell.Type.Invalid)
        {
            ajacentCell1.revealed = true;
            state[ajacentCell1.position.x, ajacentCell1.position.y] = ajacentCell1;
        }
        if (ajacentCell2.type != Cell.Type.Invalid)
        {
            ajacentCell2.revealed = true;
            state[ajacentCell2.position.x, ajacentCell2.position.y] = ajacentCell2;
        }
        if (ajacentCell3.type != Cell.Type.Invalid)
        {
            ajacentCell3.revealed = true;
            state[ajacentCell3.position.x, ajacentCell3.position.y] = ajacentCell3;
        }
        if (ajacentCell4.type != Cell.Type.Invalid)
        {
            ajacentCell4.revealed = true;
            state[ajacentCell4.position.x, ajacentCell4.position.y] = ajacentCell4;
        }
    }

    private void RevealStandard4Tiles(Cell cell) 
    {
        Cell ajacentCell1 = GetCell(cell.position.x - 1, cell.position.y);
        Cell ajacentCell2 = GetCell(cell.position.x, cell.position.y - 1);
        Cell ajacentCell3 = GetCell(cell.position.x + 1, cell.position.y);
        Cell ajacentCell4 = GetCell(cell.position.x, cell.position.y + 1);

        if (ajacentCell1.type != Cell.Type.Invalid)
        {
            ajacentCell1.revealed = true;
            state[ajacentCell1.position.x, ajacentCell1.position.y] = ajacentCell1;
        }
        if (ajacentCell2.type != Cell.Type.Invalid)
        {
            ajacentCell2.revealed = true;
            state[ajacentCell2.position.x, ajacentCell2.position.y] = ajacentCell2;
        }
        if (ajacentCell3.type != Cell.Type.Invalid)
        {
            ajacentCell3.revealed = true;
            state[ajacentCell3.position.x, ajacentCell3.position.y] = ajacentCell3;
        }
        if (ajacentCell4.type != Cell.Type.Invalid)
        {
            ajacentCell4.revealed = true;
            state[ajacentCell4.position.x, ajacentCell4.position.y] = ajacentCell4;
        }
    }

    private void ActivateMineralVision() 
    {
        updateMineralVision = false;
        int workRootsCounter = activeRootsCounter;

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                Cell workingCell = new Cell();
                workingCell.position = new Vector3Int(x, y, 0);
                workingCell.type = state[x, y].type;
                if (workingCell.type == Cell.Type.RootActive)
                {
                    RevealStandard4Tiles(workingCell);
                    RevealMineralVision4ExtraTiles(workingCell);
                    workRootsCounter--;
                    if (workRootsCounter <= 0)
                    {
                        y = 0;
                        x = width - 1;
                    }
                }
            }
        }
    }
}
