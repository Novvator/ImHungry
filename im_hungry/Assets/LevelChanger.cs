using System.Collections;
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

    public void LoadLevel(string level)
    {
        Time.timeScale = 1f; // Resume normal time scale
        string currentScene = SceneManager.GetActiveScene().name;

        if (level == "current")
        {
            StartCoroutine(LoadLevelCoroutine(currentScene));
        }
        else if (level == "next")
        {
            int stage = StageProgress.StageNumberFromScene(currentScene);

            //if last stage of a world (or not a stage at all) go to world menu to choose new world
            if (stage < 1 || StageProgress.IsLastStageOfWorld(stage) || stage >= StageProgress.TotalStages)
            {
                StartCoroutine(LoadLevelCoroutine("World Menu"));
            }
            else
            {
                StartCoroutine(LoadLevelCoroutine("Stage " + (stage + 1)));
            }
        }
        else if (level == "Level Menu" && currentScene == EndlessModeScript.SceneName)
        {
            //endless mode is started from the main menu, so its back buttons return there
            StartCoroutine(LoadLevelCoroutine("Main Menu"));
        }
        else if (level == "Level Menu" && currentScene.Contains("Stage"))
        {
            //go back to the level menu of the world this stage belongs to
            int stage = StageProgress.StageNumberFromScene(currentScene);
            int world = stage < 1 ? 1 : StageProgress.WorldOfStage(stage);
            StartCoroutine(LoadLevelCoroutine(StageProgress.LevelMenuForWorld(world)));
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
