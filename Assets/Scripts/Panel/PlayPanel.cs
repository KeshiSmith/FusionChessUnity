using Android.Bluetooth;

public class PlayPanel : AbstractPanel
{
    private Bluetooth Bluetooth
    {
        get
        {
            return Bluetooth.Instance;
        }
    }

    public void PVC()
    {
        OpenGamePanel(false);
    }
    public void PVP()
    {
        if (!Bluetooth.HasBluetoothAdapter())
        {
            new Toast("not_support_bluetooth").Show();
            return;
        }
        if (Bluetooth.IsEnable() || Bluetooth.Enable())
            OpenGamePanel(true);
    }
    public void Back()
    {
        Finish();
    }

    public void OpenGamePanel(bool pvpModel)
    {
        var datas = new DataBundle();
        datas.PutData("pvpModel", pvpModel);
        StartPanel("GamePanel", null, datas);
    }
}
