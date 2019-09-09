using UnityEngine.UI;

public class NewDialog : AbstractDialog
{
    public L10nText choose;
    public Toggle fusionModel, hiddenModel;

    private bool initiative = true;
    private bool fusionFlag = true;
    private bool hiddenFlag = false;

    private bool Initiative
    {
        set
        {
            initiative = value;
            var text = initiative ? "initiative" : "passive";
            choose.SetText(text);
        }
    }

    public bool FusionModel
    {
        set
        {
            fusionFlag = value;
            UpdateFusionModel();
        }
    }
    public bool HiddenModel
    {
        set
        {
            hiddenFlag = value;
            UpdateHiddenModel();
        }
    }

    void Start()
    {
        LoadConfig();
        InitData();
    }

    public override void OnEscapeDown()
    {
        var datas = new DataBundle();
        datas.PutData("confirm", false);
        Close(datas);
    }

    public void SwitchInitiative()
    {
        Initiative = !initiative;
    }
    public void OnConfirmClicked()
    {
        var datas = new DataBundle();
        datas.PutData("confirm", true);
        datas.PutData("initiative", initiative);
        datas.PutData("fusionModel", fusionFlag);
        datas.PutData("hiddenModel", hiddenFlag);
        Close(datas);
    }

    private void InitData()
    {
        Initiative = true;
        fusionModel.isOn = fusionFlag;
        hiddenModel.isOn = hiddenFlag;
    }
    private void LoadConfig()
    {
        var config = Config.Instance;
        var fusionModel = config.GetValue("GameModel", "FusionModel", "1");
        FusionModel = (fusionModel == "1");
        var hiddenModel = config.GetValue("GameModel", "HiddenModel", "0");
        HiddenModel = (hiddenModel == "1");
    }
    private void UpdateFusionModel()
    {
        var config = Config.Instance;
        var fusionModel = fusionFlag ? "1" : "0";
        config.SetValue("GameModel", "FusionModel", fusionModel);
    }
    private void UpdateHiddenModel()
    {
        var config = Config.Instance;
        var hiddenModel = hiddenFlag ? "1" : "0";
        config.SetValue("GameModel", "HiddenModel", hiddenModel);
    }
}
