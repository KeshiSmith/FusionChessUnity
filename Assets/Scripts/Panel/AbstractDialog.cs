public abstract class AbstractDialog : AbstractPanelBase
{
    public virtual void InitDialog(DataBundle datas = null)
    {
    }

    protected void Close(DataBundle datas = null)
    {
        PanelManager.Instance.CloseDialog(ResultId, datas);
    }
}
