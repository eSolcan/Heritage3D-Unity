using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneWeb : MonoBehaviour
{

    void Start()
    {
        
    }

    
    public void ChangeScene(int scene)
    {
        //Load scene
        SceneManager.LoadScene(scene);
    }

}
