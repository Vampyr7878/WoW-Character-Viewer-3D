using CASCLib;
using SimpleFileBrowser;
using Mono.Data.Sqlite;
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
using Assets.WoW;
using Newtonsoft.Json;
using Serializable;
using System.Net;
using static UnityEditor.Progress;
using NUnit.Framework.Internal.Filters;

// Class handling main UI input in the scene
public class ScreenInput : MonoBehaviour
{
    // Reference to the camera
    public Camera mainCamera;
    // Reference to prefeb for element on list of items to equip
    public GameObject scrollItem;
    // Reference to prefab that is used to create customization category button
    public GameObject customizationCategory;
    // Reference to prefab that is used to create customization dropdown
    public GameObject customizationDropdown;
    // Reference to prefab that is used to create customization toggle
    public GameObject customizationToggle;
    // Reference to prefab that is used to craete form button
    public GameObject formButton;
    // Reference to the panel containing gender options
    public GameObject genderPanel;
    // Reference to the panel containig Alliance races
    public GameObject alliancePanel;
    // Reference to the panel containing Horde races
    public GameObject hordePanel;
    // Reference to the pane containing class options
    public GameObject classPanel;
    // Reference to teh panel for shapeshift forms
    public GameObject formPanel;
    // Reference to the panel containing all the customization options
    public GameObject customizationPanel;
    // Reference to the panel containing equipment slots
    public GameObject gearPanel;
    // Reference to the panel that lets you choose items on the left side
    public GameObject leftPanel;
    // Reference to the panel that lets you choose items on the right side
    public GameObject rightPanel;
    // Reference to the panel that handle character import options
    public GameObject importPanel;
    // References to all class buttons
    public Button[] classButtons;
    // Reference to the button for main character form
    public Button mainFormButton;
    // Reference to the left bottom button: Exit/Back
    public Button exitButton;
    // Reference to the right bottom button: Customize/Save
    public Button customizeButton;
    // Referenec to the the button that toggles goear panel
    public Button gearButton;
    // Reference to the button that allows opening saved character from file
    public Button openButton;
    // Referene to the button that opens import panel
    public Button importButton;
    // Reference to the "Loading..." text
    public Text loading;
    // Reference to the main character object
    public Character character;
    // Reference to the main creature object
    public Creature creature;
    // Name of item set to equip in autoscreenshot mode
    public string itemSet;
    // Character's gender for start
    public bool gender;
    // Toggle on autoscreenshot mode
    public bool screenshot;
    // Autoscreenshot core races
    public bool core;
    // Autoscreenshot allied races
    public bool allied;
    // Autoscreenshot race
    public string race;

    // List of all customization option dropdowns
    private List<GameObject> customizationOptions;
    // List of all customization option dropdowns
    private List<Button> customizationCategories;
    // List of all instantiated form buttons
    private List<Button> formButtons;
    // Reference to the database connection object
    private SqliteConnection connection;
    // Dictionary mapping id to race name
    private Dictionary<int, string> races;
    // Dictionary mapping id to class name
    private Dictionary<int, string> classes;
    // Dictionary mapping id to druid form model
    private Dictionary<int, string> druidModels;
#if UNITY_EDITOR
    // Listfile dictionary to speed up debugging
    private Dictionary<int, string> listFile;
    // Path to locally unpacked game files
    private string dataPath;
#else
    // Referenece to the opened CASC storage
    private CASCHandler casc;
#endif
    // List of all items for opened slot
    private ScrollItem[] leftScrollItems;
    // List of all items displayed on the right equipment panel
    private ScrollItem[] rightScrollItems;
    // max amount of items in equipment panel
    private int maxScrollItems;
    // Reference to teh ImageConverter object for converting Bitmaps into Textures
    protected System.Drawing.ImageConverter converter;
    // Name for currently oepned equipment slot
    private string slotName;
    // Is UI in ucstomize mode?
    private bool customize;
    // Is gear panel opened?
    private bool gear;
    // This allows to block camera movement when using UI
    private bool translate;
    // This allows to block model rotation when using UI
    private bool rotate;
    // This allows to block camera zoom when using UI
    private bool zoom;
    // Path to the database file
    private string dbPath;
    // API access token
    private string token;
    // List of all the core races names
    private List<string> coreRaceNames;
    // List of all the allied races names
    private List<string> alliedRaceNames;
    // List of all class names
    private List<string> classNames;
    // Index of a current race in autoscreenshot mode
    private int r;
    // Index of a current class in autoscreenshot mode
    private int c;

