using BLPLib;
using CASCLib;
using SimpleFileBrowser;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WoW;
using System.Xml.Linq;
using UnityEditor;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections.ObjectModel;

//Class handling main UI input in the scene
public class ScreenInput : MonoBehaviour
{
    //Reference to the camera
    public Camera mainCamera;
    //Reference to prefeb for element on list of items to equip
    public GameObject scrollItem;
    //Reference to prefab that is used to create customization category button
    public GameObject customizationCategory;
    //Reference to prefab that is used to create customization dropdown
    public GameObject customizationDropdown;
    //Reference to prefab that is used to create customization toggle
    public GameObject customizationToggle;
    //Reference to prefab that is used to craete form button
    public GameObject formButton;
    //Reference to the panel containing gender options
    public GameObject genderPanel;
    //Reference to the panel containig Alliance races
    public GameObject alliancePanel;
    //Reference to the panel containing Horde races
    public GameObject hordePanel;
    //Reference to the pane containing class options
    public GameObject classPanel;
    //Reference to teh panel for shapeshift forms
    public GameObject formPanel;
    //Reference to the panel containing all the customization options
    public GameObject customizationPanel;
    //Reference to the panel containing equipment slots
    public GameObject gearPanel;
    //Reference to the panel that lets you choose items on the left side
    public GameObject leftPanel;
    //Reference to the panel that lets you choose items on the right side
    public GameObject rightPanel;
    //Reference to the panel that handle character import options
    public GameObject importPanel;
    //References to all class buttons
    public Button[] classButtons;
    //Reference to the button for main character form
    public Button mainFormButton;
    //Reference to the left bottom button: Exit/Back
    public Button exitButton;
    //Reference to the right bottom button: Customize/Save
    public Button customizeButton;
    //Referenec to the the button that toggles goear panel
    public Button gearButton;
    //Reference to the button that allows opening saved character from file
    public Button openButton;
    //Referene to the button that opens import panel
    public Button importButton;
    //Reference to the "Loading..." text
    public Text loading;
    //Reference to the main character object
    public Character character;
    //Name of item set to equip in autoscreenshot mode
    public string itemSet;
    //Character's gender for start
    public bool gender;
    //Toggle on autoscreenshot mode
    public bool screenshot;
    //Autoscreenshot core races
    public bool core;
    //Autoscreenshot allied races
    public bool allied;
    //Autoscreenshot race
    public string race;

    //List of all customization option dropdowns
    private List<GameObject> customizationOptions;
    //List of all customization option dropdowns
    private List<Button> customizationCategories;
    //List of all instantiated form buttons
    private List<Button> formButtons;
    //Reference to the database connection object
    private SqliteConnection connection;
    //Dictionary mapping id to race name
    private Dictionary<int, string> races;
    //Dictionary mapping id to class name
    private Dictionary<int, string> classes;
    //Dictionary mapping id to druid form model
    private Dictionary<int, string> druidModels;
#if UNITY_EDITOR
    //Listfile dictionary to speed up debugging
    private Dictionary<int, string> listfile;
    //Path to locally unpacked game files
    private string dataPath;
#else
    //Referenece to the opened CASC storage
    private CASCHandler casc;
#endif
    //List of all items for opened slot
    private ScrollItem[] leftScrollItems;
    //List of all items displayed on the right equipment panel
    private ScrollItem[] rightScrollItems;
    //max amount of items in equipment panel
    private int maxScrollItems;
    //Reference to teh ImageConverter object for converting Bitmaps into Textures
    protected System.Drawing.ImageConverter converter;
    //Name for currently oepned equipment slot
    private string slotName;
    //Is UI in ucstomize mode?
    private bool customize;
    //Is gear panel opened?
    private bool gear;
    //This allows to block camera movement when using UI
    private bool translate;
    //This allows to block model rotation when using UI
    private bool rotate;
    //This allows to block camera zoom when using UI
    private bool zoom;
    //Path to the database file
    private string dbPath;
    //API access token
    private string token;
    //List of all the core races names
    private List<string> coreRaceNames;
    //List of all the allied races names
    private List<string> alliedRaceNames;
    //List of all class names
    private List<string> classNames;
    //Index of a current race in autoscreenshot mode
    private int r;
    //Index of a current class in autoscreenshot mode
    private int c;

