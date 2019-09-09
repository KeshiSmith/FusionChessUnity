using UnityEngine;

namespace Android.Bluetooth
{
    public delegate void OnFoundDevice(BluetoothDevice device);
    public delegate void OnGetSocketConnect();
    public delegate void OnReceivedMsg(string message);
    public delegate void OnDisconnection();

    public class BluetoothListener : AndroidJavaProxy
    {
        public OnFoundDevice OnFoundDeviceDelegate = null;
        public OnGetSocketConnect OnGetSocketConnectDelegate = null;
        public OnReceivedMsg OnReceivedMsgDelegate = null;
        public OnDisconnection OnDisconnectionDelegate = null;

        public BluetoothListener() : base("com.keshi.bluetooth.BluetoothHelper$BluetoothHelperListener")
        {
        }

        void onFoundDevice(AndroidJavaObject deviceObject)
        {
            if (OnFoundDeviceDelegate != null)
            {
                var device = new BluetoothDevice(deviceObject);
                OnFoundDeviceDelegate(device);
            }
        }
        void onGetSocketConnect()
        {
            if (OnGetSocketConnectDelegate != null)
                OnGetSocketConnectDelegate();
        }
        void onReceivedMsg(string message)
        {
            if (OnReceivedMsgDelegate != null)
                OnReceivedMsgDelegate(message);
        }
        void onDisconnection()
        {
            if (OnDisconnectionDelegate != null)
                OnDisconnectionDelegate();
        }
    }
}