    private void Start()
    {
        // Open wow CASC storage
#if UNITY_EDITOR
        string listPath;
#endif
        string path;
        using (StreamReader reader = new("config.ini"))
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
        listFile = new Dictionary<int, string>();
        using (StreamReader reader = new($@"{listPath}\listFile.csv"))
        {
            string line;
            string[] tokens;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                tokens = line.Split(';');
                listFile.Add(int.Parse(tokens[0]), tokens[1]);
            }
        }
#else
        casc = CASCHandler.OpenLocalStorage(path, "wow");
        casc.Root.SetFlags(LocaleFlags.enUS, false, false);
#endif
        converter = new System.Drawing.ImageConverter();
        // Allow for translation, rotation and zoom
        translate = true;
        rotate = true;
        zoom = true;
        // Prepare for autoscreemshot mode
        r = core ? 0 : 12;
        c = -1;
        classNames = new List<string> { "Warrior", "Paladin", "Hunter", "Rogue", "Priest", "Shaman",
            "Mage", "Warlock", "Druid", "Monk", "DeathKnight", "DemonHunter", "Evoker" };
        coreRaceNames = new List<string> { "Human", "Orc", "Dwarf", "Undead", "NightElf", "Tauren", "Gnome",
            "Troll", "Draenei", "BloodElf", "Worgen", "Goblin", "BlueDracthyr", "RedDracthyr" };
        alliedRaceNames = new List<string> { "Tushui", "Huojin", "VoidElf", "Nightborne", "Lightforged", "Highmountain",
            "DarkIron", "Maghar", "KulTiran", "Zandalari", "Mechagnome", "Vulpera", "AzureEarthen", "RubyEarthen" };
        // Load initialize race, class and druid form dictionaries and load data into them
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
            leftScrollItems[i] = Instantiate(scrollItem, leftPanel.GetComponentInChildren<VerticalLayoutGroup>().
                transform.transform).GetComponent<ScrollItem>();
            leftScrollItems[i].GetComponent<Toggle>().group = leftPanel.GetComponentInChildren<ToggleGroup>();
            leftScrollItems[i].name = i.ToString();
            leftScrollItems[i].gameObject.SetActive(false);
            rightScrollItems[i] = Instantiate(scrollItem, rightPanel.GetComponentInChildren<VerticalLayoutGroup>().
                transform.transform).GetComponent<ScrollItem>();
            rightScrollItems[i].GetComponent<Toggle>().group = rightPanel.GetComponentInChildren<ToggleGroup>();
            rightScrollItems[i].name = i.ToString();
            rightScrollItems[i].gameObject.SetActive(false);
        }
        customize = false;
    }

    private void Update()
    {
        // Display "Loading..," text when characrter is being loaded
        if (character.Race > 0)
        {
            loading.gameObject.SetActive(!character.Loaded);
        }
        customizeButton.gameObject.SetActive(!gear);
        // Exit when autoscreenshotting is done
        if (screenshot && ((!allied && r >= coreRaceNames.Count) || r >= coreRaceNames.Count + alliedRaceNames.Count) && race == "")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        // Tranlate, rotate and zoom
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y,
            Mathf.Clamp(mainCamera.transform.position.z, -5f, -0.5f));
        if (!screenshot)
        {
            TranslateCamera();
            RotateCamera();
            ZoomCamera();
        }
        // Select next ui element with Tab key
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
        }
        // Exit with Escape key
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Exit();
        }
        // Screenshot with F12 key
        if (Input.GetKeyUp(KeyCode.F12))
        {
            StartCoroutine(TakeScreenshot());
        }
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        listFile.Clear();
#else
        casc.Clear();
