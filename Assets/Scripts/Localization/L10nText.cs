using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class L10nText : MonoBehaviour {

    public string alias;

    private Text textComponent;

    void Awake()
    {
        Localization.Instance.SubscribeL10nText(this);
        textComponent = GetComponent<Text>();
        UpdateText();
    }
    void OnDestroy()
    {
        Localization.Instance.UnsubcribeL10nText(this);
    }

    public void SetText(string alias)
    {
        this.alias = alias;
        UpdateText();
    }
    public void UpdateText()
    {
        var text = Localization.Instance.GetText(alias);
        textComponent.text = text;
    }
}
