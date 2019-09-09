using Android.Bluetooth;
using System.Collections.Generic;
using UnityEngine;

public class JoinDialog : AbstractDialog
{
    public Transform content;
    public GameObject prefabs;

    private HashSet<BluetoothDevice> devices = new HashSet<BluetoothDevice>();
    private Queue<BluetoothDevice> newDevices = new Queue<BluetoothDevice>();
    private object devicesLock = new object();
    private bool closeFlag = false;

    private Bluetooth Bluetooth
    {
        get
        {
            return Bluetooth.Instance;
        }
    }
    private BluetoothListener BluetoothListener
    {
        get
        {
            return Bluetooth.GetBluetoothListener();
        }
    }

    public override void InitDialog(DataBundle datas = null)
    {
        var listener = Bluetooth.GetBluetoothListener();
        listener.OnFoundDeviceDelegate = delegate(BluetoothDevice device)
        {
            lock (devicesLock)
            {
                newDevices.Enqueue(device);
            }
        };
        listener.OnGetSocketConnectDelegate = delegate
        {
            closeFlag = true;
        };
        ScanBondedDevices();
        Bluetooth.StartDiscovery();
    }
    public override void OnEscapeDown()
    {
        Cancel();
    }

    void Update()
    {
        UpdateNewDevices();
        UpdateCloseFlag();
    }

    public void Refresh()
    {
        if(!Bluetooth.IsDiscovering())
            Bluetooth.StartDiscovery();
    }
    public void Cancel()
    {
        Bluetooth.Disconnect();
        Close();
    }

    private void AddNewDevice(BluetoothDevice device)
    {
        if (!devices.Contains(device))
        {
            devices.Add(device);
            var deviceItemObject = Instantiate(prefabs, content);
            var deviceItemScript = deviceItemObject.transform.GetComponent<DeviceItem>();
            deviceItemScript.BindDevice(device);
        }
    }
    private void UpdateCloseFlag()
    {
        if (closeFlag) Close();
    }
    private void UpdateNewDevices()
    {
        lock (devicesLock)
        {
            while (newDevices.Count > 0)
            {
                var device = newDevices.Dequeue();
                AddNewDevice(device);
            }
        }
    }
    private void ScanBondedDevices()
    {
        lock (devicesLock)
        {
            var bondedDevices = Bluetooth.getBondedDevices();
            foreach (var device in bondedDevices)
                newDevices.Enqueue(device);
        }
    }
}
