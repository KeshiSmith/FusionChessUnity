using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PanelData
{
    public Transform panelBox;
    public GameObject[] panelPrefabs;
    public string defaultPanelName;
}
[System.Serializable]
public class DialogData
{
    public Transform dialogBox;
    public GameObject dialogBlock;
    public GameObject[] dialogPrefabs;
}
[System.Serializable]
public class PrefabData
{
    public GameObject toast;
    public GameObject messageDialog;
}

public class PanelManager : MonoBehaviour {

    public static PanelManager Instance { get; private set; }

    public PanelData panelData;
    public DialogData dialogData;
    public PrefabData prefabData;

    private Dictionary<string, GameObject> panelPrefabDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> dialogPrefabDict = new Dictionary<string, GameObject>();
    private Stack<GameObject> panelStack = new Stack<GameObject>();

    private GameObject activeDialog = null;
    private bool ModelFlag { get; set; }

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        LoadPanels();
        LoadDialogs();
        LoadDefaultPanel();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var panelObj = activeDialog != null ? activeDialog : panelStack.Peek();
            var panelScript = panelObj.GetComponent<AbstractPanelBase>();
            panelScript.OnEscapeDown();
        }
    }

    public void StartPanel(string name, int? resultCode = null, DataBundle datas = null)
    {
        panelStack.Peek().SetActive(false);
        var activePanel = InstantiatePanel(name);
        panelStack.Push(activePanel);
        activePanel.SetActive(true);

        var activePanelScript = activePanel.GetComponent<AbstractPanel>();
        activePanelScript.InitResultId(resultCode);
        activePanelScript.InitPanel(datas);
    }
    public void FinishPanel(int? id = null, DataBundle datas = null)
    {
        if (panelStack.Count > 1)
        {
            var topPanel = panelStack.Pop();
            Destroy(topPanel);
            var activePanel = panelStack.Peek();
            activePanel.SetActive(true);

            var activePanelScript = activePanel.GetComponent<AbstractPanel>();
            activePanelScript.OnPanelResult(id, datas);
        }
    }

    public void OpenDialog(DialogBase dialog)
    {
        if (activeDialog == null)
        {
            dialogData.dialogBlock.SetActive(dialog.BlockFlag);
            ModelFlag = dialog.ModelFlag;

            activeDialog = InstantiateDialog(dialog.Name);
            activeDialog.SetActive(true);

            var activeDialogScript = activeDialog.GetComponent<AbstractDialog>();
            activeDialogScript.InitResultId(dialog.ResultCode);
            activeDialogScript.InitDialog(dialog.Datas);
        }
    }
    public void CloseDialog(int? id = null, DataBundle datas = null)
    {
        if (activeDialog != null)
        {
            Destroy(activeDialog);
            dialogData.dialogBlock.SetActive(false);
            activeDialog = null;

            var activePanel = panelStack.Peek();
            var activePanelScript = activePanel.GetComponent<AbstractPanel>();
            activePanelScript.OnPanelResult(id, datas);
        }
    }
    public void OpenOnceDialog(DialogBase dialog)
    {
        var activeDialog = InstantiateDialog(dialog.Name);
        activeDialog.SetActive(true);

        var activeDialogScript = activeDialog.GetComponent<AbstractDialog>();
        activeDialogScript.InitDialog(dialog.Datas);
    }

    public void OnDialogBlockClicked()
    {
        if (!ModelFlag)
            CloseDialog();
    }

    private void LoadPanels()
    {
        foreach (var panel in panelData.panelPrefabs)
            panelPrefabDict[panel.name] = panel;
    }
    private void LoadDialogs()
    {
        foreach (var dialog in dialogData.dialogPrefabs)
            dialogPrefabDict[dialog.name] = dialog;
        dialogPrefabDict["_MessageDialog"] = prefabData.messageDialog;
        dialogPrefabDict["_Toast"] = prefabData.toast;
    }
    private void LoadDefaultPanel()
    {
        var defaultPanel = InstantiatePanel(panelData.defaultPanelName);
        panelStack.Push(defaultPanel);
        defaultPanel.SetActive(true);
    }
    private GameObject Instantiate(string name, Dictionary<string, GameObject> prefabs, Transform parent)
    {
        var prefab = prefabs[name];
        var obj = Instantiate(prefab, parent);
        return obj;
    }
    private GameObject InstantiatePanel(string name)
    {
        return Instantiate(name, panelPrefabDict, panelData.panelBox);
    }
    private GameObject InstantiateDialog(string name)
    {
        return Instantiate(name, dialogPrefabDict, dialogData.dialogBox);
    }
}
