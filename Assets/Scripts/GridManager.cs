using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates and manages the grid of blocks for the game.
/// </summary>
public class GridManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static GridManager Instance { get; private set; }
    [Header("Grid Settings")]
    public int gridWidth = 5;
    public int gridHeight = 6;
    public float cellSize = 100f;
    // Offsets to adjust grid position
    public float xOffset = 0f;
    public float yOffset = 0f;
    // Added padding
    public float xPadding = 0f;
    public float yPadding = 0f;

    [Header("References")]
    public GameObject blockPrefab;
    public Transform gridParent;

    [Header("Blocks")]
    [SerializeField]
    private BlocksDb blocksDb;
    private Block[,] grid;
    private bool isProcessing = false;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Clear existing grid
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        grid = new Block[gridWidth, gridHeight];

        // Calculate grid offset to center the grid relative to gridParent
        Vector2 gridOffset = CalculateGridOffset();

        // Create initial grid with random colors
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                CreateBlock(x, y, gridOffset);
            }
        }
    }

    Vector2 CalculateGridOffset()
    {
        // Calculate the offset to center the grid within gridParent
        float offsetX = -(gridWidth - 1) * (cellSize + xPadding) * 0.5f;
        float offsetY = -(gridHeight - 1) * (cellSize + yPadding) * 0.5f;

        return new Vector2(offsetX, offsetY);
    }

    void CreateBlock(int x, int y, Vector2 gridOffset)
    {
        // Calculate local position relative to gridParent with offsets and padding
        Vector3 localPosition = new Vector3(
            x * (cellSize + xPadding) + gridOffset.x + xOffset,
            y * (cellSize + yPadding) + gridOffset.y + yOffset,
            0
        );

        // Instantiate as child of gridParent and set local position
        GameObject blockObj = Instantiate(blockPrefab, gridParent);
        blockObj.transform.localPosition = localPosition;

        int iconIndex = Random.Range(0, blocksDb.GetIconsCount());
        Block block = blockObj.GetComponent<Block>();
        block.Initialize(x, y, iconIndex, blocksDb.GetIcon(iconIndex), this);

        grid[x, y] = block;
    }

    void CreateBlock(int x, int y, int specificColor = -1)
    {
        // Overload for refill - recalculate offset
        Vector2 gridOffset = CalculateGridOffset();
        Vector3 localPosition = new Vector3(
            x * (cellSize + xPadding) + gridOffset.x + xOffset,
            y * (cellSize + yPadding) + gridOffset.y + yOffset,
            0
        );

        GameObject blockObj = Instantiate(blockPrefab, gridParent);
        blockObj.transform.localPosition = localPosition;

        int iconIndex = specificColor == -1 ? Random.Range(0, blocksDb.GetIconsCount()) : specificColor;
        Block block = blockObj.GetComponent<Block>();
        block.Initialize(x, y, iconIndex, blocksDb.GetIcon(iconIndex), this);

        grid[x, y] = block;
    }

    public void OnBlockClicked(Block clickedBlock)
    {
        if (isProcessing) return;

        StartCoroutine(ProcessBlockCollection(clickedBlock));
    }

    IEnumerator ProcessBlockCollection(Block clickedBlock)
    {
        isProcessing = true;

        // Find all connected blocks of same color
        List<Block> connectedBlocks = FindConnectedBlocks(clickedBlock);

        // Only remove if there are 2 or more connected blocks
        if (connectedBlocks.Count > 1)
        {
            // Store the original clicked position for chain reaction checking
            Vector2Int originalPosition = new Vector2Int(clickedBlock.X, clickedBlock.Y);

            // Calculate score (1 for 1 block, 2 for 2, etc. - triangular numbers)
            int blocksCollected = connectedBlocks.Count;

            // Remove blocks
            foreach (Block block in connectedBlocks)
            {
                RemoveBlock(block.X, block.Y);
            }

            // Update game state
            GameManager.Instance.AddScore(blocksCollected);
            GameManager.Instance.UseMove();

            // Wait before refilling
            yield return new WaitForSeconds(1f);

            // Refill grid and check for chain reactions at the original position
            /// Note: THIS IS AN EXTRA FEATURE beyond the basic requirements
            yield return StartCoroutine(RefillAndCheckPositionMatches(originalPosition));
        }
        else
        {
            // Single block clicked - no removal
            Debug.Log("Single block clicked - no removal");
        }

        // Always check for game over after all refills and chain reactions are complete
        if (GameManager.Instance.GetMovesRemaining() <= 0)
        {
            GameManager.Instance.ShowGameOver();
        }

        isProcessing = false;
    }

    IEnumerator RefillAndCheckPositionMatches(Vector2Int checkPosition)
    {
        bool foundMatches = true;
        int chainCount = 0;

        while (foundMatches)
        {
            // First refill the grid
            yield return StartCoroutine(RefillGrid());

            // Wait a moment for blocks to settle
            yield return new WaitForSeconds(0.5f);

            // Check for matches only at the original clicked position
            Block blockAtPosition = GetBlockAtPosition(checkPosition.x, checkPosition.y);

            if (blockAtPosition != null)
            {
                List<Block> positionMatches = FindConnectedBlocks(blockAtPosition);

                // Only process if it's a valid match (2+ blocks)
                if (positionMatches.Count > 1)
                {
                    chainCount++;
                    foundMatches = true;

                    int blocksCollected = positionMatches.Count;

                    // Remove all matched blocks
                    foreach (Block block in positionMatches)
                    {
                        RemoveBlock(block.X, block.Y);
                    }

                    // Award points for chain reaction (no move used)
                    GameManager.Instance.AddScore(blocksCollected);

                    // Wait before next refill
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    foundMatches = false;
                }
            }
            else
            {
                foundMatches = false;
            }
        }
    }

    Block GetBlockAtPosition(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return grid[x, y];
        }
        return null;
    }

    List<Block> FindConnectedBlocks(Block startBlock)
    {
        List<Block> connected = new List<Block>();
        HashSet<Block> visited = new HashSet<Block>();
        Queue<Block> toCheck = new Queue<Block>();

        int targetColor = startBlock.IconIndex;

        toCheck.Enqueue(startBlock);
        visited.Add(startBlock);

        while (toCheck.Count > 0)
        {
            Block current = toCheck.Dequeue();
            connected.Add(current);

            // Check adjacent blocks (up, down, left, right)
            CheckAdjacent(current.X + 1, current.Y, targetColor, visited, toCheck);
            CheckAdjacent(current.X - 1, current.Y, targetColor, visited, toCheck);
            CheckAdjacent(current.X, current.Y + 1, targetColor, visited, toCheck);
            CheckAdjacent(current.X, current.Y - 1, targetColor, visited, toCheck);
        }

        return connected;
    }

    void CheckAdjacent(int x, int y, int targetColor, HashSet<Block> visited, Queue<Block> toCheck)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            Block block = grid[x, y];
            if (block != null && block.IconIndex == targetColor && !visited.Contains(block))
            {
                visited.Add(block);
                toCheck.Enqueue(block);
            }
        }
    }

    void RemoveBlock(int x, int y)
    {
        if (grid[x, y] != null)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    IEnumerator RefillGrid()
    {
        Vector2 gridOffset = CalculateGridOffset();
        bool blocksMoved = true;

        while (blocksMoved)
        {
            blocksMoved = false;

            // Make blocks fall down
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] == null)
                    {
                        // Look for blocks above to fall down
                        for (int aboveY = y + 1; aboveY < gridHeight; aboveY++)
                        {
                            if (grid[x, aboveY] != null)
                            {
                                // Move block down
                                Block fallingBlock = grid[x, aboveY];
                                grid[x, y] = fallingBlock;
                                grid[x, aboveY] = null;
                                fallingBlock.X = x;
                                fallingBlock.Y = y;

                                // Update LOCAL position with centering, offsets, and padding
                                Vector3 newLocalPosition = new Vector3(
                                    x * (cellSize + xPadding) + gridOffset.x + xOffset,
                                    y * (cellSize + yPadding) + gridOffset.y + yOffset,
                                    0
                                );
                                fallingBlock.transform.localPosition = newLocalPosition;
                                blocksMoved = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Small delay between fall steps for visual effect (even without animations)
            if (blocksMoved)
                yield return new WaitForSeconds(0.1f);
        }

        // Fill empty spaces at top with new blocks
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    CreateBlock(x, y);
                }
            }
        }

        yield return null;
    }

    public void ResetGrid()
    {
        InitializeGrid();
    }

    // Debug visualization
    void OnDrawGizmos()
    {
        if (gridParent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gridParent.position, 0.2f);

            // Calculate grid bounds in world space including offsets and padding
            Vector2 offset = CalculateGridOffset();
            Vector3 worldCenter = gridParent.TransformPoint(new Vector3(xOffset, yOffset, 0));
            Vector3 worldSize = new Vector3(
                gridWidth * (cellSize + xPadding) - xPadding,
                gridHeight * (cellSize + yPadding) - yPadding,
                0.1f
            );

            Gizmos.color = Color.blue;
            Gizmos.matrix = gridParent.localToWorldMatrix;
            Gizmos.DrawWireCube(new Vector3(xOffset, yOffset, 0), worldSize);
        }
    }
}