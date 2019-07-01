using System;
using UnityEngine;

namespace UI.Parallax {
    
    [ExecuteInEditMode]
    public class ParallaxCamera : MonoBehaviour {
        
        public delegate void ParallaxCameraDelegate(float deltaMovement);
        public ParallaxCameraDelegate onCameraTranslate;
        private float oldPosition;
        
        void Start() {
            oldPosition = transform.position.x;
        }
        
        void Update() {
            if (Math.Abs(transform.position.x - oldPosition) > 0.01f) {
                
                if (onCameraTranslate != null) {
                    float delta = oldPosition - transform.position.x;
                    onCameraTranslate(delta);
                }
                oldPosition = transform.position.x;
            }
        }
    }
}