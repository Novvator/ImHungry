using UnityEngine;

// Single-finger stages. Pattern children: 0 = "fdcc" start (child 0 is the start circle),
// 1 and 2 = checkpoints, 3 = finish.
public class PatternScript : PatternGameBase
{
    private GameObject activetrail;
    private TrailScript activetrailscript;

    [SerializeField] GameObject Trail;

    public bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;

    protected override bool IsTracing => hitted;

    protected override void HandleInput()
    {
        foreach (Touch touch in TouchInput.touches)
        {
            if (touch.fingerId != 0)
            {
                continue; // Ignore touches with different IDs
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);
            if (touch.phase == TouchPhase.Began)
            {
                activetrail = Instantiate(Trail, pos, Quaternion.identity);
                activetrailscript = activetrail.GetComponent<TrailScript>();
            }

            if (completedlvl) continue;

            if (hit.collider != null && touch.phase != TouchPhase.Ended)
            {
                HandlePatternInteraction(hit);
            }
            else
            {
                FailAttempt();
            }
        }
    }

    protected override void HandlePatternInteraction(RaycastHit2D hit)
    {
        string name = hit.collider.name;

        if (name == "fdcc")
        {
            hitted = true;
            startTimer = true;
            chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

        if (name == chosenpat.transform.GetChild(1).name && hitted)
        {
            SoundManagerScript.PlaySound("eating1");
            //prevent replay of sound
            chosenpat.transform.GetChild(1).gameObject.SetActive(false);
            AdvanceFood(2);
            trg1 = true;
        }

        if (name == chosenpat.transform.GetChild(2).name && hitted && trg1)
        {
            SoundManagerScript.PlaySound("eating1");
            //prevent replay of sound
            chosenpat.transform.GetChild(2).gameObject.SetActive(false);
            AdvanceFood(3);
            trg2 = true;
        }

        if (name == chosenpat.transform.GetChild(3).name && hitted && trg1 && trg2)
        {
            CompletePattern();
        }
    }

    protected override void ResetPatternState()
    {
        hitted = false;
        trg1 = false;
        trg2 = false;

        //reset stage triggers and start circle
        chosenpat.transform.GetChild(1).gameObject.SetActive(true);
        chosenpat.transform.GetChild(2).gameObject.SetActive(true);
        chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
}
