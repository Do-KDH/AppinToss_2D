using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Diagnostics;

public class RandomCreate : MonoBehaviour
{
    private const int ROWS = 12;
    private const int COLS = 8;
    private const int PATH_LENGTH = 7;
    private const int PATH_COUNT = 9;

    private const int EMPTY = -1;
    private const int DEFAULT_GRID = 1;
    private const int PATH_START = 2;

    private int[,] grid = new int[ROWS, COLS];
    private bool[,] used = new bool[ROWS, COLS];
    private int[,] currentActiveGrid;

    public GameObject[] colorButtonList;
    private int ranColor;

    private static readonly List<Vector2Int> Directions = new()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    public void InitializeCurrentActiveGrid()
    {
        currentActiveGrid = new int[ROWS, COLS];
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
                currentActiveGrid[r, c] = EMPTY;

        foreach (Transform child in transform)
        {
            ButtonImage buttonImage = child.GetComponent<ButtonImage>();
            if (buttonImage != null && child.gameObject.activeSelf)
                currentActiveGrid[buttonImage.mat.x, buttonImage.mat.y] = buttonImage.colorValue;
        }
    }

    public bool HasNoRemainingSequences()
    {
        InitializeCurrentActiveGrid();

        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                if (currentActiveGrid[r, c] != EMPTY)
                {
                    var path = new List<Vector2Int>();
                    var visited = new bool[ROWS, COLS];

                    if (FindSequenceDFS(r, c, 3, path, visited, currentActiveGrid))
                        return false;
                }
            }
        }
        return true;
    }

    bool FindSequenceDFS(int row, int col, int targetLength, List<Vector2Int> currentPath, bool[,] visited, int[,] gridToSearch)
    {
        if (row < 0 || row >= ROWS || col < 0 || col >= COLS || visited[row, col] || gridToSearch[row, col] == EMPTY)
            return false;

        if (currentPath.Count > 0)
        {
            Vector2Int last = currentPath[^1];
            if (gridToSearch[row, col] != gridToSearch[last.x, last.y] + 1)
                return false;
        }

        visited[row, col] = true;
        currentPath.Add(new(row, col));

        if (currentPath.Count >= targetLength)
            return true;

        var directions = new List<Vector2Int>(Directions);
        Utils.Shuffle(directions);

        foreach (var dir in directions)
        {
            if (FindSequenceDFS(row + dir.x, col + dir.y, targetLength, currentPath, visited, gridToSearch))
                return true;
        }

        visited[row, col] = false;
        currentPath.RemoveAt(currentPath.Count - 1);
        return false;
    }

    public void InColorButton()
    {
        float cellWidth = 80f, cellHeight = 80f;
        Vector2 startPosition = new(-283f, 432f);
        RectTransform parent = GetComponent<RectTransform>();

        float scale = Mathf.Min(parent.rect.width / (COLS * cellWidth), parent.rect.height / (ROWS * cellHeight), 1f);

        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                ranColor = (grid[r, c] == DEFAULT_GRID) ? Random.Range(0, colorButtonList.Length)
                            : Mathf.Clamp(grid[r, c] - PATH_START, 0, colorButtonList.Length - 1);

                GameObject child = Instantiate(colorButtonList[ranColor], transform);
                RectTransform rect = child.GetComponent<RectTransform>();

                rect.anchoredPosition = new(startPosition.x + c * cellWidth, startPosition.y - r * cellHeight);
                rect.localScale = Vector3.one;

                ButtonImage button = child.GetComponent<ButtonImage>();
                if (button)
                {
                    button.colorValue = ranColor;
                    button.mat = new Vector2Int(r, c);
                }
            }
        }
    }

    void Start()
    {
        InitializeGrid();
        GeneratePaths(PATH_COUNT, PATH_LENGTH);
        PrintGrid();
        InColorButton();
    }

    void InitializeGrid()
    {
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
                grid[r, c] = DEFAULT_GRID;
    }

    void GeneratePaths(int totalPaths, int pathLength)
    {
        int pathId = PATH_START;
        int attempts = 0;

        List<Vector2Int> candidates = new();
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
                candidates.Add(new Vector2Int(r, c));

        Utils.Shuffle(candidates);

        foreach (var start in candidates)
        {
            if (pathId >= PATH_START + totalPaths) break;

            List<Vector2Int> path = new();
            bool[,] visited = new bool[ROWS, COLS];

            if (!used[start.x, start.y] && DFS(start.x, start.y, pathLength, path, visited))
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var cell = path[i];
                    grid[cell.x, cell.y] = PATH_START + i;
                    used[cell.x, cell.y] = true;
                }
                pathId++;
            }

            if (++attempts > 5000) break;
        }
    }

    bool DFS(int row, int col, int remaining, List<Vector2Int> path, bool[,] visited)
    {
        if (row < 0 || row >= ROWS || col < 0 || col >= COLS || visited[row, col] || used[row, col])
            return false;

        visited[row, col] = true;
        path.Add(new Vector2Int(row, col));

        if (remaining == 1) return true;

        var dirs = new List<Vector2Int>(Directions);
        Utils.Shuffle(dirs);

        foreach (var dir in dirs)
        {
            if (DFS(row + dir.x, col + dir.y, remaining - 1, path, visited))
                return true;
        }

        visited[row, col] = false;
        path.RemoveAt(path.Count - 1);
        return false;
    }

    void PrintGrid()
    {
        string output = "";
        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
                output += grid[r, c].ToString().PadLeft(2) + " ";
            output += "\n";
        }
        Debug.Log(output);
    }

    public void ResetGrid()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);  // 기존 버튼 제거

        InitializeGrid();
        GeneratePaths(PATH_COUNT, PATH_LENGTH);
        PrintGrid();
        InColorButton();
    }
}
