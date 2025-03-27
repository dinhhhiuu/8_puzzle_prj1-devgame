using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // Hiển thị thời gian
    private float elapsedTime = 0f;
    private bool isRunning = false; // Biến kiểm soát

    void Update()
    {
        if (isRunning) // Chỉ cập nhật khi đang chạy
        {
            elapsedTime += Time.deltaTime;
            int min = Mathf.FloorToInt(elapsedTime / 60);
            int sec = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", min, sec);
        }
    }

    // Hàm để bắt đầu đếm thời gian
    public void StartTimer()
    {
        isRunning = true;
        elapsedTime = 0f; // Reset thời gian nếu cần
    }
    
}
