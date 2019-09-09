using Android.Bluetooth;
using FusionChess;
using System.Collections.Generic;

public class GamePanel : AbstractPanel
{
    private enum ResultCode
    {
        BluetoothPanael,
        NewDialog,
        NewOkDialog,
        RegretDialog,
        RegretOkDialog,
        ResignDialog,
        DrawDialog,
        DrawOkDialog,
        SettingsDialog,
        BackDialog
    }

    private Queue<string> messageQueue = new Queue<string>();
    private object messageLock = new object();

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
    private GameController GameController
    {
        get
        {
            return GameController.Instance;
        }
    }

    private bool PVPModel
    {
        get
        {
            return GameController.PVPModel;
        }
        set
        {
            GameController.PVPModel = value;
        }
    }
    private bool IsHost
    {
        get
        {
            return GameController.IsHost;
        }
        set
        {
            GameController.IsHost = value;
        }
    }
    private bool IsPlaying
    {
        get
        {
            return GameController.IsPlaying;
        }
    }
    private bool CanRegret
    {
        get
        {
            return GameController.CanRegret;
        }
    }
    private bool AnimeFlag
    {
        set
        {
            GameParams.animeFlag = value;
        }
    }
    private bool SelectFlag
    {
        get
        {
            return GameParams.selectFlag;
        }
        set
        {
            GameParams.selectFlag = value;
        }
    }

    void Update()
    {
        UpdateBluetoothMessage();
    }

    public void New()
    {
        if (PVPModel && !IsHost)
        {
            new Toast("not_host").Show();
            return;
        }
        if (IsPlaying)
            OpenNewDialog();
        else
            OpenNewOkDialog();
    }
    public void Regret()
    {
        if (IsPlaying && CanRegret)
            OpenRegretDialog();
    }
    public void Resign()
    {
        if (IsPlaying)
            OpenResignDialog();
    }
    public void Draw()
    {
        if (IsPlaying)
            OpenDrawDialog();
    }
    public void Settings()
    {
        OpenSettingsDialog();
    }
    public void Back()
    {
        new Message("back", "back_message", (int)ResultCode.BackDialog).Open();
    }

    private void OnNew(PieceColor myColor, bool fusionModel, bool hiddenModel)
    {
        if (PVPModel)
        {
            var color = myColor == PieceColor.Red ? 1 : 0;
            var fusion = fusionModel ? 1 : 0;
            var hidden = hiddenModel ? 1 : 0;
            string message = string.Format("new {0} {1} {2}", color, fusion, hidden);
            Bluetooth.Send(message);
        }
        OnNewOk(myColor, fusionModel, hiddenModel);
    }
    private void OnNewOk(PieceColor myColor, bool fusionModel, bool hiddenModel)
    {
        GameController.NewGame(myColor, fusionModel, hiddenModel);
    }
    private void OnRegret()
    {
        if (PVPModel)
            Bluetooth.Send("regret");
        else OnRegretOk(GameController.MyColor);
    }
    private void OnRegretOk(PieceColor color)
    {
        GameController.Regret(color);
    }
    private void OnResign()
    {
        var myColor = GameController.MyColor;
        if (PVPModel)
            Bluetooth.Send("resign");
        OnResignOk(myColor);
    }
    private void OnResignOk(PieceColor color)
    {
        GameController.Resign(color);
    }
    private void OnDraw()
    {
        if (PVPModel)
            Bluetooth.Send("draw");
        else OnDrawOk();
    }
    private void OnDrawOk()
    {
        GameController.Draw();
    }
    private void OnBack()
    {
        if (Bluetooth.IsConnected())
            Bluetooth.Disconnect();
        Finish();
    }
    private void OnDisconnect()
    {
        new Toast("player_exit").Show();
        Bluetooth.Disconnect();
        GameController.GameOver();
    }

