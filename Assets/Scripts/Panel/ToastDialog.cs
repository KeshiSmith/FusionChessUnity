public class ToastDialog : AbstractDialog
{
    public L10nText messageText;

    public override void InitDialog(DataBundle datas = null)
    {
        if(datas != null)
        {
            var message = datas.ContainsKey("message") ? (string)datas.GetData("message") : "";
            messageText.SetText(message);
        }
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
