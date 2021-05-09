using M2Lib;
using Newtonsoft.Json;
using UnityEngine;

//Class to contain data from M2 file
public class M2Model : MonoBehaviour
{
    //Json asset containing the data
    public TextAsset json;

    //Load all the date into the object
    public M2 LoadModel(string text)
    {
        return JsonConvert.DeserializeObject<M2>(text);
    }
}
