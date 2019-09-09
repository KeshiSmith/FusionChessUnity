package com.keshi.bluetooth;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Set;
import java.util.UUID;

public class BluetoothHelper extends BroadcastReceiver {

    public interface BluetoothHelperListener {
        void onFoundDevice(BluetoothDevice device);

        void onGetSocketConnect();

        void onReceivedMsg(String message);

        void onDisconnection();
    }

    private static final String NAME = "BluetoothServer";
    private static final UUID MY_UUID = UUID.fromString("22ed8386-3358-5243-8463-738f615fbce2");
    private static final int VISIBLE_SECOND = 120;

    private static BluetoothHelper bluetoothHelper = null;

    private Context context;
    private BluetoothAdapter adapter;
    private BluetoothHelperListener listener;

    private AcceptThread acceptThread = null;
    private ConnectedThread connectedThread = null;

    public static void initBluetoothHelper(Context context) {
        if (bluetoothHelper == null)
            bluetoothHelper = new BluetoothHelper(context);
    }

    public static BluetoothHelper getBluetoothHelper() {
        return bluetoothHelper;
    }

    private BluetoothHelper(Context context) {
        this.context = context;
        adapter = BluetoothAdapter.getDefaultAdapter();
        registerReceiver();
    }

    protected void finalize() {
        bluetoothHelper.unregisterReceiver();
    }

    public void bindListener(BluetoothHelperListener listener) {
        this.listener = listener;
    }

    public void setBluetoothVisible() {
        Intent discoverableIntent = new
                Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
        discoverableIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION, VISIBLE_SECOND);
        context.startActivity(discoverableIntent);
    }

    public boolean hasBluetoothAdapter() {
        return adapter != null;
    }

    public boolean isEnable() {
        return adapter.isEnabled();
    }

    public boolean enable() {
        return adapter.enable();
    }

    public Set<BluetoothDevice> getBondedDevices() {
        return adapter.getBondedDevices();
    }

    public boolean isDiscovering() {
        return adapter.isDiscovering();
    }

    public void startDiscovery() {
        adapter.startDiscovery();
    }

    public void cancelDiscovery() {
        adapter.cancelDiscovery();
    }

    public boolean isConnected() {
        return connectedThread != null;
    }

    public void connect(BluetoothDevice device) {
        disconnect();
        ConnectThread connectThread = new ConnectThread(device);
        connectThread.start();
    }

    public void disconnect() {
        if (isConnected()) {
            connectedThread.cancel();
            connectedThread = null;
        }
    }

    public void startAcceptThread() {
        cancelAcceptThread();
        acceptThread = new AcceptThread();
        acceptThread.start();
    }

    public void cancelAcceptThread() {
        if (acceptThread != null) {
            acceptThread.cancel();
            acceptThread = null;
        }
    }

    public void send(String message){
        if(connectedThread != null)
            connectedThread.send(message);
    }

    private void registerReceiver() {
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        context.registerReceiver(this, filter);
    }

    private void unregisterReceiver() {
        context.unregisterReceiver(this);
    }

    private void manageConnectedSocket(BluetoothSocket socket) {
        connectedThread = new ConnectedThread(socket);
        connectedThread.start();
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        String action = intent.getAction();
        if (BluetoothDevice.ACTION_FOUND.equals(action)) {
            BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
            if (listener != null)
                listener.onFoundDevice(device);
        }
    }

    private class AcceptThread extends Thread {
        private final BluetoothServerSocket serverSocket;

        public AcceptThread() {
            BluetoothServerSocket tmp = null;
            try {
                tmp = adapter.listenUsingRfcommWithServiceRecord(NAME, MY_UUID);
            } catch (IOException e) {
            }
            serverSocket = tmp;
        }

        public void run() {
            BluetoothSocket socket;
            while (true) {
                try {
                    socket = serverSocket.accept();
                } catch (IOException e) {
                    break;
                }
                if (socket != null) {
                    manageConnectedSocket(socket);
                    cancel();
                    break;
                }
            }
        }

        public void cancel() {
            try {
                serverSocket.close();
            } catch (IOException e) {
            }
        }
    }

    private class ConnectThread extends Thread {
        private final BluetoothSocket socket;

        public ConnectThread(final BluetoothDevice device) {
            BluetoothSocket tmp = null;
            try {
                tmp = device.createRfcommSocketToServiceRecord(MY_UUID);
            } catch (IOException e) {
            }
            socket = tmp;
        }

        public void run() {
            adapter.cancelDiscovery();

            try {
                socket.connect();
            } catch (IOException connectException) {
                try {
                    socket.close();
                } catch (IOException closeException) {
                }
                return;
            }
            manageConnectedSocket(socket);
        }
    }

    private class ConnectedThread extends Thread {
        private final BluetoothSocket socket;
        private final InputStream inStream;
        private final OutputStream outStream;

        public ConnectedThread(BluetoothSocket socket) {
            this.socket = socket;
            InputStream tmpIn = null;
            OutputStream tmpOut = null;
            try {
                tmpIn = socket.getInputStream();
                tmpOut = socket.getOutputStream();
            } catch (IOException e) {
            }
            inStream = tmpIn;
            outStream = tmpOut;

            if (listener != null)
                listener.onGetSocketConnect();
        }

        public void run() {
            byte[] buffer = new byte[1024];
            int length;
            while (true) {
                try {
                    length = inStream.read(buffer);
                    if (length != -1 && listener != null) {
                        String message = new String(buffer, 0, length);
                        listener.onReceivedMsg(message);
                    }
                } catch (IOException e) {
                    break;
                }
            }
            if(listener != null)
                listener.onDisconnection();
        }

        public void send(String message) {
            byte[] bytes = message.getBytes();
            try {
                outStream.write(bytes);
                outStream.flush();
            } catch (IOException e) {
            }
        }

        public void cancel() {
            try {
                socket.close();
            } catch (IOException e) {
            }
        }
    }
}
