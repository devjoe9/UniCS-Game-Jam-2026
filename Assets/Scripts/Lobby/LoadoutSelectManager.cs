using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clean loadout selector:
/// - 4 slots: TOP / RIGHT / BOTTOM / LEFT
/// - each slot cycles through a list of items
/// - optional center display / info panel / preview pips
/// </summary>
public class LoadoutSelectManager : MonoBehaviour
{
    [Header("Slot UI roots (TOP / RIGHT / BOTTOM / LEFT)")]
    public SlotUI[] slots = new SlotUI[4];

    [Header("Items for each slot")]
    public SlotItemData[] topItems;
    public SlotItemData[] rightItems;
    public SlotItemData[] bottomItems;
    public SlotItemData[] leftItems;

    [Header("Center Player (optional)")]
    public Image centerPlayerFrame;
    public Sprite centerPlayerImage;
    public TextMeshProUGUI centerPlayerLabel;

    [Header("Preview Pips Around Center (optional)")]
    public Image[] previewPips = new Image[4];

    [Header("Info Panel (optional)")]
    public Image infoPanelFrame;
    public TextMeshProUGUI infoTitleText;
    public TextMeshProUGUI infoDescriptionText;

    [Header("Main UI (optional)")]
    public Button startButton;
    public TextMeshProUGUI titleText;

    [Header("Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip cycleClip;
    public AudioClip confirmClip;

    private readonly string[] sideNames = { "TOP", "RIGHT", "BOTTOM", "LEFT" };
    private SlotItemData[][] allItems;
    private int[] currentIndex = new int[4];

    private const int SLOT_TOP = 0;
    private const int SLOT_RIGHT = 1;
    private const int SLOT_BOTTOM = 2;
    private const int SLOT_LEFT = 3;

    private int focusedSlot = 0;

    private void Awake()
    {
        BuildSlotArrays();
        EnsureDefaultDataIfEmpty();
    }

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SetupSlot(i);
            RefreshSlot(i, false);
        }

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        RefreshCenterDisplay();
        RefreshFocusVisuals();
        RefreshInfoPanel(focusedSlot);
    }

