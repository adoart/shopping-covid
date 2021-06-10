using UnityEngine;

public class MoveItemPopup : MonoBehaviour {
    private Camera cam;
    public Transform itemTransform;
    public int viewPosOffset = 60;
    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        if (itemTransform != null) {
            Vector3 screenPos = cam.WorldToScreenPoint(itemTransform.position);
            Vector3 viewPos = new Vector3(Mathf.Clamp(screenPos.x, 0.0f + viewPosOffset, Screen.width - viewPosOffset),
                Mathf.Clamp(screenPos.y, 0.0f + viewPosOffset, Screen.height - viewPosOffset), screenPos.z);
            if (!screenPos.Equals(viewPos)) {
                transform.position = viewPos;
            } else {
                //Move popup outside the screen
                transform.position = new Vector3(-10000, -10000);
            }
        }
    }
}