#endif
        GC.Collect();
    }

    // Get API access token
    private string GetToken()
    {
        try
        {
            using HttpClient client = new();
            using HttpRequestMessage request = new(new HttpMethod("POST"), "https://us.battle.net/oauth/token");
            TextAsset api = Resources.Load<TextAsset>("API");
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(api.text));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {auth}");
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = client.SendAsync(request).Result;
            string resp = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.OK)
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

    // Make screenshot
    private IEnumerator TakeScreenshot()
    {
        if (!Directory.Exists("Screenshots"))
        {
            Directory.CreateDirectory("Screenshots");
        }
        yield return new WaitForEndOfFrame();
        openButton.transform.parent.gameObject.SetActive(false);
        // zzTransparencyCapture.captureScreenshot(@$"Screenshots\{(character.Gender ? "Male" :
        // "Female")}{races[character.Race].Replace(" ", "").Replace("'", "")}{classes[character.Class].Replace(" ", "")}.png");
        openButton.transform.parent.gameObject.SetActive(true);
    }

    // Make autoscreenshot
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
        // string folder = character.Gender ? "Male" : "Female";
        // if (r < coreRaceNames.Count)
        // {
        //     if (c < classNames.Count &&
        //     File.Exists(@$"Save\{folder}\{itemSet}\{coreRaceNames[r]}\{coreRaceNames[r]}{classNames[c]}.chr"))
        //     {
        //         Open(@$"Save\{folder}\{itemSet}\{coreRaceNames[r]}\{coreRaceNames[r]}{classNames[c]}.chr");
        //     }
        // }
        // else if (allied && r < coreRaceNames.Count + alliedRaceNames.Count)
        // {
        //     if (c < classNames.Count &&
        //     File.Exists(@$"Save\{folder}\{itemSet}\{alliedRaceNames[r - coreRaceNames.Count]}\{alliedRaceNames[r - coreRaceNames.Count]}{classNames[c]}.chr"))
        //     {
        //         Open(@$"Save\{folder}\{itemSet}\{alliedRaceNames[r - coreRaceNames.Count]}\{alliedRaceNames[r - coreRaceNames.Count]}{classNames[c]}.chr");
        //     }
        // }
        yield return new WaitForSeconds(4);
        screenshot = true;
    }

    // Translate camera
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

    // Rotate model
    private void RotateCamera()
    {
        if (rotate && Input.GetMouseButton(0) && !FileBrowser.IsOpen)
        {
            character.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10f, 0f);
            creature.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10f, 0f);
        }
    }

    // Zoom camare
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

    // Load customization categories and prepare UI for them
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
                $"ChrModelID = {character.ModelID} AND Requirement <> 10 AND Requirement <> 12 " +
                $"GROUP BY CategoryName_lang ORDER BY ChrCustomizationCategory.OrderIndex;";
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

    // Load customization options and prepare UI for them
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
                $"ChrModelID = {character.MainFormID} AND Requirement <> 10 AND Requirement <> 12 ORDER BY SecondaryOrderIndex;";
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
            foreach (var form in character.CreatureForms)
            {
                command.CommandText = $"SELECT Name_lang, ID, ChrModelID, ChrCustomizationCategoryID, OptionType FROM ChrCustomizationOption WHERE " +
                    $"ChrModelID = {form.ID} AND Requirement <> 10 AND Requirement <> 12 ORDER BY SecondaryOrderIndex;";
                using SqliteDataReader reader = command.ExecuteReader();
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
        }
        connection.Close();
        GameObject uiObject;
        for (int i = 0; i < character.Options.Length; i++)
        {
            if (character.Options[i].Type == WoWHelper.CustomizationType.Dropdown)
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

    // Load customization choices and fill the panel with them
    private void GetCustomizationChoices()
    {
        Sprite sprite = Resources.LoadAll<Sprite>("Icons/charactercreate").Single(s => s.name == "color1");
        character.Customization = new int[character.Options.Length];
        connection.Open();
        for (int i = 0; i < character.Options.Length; i++)
        {
            using (SqliteCommand command = connection.CreateCommand())
            {
                Dictionary<int, CustomizationChoice> choices = new();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT Name_lang, ChrCustomizationChoice.ID, ChrCustomizationReqID, SwatchColor_0, SwatchColor_1 FROM " +
                    $"ChrCustomizationChoice JOIN ChrCustomizationReq ON ChrCustomizationReqID = ChrCustomizationReq.ID WHERE ChrCustomizationOptionID = " +
                    $"{character.Options[i].ID} AND ReqType & 1 AND (ClassMask = 0 OR ClassMask & {1 << ((int)character.Class - 1)}) " +
                    $"AND ChrCustomizationReqID <> 3128 AND ChrCustomizationReqID <> 407 ORDER BY UiOrderIndex;";
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int id = reader.GetInt32(1);
                        int requirement = reader.GetInt32(2);
                        ColorUtility.TryParseHtmlString($"#{reader.GetInt32(3).ToString("X8").Substring(2)}", out Color color1);
                        ColorUtility.TryParseHtmlString($"#{reader.GetInt32(4).ToString("X8").Substring(2)}", out Color color2);
                        choices.Add(id, new CustomizationChoice(name, id, requirement, color1, color2));
                    }
                }
                character.Options[i].LoadAllChoices(choices);
                character.Options[i].SetChoices(character.Options[i].AllChoices);
            }
            if (character.Options[i].Type == WoWHelper.CustomizationType.Dropdown)
            {
                Dropdown dropdown = customizationOptions[i].GetComponentInChildren<Dropdown>();
                dropdown.options.Clear();
                int j = 0;
                foreach(var choice in character.Options[i].Choices)
                {
                    string name;
                    if (choice.Value.Color1 != Color.black)
                    {
                        name = $"{j + 1}:";
                    }
                    else if (string.IsNullOrEmpty(choice.Value.Name))
                    {
                        name = $"{j + 1}";
                    }
                    else
                    {
                        name = $"{j + 1}: {choice.Value.Name}";
                    }
                    CustomOptionData data = new(name, choice.Value.Color1, choice.Value.Color2, sprite, j, choice.Key);
                    dropdown.options.Add(data);
                    j++;
                }
                ((CustomDropdown)dropdown).RefreshShownValue();
            }
            character.Customization[i] = character.Options[i].Choices.First().Key;
        }
        connection.Close();
        for (int i = 0; i < character.Categories.Length; i++)
        {
            customizationCategories[i].gameObject.SetActive(false);
            for (int j = 0; j < character.Options.Length; j++)
            {
                if (character.Options[j].Category == character.Categories[i].ID && character.Options[j].Choices.Count > 1)
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

    // Load customization choices and fill the panel with them
    private void GetCustomizationElements()
    {
        connection.Open();
        foreach (var option in character.Options)
        {
            foreach (var choice in option.Choices)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationGeoset> geosets = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, GeosetType, GeosetID FROM ChrCustomizationElement JOIN " +
                        $"ChrCustomizationGeoset ON ChrCustomizationGeosetID = ChrCustomizationGeoset.ID WHERE " +
                        $"ChrCustomizationChoiceID = {choice.Value.ID} AND ChrCustomizationGeosetID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int type = reader.GetInt32(1);
                            int id = reader.GetInt32(2);
                            geosets.Add(new CustomizationGeoset(related, type, id));
                        }
                        choice.Value.LoadGeosets(geosets);
                    }
                }
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationGeoset> geosets = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, GeosetType, GeosetID FROM ChrCustomizationElement JOIN " +
                        $"ChrCustomizationSkinnedModel ON ChrCustomizationSkinnedModelID = ChrCustomizationSkinnedModel.ID WHERE " +
                        $"ChrCustomizationChoiceID = {choice.Value.ID} AND ChrCustomizationSkinnedModelID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int type = reader.GetInt32(1);
                            int id = reader.GetInt32(2);
                            geosets.Add(new CustomizationGeoset(related, type, id));
                        }
                    }
                    choice.Value.LoadSkinnedGeosets(geosets);
                }
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationTexture> textures = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, ChrModelTextureTargetID, FileDataID, UsageType FROM ChrCustomizationElement JOIN " +
                        $"ChrCustomizationMaterial ON ChrCustomizationMAterialID = ChrCustomizationMAterial.ID JOIN TextureFileData ON " +
                        $"ChrCustomizationMAterial.MaterialResourcesID = TextureFileData.MaterialResourcesID WHERE " +
                        $"ChrCustomizationChoiceID = {choice.Value.ID} AND ChrCustomizationMAterialID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int target = reader.GetInt32(1);
                            int id = reader.GetInt32(2);
                            int usage = reader.GetInt32(3);
                            textures.Add(new CustomizationTexture(related, target, id, usage));
                        }
                    }
                    choice.Value.LoadTextures(textures);
                }
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationDisplayInfo> creatures = new();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, CreatureDisplayInfo.ID, Path, CreatureDisplayInfo.ParticleColorID," +
                        $" TextureVariationFileDataID_0, TextureVariationFileDataID_1, TextureVariationFileDataID_2, TextureVariationFileDataID_3 FROM " +
                        $"ChrCustomizationElement JOIN ChrCustomizationDisplayInfo ON ChrCustomizationDisplayInfoID = ChrCustomizationDisplayInfo.ID JOIN " +
                        $"CreatureDisplayInfo ON CreatureDisplayInfoID = CreatureDisplayInfo.ID JOIN CreatureModels ON ModelID = CreatureModels.ID WHERE " +
                        $"ChrCustomizationChoiceID = {choice.Value.ID} AND ChrCustomizationDisplayInfoID > 0;";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int related = reader.GetInt32(0);
                            int id = reader.GetInt32(1);
                            string model = reader.GetString(2);
                            int particle = reader.GetInt32(3);
                            int texture0 = reader.GetInt32(4);
                            int texture1 = reader.GetInt32(5);
                            int texture2 = reader.GetInt32(6);
                            int texture3 = reader.GetInt32(7);
                            creatures.Add(new(related, id, model, particle, texture0, texture1, texture2, texture3));
                        }
                    }
                    choice.Value.LoadCreature(creatures);
                }
                if (choice.Value.Creatures.Length > 0)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        List<CustomizationGeoset> geosets = new();
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"SELECT GeosetIndex, GeosetValue FROM CreatureDisplayInfoGeosetData WHERE" +
                            $" CreatureDisplayInfoID = {choice.Value.Creatures[0].ID};";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int type = reader.GetInt32(0);
                                int id = reader.GetInt32(1);
                                geosets.Add(new(-1, type + 1, id));
                            }
                        }
                        choice.Value.Creatures[0].LoadGeosets(geosets);
                    }
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        List<ParticleColor> colors = new();
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"SELECT Start_0, MID_0, End_0, Start_1, MID_1, End_1, Start_2, MID_2, End_2 " +
                            $"FROM ParticleColor WHERE ID = {choice.Value.Creatures[0].ParticleColor};";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int start = reader.GetInt32(0);
                                int mid = reader.GetInt32(1);
                                int end = reader.GetInt32(2);
                                colors.Add(new ParticleColor(start, mid, end));
                                start = reader.GetInt32(3);
                                mid = reader.GetInt32(4);
                                end = reader.GetInt32(5);
                                colors.Add(new ParticleColor(start, mid, end));
                                start = reader.GetInt32(6);
                                mid = reader.GetInt32(7);
                                end = reader.GetInt32(8);
                                colors.Add(new ParticleColor(start, mid, end));
                            }
                        }
                        choice.Value.Creatures[0].LoadParticleColors(colors);
                    }
                }
            }
        }
        connection.Close();
    }

    // Create icon from BLP file
    private Sprite IconFromBLP(int file)
    {
        try
        {
#if UNITY_EDITOR
            BLP blp = new($@"{dataPath}\{listFile[file]}");
#else
            BLP blp = new(casc.OpenFile(file));
#endif
            Texture2D texture = blp.GetImage();
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        }
        catch
        {
            Debug.LogWarning($"Can't load icon: {file}");
#if UNITY_EDITOR
            BLP blp = new($@"{dataPath}\{listFile[134400]}");
#else
            BLP blp = new(casc.OpenFile(134400));
#endif
            Texture2D texture = blp.GetImage();
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        }
    }

    // Change colors for ui buttons for hero classes
    private void ChangeButtonColors()
    {
        Color32 color = character.Class switch
        {
            WoWHelper.Class.DeathKnight => new Color32(0, 128, 255, 255),
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

    // Change circle button's border so only selected one glows
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

    // Setup shadpeshift forms panel
    private void SetupFormPanel()
    {
        foreach (var button in formButtons)
        {
            Destroy(button.transform.parent.parent.gameObject);
        }
        formButtons.Clear();
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            List<CreatureForm> forms = new();
            if (character.Race == WoWHelper.Race.Worgen)
            {
                forms.Add(new CreatureForm(character.Gender ? 1 : 2, "Gilnean", character.Gender ? "race_humanmale" : "race_humanfemale", false));
            }
            else if (character.Race == WoWHelper.Race.Dracthyr)
            {
                forms.Add(new CreatureForm(character.Gender ? 127 : 128, "Visage", character.Gender ? "race_visagemale" : "race_visagefemale", false));
            }
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ID, Name, Icon FROM CreatureForms WHERE Class = {(int)character.Class} ORDER BY OrderIndex;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string icon = reader.GetInt32(2).ToString();
                    forms.Add(new CreatureForm(id, name, icon));
                }
            }
            character.CreatureForms = forms.ToArray();
        }
        connection.Close();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Icons/charactercreateicons");
        for (int i = 0; i < character.CreatureForms.Length; i++)
        {
            int formIndex = i + 1;
            GameObject uiElement = Instantiate(formButton, formPanel.transform);
            uiElement.transform.SetAsFirstSibling();
            Button button = uiElement.GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate { FormButton(formIndex); });
            Sprite sprite = sprites.Single(s => s.name == character.CreatureForms[i].Icon);
            button.image.sprite = sprite;
            button.transform.localScale = new Vector3(character.CreatureForms[i].Flip ? -1f : 1f, 1f, 1f);
            formButtons.Add(button);
        }
        ChangeBorder(mainFormButton.gameObject);
    }

    // Reset round buttons borders
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

    // Change border for race buttons so only one is glowing
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

    // Swap race buttons icons depending on the gender
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

    // Link customization dropdowns to customization options stored in Character object
    private void LinkDropdowns()
    {
        character.CustomizationDropdowns.Clear();
        foreach (var option in customizationOptions)
        {
            character.CustomizationDropdowns.Add(option.GetComponentInChildren<CustomDropdown>(true));
        }
    }

    // Link customization dropdowns to customization options stored in Character object
    private void LinkToggles()
    {
        character.CustomizationToggles.Clear();
        foreach (var option in customizationOptions)
        {
            character.CustomizationToggles.Add(option.GetComponentInChildren<Toggle>(true));
        }
    }

    // Link customization categories to customization options stored in Character object
    private void LinkCategories()
    {
        character.CustomizationCategories.Clear();
        foreach (var category in customizationCategories)
        {
            character.CustomizationCategories.Add(category);
        }
    }

    // Set gender and load current race model for that gender
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
        string model, extra, collection, armor, extraCollection;
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = {(int)character.Race} AND Gender = {character.Race == WoWHelper.Race.Dracthyr || gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            character.ModelID = character.MainFormID = reader.GetInt32(0);
            model = reader.GetString(4);
            if (character.Race == WoWHelper.Race.Dracthyr)
            {
                extra = null;
                armor = reader.GetString(5);
            }
            else
            {
                extra = reader.IsDBNull(5) ? null : reader.GetString(5);
                armor = null;
            }
            character.RacePath = reader.GetString(6);
            collection = reader.IsDBNull(7) ? null : reader.GetString(7);
        }
        if (character.Race == WoWHelper.Race.Dracthyr)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = 75 AND Gender = {gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            extra = reader.GetString(4);
            character.ExtraPath = reader.GetString(6);
            extraCollection = reader.GetString(7);
        }
        else
        {
            extraCollection = null;
        }
        connection.Close();
        WoWHelper.Class c = character.Class;
        character.Class = WoWHelper.Class.Warrior;
        SetupFormPanel();
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        LinkDropdowns();
        LinkToggles();
        LinkCategories();
        ChangeButtonColors();
        Category(0);
        ClassButton((int)c);
        Resources.UnloadUnusedAssets();
        GC.Collect();
