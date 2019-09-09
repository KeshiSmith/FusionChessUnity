using UnityEngine.UI;
using FusionChess;

public class SettingsDialog : AbstractDialog
{
    public Toggle animeToggle, selectToggle;

    private bool animeFlag = true;
    private bool selectFlag = true;

    public bool AnimeFlag
    {
        set
        {
            animeFlag = value;
            UpdateAnimeFlag();
        }
    }
    public bool SelectFlag
    {
        set
        {
            selectFlag = value;
            UpdateSelectFlag();
        }
    }

    void Start()
    {
        LoadConfig();
        InitData();
    }

    public override void OnEscapeDown()
    {
        OnCancelClicked();
    }

    public void OnConfirmClicked()
    {
        var datas = new DataBundle();
        datas.PutData("confirm", true);
        datas.PutData("animeFlag", animeFlag);
        datas.PutData("selectFlag", selectFlag);
        Close(datas);
    }
    public void OnCancelClicked()
    {
        var datas = new DataBundle();
        datas.PutData("confirm", false);
        Close(datas);
    }

    private void InitData()
    {
        animeToggle.isOn = animeFlag;
        selectToggle.isOn = selectFlag;
    }
    private void LoadConfig()
    {
        var config = Config.Instance;
        animeFlag = GameParams.animeFlag;
        selectFlag = GameParams.selectFlag;
    }
    private void UpdateAnimeFlag()
    {
        var config = Config.Instance;
        var animeValue = animeFlag ? "1" : "0";
        config.SetValue("Settings", "AnimeFlag", animeValue);
    }
    private void UpdateSelectFlag()
    {
        var config = Config.Instance;
        var selectValue =  selectFlag? "1" : "0";
        config.SetValue("Settings", "SelectFlag", selectValue);
    }
}