    public override void InitPanel(DataBundle datas = null)
    {
        LoadConfig();
        PVPModel = (bool)datas.GetData("pvpModel");
        if (PVPModel)
            OpenBluetoothPanel();
        else
            OpenNewOkDialog();
    }
    public override void OnEscapeDown()
    {
        Back();
    }
    public override void OnPanelResult(int? resultCode, DataBundle datas)
    {
        switch (resultCode)
        {
            case (int)ResultCode.BluetoothPanael:
                if (datas == null)
                    Finish();
                else
                {
                    IsHost = (bool)datas.GetData("isHost");
                    InitBluetoothModel();
                }
                break;
            case (int)ResultCode.NewDialog:
                var confirm = (bool)datas.GetData("confirm");
                if (confirm)
                    OpenNewOkDialog();
                break;
            case (int)ResultCode.NewOkDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                {
                    var myColor = (bool)datas.GetData("initiative") ? PieceColor.Red : PieceColor.Black;
                    var fusionModel = (bool)datas.GetData("fusionModel");
                    var hiddenModel = (bool)datas.GetData("hiddenModel");
                    OnNew(myColor, fusionModel, hiddenModel);
                }
                else
                    Finish();
                break;
            case (int)ResultCode.RegretDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                    OnRegret();
                break;
            case (int)ResultCode.RegretOkDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                {
                    Bluetooth.Send("regretOk");
                    var regretColor = !GameController.MyColor;
                    OnRegretOk(regretColor);
                }
                else Bluetooth.Send("regretNot");
                break;
            case (int)ResultCode.ResignDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                    OnResign();
                break;
            case (int)ResultCode.DrawDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                    OnDraw();
                break;
            case (int)ResultCode.DrawOkDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                {
                    Bluetooth.Send("drawOk");
                    OnDrawOk();
                }
                else Bluetooth.Send("drawNot");
                break;
            case (int)ResultCode.SettingsDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                {
                    var oldSlectFlag = SelectFlag;
                    AnimeFlag = (bool)datas.GetData("animeFlag");
                    SelectFlag = (bool)datas.GetData("selectFlag");
                    if (oldSlectFlag != SelectFlag)
                        GameController.UpdateSelects();
                }
                break;
            case (int)ResultCode.BackDialog:
                confirm = (bool)datas.GetData("confirm");
                if (confirm)
                    OnBack();
                break;
        }
    }

    private void LoadConfig()
    {
        Config config = Config.Instance;
        var animeFlag = config.GetValue("Settings", "AnimeFlag", "1");
        AnimeFlag = (animeFlag == "1");
        var selectFlag = config.GetValue("Settings", "SelectFlag", "1");
        SelectFlag = (selectFlag == "1");
    }
    private void OpenBluetoothPanel()
    {
        StartPanel("BluetoothPanel", (int)ResultCode.BluetoothPanael);
    }
    private void OpenNewDialog()
    {
        new Message("new", "new_message", (int)ResultCode.NewDialog).Open();
    }
    private void OpenNewOkDialog()
    {
        new Dialog("NewDialog", (int)ResultCode.NewOkDialog).Open();
    }
    private void OpenRegretDialog()
    {
        new Message("regret", "regret_message", (int)ResultCode.RegretDialog).Open();
    }
    private void OpenRegretOkDialog()
    {
        new Message("regret", "regret_ok", (int)ResultCode.RegretOkDialog).Open();
    }
    private void OpenResignDialog()
    {
        new Message("resign", "resign_message", (int)ResultCode.ResignDialog).Open();
    }
    private void OpenDrawDialog()
    {
        new Message("draw", "draw_message", (int)ResultCode.DrawDialog).Open();
    }
    private void OpenDrawOkDialog()
    {
        new Message("draw", "draw_ok", (int)ResultCode.DrawOkDialog).Open();
    }
    private void OpenSettingsDialog()
    {
        new Dialog("SettingsDialog", (int)ResultCode.SettingsDialog).Open();
    }

    private void InitBluetoothModel()
    {
        BluetoothListener.OnReceivedMsgDelegate = delegate (string message)
        {
            lock (messageLock)
            {
                messageQueue.Enqueue(message);
            }
        };
        BluetoothListener.OnDisconnectionDelegate = delegate ()
        {
            lock (messageLock)
            {
                messageQueue.Enqueue("disconnect");
            }
        };
        if (IsHost)
            OpenNewOkDialog();
        else new Toast("player_ok").Show();
    }
    private void UpdateBluetoothMessage()
    {
        lock (messageLock)
        {
            while (messageQueue.Count > 0)
            {
                var message = messageQueue.Dequeue();
                ProcessBluetoothMessage(message);
            }
        }
    }
    private void ProcessBluetoothMessage(string message)
    {
        var messages = message.Split(' ');
        switch (messages[0])
        {
            case "new":
                var myColor = messages[1] == "1" ? PieceColor.Black : PieceColor.Red;
                var fusionModel = (messages[2] == "1");
                var hiddenModel = (messages[3] == "1");
                OnNewOk(myColor, fusionModel, hiddenModel);
                break;
            case "move":
                GameController.MovePiece(messages);
                break;
            case "regret":
                OpenRegretOkDialog();
                break;
            case "regretOk":
                OnRegretOk(GameController.MyColor);
                break;
            case "regretNot":
                new Toast("regret_not").Show();
                break;
            case "resign":
                var notColor = !GameController.MyColor;
                OnResignOk(notColor);
                break;
            case "draw":
                OpenDrawOkDialog();
                break;
            case "drawOk":
                OnDrawOk();
                break;
            case "drawNot":
                new Toast("draw_not").Show();
                break;
            case "disconnect":
                OnDisconnect();
                break;
        }
    }
}
