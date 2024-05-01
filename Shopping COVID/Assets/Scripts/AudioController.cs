using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour {
    private AudioSource audioSource;
    [SerializeField] private Toggle audioToggle;
    
    // Start is called before the first frame update
    void Start() {
        //get singleton gameObject called "BackgroundMusic" and set it as audio source
        audioSource = GameObject.Find( "BackgroundMusic" ).GetComponent<AudioSource>();
        audioToggle = GameObject.Find( "AudioToggle" ).GetComponent<Toggle>();
        audioToggle.isOn = !audioSource.mute;
        
        audioToggle.onValueChanged.AddListener( ( value ) => {
            audioSource.mute = !value;
        } );
        
    }
}