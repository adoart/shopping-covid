using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBarController : MonoBehaviour
{
    public int lifeTotal;
    public int lifeNumber;

    [SerializeField] private Sprite heartSpriteFull;
    [SerializeField] private Sprite heartSpriteEmpty;

    private List<HeartImage> heartImageList;

    private void Awake() {
        heartImageList = new List<HeartImage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateHeartImage(new Vector2(10, 0));
        CreateHeartImage(new Vector2(60, 0));
        CreateHeartImage(new Vector2(110, 0));
    }

    public void DecreaseLife() {
        heartImageList[--lifeNumber].SetEmptyHeart();
    }
    
    private HeartImage CreateHeartImage(Vector2 anchoredPosition) {
        GameObject heartGameObject = new GameObject("Heart", typeof(Image));
        // Set as child of this transform
        heartGameObject.transform.SetParent(transform, false);
        // Locate and Size heart
        heartGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        heartGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

        // Set heart sprite
        Image heartImageUI = heartGameObject.GetComponent<Image>();
        heartImageUI.sprite = heartSpriteFull;

        HeartImage heartImage = new HeartImage(this, heartImageUI);
        heartImageList.Add(heartImage);

        return heartImage;
    }

    public class HeartImage
    {
        private LifeBarController lifeBarController;
        private Image heartImage;

        public HeartImage (LifeBarController lifeBarController, Image heartImage) {
            this.lifeBarController = lifeBarController;
            this.heartImage = heartImage;
        }

        public void SetEmptyHeart() {
            this.heartImage.sprite = lifeBarController.heartSpriteEmpty;
        }
    } 
}
