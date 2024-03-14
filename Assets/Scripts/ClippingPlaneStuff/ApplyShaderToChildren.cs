using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyShaderToChildren : MonoBehaviour
{

    public Shader shaderToAdd;
    private Transform thisTransform;

    void Start()
    {
        thisTransform = this.gameObject.transform;

        int nrChildren = thisTransform.childCount;
        // Apply specific shader to all children of current game object
        for (int i = 0; i < nrChildren; i++)
        {
            GameObject objToEdit = thisTransform.GetChild(i).gameObject;
            objToEdit.GetComponent<Renderer>().material.shader = shaderToAdd;
            objToEdit.AddComponent<ApplyClipping>();
        }
    }


}
