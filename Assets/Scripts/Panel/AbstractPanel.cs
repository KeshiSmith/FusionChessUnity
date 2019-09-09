public abstract class AbstractPanel : AbstractPanelBase
{
    public virtual void InitPanel(DataBundle datas = null)
    {
    }
    public virtual void OnPanelResult(int? resultCode, DataBundle datas)
    {
    }

    public override void OnEscapeDown()
    {
        Finish();
    }

    protected void StartPanel(string name, int? resultCode = null, DataBundle datas = null)
    {
        PanelManager.Instance.StartPanel(name, resultCode, datas);
    }

    protected void Finish(DataBundle datas = null)
    {
        PanelManager.Instance.FinishPanel(ResultId, datas);
    }
}
