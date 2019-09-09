public abstract class DialogBase
{
    public string Name { get; protected set; }
    public int? ResultCode { get; protected set; }
    public DataBundle Datas {get; protected set;}
    public bool ModelFlag { get; protected set; }
    public bool BlockFlag { get; protected set; }

    public DialogBase(string name, int? resultCode = null, DataBundle datas = null, bool blockFlag = true, bool modelFlag = true)
    {
        Name = name;
        ResultCode = resultCode;
        Datas = datas;
        BlockFlag = blockFlag;
        ModelFlag = modelFlag;
    }

    protected void OpenDialog()
    {
        PanelManager.Instance.OpenDialog(this);
    }
}

public class Dialog : DialogBase
{
    public Dialog(string name, int? resultCode = null, DataBundle datas = null, bool blockFlag = true, bool modelFlag = true)
        : base(name, resultCode, datas, blockFlag, modelFlag)
    {
    }

    public void Open()
    {
        OpenDialog();
    }
}

public class Toast : DialogBase
{
    public Toast(string message) : base("_Toast")
    {
        Datas = new DataBundle();
        Datas.PutData("message", message);
        BlockFlag = false;
    }

    public void Show()
    {
        PanelManager.Instance.OpenOnceDialog(this);
    }
}

public class Message : Dialog
{
    public Message(string title, string message, int? resultCode) : base("_MessageDialog", resultCode)
    {

        Datas = new DataBundle();
        Datas.PutData("title", title);
        Datas.PutData("message", message);
    }
}

