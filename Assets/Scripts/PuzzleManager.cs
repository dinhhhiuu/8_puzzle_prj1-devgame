using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PuzzleManager : MonoBehaviour
{
    public GridLayoutGroup gridLayout; 
    private List<Transform> pieces = new List<Transform>();
    private Transform emptyPiece; 
    public Sprite finalImage;
    
    [SerializeField] private GameObject gameWinUI;
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject grid3x3;
    [SerializeField] private GameObject grid4x4;
    [SerializeField] private GameObject line3x3;
    [SerializeField] private GameObject line4x4;
    private GameObject activeGrid;

    void Start()
    {
        grid3x3.SetActive(true);
        line3x3.SetActive(true);
        grid4x4.SetActive(false);
        line4x4.SetActive(false);
        
        activeGrid = grid3x3;
        InitializePieces();
        AssignClickEvents();
        gameWinUI.SetActive(false);
        SettingPanel.SetActive(false);
    }

    public void InitializePieces()
    {
        pieces.Clear();
        foreach (Transform child in activeGrid.transform)
        {
            pieces.Add(child);
            if (child.CompareTag("Empty"))
            {
                emptyPiece = child;
                Image img = emptyPiece.GetComponent<Image>();
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0);
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
                //button.onClick.AddListener(() => TryMovePiece(piece));
                button.onClick.RemoveAllListeners();  // Xóa tất cả listener trước khi thêm mới
                button.onClick.AddListener(delegate { TryMovePiece(piece); });
            }
        }
    }

    void TryMovePiece(Transform clickedPiece)
    {
        if (clickedPiece == emptyPiece) return;

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
        int gridSize = activeGrid == grid3x3 ? 3 : 4; 
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
        GameWin();

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
    public void GameWin()
    {
        Time.timeScale = 0;
        gameWinUI.SetActive(true);
        Debug.Log("✅ gameWinUI đã được kích hoạt!");
    }

    public void RestartGame(bool again)
    {
        Time.timeScale = 1;
        gameWinUI.SetActive(false);
        InitializePieces();  // Load lại danh sách pieces từ activeGrid
        AssignClickEvents(); // Gán lại sự kiện click        
        if(again)
        {
            SceneManager.LoadScene("SampleScene");
        }
        //SceneManager.LoadScene("SampleScene");
    }
    public void onClickSetting()
    {
        //SceneManager.LoadScene("Setting");
        SettingPanel.SetActive(true);
    }
    public void SetGridSize(int size)
{
    if (size == 3)
    {
        grid3x3.SetActive(true);
        line3x3.SetActive(true);
        grid4x4.SetActive(false);
        line4x4.SetActive(false);
        activeGrid = grid3x3;
    }
    else if (size == 4)
    {
        grid3x3.SetActive(false);
        line3x3.SetActive(false);
        grid4x4.SetActive(true);
        line4x4.SetActive(true);
        activeGrid = grid4x4; // Cập nhật activeGrid
    }

    InitializePieces();  // Load lại danh sách pieces
    AssignClickEvents(); // Gán lại sự kiện click cho từng ô
    emptyPiece = pieces.Find(piece => piece.CompareTag("Empty"));

}

    public void onClick3x3()
    {
        SetGridSize(3);
        SettingPanel.SetActive(false);
    }
    public void onClick4x4()
    {
        SetGridSize(4);
        SettingPanel.SetActive(false);
    }
}


