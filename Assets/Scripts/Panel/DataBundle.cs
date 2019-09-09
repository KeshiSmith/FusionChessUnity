using System.Collections.Generic;

public class DataBundle
{
    private Dictionary<string, object> datas = new Dictionary<string, object>();

    public void ClearData()
    {
        datas.Clear();
    }
    public void RemoveData(string key)
    {
        datas.Remove(key);
    }
    public bool ContainsKey(string key)
    {
        return datas.ContainsKey(key);
    }
    public void PutData(string key, object data = null)
    {
        datas[key] = data;
    }
    public object GetData(string key)
    {
        return datas[key];
    }
}
