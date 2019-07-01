using System.Collections.Generic;
using UI.Parallax;
using UnityEngine;

namespace UI.Background.Parallax {
    
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour {
        
        [SerializeField] private ParallaxCamera parallaxCamera;
        [SerializeField] private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();
  
        void Start() {
            if (parallaxCamera == null)
                parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
            if (parallaxCamera != null)
                parallaxCamera.onCameraTranslate += Move;
        }

        void Move(float delta) {
            foreach (ParallaxLayer layer in parallaxLayers) {
                layer.Move(delta);
            }
        }
    }
}