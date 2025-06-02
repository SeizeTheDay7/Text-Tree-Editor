using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] float hold_time_ref; // if hold time exceeds the ref, consider as hold
    [SerializeField] float hold_dist_lim; // if drag dist exceeds the lim, not considered as hold
    [SerializeField] float drag_dist_ref;
    [SerializeField] float drag_time_lim;
    bool canBeHold = true;
    bool canBeDrag = true;
    int clickCount = 0;
    float clickStartTime;
    Vector2 clickStartPos;

    void OnEnable()
    {
        canBeHold = false;
        canBeDrag = false;
        clickCount = 0;
        clickStartPos = Vector2.zero;
    }

    void Update()
    {
        // Synth.Play();
        if (Input.GetKeyDown(KeyCode.Mouse0)) { CheckClick(); }
        else if (Input.GetKey(KeyCode.Mouse0)) { CheckHold(); CheckDrag(); }
    }
    private void CheckClick()
    {
        canBeHold = true;
        canBeDrag = true;
        clickStartTime = Time.time;
        clickStartPos = Input.mousePosition;

        clickCount++;
        if (clickCount == 3)
        {
            clickCount = 0;
            print("CheckClick() :: Click");
        }
    }

    private void CheckHold()
    {
        // prevent 1. re-enter hold radius 2. hold before starts interaction
        if (!canBeHold) return;

        Vector2 clickEndPoint = Input.mousePosition;
        float drag_dist = Vector2.Distance(clickStartPos, clickEndPoint);
        if (drag_dist > hold_dist_lim)
        {
            print($"CheckHold() :: canbeHold = false,  drag_dist : {drag_dist}");
            canBeHold = false;
            return;
        }

        float holdTime = Time.time - clickStartTime;
        if (holdTime >= hold_time_ref)
        {
            print("CheckHold :: Hold");
        }
    }

    private void CheckDrag()
    {
        // prevent 1. slow drag 2. drag before starts interaction
        if (!canBeDrag) return;

        float holdTime = Time.time - clickStartTime;
        if (holdTime > drag_time_lim)
        {
            canBeDrag = false;
            return;
        }

        Vector2 clickEndPoint = Input.mousePosition;
        float drag_dist = Vector2.Distance(clickStartPos, clickEndPoint);
        if (drag_dist >= drag_dist_ref)
        {
            print($"CheckDrag :: Drag : {drag_dist_ref}");
        }
    }
}
