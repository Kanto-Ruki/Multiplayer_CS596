using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class PlayerControl : NetworkBehaviour {
    
    private float move;
    private float moveSpeed;
    private float rotate;
    private float rotateSpeed;
    static string s_ObjectPoolTag = "ObjectPool";
    NetworkObjectPool m_ObjectPool;
    Rigidbody2D m_Rigidbody2D;

    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_ObjectPool = GameObject.FindWithTag(s_ObjectPoolTag).GetComponent<NetworkObjectPool>();
        Assert.IsNotNull(m_ObjectPool, $"{nameof(NetworkObjectPool)} not found in scene. Did you apply the {s_ObjectPoolTag} to the GameObject?");
        
    }

    // Start is called before the first frame update
    void Start() {

        DontDestroyOnLoad(gameObject);
        //SetPlayerUIVisibility(true);
    }

    // Update is called once per frame
    void Update() {
        if (IsServer)
        {
            UpdateServer();
        }

        if (IsClient)
        {
            UpdateClient();
        }
    }

    void UpdateServer() {
        
    }

    void UpdateClient() {
        if (!IsLocalPlayer) {
            return;
        }

        // movement
        move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        rotate = Input.GetAxis("Horizontal") * (-rotateSpeed) * Time.deltaTime;
    }
}
