using UnityEngine;
using UnityEngine.Serialization;

public enum ItemTypeDefinitions {
    ITEM,
    POWERUP,
    EMPTY
};

public enum ItemSubType {
    None,
    Item,
    Mask,
    Trolley
};

[CreateAssetMenu(fileName = "NewItem", menuName = "Spawnable Item/New Pick-up", order = 1)]
public class ItemPickUps_SO : ScriptableObject {
    public string itemName = "New Item";
    public string itemTag = ItemTypeDefinitions.EMPTY.ToString();
    public ItemTypeDefinitions itemType = ItemTypeDefinitions.EMPTY;
    [FormerlySerializedAs("itemPowerupSubType")]
    public ItemSubType itemSubType = ItemSubType.None;
    public int itemAmount = 0;
    public int spawnChanceWeight = 0;

    public Material itemMaterial = null;
    public Sprite itemIcon = null;
    public GameObject itemSpawnObject = null;
    public GameObject itemCameraObject = null;

    public bool isEquipped = false;
    public bool isInteractable = false;
    public bool isStorable = false;
    public bool isQuestItem = false;
    public bool isStackable = false;
    public bool destroyOnUse = false;
}