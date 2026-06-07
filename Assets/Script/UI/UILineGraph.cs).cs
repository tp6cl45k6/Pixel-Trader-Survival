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

    [Header("圖表設定")]
    public float lineThickness = 3f;
    public int yAxisGridCount = 4; // 切成 4 等分 (平盤、一半、漲跌停)
    public int maxVisiblePoints = 60; // 新增：鎖定 X 軸最大顯示 60 筆 (60秒)

    private float yMaximum;
    private float yMinimum;
    private List<GameObject> activeUIElements = new List<GameObject>();

    // 新增 basePrice (昨收平盤價)，用來計算 10% 漲跌停
    public void ShowGraph(List<float> valueList, float basePrice)
    {
        ClearGraph();

        // 核心邏輯：台灣股市上下 10% 作為 Y 軸的極限
        yMaximum = basePrice * 1.1f; 
        yMinimum = basePrice * 0.9f;

        DrawGrid(basePrice);

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = graphWidth / (maxVisiblePoints > 1 ? maxVisiblePoints - 1 : 1);

        Vector2 lastCirclePosition = Vector2.zero;

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
    }

    private void DrawGrid(float basePrice)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        for (int i = 0; i <= yAxisGridCount; i++)
        {
            float normalizedY = (float)i / yAxisGridCount;
            float yPosition = normalizedY * graphHeight;

            // --- 強制對齊背景格線 ---
            if (gridLineTemplate != null)
            {
                RectTransform gridLine = Instantiate(gridLineTemplate, graphContainer);
                gridLine.gameObject.SetActive(true);
                // 強制鎖死：左下角錨點，中心在左邊緣
                gridLine.anchorMin = new Vector2(0, 0);
                gridLine.anchorMax = new Vector2(0, 0);
                gridLine.pivot = new Vector2(0, 0.5f);
                
                gridLine.anchoredPosition = new Vector2(0, yPosition);
                gridLine.sizeDelta = new Vector2(graphWidth, 2f);
                activeUIElements.Add(gridLine.gameObject);
            }

            // --- 強制對齊價格數字標籤 ---
            if (labelTemplate != null)
            {
                Text label = Instantiate(labelTemplate, graphContainer);
                label.gameObject.SetActive(true);
                
                // 強制鎖死：左下角錨點，中心在右邊緣 (這樣數字會往左長)
                label.rectTransform.anchorMin = new Vector2(0, 0);
                label.rectTransform.anchorMax = new Vector2(0, 0);
                label.rectTransform.pivot = new Vector2(1, 0.5f);
                
                // 往左退 10 像素，避免撞到線
                label.rectTransform.anchoredPosition = new Vector2(-10f, yPosition); 
                
                float priceValue = yMinimum + (normalizedY * (yMaximum - yMinimum));
                
                // 顯示小數點第一位，方便看清楚微小跳動
                label.text = priceValue.ToString("0.0"); 
                
                // 如果是平盤價，可以用顏色標記 (選用)
                if (Mathf.Approximately(priceValue, basePrice))
                {
                    label.color = Color.yellow; // 平盤價顯示黃色
                }

                activeUIElements.Add(label.gameObject);
            }
        }
    }

    // CreateCircle 和 CreateDotConnection 保持原樣...
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