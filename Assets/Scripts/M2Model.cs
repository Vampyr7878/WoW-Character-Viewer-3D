using M2Lib;
using UnityEngine;

//Class to contain data from M2 file
public class M2Model : MonoBehaviour
{
    //Binary asset containing the main data
    public TextAsset data;

    //Binary asset containing the skin data
    public TextAsset skin;

    //Binary asset containing the skel data
    public TextAsset skel;

    //Load all the date into the object
    public M2 LoadModel(byte[] dataBytes, byte[] skinBytes, byte[] skelBytes)
    {
        M2 model = new M2();
        model.LoadFile(dataBytes);
        model.Skin.LoadFile(skinBytes);
        model.Skeleton.LoadFile(model.SkelFileID == 0 ? dataBytes : skelBytes, model.SkelFileID);
        return model;
    }
}
