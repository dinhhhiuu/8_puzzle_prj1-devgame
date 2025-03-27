using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public GridLayoutGroup gridLayout; 
    private List<Transform> pieces = new List<Transform>();
    private Transform emptyPiece; 
    public Sprite finalImage; 

    void Start()
    {
        InitializePieces();
        AssignClickEvents();
    }

    void InitializePieces()
    {
        pieces.Clear();
        foreach (Transform child in gridLayout.transform)
        {
            pieces.Add(child);
            if (child.CompareTag("Empty"))
            {
                emptyPiece = child;
            }
        }
    }

    public void ShufflePieces()
    {
        do
        {
            for (int i = 0; i < pieces.Count - 1; i++)
            {
                int randomIndex = Random.Range(0, pieces.Count - 1);
                SwapPieces(i, randomIndex);
            }
        } while (!IsSolvable(pieces)); // Lặp lại nếu không giải được
    }

    void AssignClickEvents()
    {
        foreach (Transform piece in pieces)
        {
            Button button = piece.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => TryMovePiece(piece));
            }
        }
    }

    void TryMovePiece(Transform clickedPiece)
    {
        int clickedIndex = pieces.IndexOf(clickedPiece);
        int emptyIndex = pieces.IndexOf(emptyPiece);

        if (IsAdjacent(clickedIndex, emptyIndex))
        {
            SwapPieces(clickedIndex, emptyIndex);
            CheckWinCondition(); 
        }
    }

    bool IsAdjacent(int indexA, int indexB)
    {
        int gridSize = 3; 
        int rowA = indexA / gridSize, colA = indexA % gridSize;
        int rowB = indexB / gridSize, colB = indexB % gridSize;

        return (Mathf.Abs(rowA - rowB) == 1 && colA == colB) ||  // Cùng cột, khác hàng
               (Mathf.Abs(colA - colB) == 1 && rowA == rowB);   // Cùng hàng, khác cột
    }

    void SwapPieces(int indexA, int indexB)
    {
        Transform temp = pieces[indexA];
        pieces[indexA] = pieces[indexB];
        pieces[indexB] = temp;

        // Cập nhật thứ tự trong Grid
        pieces[indexA].SetSiblingIndex(indexA);
        pieces[indexB].SetSiblingIndex(indexB);
    }

    void CheckWinCondition(){
        for (int i = 0; i < pieces.Count; i++)
        {
            string expectedName = "hcmut_" + i; 
            if (!pieces[i].name.StartsWith(expectedName))
            {
                return;
            }
        }

        Debug.Log("🎉 Chúc mừng! Bạn đã hoàn thành trò chơi!");

        Image img = emptyPiece.GetComponent<Image>();

        img.sprite = finalImage;
        img.color = new Color(1, 1, 1, 1); 
    }

    bool IsSolvable(List<Transform> pieces)
{
    List<int> numbers = new List<int>();

    foreach (Transform piece in pieces)
    {
        string name = piece.name; 
        if (name.StartsWith("hcmut_"))
        {
            int num = int.Parse(name.Split('_')[1]);
            numbers.Add(num);
        }
    }

    int inversionCount = 0;
    for (int i = 0; i < numbers.Count - 1; i++)
    {
        for (int j = i + 1; j < numbers.Count; j++)
        {
            if (numbers[i] > numbers[j] && numbers[i] != 8 && numbers[j] != 8)
            {
                inversionCount++;
            }
        }
    }

    return inversionCount % 2 == 0; // Chỉ giải được nếu chẵn
}
}


