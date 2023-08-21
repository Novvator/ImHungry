using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;
    public float transitionTime = 1f;


    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLevel(string level)
    {
        StartCoroutine(LoadLevelCoroutine(level));
    }

    IEnumerator LoadLevelCoroutine(string level)
    {
        //Play Animation
        animator.SetTrigger("Start");
        //Wait
        yield return new WaitForSeconds(transitionTime);
        //Load scene
        SceneManager.LoadScene(level);

    }

    

    
}