    private void Start()
    {
        //Open wow CASC storage
#if UNITY_EDITOR
        string listPath;
#endif
        string path;
        using (StreamReader reader = new StreamReader("config.ini"))
        {
            path = reader.ReadLine();
#if UNITY_EDITOR
            listPath = reader.ReadLine();
            dataPath = reader.ReadLine();
#endif
        }
        FileBrowser.SetFilters(true, new FileBrowser.Filter("character(.chr)", ".chr"));
        FileBrowser.SetDefaultFilter(".chr");
#if UNITY_EDITOR
        listfile = new Dictionary<int, string>();
        using (StreamReader reader = new StreamReader($@"{listPath}\listfile.csv"))
        {
            string line;
            string[] tokens;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                tokens = line.Split(';');
                listfile.Add(int.Parse(tokens[0]), tokens[1]);
            }
        }
#else
        casc = CASCHandler.OpenLocalStorage(path, "wow");
        casc.Root.SetFlags(LocaleFlags.enUS, false, false);
#endif
        converter = new System.Drawing.ImageConverter();
        //Allow for translation, rotation and zoom
        translate = true;
        rotate = true;
        zoom = true;
        //Prepare for autoscreemshot mode
        r = core ? 0 : 12;
        c = -1;
        classNames = new List<string> { "Warrior", "Paladin", "Hunter", "Rogue", "Priest", "Shaman", "Mage", "Warlock", "Druid", "Monk", "DeathKnight", "DemonHunter" };
        coreRaceNames = new List<string> { "Human", "Orc", "Dwarf", "Undead", "NightElf", "Tauren", "Gnome", "Troll", "Draenei", "BloodElf", "Worgen", "Goblin" };
        alliedRaceNames = new List<string> { "Tushui", "Huojin", "VoidElf", "Nightborne", "Lightforged", "Highmountain", "DarkIron", "Maghar", "KulTiran", "Zandalari", "Mechagnome", "Vulpera" };
        token = GetToken();
        //Load initialize race, class and druid form dictionaries and load data into them
        races = new Dictionary<int, string>();
        classes = new Dictionary<int, string>();
        druidModels = new Dictionary<int, string>();
        customizationOptions = new List<GameObject>();
        customizationCategories = new List<Button>();
        formButtons = new List<Button>();
        dbPath = $"URI=file:{Application.streamingAssetsPath}/database.sqlite";
        connection = new SqliteConnection(dbPath);
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Races;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                races.Add(reader.GetInt32(0), reader.GetString(1));
            }
        }
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Classes;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                classes.Add(reader.GetInt32(0), reader.GetString(1));
            }
        }
        connection.Close();
        maxScrollItems = 100;
        leftScrollItems = new ScrollItem[maxScrollItems];
        rightScrollItems = new ScrollItem[maxScrollItems];
        for (int i = 0; i < maxScrollItems; i++)
        {
            leftScrollItems[i] = Instantiate(scrollItem, leftPanel.GetComponentInChildren<VerticalLayoutGroup>().transform.transform).GetComponent<ScrollItem>();
            leftScrollItems[i].GetComponent<Toggle>().group = leftPanel.GetComponentInChildren<ToggleGroup>();
            leftScrollItems[i].name = i.ToString();
            leftScrollItems[i].gameObject.SetActive(false);
            rightScrollItems[i] = Instantiate(scrollItem, rightPanel.GetComponentInChildren<VerticalLayoutGroup>().transform.transform).GetComponent<ScrollItem>();
            rightScrollItems[i].GetComponent<Toggle>().group = rightPanel.GetComponentInChildren<ToggleGroup>();
            rightScrollItems[i].name = i.ToString();
            rightScrollItems[i].gameObject.SetActive(false);
        }
        customize = false;
    }

    private void Update()
    {
        //Display "Loading..," text when characrter is being loaded
        if (character.Race > 0)
        {
            loading.gameObject.SetActive(!character.Loaded);
        }
        customizeButton.gameObject.SetActive(!gear);
        //Exit when autoscreenshotting is done
        if (screenshot && ((!allied && r >= coreRaceNames.Count) || r >= coreRaceNames.Count + alliedRaceNames.Count) && race == "")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        //Tranlate, rotate and zoom
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, Mathf.Clamp(mainCamera.transform.position.z, -5f, -0.5f));
        if (!screenshot)
        {
            TranslateCamera();
            RotateCamera();
            ZoomCamera();
        }
        //Select next ui element with Tab key
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
        }
        //Exit with Escape key
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Exit();
        }
        //Screenshot with F12 key
        if (Input.GetKeyUp(KeyCode.F12))
        {
            StartCoroutine(TakeScreenshot());
        }
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        listfile.Clear();
#else
        casc.Clear();
