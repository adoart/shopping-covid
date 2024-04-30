using MarksAssets.FullscreenWebGL;

using UnityEngine;

using status = MarksAssets.FullscreenWebGL.FullscreenWebGL.status;
using navigationUI = MarksAssets.FullscreenWebGL.FullscreenWebGL.navigationUI;

public class FullScreenWebGLExample : MonoBehaviour {
    public GameObject enterFullscreenBtn;
    public GameObject exitFullscreenBtn;
    public GameObject fullscreenNotSupportedText;
    
    void Start() {
        if( FullscreenWebGL.isFullscreenSupported() ) {
            //if fullscreen is supported
            FullscreenWebGL
                .subscribeToFullscreenchangedEvent(); //I'm interested in listening to fullscreen changes, so I subscribe to the event.
            FullscreenWebGL.onfullscreenchange += () => {
                //and then I add a callback that will run once the user enters or exits fullscreen
                if( FullscreenWebGL.isFullscreen() ) {
                    //if it's fullscreen
                    enterFullscreenBtn.SetActive( false ); //deactivate fullscreen button
                    exitFullscreenBtn.SetActive( true ); //activate exitfullscreen button
                } else {
                    //otherwise do the opposite
                    enterFullscreenBtn.SetActive( true );
                    exitFullscreenBtn.SetActive( false );
                }
            };
        } else {
            //if fullscreen is not supported don't show the button to enter fullscreen. This will happen on Safari.
            enterFullscreenBtn.SetActive( false );
            fullscreenNotSupportedText.SetActive( true ); //just show a message saying fullscreen is not supported.
        }
    }
    
    //call this on a pointerdown event
    public void enterfullscreen() {
        FullscreenWebGL.requestFullscreen( stat => {
                if( stat == status.Success ) {
                    enterFullscreenBtn.SetActive( false );
                    exitFullscreenBtn.SetActive( true );
                }
            },
            navigationUI
                .hide ); //setting navigationUI.hide here is redundant because it's the default value, but I'm doing it for completion. This is an example after all.
    }
    
    public void exitfullscreen() {
        FullscreenWebGL.exitFullscreen( stat => {
            if( stat == status.Success ) {
                enterFullscreenBtn.SetActive( true );
                exitFullscreenBtn.SetActive( false );
            }
        } );
    }
}