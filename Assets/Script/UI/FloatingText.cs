using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 50f;     // 往上飄的速度
    public float fadeSpeed = 1.5f;    // 消失的速度
    public float destroyTime = 2f;    // 幾秒後自我銷毀

    private Text textComponent;
    private Color textColor;

    void Start()
    {
        textComponent = GetComponent<Text>();
        if (textComponent != null) textColor = textComponent.color;
        
        // 設定時間到自動砍掉物件，避免佔用記憶體
        Destroy(gameObject, destroyTime); 
    }

    void Update()
    {
        // 讓文字往上移動
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 讓文字漸漸透明
        if (textComponent != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textComponent.color = textColor;
        }
    }
}
