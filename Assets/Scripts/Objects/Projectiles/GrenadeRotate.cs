using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeRotate : MonoBehaviour {

    [SerializeField] private float RotationSpeed;

    private Transform m_Transform;

    void Awake() {
        m_Transform = GetComponent<Transform>();
    }
    
    void Update() {
        m_Transform.Rotate(-Vector3.forward * RotationSpeed * Time.deltaTime, Space.Self);
    }
}