#if UNITY_EDITOR
        character.LoadModel(model, extra, collection, armor, extraCollection, listFile, dataPath);
#else
        character.LoadModel(model, extra, collection, armor, extraCollection, casc);
#endif
    }

    // Set Race and load current gender model for that race
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
        if ((int)character.Race == race)
        {
            return;
        }
        bool value;
        string model, extra, collection, armor, extraCollection;
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
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = {race} AND Gender = {race == (int)WoWHelper.Race.Dracthyr || character.Gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            character.ModelID = character.MainFormID = reader.GetInt32(0);
            model = reader.GetString(4);
            if (race == (int)WoWHelper.Race.Dracthyr)
            {
                extra = null;
                armor = reader.GetString(5);
            }
            else
            {
                extra = reader.IsDBNull(5) ? null : reader.GetString(5);
                armor = null;
            }
            character.RacePath = reader.GetString(6);
            collection = reader.IsDBNull(7) ? null : reader.GetString(7);
        }
        if (race == (int)WoWHelper.Race.Dracthyr)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = 75 AND Gender = {character.Gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            extra = reader.GetString(4);
            character.ExtraPath = reader.GetString(6);
            extraCollection = reader.GetString(7);
        }
        else
        {
            character.ExtraPath = "";
            extraCollection = null;
        }
        connection.Close();
        character.Race = (WoWHelper.Race)race;
        character.Class = WoWHelper.Class.Warrior;
        SetupFormPanel();
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        LinkDropdowns();
        LinkToggles();
        LinkCategories();
        ResetBorder();
        ChangeButtonColors();
        Category(0);
        Resources.UnloadUnusedAssets();
        GC.Collect();
