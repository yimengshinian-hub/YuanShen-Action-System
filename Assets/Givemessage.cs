using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class Givemessage : MonoBehaviour
    {
        private SkinnedMeshRenderer SkinnedMeshRenderer;
        // Start is called before the first frame update
        void Start()
        {
            SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            Debug.Log(SkinnedMeshRenderer.bounds.size);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
