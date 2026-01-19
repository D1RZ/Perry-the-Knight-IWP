using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TMPHoverColorLerp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI tmpText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    [Header("Lerp")]
    [SerializeField] private float lerpSpeed = 10f;

    private Color targetColor;

    private void Reset()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();

        tmpText.color = normalColor;
        targetColor = normalColor;
    }

    private void Update()
    {
        // Unscaled so it still works if you pause / slow time
        tmpText.color = Color.Lerp(
            tmpText.color,
            targetColor,
            Time.unscaledDeltaTime * lerpSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetColor = normalColor;
    }
}