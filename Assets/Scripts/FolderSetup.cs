using SimpleFileBrowser;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Handle starter UI
public class FolderSetup : MonoBehaviour
{
    // Reference to the main object handling UI input
    public GameObject screenInput;
    // Reference to the main UI canvas
    public GameObject main;
    // Reference to starter UI canvas
    public GameObject folderInput;
    // Reference to the input field where you put World of Warcraft path
    public InputField path;

    void Start()
    {
        // If config.ini exist read it and check if it contains valid World of Warcraft path; if it does show main UI
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

    // Show main UI
    private void ShowMainCanvas()
    {
        folderInput.SetActive(false);
        main.SetActive(true);
        screenInput.SetActive(true);
    }

    // Open folder browser so you can select your World of Warcraft folder
    public void Browse()
    {
        StartCoroutine(BrowseFolder());
    }

    // Browse for a folder
    private IEnumerator BrowseFolder()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, null, null, "Find your main World of Warcraft folder", "Select");
        path.text = FileBrowser.Success ? FileBrowser.Result[0] : "";
    }

    // If selected path is valid store it in config.ini file and show main UI after pressing OK button
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

    // Pressing Cancel button will close the applicatoin
    public void Cancel()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
