using Android.Bluetooth;

public class HostDialog : AbstractDialog
{
    private bool closeFlag = false;

    private Bluetooth Bluetooth
    {
        get
        {
            return Bluetooth.Instance;
        }
    }

    public override void InitDialog(DataBundle datas = null)
    {
        Bluetooth.SetBluetoothVisible();
        var listener = Bluetooth.GetBluetoothListener();
        listener.OnGetSocketConnectDelegate = delegate
        {
            closeFlag = true;
        };
        Bluetooth.StartAcceptThread();
    }
    public override void OnEscapeDown()
    {
        Cancel();
    }

    void Update()
    {
        UpdateCloseFlag();
    }

    public void Cancel()
    {
        Bluetooth.CancelAcceptThread();
        Close();
    }

    private void UpdateCloseFlag()
    {
        if (closeFlag) Close();
    }
}