#endif
        GC.Collect();
    }

    //Get API access token
    string GetToken()
    {
        try
        {
            using HttpClient client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "https://us.battle.net/oauth/token");
            TextAsset api = Resources.Load<TextAsset>("API");
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(api.text));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {auth}");
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = client.SendAsync(request).Result;
            string resp = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject access = JObject.Parse(resp);
                return access.Value<string>("access_token");
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}");
            return null;
        }
    }

    //Make screenshot
    private IEnumerator TakeScreenshot()
    {
        if (!Directory.Exists("Screenshots"))
        {
            Directory.CreateDirectory("Screenshots");
        }
        yield return new WaitForEndOfFrame();
        openButton.transform.parent.gameObject.SetActive(false);
        //zzTransparencyCapture.captureScreenshot(@$"Screenshots\{(character.Gender ? "Male" : "Female")}{races[character.Race].Replace(" ", "").Replace("'", "")}{classes[character.Class].Replace(" ", "")}.png");
        openButton.transform.parent.gameObject.SetActive(true);
    }

    //Make autoscreenshot
    private IEnumerator TakeScreenshot(string filename)
    {
        if (!Directory.Exists("Screenshots"))
        {
            Directory.CreateDirectory("Screenshots");
        }
        screenshot = false;
        yield return new WaitForEndOfFrame();
        openButton.transform.parent.gameObject.SetActive(false);
        zzTransparencyCapture.captureScreenshot(filename);
        openButton.transform.parent.gameObject.SetActive(true);
        c++;
        if (c == classNames.Count)
        {
            c = 0;
            r++;
        }
        //string folder = character.Gender ? "Male" : "Female";
        //if (r < coreRaceNames.Count)
        //{
        //    if (c < classNames.Count && File.Exists(@$"Save\{folder}\{itemSet}\{coreRaceNames[r]}\{coreRaceNames[r]}{classNames[c]}.chr"))
        //    {
        //        Open(@$"Save\{folder}\{itemSet}\{coreRaceNames[r]}\{coreRaceNames[r]}{classNames[c]}.chr");
        //    }
        //}
        //else if (allied && r < coreRaceNames.Count + alliedRaceNames.Count)
        //{
        //    if (c < classNames.Count && File.Exists(@$"Save\{folder}\{itemSet}\{alliedRaceNames[r - coreRaceNames.Count]}\{alliedRaceNames[r - coreRaceNames.Count]}{classNames[c]}.chr"))
        //    {
        //        Open(@$"Save\{folder}\{itemSet}\{alliedRaceNames[r - coreRaceNames.Count]}\{alliedRaceNames[r - coreRaceNames.Count]}{classNames[c]}.chr");
        //    }
        //}
        yield return new WaitForSeconds(4);
        screenshot = true;
    }

    //Translate camera
    private void TranslateCamera()
    {
        if (translate && Input.GetMouseButton(1) && !FileBrowser.IsOpen)
        {
            mainCamera.transform.Translate(-Input.GetAxis("Mouse X") / 4f, -Input.GetAxis("Mouse Y") / 4, 0f);
            Vector3 value = mainCamera.transform.position;
            value.x = Mathf.Clamp(value.x, -1.5f, 1.5f);
            value.y = Mathf.Clamp(value.y, -0.5f, 1.5f);
            mainCamera.transform.position = value;
        }
    }

    //Rotate model
    private void RotateCamera()
    {
        if (rotate && Input.GetMouseButton(0) && !FileBrowser.IsOpen)
        {
            character.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10f, 0f);
        }
    }

    //Zoom camare
    private void ZoomCamera()
    {
        if (zoom && !FileBrowser.IsOpen)
        {
            mainCamera.transform.Translate(0f, 0f, Input.GetAxis("Mouse ScrollWheel"));
            Vector3 value = mainCamera.transform.position;
            value.z = Mathf.Clamp(value.z, -3f, -0.25f);
            mainCamera.transform.position = value;
        }
    }

    //Load customization categories and prepare UI for them
    private void SetupCategories()
    {
        foreach (var customizaionCategory in customizationCategories)
        {
            Destroy(customizaionCategory.gameObject);
        }
        customizationCategories.Clear();
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            List<CustomizationCategory> categories = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ChrCustomizationCategory.ID, CategoryName_lang, CustomizeIcon, CustomizeIconSelected FROM " +
                $"ChrCustomizationCategory JOIN ChrCustomizationOption ON ChrCustomizationCategoryID = ChrCustomizationCategory.ID WHERE " +
                $"ChrModelID = {character.ModelID} AND Requirement <> 10 AND Requirement <> 12 GROUP BY CategoryName_lang ORDER BY ChrCustomizationCategory.OrderIndex;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int icon = reader.GetInt32(2);
                    int selected = reader.GetInt32(3);
                    categories.Add(new CustomizationCategory(id, name, icon, selected));
                }
            }
            character.Categories = categories.ToArray();
        }
        connection.Close();
        GameObject uiObject;
        for (int i = 0; i < character.Categories.Length; i++)
        {
            uiObject = Instantiate(customizationCategory, customizationPanel.GetComponentInChildren<HorizontalLayoutGroup>().transform);
            Button button = uiObject.GetComponent<Button>();
            int customizationValue = i;
            button.onClick.AddListener(delegate { Category(customizationValue); });
            EventTrigger trigger = uiObject.GetComponent<EventTrigger>();
            trigger.triggers[0].callback.AddListener(delegate { PointerEnter(); });
            trigger.triggers[1].callback.AddListener(delegate { PointerExit(); });
            customizationCategories.Add(button);
        }
    }

    //Load customization options and prepare UI for them
    private void SetupCustomizationPanel()
    {
        foreach (var customizaionOption in customizationOptions)
        {
            Destroy(customizaionOption);
        }
        customizationOptions.Clear();
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            List<CustomizationOption> options = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT Name_lang, ID, ChrModelID, ChrCustomizationCategoryID, OptionType FROM ChrCustomizationOption WHERE " +
                $"ChrModelID = {character.ModelID} AND Requirement <> 10 AND Requirement <> 12 ORDER BY SecondaryOrderIndex;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    int id = reader.GetInt32(1);
                    int model = reader.GetInt32(2);
                    int category = reader.GetInt32(3);
                    int type = reader.GetInt32(4);
                    options.Add(new CustomizationOption(name, id, model, category, type));
                }
            }
            if (character.Race == 22)
            {
                command.CommandText = $"SELECT Name_lang, ID, ChrModelID, ChrCustomizationCategoryID, OptionType FROM ChrCustomizationOption WHERE " +
                    $"ChrModelID = {(character.Gender ? 1 : 2)} AND Requirement <> 10 AND Requirement <> 12 ORDER BY SecondaryOrderIndex;";
                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int id = reader.GetInt32(1);
                        int model = reader.GetInt32(2);
                        int category = reader.GetInt32(3);
                        int type = reader.GetInt32(4);
                        options.Add(new CustomizationOption(name, id, model, category, type));
                    }
            }
            character.Options = options.ToArray();
            character.Customization = new int[options.Count];
            for (int i = 0; i < character.Customization.Length; i++)
            {
                character.Customization[i] = 0;
            }
        }
        connection.Close();
        GameObject uiObject;
        for (int i = 0; i < character.Options.Length; i++)
        {
            if (character.Options[i].Type == 0)
            {
                uiObject = Instantiate(customizationDropdown, customizationPanel.GetComponentInChildren<VerticalLayoutGroup>().transform);
                Button[] buttons = uiObject.GetComponentsInChildren<Button>();
                int customizationValue = i;
                foreach (var button in buttons)
                {
                    if (button.name.Contains("prev"))
                    {
                        button.onClick.AddListener(delegate { PrevButton(customizationValue); });
                    }
                    else if (button.name.Contains("next"))
                    {
                        button.onClick.AddListener(delegate { NextButton(customizationValue); });
                    }
                }
                Dropdown dropdown = uiObject.GetComponentInChildren<Dropdown>();
                dropdown.onValueChanged.AddListener(delegate { Dropdown(customizationValue); });
            }
            else
            {
                uiObject = Instantiate(customizationToggle, customizationPanel.GetComponentInChildren<VerticalLayoutGroup>().transform);
                int customizationValue = i;
                Toggle toggle = uiObject.GetComponentInChildren<Toggle>();
                toggle.onValueChanged.AddListener(delegate { Toggle(customizationValue); });
                toggle.isOn = false;
            }
            EventTrigger trigger = uiObject.GetComponent<EventTrigger>();
            trigger.triggers[0].callback.AddListener(delegate { PointerEnter(); });
            trigger.triggers[1].callback.AddListener(delegate { PointerExit(); });
            uiObject.GetComponentInChildren<Text>().text = character.Options[i].Name;
            customizationOptions.Add(uiObject);
        }
    }

    //Load customization choices and fill the panel with them
    private void GetCustomizationChoices()
    {
        Sprite sprite = Resources.LoadAll<Sprite>("Icons/charactercreate").Single(s => s.name == "color1");
        connection.Open();
        for (int i = 0; i < character.Options.Length; i++)
        {
            using (SqliteCommand command = connection.CreateCommand())
            {
                List<CustomizationChoice> choices = new();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT Name_lang, ChrCustomizationChoice.ID, ChrCustomizationReqID, SwatchColor_0, SwatchColor_1 FROM " +
                    $"ChrCustomizationChoice JOIN ChrCustomizationReq ON ChrCustomizationReqID = ChrCustomizationReq.ID WHERE ChrCustomizationOptionID = " +
                    $"{character.Options[i].ID} AND ReqType & 1 AND (ClassMask = 0 OR ClassMask & {1 << (character.Class - 1)}) " +
                    $"AND ChrCustomizationReqID <> 3128 AND ChrCustomizationReqID <> 407 ORDER BY UiOrderIndex;";
                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int id = reader.GetInt32(1);
                        int requirement = reader.GetInt32(2);
                        ColorUtility.TryParseHtmlString($"#{reader.GetInt32(3).ToString("X8").Substring(2)}", out Color color1);
                        ColorUtility.TryParseHtmlString($"#{reader.GetInt32(4).ToString("X8").Substring(2)}", out Color color2);
                        choices.Add(new CustomizationChoice(name, id, requirement, color1, color2));
                    }
                character.Options[i].LoadAllChoices(choices);
                character.Options[i].SetChoices(character.Options[i].AllChoices);
            }
            if (character.Options[i].Type == 0)
            {
                Dropdown dropdown = customizationOptions[i].GetComponentInChildren<Dropdown>();
                dropdown.options.Clear();
                for (int j = 0; j < character.Options[i].Choices.Length; j++)
                {
                    string name;
                    if (character.Options[i].Choices[j].Color1 != Color.black)
                    {
                        name = $"{j + 1}:";
                    }
                    else if (string.IsNullOrEmpty(character.Options[i].Choices[j].Name))
                    {
                        name = $"{j + 1}";
                    }
                    else
                    {
                        name = $"{j + 1}: {character.Options[i].Choices[j].Name}";
                    }
                    CustomOptionData data = new(name, character.Options[i].Choices[j].Color1, character.Options[i].Choices[j].Color2, sprite, j);
                    dropdown.options.Add(data);
                }
                ((CustomDropdown)dropdown).RefreshShownValue();
            }
        }
        connection.Close();
        for (int i = 0; i < character.Categories.Length; i++)
        {
            customizationCategories[i].gameObject.SetActive(false);
            for (int j = 0; j < character.Options.Length; j++)
            {
                if (character.Options[j].Category == character.Categories[i].ID && character.Options[j].Choices.Length > 1)
                {
                    customizationCategories[i].gameObject.SetActive(true);
                }
            }
        }
        if (customizationCategories.Where(c => c.gameObject.activeSelf).Count() <= 1)
        {
            customizationCategories[0].gameObject.SetActive(false);
        }
    }

    //Load customization choices and fill the panel with them
    private void GetCustomizationElements()
    {
        connection.Open();
        for (int i = 0; i < character.Options.Length; i++)
        {
            for (int j = 0; j < character.Options[i].Choices.Length; j++)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationGeoset> geosets = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, GeosetType, GeosetID FROM ChrCustomizationElement JOIN " +
                        $"ChrCustomizationGeoset ON ChrCustomizationGeosetID = ChrCustomizationGeoset.ID WHERE " +
                        $"ChrCustomizationChoiceID = {character.Options[i].Choices[j].ID} AND ChrCustomizationGeosetID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int type = reader.GetInt32(1);
                            int id = reader.GetInt32(2);
                            geosets.Add(new CustomizationGeoset(related, type, id));
                        }
                    character.Options[i].Choices[j].LoadGeosets(geosets);
                }
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationGeoset> geosets = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, GeosetType, GeosetID FROM ChrCustomizationElement JOIN " +
                        $"ChrCustomizationSkinnedModel ON ChrCustomizationSkinnedModelID = ChrCustomizationSkinnedModel.ID WHERE " +
                        $"ChrCustomizationChoiceID = {character.Options[i].Choices[j].ID} AND ChrCustomizationSkinnedModelID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int type = reader.GetInt32(1);
                            int id = reader.GetInt32(2);
                            geosets.Add(new CustomizationGeoset(related, type, id));
                        }
                    character.Options[i].Choices[j].LoadSkinnedGeosets(geosets);
                }
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationTexture> textures = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, ChrModelTextureTargetID, FileDataID, UsageType FROM ChrCustomizationElement JOIN " +
                        $"ChrCustomizationMaterial ON ChrCustomizationMAterialID = ChrCustomizationMAterial.ID JOIN TextureFileData ON " +
                        $"ChrCustomizationMAterial.MaterialResourcesID = TextureFileData.MaterialResourcesID WHERE " +
                        $"ChrCustomizationChoiceID = {character.Options[i].Choices[j].ID} AND ChrCustomizationMAterialID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int target = reader.GetInt32(1);
                            int id = reader.GetInt32(2);
                            int usage = reader.GetInt32(3);
                            textures.Add(new CustomizationTexture(related, target, id, usage));
                        }
                    character.Options[i].Choices[j].LoadTextures(textures);
                }
            }
        }
        connection.Close();
    }

    //Create icon from BLP file
    private Sprite IconFromBLP(int file)
    {
        try
        {
#if UNITY_EDITOR
            BLP blp = new($@"{dataPath}\{listfile[file]}");
#else
            BLP blp = new(casc.OpenFile(file));
#endif
            System.Drawing.Bitmap image = blp.GetImage();
            Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, true);
            texture.LoadImage((byte[])converter.ConvertTo(image, typeof(byte[])));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        }
        catch
        {
            Debug.LogWarning($"Can't load icon: {file}");
#if UNITY_EDITOR
            BLP blp = new($@"{dataPath}\{listfile[134400]}");
#else
            BLP blp = new(casc.OpenFile(134400));
#endif
            System.Drawing.Bitmap image = blp.GetImage();
            Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, true);
            texture.LoadImage((byte[])converter.ConvertTo(image, typeof(byte[])));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        }
    }

    //Change colors for ui buttons for hero classes
    private void ChangeButtonColors()
    {
        Color32 color = character.Class switch
        {
            6 => new Color32(0, 128, 255, 255),
            _ => new Color32(192, 0, 0, 255),
        };
        exitButton.GetComponent<Image>().color = color;
        customizeButton.GetComponent<Image>().color = color;
        gearButton.GetComponent<Image>().color = color;
        importButton.GetComponent<Image>().color = color;
        openButton.GetComponent<Image>().color = color;
        foreach (var button in leftPanel.GetComponentsInChildren<Button>())
        {
            button.GetComponent<Image>().color = color;
        }
        foreach (var button in rightPanel.GetComponentsInChildren<Button>())
        {
            button.GetComponent<Image>().color = color;
        }
    }

    //Change circle button's border so only selected one glows
    private void ChangeBorder(GameObject selected)
    {
        if (selected != null)
        {
            GameObject button = selected.transform.parent.parent.gameObject;
            GameObject panel = button.transform.parent.gameObject;
            Image[] images = panel.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                if (image.gameObject.name == "glow")
                {
                    image.gameObject.SetActive(false);
                }
                if (image.gameObject.name == "border")
                {
                    image.gameObject.SetActive(true);
                }
            }
            images = button.GetComponentsInChildren<Image>(true);
            images.Single(i => i.name == "border").gameObject.SetActive(false);
            images.Single(i => i.name == "glow").gameObject.SetActive(true);
        }
    }

    //Setup shadpeshift forms panel
    private void SetupFormPanel()
    {
        foreach (var button in formButtons)
        {
            Destroy(button.transform.parent.parent.gameObject);
        }
        formButtons.Clear();
        if (character.Race == 22)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Icons/charactercreateicons");
            GameObject uiElement = Instantiate(formButton, formPanel.transform);
            uiElement.transform.SetAsFirstSibling();
            Button button = uiElement.GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate { FormButton(10); });
            string icon = character.Gender ? "race_humanmale" : "race_humanfemale";
            Sprite sprite = sprites.Single(s => s.name == icon);
            button.image.sprite = sprite;
            formButtons.Add(button);
        }
        ChangeBorder(mainFormButton.gameObject);
    }

    //Reset round buttons borders
    private void ResetBorder()
    {
        Image[] images = classPanel.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            if (image.gameObject.name == "glow")
            {
                image.gameObject.SetActive(false);
            }
            if (image.gameObject.name == "border")
            {
                image.gameObject.SetActive(true);
            }
        }
        images = classButtons[0].transform.parent.parent.GetComponentsInChildren<Image>(true);
        images.Single(i => i.name == "border").gameObject.SetActive(false);
        images.Single(i => i.name == "glow").gameObject.SetActive(true);
    }

    //Change border for race buttons so only one is glowing
    private void ChangeRaceBorder(GameObject selected)
    {
        if (selected != null)
        {
            GameObject button = selected.transform.parent.parent.gameObject;
            Image[] images = alliancePanel.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                if (image.gameObject.name == "glow")
                {
                    image.gameObject.SetActive(false);
                }
                if (image.gameObject.name == "border")
                {
                    image.gameObject.SetActive(true);
                }
            }
            images = hordePanel.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                if (image.gameObject.name == "glow")
                {
                    image.gameObject.SetActive(false);
                }
                if (image.gameObject.name == "border")
                {
                    image.gameObject.SetActive(true);
                }
            }
            images = button.GetComponentsInChildren<Image>(true);
            images.Single(i => i.name == "border").gameObject.SetActive(false);
            images.Single(i => i.name == "glow").gameObject.SetActive(true);
        }
    }

    //Swap race buttons icons depending on the gender
    private void SwapIcons(bool gender)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Icons/charactercreateicons");
        Button[] buttons = alliancePanel.GetComponentsInChildren<Button>();
        string icon;
        Sprite sprite;
        foreach (var button in buttons)
        {
            icon = button.image.sprite.name.Replace("female", "").Replace("male", "");
            icon += gender ? "male" : "female";
            sprite = sprites.Single(s => s.name == icon);
            button.image.sprite = sprite;
        }
        buttons = hordePanel.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            icon = button.image.sprite.name.Replace("female", "").Replace("male", "");
            icon += gender ? "male" : "female";
            sprite = sprites.Single(s => s.name == icon);
            button.image.sprite = sprite;
        }
        icon = mainFormButton.image.sprite.name.Replace("female", "").Replace("male", "");
        icon += gender ? "male" : "female";
        sprite = sprites.Single(s => s.name == icon);
        mainFormButton.image.sprite = sprite;
    }

    //Link customization dropdowns to customization options stored in Character object
    private void LinkDropdowns()
    {
        character.CustomizationDropdowns.Clear();
        foreach (var option in customizationOptions)
        {
            character.CustomizationDropdowns.Add(option.GetComponentInChildren<CustomDropdown>(true));
        }
    }

    //Link customization dropdowns to customization options stored in Character object
    private void LinkToggles()
    {
        character.CustomizationToggles.Clear();
        foreach (var option in customizationOptions)
        {
            character.CustomizationToggles.Add(option.GetComponentInChildren<Toggle>(true));
        }
    }

    //Set gender and load current race model for that gender
    public void GenderButton(bool gender)
    {
        if (character.Gender == gender)
        {
            return;
        }
        SwapIcons(gender);
        ChangeBorder(EventSystem.current.currentSelectedGameObject);
        character.Gender = gender;
        if (character.Race <= 0)
        {
            return;
        }
        string model, extra, collection;
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = {character.Race} AND Gender = {gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            character.ModelID = reader.GetInt32(0);
            model = reader.GetString(4);
            extra = reader.IsDBNull(5) ? null : reader.GetString(5);
            character.RacePath = reader.GetString(6);
            collection = reader.IsDBNull(7) ? null : reader.GetString(7);
        }
        connection.Close();
        int c = character.Class;
        character.Class = 1;
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        SetupFormPanel();
        LinkDropdowns();
        LinkToggles();
        ChangeButtonColors();
        Category(0);
        ClassButton(c);
        Resources.UnloadUnusedAssets();
        GC.Collect();
#if UNITY_EDITOR
        character.LoadModel(model, extra, collection, listfile, dataPath);
#else
        character.LoadModel(model, extra, collection, casc);
#endif
    }

    //Set Race and load current gender model for that race
    public void RaceButton(int race)
    {
        Button current;
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            current = alliancePanel.GetComponentsInChildren<Button>()[0];
        }
        else
        {
            current = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        }
        mainFormButton.image.sprite = current.image.sprite;
        ChangeRaceBorder(EventSystem.current.currentSelectedGameObject);
        if (character.Race == race)
        {
            return;
        }
        bool value;
        string model, extra, collection;
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceClassCombos WHERE Race = {race};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            for (int i = 0; i < classButtons.Length; i++)
            {
                value = reader.GetBoolean(i + 1);
                classButtons[i].interactable = value;
            }
        }
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = {race} AND Gender = {character.Gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            character.ModelID = reader.GetInt32(0);
            model = reader.GetString(4);
            extra = reader.IsDBNull(5) ? null : reader.GetString(5);
            character.RacePath = reader.GetString(6);
            collection = reader.IsDBNull(7) ? null : reader.GetString(7);
        }
        connection.Close();
        character.Race = race;
        character.Class = 1;
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        SetupFormPanel();
        LinkDropdowns();
        LinkToggles();
        ResetBorder();
        ChangeButtonColors();
        Category(0);
        Resources.UnloadUnusedAssets();
        GC.Collect();