#if UNITY_EDITOR
        character.LoadModel(model, extra, collection, armor, extraCollection, listFile, dataPath);
#else
        character.LoadModel(model, extra, collection, armor, extraCollection, casc);
#endif
    }

    // Toggle different shapeshift form
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
        LinkCategories();
        if (customizationCategories.Where(c => c.gameObject.activeSelf).Count() <= 1)
        {
            customizationCategories[0].gameObject.SetActive(false);
        }
        Category(0);
        character.Change = character.Loaded;
    }

    // Set class
    public void ClassButton(int id)
    {
        if (id == (int)character.Class)
        {
            return;
        }
        ChangeBorder(EventSystem.current.currentSelectedGameObject);
        character.Class = (WoWHelper.Class)id;
        int[] customization = character.Customization;
        SetupFormPanel();
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        LinkDropdowns();
        LinkToggles();
        LinkCategories();
        ChangeButtonColors();
        Category(0);
        for (int i = 0; i < character.Customization.Length; i++)
        {
            if (customization.Length <= i)
            {
                break;
            }
            if (character.Options[i].Type == WoWHelper.CustomizationType.Dropdown)
            {
                customization[i] = character.Options[i].Choices.ContainsKey(customization[i]) ? customization[i] : character.Options[i].Choices.First().Key;
                customizationOptions[i].GetComponentInChildren<CustomDropdown>().SetValue(customization[i]);
            }
            else
            {
                customizationOptions[i].GetComponentInChildren<Toggle>().isOn = customization[i] != character.Options[i].Choices.First().Key;
            }
        }
        character.creature.ChangeRacialOptions();
        character.Change = true;
    }

    // Select customization category
    public void Category(int index)
    {
        string icon;
        Sprite sprite;
        Sprite[] sprites1 = Resources.LoadAll<Sprite>("Icons/charactercreate");
        Sprite[] sprites2 = Resources.LoadAll<Sprite>("Icons/charactercreateicons");
        Sprite[] sprites3 = Resources.LoadAll<Sprite>("Icons/dragonridingcustomization");
        for (int i = 0; i < customizationCategories.Count; i++)
        {
            icon = character.Categories[i].Icon.ToString();
            sprite = sprites1.FirstOrDefault(s => s.name == icon);
            if (sprite == null)
            {
                sprite = sprites2.FirstOrDefault(s => s.name == icon);
            }
            if (sprite == null)
            {
                sprite = sprites3.FirstOrDefault(s => s.name == icon);
            }
            customizationCategories[i].image.sprite = sprite;
        }
        icon = character.Categories[index].Selected.ToString();
        sprite = sprites1.FirstOrDefault(s => s.name == icon);
        if (sprite == null)
        {
            sprite = sprites2.FirstOrDefault(s => s.name == icon);
        }
        if (sprite == null)
        {
            sprite = sprites3.FirstOrDefault(s => s.name == icon);
        }
        customizationCategories[index].image.sprite = sprite;
        for (int i = 0; i < customizationOptions.Count; i++)
        {
            customizationOptions[i].SetActive(character.Options[i].Category == character.Categories[index].ID &&
                character.Options[i].Choices.Count > 1 && character.Options[i].Model == character.ModelID);
        }
        character.Category = character.Categories[index].ID;
    }

    // Handle dropdown value change
    public void Dropdown(int index)
    {
        CustomDropdown dropdown = customizationOptions[index].GetComponentInChildren<CustomDropdown>();
        dropdown.RefreshShownValue();
        character.Customization[index] = dropdown.GetValue();
        string str = dropdown.captionText.text;
        str = str[(str.IndexOf(':') + 1)..];
        dropdown.captionText.text = str;
        Text text = dropdown.GetComponentInChildren<Text>();
        TextGenerationSettings settings = new()
        {
            textAnchor = text.alignment,
            pivot = Vector2.zero,
            font = text.font,
            fontSize = text.fontSize,
            fontStyle = text.fontStyle,
            horizontalOverflow = text.horizontalOverflow
        };
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

    // Handle toggle value change
    public void Toggle(int index)
    {
        Toggle toggle = customizationOptions[index].GetComponentInChildren<Toggle>();
        character.Customization[index] = toggle.isOn ? character.Options[index].Choices.Last().Key :
            character.Options[index].Choices.First().Key;
        character.Change = true;
    }

    // When mouse enters UI element block translation, rotation and zoom
    public void PointerEnter()
    {
        translate = false;
        rotate = false;
        zoom = false;
    }

    // When mouse leaves UI element allow translation, rotation and zoom
    public void PointerExit()
    {
        translate = true;
        rotate = true;
        zoom = true;
    }

    // Select previous customization option in Dropdown
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

    // Select next customization option in Dropdown
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

    // If in customize mode go back, else exit the application
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

    // If in customize mode open save file window, else go into customzie mode
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
                Save();
            }
        }
        customizeButton.GetComponentInChildren<Text>().text = customize ? "Save" : "Customize";
        exitButton.GetComponentInChildren<Text>().text = customize ? "Back" : "Exit";
    }

    // Show import panel
    public void Import()
    {
        importPanel.SetActive(true);
    }

    //Import character from armory when pressed OK button
    public void OKImportButton()
    {
        InputField[] boxes = importPanel.GetComponentsInChildren<InputField>();
        string name = boxes[0].text.ToLower();
        string realm = boxes[1].text.ToLower().Replace(' ', '-');
        Dropdown dropdown = importPanel.GetComponentInChildren<Dropdown>();
        string region = WoWHelper.WoWRegion(dropdown.value);
        HttpResponseMessage response = FetchCharacter(region, realm, name);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            token = GetToken();
            response = FetchCharacter(region, realm, name);
        }
        if (response.StatusCode == HttpStatusCode.OK)
        {
            string resp = response.Content.ReadAsStringAsync().Result;
            //if (gear)
            //{
            //    Gear();
            //}
            if (customize)
            {
                Exit();
            }
            JObject jCharacter = JObject.Parse(resp);
            int race = jCharacter["playable_race"].Value<int>("id");
            bool gender = jCharacter["gender"].Value<string>("name") == "Male";
            int id = jCharacter["playable_class"].Value<int>("id");
            string faction = jCharacter["faction"].Value<string>("name");
            if (race == 25 || race == 26)
            {
                race = 24;
            }
            Button button = (faction == "Alliance" ? alliancePanel.transform.Find(races[race].ToLower()) :
                hordePanel.transform.Find(races[race].ToLower())).GetComponentInChildren<Button>();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            RaceButton(race);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(gender ? "male" : "female").GetComponentInChildren<Button>().gameObject);
            GenderButton(gender);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(classes[id].ToLower()).GetComponentInChildren<Button>().gameObject);
            ClassButton(id);
            List<JToken> list = jCharacter["customizations"].Children().ToList();
            foreach (JToken custom in list)
            {
                int index = Array.FindIndex(character.Options, o => o.ID == custom["option"].Value<int>("id"));
                if (index == -1)
                {
                    continue;
                }
                int value = custom["choice"].Value<int>("id");
                character.Customization[index] = value;
                if (character.Options[index].Type == WoWHelper.CustomizationType.Dropdown)
                {
                    character.CustomizationDropdowns[index].SetValue(value);
                }
                else
                {
                    character.CustomizationToggles[index].isOn = value != character.Options[index].Choices.First().Key;
                }
            }
            //list = jCharacter["items"].Children().ToList();
            //connection.Open();
            //gearPanel.gameObject.SetActive(true);
            //foreach (JToken item in list)
            //{
            //    EquipItem(item);
            //}
            //gearPanel.gameObject.SetActive(false);
            //connection.Close();
            character.Change = true;
        }
        importPanel.SetActive(false);
        PointerExit();
    }

    // Send request to the API
    private HttpResponseMessage FetchCharacter(string region, string realm, string name)
    {
        using HttpClient client = new();
        using HttpRequestMessage request = new(new HttpMethod("GET"),
        $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{name}/appearance?namespace=profile-{region}&locale=en_us");
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        return client.SendAsync(request).Result;
    }

    // Close import panel without importing with Cancel button
    public void CancelImportButton()
    {
        importPanel.SetActive(false);
        PointerExit();
    }

    //Open saved character json file
    public void Open()
    {
        if (!Directory.Exists("Save"))
        {
            Directory.CreateDirectory("Save");
        }
        StartCoroutine(OpenCharacter());
    }

    //Open saved character json file
    public void Open(string file)
    {
        if (!Directory.Exists("Save"))
        {
            Directory.CreateDirectory("Save");
        }
        StartCoroutine(OpenCharacter(file));
    }

    //Open character file
    public IEnumerator OpenCharacter(string file = "")
    {
        string path;
        if (string.IsNullOrEmpty(file))
        {
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, "Save", "", "Open character", "Open");
            path = FileBrowser.Success ? FileBrowser.Result[0] : file;
        }
        else
        {
            path = file;
        }
        if (path != "")
        {
            SerializableCharacter sCharacter;
            using (StreamReader reader = new(path))
            {
                sCharacter = JsonConvert.DeserializeObject<SerializableCharacter>(reader.ReadToEnd());
            }
            //if (gear)
            //{
            //    Gear();
            //}
            if (customize)
            {
                Exit();
            }
            EventSystem.current.SetSelectedGameObject(GameObject.Find(races[sCharacter.raceid].ToLower()).GetComponentInChildren<Button>().gameObject);
            RaceButton(sCharacter.raceid);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(sCharacter.gender ? "male" : "female").GetComponentInChildren<Button>().gameObject);
            GenderButton(sCharacter.gender);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(classes[sCharacter.classid].ToLower()).GetComponentInChildren<Button>().gameObject);
            ClassButton(sCharacter.classid);
            for (int i = 0; i < sCharacter.customization.Length; i++)
            {
                character.Customization[i] = sCharacter.customization[i];
                if (character.Options[i].Type == WoWHelper.CustomizationType.Dropdown)
                {
                    ((CustomDropdown)customizationOptions[i].GetComponentInChildren<Dropdown>(true)).SetValue(character.Customization[i]);
                }
                else
                {
                    (customizationOptions[i].GetComponentInChildren<Toggle>(true)).isOn = character.Customization[i] != character.Options[i].Choices.First().Key;
                }
            }
            //connection.Open();
            //gearPanel.gameObject.SetActive(true);
            //int icon;
            //for (int i = 0; i < sCharacter.items.Length; i++)
            //{
            //    if (sCharacter.items[i].id != 0)
            //    {
            //        using (SqliteCommand command = connection.CreateCommand())
            //        {
            //            command.CommandType = CommandType.Text;
            //            command.CommandText = "SELECT [Display ID], Icon, Slot FROM Items WHERE ID = " + sCharacter.items[i].id + " AND Version = " + sCharacter.items[i].version + ";";
            //            SqliteDataReader reader = command.ExecuteReader();
            //            reader.Read();
            //            character.Items[i] = new ItemModel(sCharacter.items[i].id, sCharacter.items[i].version, reader.GetInt32(2), reader.GetInt32(0), character.Race, character.Gender);
            //            icon = reader.GetInt32(1);
            //        }
            //        GameObject.Find(WoWHelper.SlotName(i)).GetComponent<Image>().sprite = IconFromBLP(icon);
            //    }
            //    else
            //    {
            //        character.Items[i] = null;
            //        GameObject.Find(WoWHelper.SlotName(i)).GetComponent<Image>().sprite = Resources.Load<Sprite>(@"Icons\ui-paperdoll-slot-" + WoWHelper.SlotName(i));
            //    }
            //}
            //gearPanel.gameObject.SetActive(false);
            //connection.Close();
            character.Change = true;
        }
    }

    //Save character as json file
    public void Save()
    {
        if (!Directory.Exists("Save"))
        {
            Directory.CreateDirectory("Save");
        }
        StartCoroutine(SaveCharacter());
    }

    //Save Character
    public IEnumerator SaveCharacter()
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, "Save",
            $"{races[(int)character.Race]} {classes[(int)character.Class]}", "Save character", "Save");
        string path = FileBrowser.Success ? FileBrowser.Result[0] : "";
        if (path != "")
        {
            SerializableCharacter sCharacter = new()
            {
                raceid = (int)character.Race,
                gender = character.Gender,
                classid = (int)character.Class,
                customization = new int[character.Customization.Length]
            };
            for (int i = 0; i < sCharacter.customization.Length; i++)
            {
                sCharacter.customization[i] = character.Customization[i];
            }
            //sCharacter.items = new SerializableItems[character.Items.Length];
            //for (int i = 0; i < sCharacter.items.Length; i++)
            //{
            //    sCharacter.items[i] = new SerializableItems();
            //    if (character.Items[i] == null)
            //    {
            //        sCharacter.items[i].id = 0;
            //        sCharacter.items[i].version = 0;
            //    }
            //    else
            //    {
            //        sCharacter.items[i].id = character.Items[i].ID;
            //        sCharacter.items[i].version = character.Items[i].Version;
            //    }
            //}
            using StreamWriter writer = new(path);
            writer.Write(JsonConvert.SerializeObject(sCharacter, Formatting.Indented));
        }
    }
}
