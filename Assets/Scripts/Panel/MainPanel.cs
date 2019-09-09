using UnityEngine;

public class MainPanel : AbstractPanel
{
    private enum ResultCode
    {
        ExitDialog
    }

    public void Play()
    {
        StartPanel("PlayPanel");
    }
    public void Help()
    {
        StartPanel("HelpPanel");
    }
    public void Options()
    {
        StartPanel("OptionsPanel");
    }
    public void About()
    {
        StartPanel("AboutPanel");
    }
    public void Exit()
    {
        var exitDialog = new Message("exit", "exit_message", (int)ResultCode.ExitDialog);
        exitDialog.Open();
    }

    public override void OnEscapeDown()
    {
        Exit();
    }
    public override void OnPanelResult(int? resultCode, DataBundle datas)
    {
        switch (resultCode)
        {
            case (int)ResultCode.ExitDialog:
                bool confirm = (bool)datas.GetData("confirm");
                if (confirm)
                    Application.Quit();
                break;
        }
    }
}