#if UNITY_EDITOR
        character.LoadModel(model, extra, collection, listfile, dataPath);
#else
        character.LoadModel(model, extra, collection, casc);
#endif
    }

    //Toggle different shapeshift form
    public void FormButton(int form)
    {
        if (character.Form == form)
        {
            return;
        }
        if (form == 0)
        {
            ChangeBorder(mainFormButton.gameObject);
        }
        else
        {
            ChangeBorder(EventSystem.current.currentSelectedGameObject);
        }
        character.ChangeForm(form);
        SetupCategories();
        if (customizationCategories.Where(c => c.gameObject.activeSelf).Count() <= 1)
        {
            customizationCategories[0].gameObject.SetActive(false);
        }
        Category(0);
        character.Change = character.Loaded;
    }

    //Set class
    public void ClassButton(int id)
    {
        if (id == character.Class)
        {
            return;
        }
        ChangeBorder(EventSystem.current.currentSelectedGameObject);
        character.Class = id;
        int[] customization = character.Customization;
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        SetupFormPanel();
        LinkDropdowns();
        LinkToggles();
        ChangeButtonColors();
        Category(0);
        for (int i = 0; i < character.Customization.Length; i++)
        {
            if (character.Options[i].Type == 0)
            {
                customization[i] = customization[i] >= character.Options[i].Choices.Length ? character.Options[i].Choices.Length - 1 : customization[i];
                customizationOptions[i].GetComponentInChildren<CustomDropdown>().SetValue(customization[i]);
            }
            else
            {
                customizationOptions[i].GetComponentInChildren<Toggle>().isOn = customization[i] == 1;
            }
        }
        character.Change = true;
    }

    //Select customization category
    public void Category(int index)
    {
        string icon;
        Sprite sprite;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Icons/charactercreate");
        for (int i = 0; i < customizationCategories.Count; i++)
        {
            icon = character.Categories[i].Icon.ToString();
            sprite = sprites.Single(s => s.name == icon);
            customizationCategories[i].image.sprite = sprite;
        }
        icon = character.Categories[index].Selected.ToString();
        sprite = sprites.Single(s => s.name == icon);
        customizationCategories[index].image.sprite = sprite;
        for (int i = 0; i < customizationOptions.Count; i++)
        {
            customizationOptions[i].SetActive(character.Options[i].Category == character.Categories[index].ID &&
                character.Options[i].Choices.Length > 1 && character.Options[i].Model == character.ModelID);
        }
        character.Category = character.Categories[index].ID;
    }

    //Handle dropdown value change
    public void Dropdown(int index)
    {
        CustomDropdown dropdown = customizationOptions[index].GetComponentInChildren<CustomDropdown>();
        dropdown.RefreshShownValue();
        character.Customization[index] = dropdown.GetValue();
        string str = dropdown.captionText.text;
        str = str.Substring(str.IndexOf(':') + 1);
        dropdown.captionText.text = str;
        Text text = dropdown.GetComponentInChildren<Text>();
        TextGenerationSettings settings = new();
        settings.textAnchor = text.alignment;
        settings.pivot = Vector2.zero;
        settings.font = text.font;
        settings.fontSize = text.fontSize;
        settings.fontStyle = text.fontStyle;
        settings.horizontalOverflow = text.horizontalOverflow;
        if (text.cachedTextGenerator.GetPreferredWidth(text.text, settings) > 120f)
        {
            text.alignment = TextAnchor.MiddleLeft;
        }
        else
        {
            text.alignment = TextAnchor.MiddleCenter;
        }
        character.Change = true;
    }

    //Handle toggle value change
    public void Toggle(int index)
    {
        Toggle toggle = customizationOptions[index].GetComponentInChildren<Toggle>();
        character.Customization[index] = toggle.isOn ? 1 : 0;
        character.Change = true;
    }

    //When mouse enters UI element block translation, rotation and zoom
    public void PointerEnter()
    {
        translate = false;
        rotate = false;
        zoom = false;
    }

    //When mouse leaves UI element allow translation, rotation and zoom
    public void PointerExit()
    {
        translate = true;
        rotate = true;
        zoom = true;
    }

    //Select previous customization option in Dropdown
    public void PrevButton(int index)
    {
        Dropdown dropdown = customizationOptions[index].GetComponentInChildren<Dropdown>();
        if (dropdown.value == 0)
        {
            dropdown.value = dropdown.value;
        }
        else
        {
            int value = dropdown.value - 1;
            while (value > -1)
            {
                if (((CustomOptionData)dropdown.options[value]).Interactable)
                {
                    dropdown.value = value;
                    return;
                }
                value--;
            }
            dropdown.value = dropdown.value;
        }
    }

    //Select next customization option in Dropdown
    public void NextButton(int index)
    {
        Dropdown dropdown = customizationOptions[index].GetComponentInChildren<Dropdown>();
        if (dropdown.value == dropdown.options.Count - 1)
        {
            dropdown.value = dropdown.value;
        }
        else
        {
            int value = dropdown.value + 1;
            while (value < dropdown.options.Count)
            {
                if (((CustomOptionData)dropdown.options[value]).Interactable)
                {
                    dropdown.value = value;
                    return;
                }
                value++;
            }
            dropdown.value = dropdown.value;
        }
    }

    //If in customize mode go back, else exit the application
    public void Exit()
    {
        if (!customize)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
        else
        {
            customize = false;
            gear = false;
            genderPanel.SetActive(!customize);
            alliancePanel.SetActive(!customize);
            hordePanel.SetActive(!customize);
            classPanel.SetActive(!customize);
            customizationPanel.SetActive(customize);
            gearButton.gameObject.SetActive(customize);
            formPanel.SetActive(false);
            gearPanel.SetActive(gear);
            FormButton(0);
        }
        exitButton.GetComponentInChildren<Text>().text = customize ? "Back" : "Exit";
        customizeButton.GetComponentInChildren<Text>().text = customize ? "Save" : "Customize";
    }

    //If in customize mode open save file window, else go into customzie mode
    public void Customize()
    {
        if (character.Loaded)
        {
            if (!customize)
            {
                customize = true;
                genderPanel.SetActive(!customize);
                alliancePanel.SetActive(!customize);
                hordePanel.SetActive(!customize);
                classPanel.SetActive(!customize);
                customizationPanel.SetActive(customize);
                gearButton.gameObject.SetActive(customize);
                formPanel.SetActive(formButtons.Count > 0);
            }
            else
            {
                //Save();
            }
        }
        customizeButton.GetComponentInChildren<Text>().text = customize ? "Save" : "Customize";
        exitButton.GetComponentInChildren<Text>().text = customize ? "Back" : "Exit";
    }

    //Show import panel
    public void Import()
    {
        importPanel.SetActive(true);
    }

    //Close import panel without importing with Cancel button
    public void CancelImportButton()
    {
        importPanel.SetActive(false);
        PointerExit();
    }
}
