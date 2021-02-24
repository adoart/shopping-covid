﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItemPopup : MonoBehaviour
{
    private Camera cam;
    public GameObject item;
    public int viewPosOffset = 60;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (item != null) {
            Vector3 screenPos = cam.WorldToScreenPoint(item.transform.position);
            Vector3 viewPos = new Vector3(Mathf.Clamp(screenPos.x, 0.0f + viewPosOffset, Screen.width - viewPosOffset), Mathf.Clamp(screenPos.y, 0.0f + viewPosOffset, Screen.height - viewPosOffset), screenPos.z);
            if (!screenPos.Equals(viewPos)) {
                transform.position = viewPos;
            } else {
                //Move popup outside the screen
                transform.position = new Vector3(-10000, -10000);
            }
        }
    }
}
