using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineGraph : MonoBehaviour
{
    [Header("UI 參考")]
    public RectTransform graphContainer;
    public RectTransform circleTemplate;
    public RectTransform lineTemplate;

    [Header("座標軸與格線設定")]
    public RectTransform gridLineTemplate;
    public Text labelTemplate;
    public RectTransform costLineTemplate;

    [Header("功能性模板 (新增)")]
    public Text currentPriceLabelTemplate; // 新增：顯示當前現價的文字模板 (若用 TextMeshPro，請改型別為 TMP_Text)

    [Header("圖表設定")]
    public float lineThickness = 3f;
    public int yAxisGridCount = 4;
    public int maxVisiblePoints = 60;

    private float yMaximum;
    private float yMinimum;
    private List<GameObject> activeUIElements = new List<GameObject>();

    public void ShowGraph(List<float> valueList, float basePrice, float averageCost = 0f)
    {
        ClearGraph();

        // 核心邏輯：台股上下 10% 為 Y 軸極限
        yMaximum = basePrice * 1.1f; 
        yMinimum = basePrice * 0.9f;

        DrawGrid(basePrice);

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = graphWidth / (maxVisiblePoints > 1 ? maxVisiblePoints - 1 : 1);

        Vector2 lastCirclePosition = Vector2.zero;

        // 取得當前最新價格 (清單最後一筆)
        float currentPrice = valueList[valueList.Count - 1];

        // --- 畫出持股成本線 (維持原樣) ---
        if (averageCost > 0 && costLineTemplate != null)
        {
            float yNormalized = (averageCost - yMinimum) / (yMaximum - yMinimum);
            float yPosition = yNormalized * graphHeight;

            RectTransform costLine = Instantiate(costLineTemplate, graphContainer);
            costLine.gameObject.SetActive(true);
            
            // 強制復位 X 軸設定，避免歪掉
            costLine.anchorMin = new Vector2(0, 0);
            costLine.anchorMax = new Vector2(0, 0);
            costLine.pivot = new Vector2(0, 0.5f);
            
            costLine.anchoredPosition = new Vector2(0, yPosition); // 程式碼強制 X 為 0
            costLine.sizeDelta = new Vector2(graphWidth, 4f); 

            Image lineImage = costLine.GetComponent<Image>();
            if (lineImage != null)
            {
                lineImage.color = currentPrice >= averageCost ? new Color(1f, 0.3f, 0.3f) : new Color(0.3f, 1f, 0.3f);
            }
            activeUIElements.Add(costLine.gameObject);
        }

        // --- 畫資料點與連線 (維持原樣) ---
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * xSize;
            float yNormalized = (valueList[i] - yMinimum) / (yMaximum - yMinimum);
            float yPosition = yNormalized * graphHeight;

            Vector2 circlePosition = new Vector2(xPosition, yPosition);

            CreateCircle(circlePosition);
            if (i > 0)
            {
                CreateDotConnection(lastCirclePosition, circlePosition);
            }
            lastCirclePosition = circlePosition;
        }

        // --- 畫出「當前現價標籤」 (新增功能) ---
        if (currentPriceLabelTemplate != null)
        {
            // 計算最後一筆價格在 UI 容器的高度位置
            float yNormalized = (currentPrice - yMinimum) / (yMaximum - yMinimum);
            float yPosition = yNormalized * graphHeight;
            // 計算最右端點的 X 軸位置
            float xPosition = (valueList.Count - 1) * xSize;

            Text label = Instantiate(currentPriceLabelTemplate, graphContainer);
            label.gameObject.SetActive(true);

            // 強制鎖死錨點在左下角，Pivot 在左邊，方便定位
            label.rectTransform.anchorMin = new Vector2(0, 0);
            label.rectTransform.anchorMax = new Vector2(0, 0);
            label.rectTransform.pivot = new Vector2(0, 0.5f);

            // 將標籤移動到最後一個點的位置，並往右推 20 像素以免撞到
            label.rectTransform.anchoredPosition = new Vector2(xPosition + 20f, yPosition);

            // 更新文字內容為當前價格 (顯示小數點第一位)
            label.text = currentPrice.ToString("0.0");
            
            // 統一顏色格式 (台股紅賺綠賠)
            label.color = currentPrice >= basePrice ? new Color(1f, 0.3f, 0.3f) : new Color(0.3f, 1f, 0.3f);
            
            activeUIElements.Add(label.gameObject);
        }
    }

    // DrawGrid, CreateCircle 和 CreateDotConnection 保持原樣...
    // 但在 DrawGrid 裡更新 label.text 顯示格式為ToString("0.0")，並將顏色顏色
    private void DrawGrid(float basePrice)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        for (int i = 0; i <= yAxisGridCount; i++)
        {
            float normalizedY = (float)i / yAxisGridCount;
            float yPosition = normalizedY * graphHeight;

            if (gridLineTemplate != null)
            {
                RectTransform gridLine = Instantiate(gridLineTemplate, graphContainer);
                gridLine.gameObject.SetActive(true);
                gridLine.anchorMin = new Vector2(0, 0);
                gridLine.anchorMax = new Vector2(0, 0);
                gridLine.pivot = new Vector2(0, 0.5f);
                
                gridLine.anchoredPosition = new Vector2(0, yPosition);
                gridLine.sizeDelta = new Vector2(graphWidth, 2f);
                activeUIElements.Add(gridLine.gameObject);
            }

            if (labelTemplate != null)
            {
                Text label = Instantiate(labelTemplate, graphContainer);
                label.gameObject.SetActive(true);
                label.rectTransform.anchorMin = new Vector2(0, 0);
                label.rectTransform.anchorMax = new Vector2(0, 0);
                label.rectTransform.pivot = new Vector2(1, 0.5f);
                
                label.rectTransform.anchoredPosition = new Vector2(-10f, yPosition); 
                
                float priceValue = yMinimum + (normalizedY * (yMaximum - yMinimum));
                
                // 統一格式：顯示小數點第一位
                label.text = priceValue.ToString("0.0"); 
                // 統一顏色：白色 (與背景格線區分)
                label.color = Color.white;

                activeUIElements.Add(label.gameObject);
            }
        }
    }

    private void CreateCircle(Vector2 anchoredPosition)
    {
        RectTransform circle = Instantiate(circleTemplate, graphContainer);
        circle.gameObject.SetActive(true);
        circle.anchoredPosition = anchoredPosition;
        activeUIElements.Add(circle.gameObject);
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        RectTransform line = Instantiate(lineTemplate, graphContainer);
        line.gameObject.SetActive(true);

        Vector2 direction = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);

        line.anchorMin = new Vector2(0, 0);
        line.anchorMax = new Vector2(0, 0);
        line.pivot = new Vector2(0, 0.5f);
        line.anchoredPosition = dotPositionA;
        line.sizeDelta = new Vector2(distance, lineThickness);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        line.localEulerAngles = new Vector3(0, 0, angle);
        
        activeUIElements.Add(line.gameObject);
    }

    private void ClearGraph()
    {
        foreach (GameObject element in activeUIElements)
        {
            Destroy(element);
        }
        activeUIElements.Clear();
    }
}