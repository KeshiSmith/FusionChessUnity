using UnityEngine;

public class HelpPanel : AbstractPanel
{
    public GameObject previous, next;
    public Animator animator;


    private int currentState = 1;
    private int CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
            previous.SetActive(currentState != 1);
            next.SetActive(currentState != 3);
        }
    }

    void Start()
    {
        InitPanel();
    }

    public void Previous()
    {
        if(currentState > 1)
        {
            CurrentState--;
            animator.SetTrigger("previous");
        }
    }
    public void Next()
    {
        if(currentState < 3)
        {
            CurrentState++;
            animator.SetTrigger("next");
        }
    }
    public void Back()
    {
        Finish();
    }

    private void InitPanel()
    {
        CurrentState = 1;
    }
}
