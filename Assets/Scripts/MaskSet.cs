using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskSet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.renderQueue = 3002;
    }


}
