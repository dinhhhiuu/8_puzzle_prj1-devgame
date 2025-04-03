using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public GridLayoutGroup gridLayout_3x3;
    public GridLayoutGroup gridLayout_4x4; 
    public GameObject temp;
    private List<Transform> pieces = new List<Transform>();
    private Transform emptyPiece; 
    public Sprite finalImage;
    
    [SerializeField] private GameObject gameWinUI;
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject grid3x3;
    [SerializeField] private GameObject grid4x4;
    [SerializeField] private GameObject line3x3;
    [SerializeField] private GameObject line4x4;
    [SerializeField] private GameObject LockGame;

    private GameObject activeGrid;
    private GameObject activeLine;

    void Start() {
        LockGame.SetActive(true);
        grid3x3.SetActive(true);
        line3x3.SetActive(true);
        grid4x4.SetActive(false);
        line4x4.SetActive(false);
        
        activeGrid = grid3x3;
        activeLine = line3x3;
        InitializePieces();
        AssignClickEvents();

        gameWinUI.SetActive(false);
        SettingPanel.SetActive(false);
    }

    public void StartGame() {
        LockGame.SetActive(false);
    }
    public void InitializePieces() {
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
        int gridSize = activeGrid == grid3x3 ? 3 : 4; 
        int totalTiles = gridSize * gridSize; 

        do {
            for (int i = 0; i < totalTiles - 1; i++)
            {
                int randomIndex = Random.Range(0, totalTiles - 1);
                SwapPieces(i, randomIndex);
            }
        } while (!IsSolvable(pieces, gridSize)); // Kiểm tra trạng thái có thể giải không
    }


    void AssignClickEvents() {
        foreach (Transform piece in pieces) {
            Button button = piece.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();  // Xóa tất cả listener trước khi thêm mới
                button.onClick.AddListener(delegate { TryMovePiece(piece); });
            }
        }
    }

    void TryMovePiece(Transform clickedPiece) {
        if (clickedPiece == emptyPiece) return;

        int clickedIndex = pieces.IndexOf(clickedPiece);
        int emptyIndex = pieces.IndexOf(emptyPiece);

        if (IsAdjacent(clickedIndex, emptyIndex))
        {
            SwapPieces(clickedIndex, emptyIndex);
            CheckWinCondition(); 
        }
    }

    bool IsAdjacent(int indexA, int indexB) {
        int gridSize = activeGrid == grid3x3 ? 3 : 4; 
        int rowA = indexA / gridSize, colA = indexA % gridSize;
        int rowB = indexB / gridSize, colB = indexB % gridSize;

        return (Mathf.Abs(rowA - rowB) == 1 && colA == colB) ||  // Cùng cột, khác hàng
               (Mathf.Abs(colA - colB) == 1 && rowA == rowB);   // Cùng hàng, khác cột
    }

    void SwapPieces(int indexA, int indexB) {
        Transform temp = pieces[indexA];
        pieces[indexA] = pieces[indexB];
        pieces[indexB] = temp;

        // Cập nhật thứ tự trong Grid
        pieces[indexA].SetSiblingIndex(indexA);
        pieces[indexB].SetSiblingIndex(indexB);
    }

    void CheckWinCondition(){
        for (int i = 0; i < pieces.Count; i++) {
            string expectedName = "hcmut_" + i; 
            if (!pieces[i].name.StartsWith(expectedName))
            {
                return;
            }
        }

        Debug.Log("🎉 Chúc mừng! Bạn đã hoàn thành trò chơi!");
        GameWin();
        if (activeGrid == grid3x3) {
            Image img = emptyPiece.GetComponent<Image>();

            img.sprite = finalImage;
            img.color = new Color(1, 1, 1, 1); 
        }
    }

    bool IsSolvable(List<Transform> pieces, int gridSize) {
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
        for (int i = 0; i < numbers.Count - 1; i++) {
            for (int j = i + 1; j < numbers.Count; j++)
            {
                if (numbers[i] > numbers[j] && numbers[i] != (gridSize * gridSize - 1) && numbers[j] != (gridSize * gridSize - 1))
                {
                    inversionCount++;
                }
            }
        }

        if (gridSize == 3) {
            return inversionCount % 2 == 0; // 3x3 phải chẵn
        }
        else {
            int emptyRow = pieces.IndexOf(emptyPiece) / gridSize; 
            return (inversionCount + emptyRow) % 2 == 0; // 4x4 phải kiểm tra thêm vị trí ô trống
        }
    }

    public void GameWin()
    {
        Time.timeScale = 0;
        Debug.Log("✅ gameWinUI đã được kích hoạt!");
        if (activeGrid == grid3x3)
        {
           StartCoroutine(DoSomethingEveryHalfSecond());
        }
        else if (activeGrid == grid4x4)
        {
            StartCoroutine(DoSomethingEveryHalfSecond());
        }
    }

    public void RestartGame(bool again) {
        Time.timeScale = 1;
        gameWinUI.SetActive(false);
        InitializePieces();  // Load lại danh sách pieces từ activeGrid
        AssignClickEvents(); // Gán lại sự kiện click        
        if(again)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
    public void onClickSetting() {
        SettingPanel.SetActive(true);
        LockGame.SetActive(false);
    }
    public void SetGridSize(int size) {
        if (size == 3) {
            grid3x3.SetActive(true);
            line3x3.SetActive(true);
            grid4x4.SetActive(false);
            line4x4.SetActive(false);
            activeGrid = grid3x3;
            activeLine = line3x3;
        }
        else if (size == 4) {
            grid3x3.SetActive(false);
            line3x3.SetActive(false);
            grid4x4.SetActive(true);
            line4x4.SetActive(true);
            activeGrid = grid4x4; // Cập nhật activeGrid
            activeLine = line4x4;
        }

        InitializePieces();  // Load lại danh sách pieces
        AssignClickEvents(); // Gán lại sự kiện click cho từng ô
        emptyPiece = pieces.Find(piece => piece.CompareTag("Empty"));

    }

    public void onClick3x3() {
        SetGridSize(3);
        SettingPanel.SetActive(false);
        LockGame.SetActive(true);
    }
    public void onClick4x4() {
        SetGridSize(4);
        SettingPanel.SetActive(false);
        LockGame.SetActive(true);
    }

    IEnumerator DoSomethingEveryHalfSecond() {
        float totalTime = 2f; // Chạy trong 2 giây
        float interval = 0.5f; // Mỗi 0.5 giây làm một lần

        for (float t = 0; t < totalTime; t += interval)
        {
            Debug.Log("Hành động diễn ra lúc: " + t + " giây");
            if (t == 0) {
                foreach (Transform child in activeLine.transform)
                {
                    child.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // Trắng hoàn toàn (không trong suốt)
                    gridLayout_3x3.spacing = new Vector2(4f, 4f);
                    gridLayout_4x4.spacing = new Vector2(4f, 4f);
                }
            }
            else if (t == 0.5f) {
                foreach (Transform child in activeLine.transform)
                {
                    child.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f); // Trắng nhạt 
                    gridLayout_3x3.spacing = new Vector2(3f, 3f);
                    gridLayout_4x4.spacing = new Vector2(3f, 3f);
                }
            }
            else if (t == 1) {
                foreach (Transform child in activeLine.transform)
                {
                    child.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); // Trắng mờ 
                    gridLayout_3x3.spacing = new Vector2(1.5f, 1.5f);
                    gridLayout_4x4.spacing = new Vector2(1.5f, 1.5f);
                }
            }
            else if (t == 1.5f) {
                foreach (Transform child in activeLine.transform)
                {
                    child.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Gần như trong suốt 
                    gridLayout_3x3.spacing = new Vector2(0f, 0f);
                    gridLayout_4x4.spacing = new Vector2(0f, 0f);
                }
            }
            yield return new WaitForSecondsRealtime(interval);
        }

        Debug.Log("Kết thúc sau 2 giây!");
        

        yield return new WaitForSecondsRealtime(1f); 
        activeGrid.SetActive(false);
        activeLine.SetActive(false);
        temp.SetActive(false);
        gameWinUI.SetActive(true);
    }
}


