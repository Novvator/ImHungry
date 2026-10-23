using UnityEngine;

// Two-finger stages. Each pattern has two sets of children, "_1" for finger 0 and "_2" for
// finger 1: fdcc_N (with a "start" circle), 2_N and 3_N checkpoints, finish_N.
// Both sets must be traced before either finish counts.
public class PatternDualScript : PatternGameBase
{
    private GameObject activetrail_1;
    private GameObject activetrail_2;

    private DualTrailScript activetrailscript_1;
    private DualTrailScript activetrailscript_2;

    [SerializeField] GameObject Trail_1;
    [SerializeField] GameObject Trail_2;

    public bool hitted_1 = false;
    public bool hitted_2 = false;
    bool trg1_1 = false;
    bool trg1_2 = false;
    bool trg2_1 = false;
    bool trg2_2 = false;

    protected override bool IsTracing => hitted_1 || hitted_2;

    bool SetComplete(int set)
    {
        return set == 1 ? hitted_1 && trg1_1 && trg2_1 : hitted_2 && trg1_2 && trg2_2;
    }

    protected override void HandleInput()
    {
        foreach (Touch touch in TouchInput.touches)
        {
            if (touch.fingerId > 1)
            {
                continue; // Only the first two fingers play
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.fingerId == 0)
                {
                    activetrail_1 = Instantiate(Trail_1, pos, Quaternion.identity);
                    activetrailscript_1 = activetrail_1.GetComponent<DualTrailScript>();
                    activetrailscript_1.SetFingerId(0);
                }
                else
                {
                    activetrail_2 = Instantiate(Trail_2, pos, Quaternion.identity);
                    activetrailscript_2 = activetrail_2.GetComponent<DualTrailScript>();
                    activetrailscript_2.SetFingerId(1);
                }
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

        // ------------------------------
        // PATTERN SET 1  ( _1 )
        // ------------------------------
        if (name == "fdcc_1")
        {
            hitted_1 = true;
            startTimer = true;
            chosenpat.transform.Find("fdcc_1/start").gameObject.SetActive(false);
        }

        if (name == "2_1" && hitted_1)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("2_1").gameObject.SetActive(false);
            AdvanceFood(2);
            trg1_1 = true;
        }

        if (name == "3_1" && hitted_1 && trg1_1)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("3_1").gameObject.SetActive(false);
            AdvanceFood(3);
            trg2_1 = true;
        }

        // ------------------------------
        // PATTERN SET 2  ( _2 )
        // ------------------------------
        if (name == "fdcc_2")
        {
            hitted_2 = true;
            startTimer = true;
            chosenpat.transform.Find("fdcc_2/start").gameObject.SetActive(false);
        }

        if (name == "2_2" && hitted_2)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("2_2").gameObject.SetActive(false);
            AdvanceFood(2);
            trg1_2 = true;
        }

        if (name == "3_2" && hitted_2 && trg1_2)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("3_2").gameObject.SetActive(false);
            AdvanceFood(3);
            trg2_2 = true;
        }

        if ((name == "finish_1" || name == "finish_2") && SetComplete(1) && SetComplete(2))
        {
            CompletePattern();
        }
    }

    protected override void ResetPatternState()
    {
        hitted_1 = hitted_2 = false;
        trg1_1 = trg2_1 = false;
        trg1_2 = trg2_2 = false;

        //reset stage triggers and start circles of both sets
        chosenpat.transform.Find("fdcc_1/start").gameObject.SetActive(true);
        chosenpat.transform.Find("2_1").gameObject.SetActive(true);
        chosenpat.transform.Find("3_1").gameObject.SetActive(true);

        chosenpat.transform.Find("fdcc_2/start").gameObject.SetActive(true);
        chosenpat.transform.Find("2_2").gameObject.SetActive(true);
        chosenpat.transform.Find("3_2").gameObject.SetActive(true);
    }
}
