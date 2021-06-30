using UnityEngine;

public class Flicker : MonoBehaviour {
    public string LightStyle = "mmamammmmammamamaaamammma";
    private Light light;

    public float loopTime = 2f;
    [SerializeField]
    private int currentIndex = 0;
    private float lightTimer;

    void Start() {
        light = GetComponent<Light>();
    }

    void Update() {
        char c = GetNextChar();
        int val = c - 'a';
        float intensity = (val / 25f) * 2;
        light.intensity = intensity;
    }


    private char GetNextChar() {
        lightTimer += Time.deltaTime;
        var step = loopTime / LightStyle.Length;

        if (step < lightTimer) {
            lightTimer -= step;
            currentIndex++;
            if (currentIndex >= LightStyle.Length)
                currentIndex = 0;
        }

        return LightStyle[currentIndex];
    }
}