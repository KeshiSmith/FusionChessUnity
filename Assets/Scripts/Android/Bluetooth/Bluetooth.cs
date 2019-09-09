using System.Collections.Generic;
using UnityEngine;

namespace Android.Bluetooth
{
    public class Bluetooth
    {
        private static Bluetooth instance = null;
        private AndroidJavaObject bluetoothHelper = null;
        private BluetoothListener listener;

        public static Bluetooth Instance
        {
            get
            {
                if(instance == null)
                    instance = new Bluetooth();
                return instance;
            }
        }

        private Bluetooth()
        {
            var bluetoothHelperClass = new AndroidJavaClass("com.keshi.bluetooth.BluetoothHelper");
            var activity = Content.GetCurrentActivity();
            bluetoothHelperClass.CallStatic("initBluetoothHelper", activity);
            bluetoothHelper = bluetoothHelperClass.CallStatic<AndroidJavaObject>("getBluetoothHelper");
            if (bluetoothHelper != null)
            {
                listener = new BluetoothListener();
                bluetoothHelper.Call("bindListener", listener);
            }
        }

        public BluetoothListener GetBluetoothListener()
        {
            return listener;
        }

        public bool HasBluetoothAdapter()
        {
            return bluetoothHelper != null && bluetoothHelper.Call<bool>("hasBluetoothAdapter");
        }
        public bool IsEnable()
        {
            return bluetoothHelper.Call<bool>("isEnable");
        }
        public bool Enable()
        {
            return bluetoothHelper.Call<bool>("enable");
        }

        public void SetBluetoothVisible()
        {
            bluetoothHelper.Call("setBluetoothVisible");
        }
        public HashSet<BluetoothDevice> getBondedDevices()
        {
            var devices = new HashSet<BluetoothDevice>();
            var bondedDevices = bluetoothHelper.Call<AndroidJavaObject>("getBondedDevices");
            if (bondedDevices != null)
            {
                var devicesObject = bondedDevices.Call<AndroidJavaObject[]>("toArray");
                foreach (var deviceObject in devicesObject)
                {
                    var device = new BluetoothDevice(deviceObject);
                    devices.Add(device);
                }
            }
            return devices;
        }

        public bool IsDiscovering()
        {
            return bluetoothHelper.Call<bool>("isDiscovering");
        }
        public void StartDiscovery()
        {
            bluetoothHelper.Call("startDiscovery");
        }
        public void CancelDiscovery()
        {
            bluetoothHelper.Call("cancelDiscovery");
        }

        public void Connect(BluetoothDevice device)
        {
            var deviceObject = device.GetAndroidJavaObject();
            bluetoothHelper.Call("connect", deviceObject);
        }

        public void StartAcceptThread()
        {
            bluetoothHelper.Call("startAcceptThread");
        }
        public void CancelAcceptThread()
        {
            bluetoothHelper.Call("cancelAcceptThread");
        }

        public void Disconnect()
        {
            bluetoothHelper.Call("disconnect");
        }
        public bool IsConnected()
        {
            return bluetoothHelper != null && bluetoothHelper.Call<bool>("isConnected");
        }
        public void Send(string message)
        {
            bluetoothHelper.Call("send", message);
        }
    }
}
