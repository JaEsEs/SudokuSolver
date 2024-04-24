using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Sudoku : MonoBehaviour
{
    public GameObject inputPrefab; // prefab of the cell
    private const int ROWS = 9;
    private const int COLS = 9;
    public float spacing = 60f;
    public Color mainColor, secondaryColor;

    private TMP_InputField[,] board;

    private void Start()
    {
        CreateBoard();
    }

    #region Board Creation
    private void CreateBoard()
    {
        // Calculate the offset to center the grid
        float offsetX = (COLS - 1) * spacing / 2f;
        float offsetY = (ROWS - 1) * spacing / 2f;

        // Initialize the array of cells
        board = new TMP_InputField[ROWS, COLS];

        // Create each cell and assign its position and color
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                CreateCell(row, col, offsetX, offsetY);
                PaintCellBlock(row, col);
            }
        }
    }

    // Create a cell at the given position and add it to the array of cells
    private void CreateCell(int row, int col, float offsetX, float offsetY)
    {
        // Create the cell
        GameObject input = Instantiate(inputPrefab, transform);
        // Assign the position of the cell
        input.GetComponent<RectTransform>().localPosition = new Vector3(col * spacing - offsetX, offsetY - row * spacing, 0);
        // Add the cell to the array of cells
        board[row, col] = input.GetComponent<TMP_InputField>();
    }

    // Assign the color of the cell based on its position in the 3x3 block
    private void PaintCellBlock(int row, int col)
    {
        bool isInSecondaryBlock = (row >= 0 && row <= 2 || row >= 6 && row <= 8) && (col >= 0 && col <= 2 || col >= 6 && col <= 8);
        bool isInMainBlock = row >= 3 && row <= 5 && col >= 3 && col <= 5;

        // Assign the color of the cell based on its position in the 3x3 block
        board[row, col].GetComponent<Image>().color = isInSecondaryBlock || isInMainBlock ? secondaryColor : mainColor;
    }

    #endregion

    #region Step by step solution
    public void StepbyStep()
    {
        ChangeNumberColor();
        StartCoroutine(SolverStepbyStep());
    }

    private bool result = false;
    private IEnumerator SolverStepbyStep()
    {
        // Search for an empty cell
        int[] cellsExist = EmptyCellsExist();

        // If there are no more empty cells, a solution has been found
        if (cellsExist == null)
        {
            result = true;
            yield break;
        }

        int row = cellsExist[0], col = cellsExist[1];
        for (int num = 1; num <= 9; num++)
        {
            if (CheckNumber(row, col, num.ToString()))
            {
                yield return new WaitForSeconds(0.01f);
                // If the number is valid, assign the number to the empty cell.
                board[row, col].text = num.ToString();
                result = false;

                yield return StartCoroutine(SolverStepbyStep());
                // Recursion -- If true, a solution was found.
                if (result)
                {
                    yield break;
                }
                else
                {
                    yield return new WaitForSeconds(0.01f);
                    // If it returns false, clear the cell and try the next number in the for loop.
                    board[row, col].text = "";
                }
            }
        }
    }
    #endregion

    #region Inmediate solution

    public void Solution()
    {
        ChangeNumberColor();

        if (InmediateSololution())
        {
            print("Solved");
        }
        else
        {
            print("no solution");
        }
    }

    private bool InmediateSololution()
    {
        // Search for an empty cell
        int[] cellsExist = EmptyCellsExist();

        // If there are no more empty cells, a solution has been found
        if (cellsExist == null)
        {
            return true;
        }

        int row = cellsExist[0], col = cellsExist[1];
        for (int num = 1; num <= 9; num++)
        {
            if (CheckNumber(row, col, num.ToString()))
            {
                // If the number is valid, assign the number to the empty cell.
                board[row, col].text = num.ToString();
                // Recursion -- If true, a solution was found.
                if (InmediateSololution())
                {
                    return true;
                }
                else
                {
                    // If it returns false, clear the cell and try the next number in the for loop.
                    board[row, col].text = "";
                }
            }
        }

        return false;
    }
    #endregion


    // Get the first unfilled cell
    private int[] EmptyCellsExist()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                if (board[row, col].text == "")
                {
                    return new int[] { row, col };
                }
            }
        }

        return null;
    }

    // Validate the number to ensure it doesn't violate Sudoku rules
    private bool CheckNumber(int r, int c, string num)
    {
        // Check horizontally
        for (int col = 0; col < COLS; col++)
        {
            // If the number exists in the same row, return false
            if (board[r, col].text == num)
            {
                return false;
            }
        }

        // Check Vertically
        for (int row = 0; row < ROWS; row++)
        {
            // If the number exists in the same column, return false
            if (board[row, c].text == num)
            {
                return false;
            }
        }

        // Calculate the starting row and column of the 3x3 box
        int boxRow = (r / 3) * 3;
        int boxCol = (c / 3) * 3;

        // Check the 3x3 box
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                // If the number exists in the 3x3 box, return false
                if (board[boxRow + row, boxCol + col].text == num)
                {
                    return false;
                }
            }
        }

        // If the number passes all checks, return true
        return true;
    }

    // Change the color of the text
    private void ChangeNumberColor()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                if (board[row, col].text == "")
                {
                    board[row, col].textComponent.color = Color.grey;
                }
            }
        }
    }

    public void RestartBoard()
    {
        StopAllCoroutines();

        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                if (board[row, col].text != "")
                {
                    board[row, col].text = "";
                }
                board[row, col].textComponent.color = Color.black;
            }
        }
    }

}