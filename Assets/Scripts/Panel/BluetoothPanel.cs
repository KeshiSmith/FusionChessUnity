using Android.Bluetooth;

public class BluetoothPanel : AbstractPanel
{
    private enum ResultCode
    {
        HostDialog,
        JoinDialog
    }

    public override void OnPanelResult(int? resultCode, DataBundle datas)
    {
        switch (resultCode)
        {
            case (int)ResultCode.HostDialog:
                FinishWithDataIfConnected(true);
                break;
            case (int)ResultCode.JoinDialog:
                FinishWithDataIfConnected(false);
                break;
        }
    }
    public override void OnEscapeDown()
    {
        Finish();
    }

    public void Host()
    {
        new Dialog("HostDialog", (int)ResultCode.HostDialog).Open();
    }
    public void Join()
    {
        new Dialog("JoinDialog", (int)ResultCode.JoinDialog).Open();
    }
    public void Back()
    {
        Finish();
    }

    private void FinishWithDataIfConnected(bool isHost)
    {
        if (Bluetooth.Instance.IsConnected())
        {
            var datas = new DataBundle();
            datas.PutData("isHost", isHost);
            Finish(datas);
        }
    }
}
