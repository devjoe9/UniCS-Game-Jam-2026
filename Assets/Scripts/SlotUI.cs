using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Main visuals")]
    public Image frameImage;
    public Image frameTintTarget;
    public Image itemImage;
    public Image focusHighlight;

    [Header("Text")]
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI label;
    public TextMeshProUGUI positionLabel;

    [Header("Dots (optional)")]
    public Image[] dots = new Image[4];

    [Header("Colors")]
    public Color activeColor = Color.cyan;
    public Color inactiveColor = new Color(0f, 1f, 1f, 0.2f);
}