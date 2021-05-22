using Crosstales.FB;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FolderSetup : MonoBehaviour
{
    public GameObject screenInput;
    public Canvas canvas;
    public Canvas folderPrompt;
    public InputField path;

    void Start()
    {
        if (File.Exists("config.ini"))
        {
            string file;
            using (StreamReader reader = new StreamReader("config.ini"))
            {
                file = $@"{reader.ReadLine()}\_retail_\WoW.exe";
            }
            if (File.Exists(file))
            {
                ShowMainCanvas();
            }
        }
    }

    private void ShowMainCanvas()
    {
        folderPrompt.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        screenInput.SetActive(true);
    }

    public void Browse()
    {
        string file = FileBrowser.OpenSingleFolder("Find your main World of Warcraft folder");
        path.text = file;
    }

    public void Ok()
    {
        string file = $@"{path.text}\_retail_\WoW.exe";
        if (File.Exists(file))
        {
            using (StreamWriter writer = new StreamWriter("config.ini"))
            {
                writer.WriteLine(path.text);
            }
            ShowMainCanvas();
        }
    }

    public void Cancel()
    {
        #if UNITY_EDITOR
             UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
