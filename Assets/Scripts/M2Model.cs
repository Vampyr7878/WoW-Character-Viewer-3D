using M2Lib;
using Newtonsoft.Json;
using UnityEngine;

//Class to contain data from M2 file
public class M2Model : MonoBehaviour
{
    //Json asset containing the data
    public TextAsset json;

    //Object containing the date
    public M2 model;

    void Start()
    {
        //Load all the date into the object
        model = JsonConvert.DeserializeObject<M2>(json.text);
    }
}
