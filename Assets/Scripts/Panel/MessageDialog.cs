public class MessageDialog : AbstractDialog
{
    public L10nText titleText, messageText;

    public override void InitDialog(DataBundle datas = null)
    {
        if (datas != null)
        {
            var title = datas.ContainsKey("title") ? (string)datas.GetData("title") : "";
            titleText.SetText(title);
            var message = datas.ContainsKey("message") ? (string)datas.GetData("message") : "";
            messageText.SetText(message);
        }
    }
    public override void OnEscapeDown()
    {
        OnCancelClicked();
    }

    public void OnConfirmClicked()
    {
        CloseMessageDialog(true);
    }
    public void OnCancelClicked()
    {
        CloseMessageDialog(false);
    }
    private void CloseMessageDialog(bool confirm)
    {
        var datas = new DataBundle();
        datas.PutData("confirm", confirm);
        Close(datas);
    }
}
