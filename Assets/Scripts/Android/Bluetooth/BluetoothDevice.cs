using UnityEngine;

namespace Android.Bluetooth
{
    public class BluetoothDevice
    {
        private AndroidJavaObject bluetoothDeviceObject;

        public string Address
        {
            get
            {
                return bluetoothDeviceObject.Call<string>("getAddress");
            }
        }
        public string Name
        {
            get
            {
                return bluetoothDeviceObject.Call<string>("getName");
            }
        }

        public BluetoothDevice(AndroidJavaObject bluetoothDevice)
        {
            bluetoothDeviceObject = bluetoothDevice;
        }

        public override bool Equals(object obj)
        {
            var device = obj as BluetoothDevice;
            return device.Name == Name && device.Address == Address;
        }
        public override int GetHashCode()
        {
            var hashCode = -1717786383;
            hashCode = hashCode * -1521134295 + Address.GetHashCode();
            hashCode = hashCode * -1521134295 + Name.GetHashCode();
            return hashCode;
        }

        public AndroidJavaObject GetAndroidJavaObject()
        {
            return bluetoothDeviceObject;
        }
    }
}