    private void Update()
    {
        // Remove this whole Update() if you do not want keyboard support.
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            focusedSlot = (focusedSlot - 1 + 4) % 4;
            RefreshFocusVisuals();
            RefreshInfoPanel(focusedSlot);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            focusedSlot = (focusedSlot + 1) % 4;
            RefreshFocusVisuals();
            RefreshInfoPanel(focusedSlot);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CycleSlot(focusedSlot, -1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            CycleSlot(focusedSlot, 1);

        if (Input.GetKeyDown(KeyCode.Return))
            OnStartClicked();
    }

    private void BuildSlotArrays()
    {
        allItems = new SlotItemData[4][];
        allItems[SLOT_TOP] = topItems;
        allItems[SLOT_RIGHT] = rightItems;
        allItems[SLOT_BOTTOM] = bottomItems;
        allItems[SLOT_LEFT] = leftItems;
    }

    private void EnsureDefaultDataIfEmpty()
    {
        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i] == null || allItems[i].Length == 0)
            {
                allItems[i] = new SlotItemData[4];

                for (int j = 0; j < 4; j++)
                {
                    allItems[i][j] = new SlotItemData
                    {
                        displayName = $"Weapon {j + 1}",
                        description = $"Description for Weapon {j + 1}",
                        sprite = null,
                        previewSprite = null,
                        slotFrameSprite = null,
                        pipColor = Color.cyan
                    };
                }
            }
        }

        // Write back so inspector/runtime arrays stay synced
        topItems = allItems[SLOT_TOP];
        rightItems = allItems[SLOT_RIGHT];
        bottomItems = allItems[SLOT_BOTTOM];
        leftItems = allItems[SLOT_LEFT];
    }

    private void SetupSlot(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex)) return;
        if (slots[slotIndex] == null) return;

        SlotUI ui = slots[slotIndex];

        if (ui.label != null)
            ui.label.text = sideNames[slotIndex];

        if (ui.leftButton != null)
        {
            int captured = slotIndex;
            ui.leftButton.onClick.RemoveAllListeners();
            ui.leftButton.onClick.AddListener(() => CycleSlot(captured, -1));
        }

        if (ui.rightButton != null)
        {
            int captured = slotIndex;
            ui.rightButton.onClick.RemoveAllListeners();
            ui.rightButton.onClick.AddListener(() => CycleSlot(captured, 1));
        }

        currentIndex[slotIndex] = Mathf.Clamp(currentIndex[slotIndex], 0, Mathf.Max(0, GetItemCount(slotIndex) - 1));
    }

    public void CycleSlot(int slotIndex, int direction)
    {
        Debug.Log($"CycleSlot called | slot={slotIndex} | direction={direction}");
        
        if (!IsValidSlotIndex(slotIndex)) return;
        if (GetItemCount(slotIndex) == 0) return;

        int count = GetItemCount(slotIndex);
        currentIndex[slotIndex] = (currentIndex[slotIndex] + direction + count) % count;
       
        Debug.Log($"New index for slot {slotIndex}: {currentIndex[slotIndex]}");
        
        RefreshSlot(slotIndex, true);
        RefreshInfoPanel(slotIndex);
        RefreshFocusVisuals();
        PlaySound(cycleClip);
    }

    private void RefreshSlot(int slotIndex, bool animate)
    {
        if (!IsValidSlotIndex(slotIndex)) return;
        if (slots[slotIndex] == null) return;
        if (GetItemCount(slotIndex) == 0) return;

        SlotUI ui = slots[slotIndex];
        SlotItemData item = GetCurrentItem(slotIndex);
        int idx = currentIndex[slotIndex];
        int count = GetItemCount(slotIndex);

        if (ui.itemImage != null)
    {
        ui.itemImage.enabled = true;
        ui.itemImage.sprite = item.sprite;
        ui.itemImage.preserveAspect = true;
        ui.itemImage.color = item.sprite != null
            ? Color.white
            : new Color(1f, 1f, 1f, 0.15f);
    }

        if (ui.itemName != null)
            ui.itemName.text = item.displayName;

        if (ui.positionLabel != null)
            ui.positionLabel.text = $"{idx + 1} / {count}";

        if (ui.frameImage != null && item.slotFrameSprite != null)
            ui.frameImage.sprite = item.slotFrameSprite;

        if (ui.frameTintTarget != null)
            ui.frameTintTarget.color = item.pipColor;

        if (ui.dots != null)
        {
            for (int i = 0; i < ui.dots.Length; i++)
            {
                if (ui.dots[i] == null) continue;
                ui.dots[i].color = (i == idx) ? ui.activeColor : ui.inactiveColor;
            }
        }

        UpdatePreviewPip(slotIndex, item);

        if (animate && ui.itemImage != null)
        {
            ui.itemImage.rectTransform.localScale = Vector3.one * 1.08f;
            CancelInvoke(nameof(ResetItemScale));
            Invoke(nameof(ResetItemScale), 0.08f);
        }
    }

    private void ResetItemScale()
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot.itemImage != null)
                slot.itemImage.rectTransform.localScale = Vector3.one;
        }
    }

    private void RefreshCenterDisplay()
    {
        if (centerPlayerLabel != null)
            centerPlayerLabel.text = "PLAYER";

        if (centerPlayerFrame != null)
            centerPlayerFrame.color = Color.white;

        // if (centerPlayerImage != null)
        //     centerPlayerImage.color = Color.white;
    }

    private void RefreshFocusVisuals()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            if (GetItemCount(i) == 0) continue;

            bool focused = (i == focusedSlot);
            Color baseColor = GetCurrentItem(i).pipColor;

            if (slots[i].focusHighlight != null)
                slots[i].focusHighlight.enabled = focused;

            if (slots[i].frameTintTarget != null)
                slots[i].frameTintTarget.color = focused ? baseColor * 1.15f : baseColor;
        }
    }

    private void UpdatePreviewPip(int side, SlotItemData item)
    {
        if (previewPips == null || previewPips.Length <= side) return;
        if (previewPips[side] == null) return;

        previewPips[side].enabled = true;
        previewPips[side].sprite = item.previewSprite != null ? item.previewSprite : item.sprite;
        previewPips[side].color = item.pipColor;
        previewPips[side].preserveAspect = true;
    }

    private void RefreshInfoPanel(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex)) return;
        if (GetItemCount(slotIndex) == 0) return;

        SlotItemData item = GetCurrentItem(slotIndex);

        if (infoTitleText != null)
            infoTitleText.text = item.displayName;

        if (infoDescriptionText != null)
            infoDescriptionText.text = string.IsNullOrWhiteSpace(item.description)
                ? "Weapon info goes here."
                : item.description;

        if (infoPanelFrame != null)
            infoPanelFrame.color = item.pipColor;
    }

    private void OnStartClicked()
    {
        LoadoutData result = new LoadoutData();

        for (int i = 0; i < 4; i++)
        {
            SlotItemData item = GetCurrentItem(i);
            result.selections[i] = new LoadoutData.Selection
            {
                side = (LoadoutData.Side)i,
                itemIndex = currentIndex[i],
                itemName = item != null ? item.displayName : "None",
                itemData = item
            };
        }

        LoadoutData.Selected = result;
        PlaySound(confirmClip);

        Debug.Log("[LoadoutSelect] Starting game with:\n" + result);
    }

    private SlotItemData GetCurrentItem(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex)) return null;
        if (allItems[slotIndex] == null || allItems[slotIndex].Length == 0) return null;

        int idx = Mathf.Clamp(currentIndex[slotIndex], 0, allItems[slotIndex].Length - 1);
        return allItems[slotIndex][idx];
    }

    private int GetItemCount(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex)) return 0;
        return allItems[slotIndex] == null ? 0 : allItems[slotIndex].Length;
    }

    private bool IsValidSlotIndex(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < 4;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}

[Serializable]
public class SlotItemData
{
    public string displayName;
    [TextArea(2, 4)] public string description;
    public Sprite sprite;
    public Sprite previewSprite;
    public Sprite slotFrameSprite;
    public Color pipColor = Color.cyan;
    public GameObject prefab;
    public bool isBlue;
}

public class LoadoutData
{
    public enum Side { Top = 0, Right = 1, Bottom = 2, Left = 3 }

    [Serializable]
    public class Selection
    {
        public Side side;
        public int itemIndex;
        public string itemName;
        public SlotItemData itemData;
    }

    public Selection[] selections = new Selection[4];
    public static LoadoutData Selected { get; set; }

    public override string ToString()
    {
        if (selections == null) return "null";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var s in selections)
            sb.AppendLine($"  {s.side,6}: [{s.itemIndex}] {s.itemName}");

        return sb.ToString();
    }
}