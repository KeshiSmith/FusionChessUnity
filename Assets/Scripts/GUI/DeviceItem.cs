using Android.Bluetooth;
using UnityEngine;
using UnityEngine.UI;

public class DeviceItem : MonoBehaviour
{
    public Text nameText;

    private BluetoothDevice bluetoothDevice;

    public void BindDevice(BluetoothDevice device)
    {
        bluetoothDevice = device;
        nameText.text = device.Name;
    }

    public void OnClick()
    {
        if(bluetoothDevice != null)
            Bluetooth.Instance.Connect(bluetoothDevice);
    }
}
