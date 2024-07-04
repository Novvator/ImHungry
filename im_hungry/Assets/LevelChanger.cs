using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeToLevel ()
    {
<<<<<<< Updated upstream
        animator.SetTrigger("FadeOut");
=======
        Time.timeScale = 1f; // Resume normal time scale
        if (level == "current")
        {   
            StartCoroutine(LoadLevelCoroutine(SceneManager.GetActiveScene().name));
        }
        else
        {
            StartCoroutine(LoadLevelCoroutine(level));
        }
>>>>>>> Stashed changes
    }

    public void OnFadeOutComplete()
    {
        SceneManager.LoadScene(1);
        animator.SetTrigger("FadeIn");

    }
}
