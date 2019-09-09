using UnityEngine;

public abstract class AbstractPanelBase : MonoBehaviour
{
    public int? ResultId { get; private set; }

    public void InitResultId(int? id = null)
    {
        ResultId = id;
    }

    public virtual void OnEscapeDown()
    {
    }
}
