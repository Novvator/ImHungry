using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;
    public float transitionTime = 1f;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLevel(string level)
    {
        Time.timeScale = 1f; // Resume normal time scale
        if (level == "current")
        {   
            StartCoroutine(LoadLevelCoroutine(SceneManager.GetActiveScene().name));
        }
        else
        {
            StartCoroutine(LoadLevelCoroutine(level));
        }
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
