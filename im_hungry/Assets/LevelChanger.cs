using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;
    public float transitionTime = 1f;
    private int world1Levels = 3;
    private int world2Levels = 3;
    private int world3Levels = 3;

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
        else if (level == "next")
        {
            string subjectString = SceneManager.GetActiveScene().name;
            int resultInt = Int32.Parse(Regex.Match(subjectString, @"\d+").Value); //extract stage number

            //if last stage go to world menu to choose new world
            if (resultInt == 5) 
            { 
                StartCoroutine(LoadLevelCoroutine("World Menu"));
            }
            else
            {
                string level_to_load = "Stage " + (resultInt + 1).ToString();
                StartCoroutine(LoadLevelCoroutine(level_to_load));
            }
            
        }
        else if (level == "Level Menu")
        {
            if (SceneManager.GetActiveScene().name.Contains("Stage"))
            {
                string subjectString = SceneManager.GetActiveScene().name;
                int resultInt = Int32.Parse(Regex.Match(subjectString, @"\d+").Value); //extract stage number
                string level_to_load = "Level Menu" + LevelMenuLoad(resultInt);
                StartCoroutine(LoadLevelCoroutine(level_to_load));
            }
            else
            {
                StartCoroutine(LoadLevelCoroutine(level));
            }
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

    private string LevelMenuLoad(int level)
    {
        if (level <= world1Levels)
        {
            return "";
        }
        else if (world1Levels < level && level <= world1Levels + world2Levels)
        {
            return " 2";
        }
        else
        {
            return " 3";
        }





    }
}
