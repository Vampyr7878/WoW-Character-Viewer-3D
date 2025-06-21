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

// Class handling main UI input in the scene
public class ScreenInput : MonoBehaviour
{
    // Reference to the camera
    public Camera mainCamera;
    // Reference to prefeb for element on list of items on the left to equip
    public GameObject leftScrollItem;
    // Reference to prefeb for element on list of items on the right to equip
    public GameObject rightScrollItem;
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
    //List of all items for opened slot
    private Dictionary<int, Item> items;
    // List of all items for opened slot
    private ScrollItem[] leftScrollItems;
    // List of all items displayed on the right equipment panel
    private ScrollItem[] rightScrollItems;
    // max amount of items in equipment panel
    private int maxScrollItems;
    // currently oepened equipment slot
    private WoWHelper.ItemSlot slot;
    // currently oepened slot button
    private GameObject slotButton;
    // Is UI in customize mode?
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
        druidModels = new Dictionary<int, string>();
        customizationOptions = new List<GameObject>();
        customizationCategories = new List<Button>();
        formButtons = new List<Button>();
        dbPath = $"URI=file:{Application.streamingAssetsPath}/database.sqlite";
        connection = new SqliteConnection(dbPath);
        connection.Open();
        races = DB.GetRaces(connection);
        classes = DB.GetClasses(connection);
        connection.Close();
        items = new Dictionary<int, Item>();
        maxScrollItems = 100;
        leftScrollItems = new ScrollItem[maxScrollItems];
        rightScrollItems = new ScrollItem[maxScrollItems];
        for (int i = 0; i < maxScrollItems; i++)
        {
            leftScrollItems[i] = Instantiate(leftScrollItem, leftPanel.GetComponentInChildren<VerticalLayoutGroup>().
                transform.transform).GetComponent<ScrollItem>();
            leftScrollItems[i].GetComponent<Toggle>().group = leftPanel.GetComponentInChildren<ToggleGroup>();
            leftScrollItems[i].name = i.ToString();
            leftScrollItems[i].gameObject.SetActive(false);
            rightScrollItems[i] = Instantiate(rightScrollItem, rightPanel.GetComponentInChildren<VerticalLayoutGroup>().
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
            character.transform.Rotate(0f, Input.GetAxis("Mouse X") * 10f, 0f);
            creature.transform.Rotate(0f, Input.GetAxis("Mouse X") * 10f, 0f);
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
        character.Categories = DB.GetCategories(connection, character.ModelID);
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
        character.Options = DB.GetOptions(connection, character.MainFormID, character.CreatureForms);
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
        for (int i = 0; i < character.Options.Length; i++)
        {
            character.Options[i].AllChoices = DB.GetChoices(connection, character.Options[i].ID, (int)character.Class);
            character.Options[i].Choices = character.Options[i].AllChoices;
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
        foreach (var option in character.Options)
        {
            foreach (var choice in option.Choices)
            {
                choice.Value.Geosets = DB.GetGeosets(connection, choice.Value.ID);
                choice.Value.SkinnedGeosets = DB.GetSkinnedGeosets(connection, choice.Value.ID);
                choice.Value.Textures = DB.GetCustomizationTextures(connection, choice.Value.ID);
                choice.Value.Creatures = DB.GetCustomizationDisplayInfos(connection, choice.Value.ID);
                if (choice.Value.Creatures.Length > 0)
                {
                    choice.Value.Creatures[0].Geosets = DB.GetCreatureGeosets(connection, choice.Value.Creatures[0].ID);
                    choice.Value.Creatures[0].ParticleColors = DB.GetParticleColors(connection, choice.Value.Creatures[0].ParticleColor);
                }
            }
        }
    }

    // Load all the items matching the opened equipment slot
    private void FillItems(GameObject panel)
    {
        items.Clear();
        connection.Open();
        string condition = slot switch
        {
            WoWHelper.ItemSlot.Chest => $"InventoryType IN ({(int)slot}, 20) AND ClassID = 4",
            WoWHelper.ItemSlot.MainHand => $"InventoryType IN (13, 15, 17, {(int)slot}, 26) AND " +
                $"ClassID = 2 AND SubclassID IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 15, 18, 19)",
            WoWHelper.ItemSlot.OffHand => $"InventoryType IN (13, 14, 17, {(int)slot}, 23) AND ((ClassID = 2 AND " +
                $"SubclassID IN (0, 1, 4, 5, 6, 7, 8, 9, 10, 13, 15)) OR (ClassID = 4 AND SubClassID IN (0, 6)))",
            _ => $"InventoryType = {(int)slot} AND ClassID = 4",
        };
        items = DB.GetItems(connection, condition);
        DB.GetAppearances(connection, items);
        connection.Close();
        Toggle none = panel.GetComponentInChildren<Toggle>();
        if (panel == leftPanel)
        {
            LoadItems(leftScrollItems);
        }
        else
        {
            LoadItems(rightScrollItems);
        }
        none.isOn = true;
    }

    // Load items to fill the panel
    private void LoadItems(ScrollItem[] scrollItems)
    {
        int i = 0;
        foreach(var item in items)
        {
            if (i == maxScrollItems)
            {
                break;
            }
            if (item.Value.Appearances == null || item.Value.Appearances.Count == 0)
            {
                SetItem(scrollItems[i], item.Value, null);
                i++;
                continue;
            }
            foreach (var appearance in item.Value.Appearances)
            {
                SetItem(scrollItems[i], item.Value, appearance.Value);
                i++;
                if (i == maxScrollItems)
                {
                    break;
                }
            }
        }
    }

    // Set loaded item
    private void SetItem(ScrollItem scrollItem, Item item, ItemAppearance appearance)
    {
        scrollItem.label.text = item.Name;
        scrollItem.label.color = WoWHelper.QualityColor(item.Quality);
        scrollItem.background.sprite = IconFromBLP(item.Icon == 0 ? appearance == null ? 134400 : appearance.Icon : item.Icon);
        scrollItem.checkmark.color = WoWHelper.QualityColor(item.Quality);
        scrollItem.gameObject.SetActive(true);
        scrollItem.tooltip.gameObject.SetActive(true);
        string description;
        Color32 color;
        if (string.IsNullOrEmpty(item.Description))
        {
            description = appearance == null || appearance.Modifier == 0 ? (item.Flags & WoWHelper.Heroic) == 0 ? "" :
                "Heroic" : WoWHelper.AppearanceModifierName(appearance.Modifier);
            color = Color.green;
        }
        else
        {
            description = item.Description;
            color = item.Color;
        }
        scrollItem.SetTooltip(item.Name, WoWHelper.QualityColor(item.Quality), description, color,
            WoWHelper.SlotName(item.ItemSlot), item.ItemClass == WoWHelper.ItemClass.Weapon ?
            WoWHelper.WeaponTypeName(item.WeaponType) : WoWHelper.ArmorTypeName(item.ArmorType));
        scrollItem.tooltip.gameObject.SetActive(false);
        scrollItem.Item = new(item.ID, appearance == null ? 0 : appearance.Modifier);
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
        character.CreatureForms = DB.GetCreatureForms(connection, character.Race, character.Gender, (int)character.Class);
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
        string extraCollection;
        connection.Open();
        character.ModelID = character.MainFormID = DB.GetModel(connection, character.Race, gender,
            out string path, out string model, out string extra, out string armor, out string collection);
        character.RacePath = path;
        if (character.Race == WoWHelper.Race.Dracthyr)
        {
            character.ExtraPath = DB.GetModel(connection, gender, out extra, out extraCollection);
        }
        else
        {
            extraCollection = null;
        }
        WoWHelper.Class c = character.Class;
        character.Class = WoWHelper.Class.Warrior;
        SetupFormPanel();
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        connection.Close();
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
        string extraCollection;
        connection.Open();
        bool[] classes = DB.GetRaceClasses(connection, race, classButtons.Length);
        for (int i = 0; i < classButtons.Length; i++)
        {
            classButtons[i].interactable = classes[i];
        }
        character.ModelID = character.MainFormID = DB.GetModel(connection, (WoWHelper.Race)race, character.Gender,
            out string path, out string model, out string extra, out string armor, out string collection);
        character.RacePath = path;
        if (race == (int)WoWHelper.Race.Dracthyr)
        {
            character.ExtraPath = DB.GetModel(connection, character.Gender, out extra, out extraCollection);
        }
        else
        {
            character.ExtraPath = "";
            extraCollection = null;
        }
        gearPanel.SetActive(true);
        ClearItems();
        gearPanel.SetActive(false);
        character.Race = (WoWHelper.Race)race;
        character.Class = WoWHelper.Class.Warrior;
        SetupFormPanel();
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        connection.Close();
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
        connection.Open();
        SetupCategories();
        connection.Close();
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
        if (character.ModelID == 0 || id == (int)character.Class)
        {
            return;
        }
        ChangeBorder(EventSystem.current.currentSelectedGameObject);
        character.Class = (WoWHelper.Class)id;
        int[] customization = character.Customization;
        connection.Open();
        SetupFormPanel();
        SetupCustomizationPanel();
        SetupCategories();
        GetCustomizationChoices();
        GetCustomizationElements();
        connection.Close();
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

    // Equipment button on the left toggles equipement panel on the left side
    public void LeftButton(int slot)
    {
        rightPanel.SetActive(false);
        leftPanel.SetActive(true);
        leftPanel.GetComponentInChildren<InputField>().text = "";
        this.slot = (WoWHelper.ItemSlot)slot;
        slotButton = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject;
        FillItems(leftPanel);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    // Equipment button on the right toggles equipement panel on the righ side
    public void RightButton(int slot)
    {
        leftPanel.SetActive(false);
        rightPanel.SetActive(true);
        rightPanel.GetComponentInChildren<InputField>().text = "";
        this.slot = (WoWHelper.ItemSlot)slot;
        slotButton = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject;
        FillItems(rightPanel);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    // Equip selected item when pressed OK on equipment panel
    public void OkButton()
    {
        rotate = false;
        translate = false;
        bool none = true;
        int slot = (int)WoWHelper.MapSlots(slotButton.name);
        GameObject panel = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;
        ScrollItem scrollItem;
        for (int i = 0; i < maxScrollItems; i++)
        {
            scrollItem = panel == leftPanel ? leftScrollItems[i] : rightScrollItems[i];
            if (scrollItem.gameObject.activeSelf && scrollItem.toggle.isOn)
            {
                none = false;
                slotButton.GetComponent<Image>().sprite = scrollItem.background.sprite;
                int id = character.Items[slot] == null ? 0 : character.Items[slot].ID;
                int appearance = character.Items[slot] == null ? 0 : character.Items[slot].Appearance;
                character.Items[slot] = new(scrollItem.Item.ID, scrollItem.Item.Appearance, items[scrollItem.Item.ID])
                {
                    Changed = scrollItem.Item.ID != id || scrollItem.Item.Appearance != appearance
                };
                var itemAppearance = character.Items[slot].Item.Appearances[character.Items[slot].Appearance];
                if (itemAppearance.DisplayInfoID != 0)
                {
                    connection.Open();
                    DB.GetDisplayInfo(connection, itemAppearance);
                    itemAppearance.DisplayInfo.ParticleColors = DB.GetParticleColors(connection, itemAppearance.DisplayInfo.ParticleColor);
                    DB.GetItemModels(connection, itemAppearance.DisplayInfo);
                    DB.GetItemTextures(connection, itemAppearance.DisplayInfo);
                    DB.GetHelmetGeosets(connection, itemAppearance.DisplayInfo);
                    DB.GetComponents(connection, itemAppearance);
                    DB.GetComponentTextures(connection, itemAppearance.DisplayInfo);
                    connection.Close();
                }
                if (character.Items[(int)WoWHelper.SlotIndex.MainHand] != null &&
                    (character.Items[(int)WoWHelper.SlotIndex.MainHand].Item.ItemSlot == WoWHelper.ItemSlot.Bow))
                {
                    character.Items[(int)WoWHelper.SlotIndex.OffHand] = null;
                    GameObject.Find("offhand").GetComponent<Image>().sprite = Resources.Load<Sprite>(@"Icons\ui-paperdoll-slot-offhand");
                }
                break;
            }
        }
        if (none)
        {
            if (slot == (int)WoWHelper.SlotIndex.LeftShoulder && character.Items[(int)WoWHelper.SlotIndex.RightShoulder] != null)
            {
                character.Items[(int)WoWHelper.SlotIndex.RightShoulder].Changed = true;
            }
            character.Items[(int)WoWHelper.MapSlots(slotButton.name)] = null;
            slotButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(@$"Icons\ui-paperdoll-slot-{slotButton.name}");
        }
        panel.GetComponentInChildren<InputField>().text = "";
        panel.SetActive(false);
        items.Clear();
        character.Change = true;
        PointerExit();
    }

    // Close equipment panel without equiping anything with Cancel button
    public void CancelButton()
    {
        rotate = false;
        translate = false;
        GameObject panel = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;
        panel.GetComponentInChildren<InputField>().text = "";
        EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
        items.Clear();
        PointerExit();
    }

    // Filter displayed items using filter box
    public void Filter(string text)
    {
        GameObject panel = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;
        if (text == "")
        {
            if (panel == leftPanel)
            {
                LoadItems(leftScrollItems);
            }
            else
            {
                LoadItems(rightScrollItems);
            }
        }
        else
        {
            if (panel == leftPanel)
            {
                FilterItems(leftScrollItems, text);
            }
            else
            {
                FilterItems(rightScrollItems, text);
            }
        }
        Toggle none = panel.GetComponentInChildren<Toggle>();
        if (none != null)
        {
            none.isOn = true;
        }
    }

    // Find items matching criteria
    private void FilterItems(ScrollItem[] scrollItems, string text)
    {
        int j = 0, k = 0;
        var filtered = items.Where(i => i.Value.Name.ToLower().Contains(text.ToLower())).ToArray();
        for (; j < maxScrollItems; k++)
        {
            if (k == filtered.Length)
            {
                break;
            }
            if (filtered[k].Value.Appearances == null || filtered[k].Value.Appearances.Count == 0)
            {
                SetItem(scrollItems[j], filtered[k].Value, null);
                j++;
                continue;
            }
            foreach (var appearance in filtered[k].Value.Appearances)
            {
                SetItem(scrollItems[j], filtered[k].Value, appearance.Value);
                j++;
                if (j == maxScrollItems)
                {
                    break;
                }
            }
        }
        for (; j < maxScrollItems; j++)
        {
            scrollItems[j].gameObject.SetActive(false);
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
            rightPanel.SetActive(gear);
            leftPanel.SetActive(gear);
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

    // Toggle gear panel
    public void Gear()
    {
        gear = !gear;
        formPanel.SetActive(!gear);
        customizationPanel.SetActive(!gear);
        gearPanel.SetActive(gear);
        if (!gear)
        {
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
        }
    }

    // Show import panel
    public void Import()
    {
        importPanel.SetActive(true);
#if UNITY_EDITOR
        InputField[] boxes = importPanel.GetComponentsInChildren<InputField>();
        boxes[0].text = string.IsNullOrEmpty(boxes[0].text) ? "Cedrad" : boxes[0].text;
        boxes[1].text = string.IsNullOrEmpty(boxes[1].text) ? "The Maelstrom" : boxes[1].text;
        Dropdown dropdown = importPanel.GetComponentInChildren<Dropdown>();
        dropdown.value = 1;
#endif
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
            if (gear)
            {
                Gear();
            }
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
            EventSystem.current.SetSelectedGameObject(GameObject.Find(gender ? "male" : "female").GetComponentInChildren<Button>().gameObject);
            GenderButton(gender);
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            RaceButton(race);
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
            list = jCharacter["items"].Children().ToList();
            connection.Open();
            gearPanel.SetActive(true);
            WoWHelper.SlotIndex slot;
            ClearItems();
            foreach (JToken item in list)
            {
                slot = WoWHelper.MapSlot(item.Value<int>("internal_slot_id"));
                EquipItem(item.Value<int>("id"), item.Value<int>("item_appearance_modifier_id"), slot);
                if (slot == WoWHelper.SlotIndex.RightShoulder)
                {
                    EquipItem(item.Value<int>("secondary_id"), item.Value<int>("secondary_item_appearance_modifier_id"), WoWHelper.SlotIndex.LeftShoulder);
                }
            }
            gearPanel.SetActive(false);
            connection.Close();
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

    // Clear all equipped items
    private void ClearItems()
    {
        string slot;
        for (int i = 0; i < character.Items.Length; i++)
        {
            slot = WoWHelper.MapSlots((WoWHelper.SlotIndex)i);
            character.Items[i] = null;
            GameObject.Find(slot).GetComponent<Image>().sprite = Resources.Load<Sprite>(@$"Icons\ui-paperdoll-slot-{slot}");
        }
    }

    // Equip item from loaded character
    private void EquipItem(int id, int modifier, WoWHelper.SlotIndex slotIndex)
    {
        if (id == 0)
        {
            return;
        }
        Item item;
        string slot = WoWHelper.MapSlots(slotIndex), condition;
        condition = $"ID = {id}";
        item = DB.GetItems(connection, condition).First().Value;
        DB.GetAppearances(connection, item);
        foreach (var appearance in item.Appearances)
        {
            if (appearance.Value.DisplayInfoID != 0)
            {
                DB.GetDisplayInfo(connection, appearance.Value);
                appearance.Value.DisplayInfo.ParticleColors = DB.GetParticleColors(connection,
                    appearance.Value.DisplayInfo.ParticleColor);
                DB.GetItemModels(connection, appearance.Value.DisplayInfo);
                DB.GetItemTextures(connection, appearance.Value.DisplayInfo);
                DB.GetHelmetGeosets(connection, appearance.Value.DisplayInfo);
                DB.GetComponents(connection, appearance.Value);
                DB.GetComponentTextures(connection, appearance.Value.DisplayInfo);
            }
        }
        character.Items[(int)slotIndex] = new(id, modifier, item)
        {
            Changed = true
        };
        GameObject.Find(slot).GetComponent<Image>().sprite = IconFromBLP(item.Icon == 0 ?
            item.Appearances.Count == 0 ? 134400 : item.Appearances[modifier].Icon : item.Icon);
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
            if (gear)
            {
                Gear();
            }
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
            connection.Open();
            gearPanel.SetActive(true);
            ClearItems();
            for (int i = 0; i < sCharacter.items.Length; i++)
            {
                EquipItem(sCharacter.items[i].id, sCharacter.items[i].appearance, (WoWHelper.SlotIndex)i);
            }
            gearPanel.SetActive(false);
            connection.Close();
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
            sCharacter.items = new SerializableItem[character.Items.Length];
            for (int i = 0; i < sCharacter.items.Length; i++)
            {
                sCharacter.items[i] = new SerializableItem();
                if (character.Items[i] == null)
                {
                    sCharacter.items[i].id = 0;
                    sCharacter.items[i].appearance = 0;
                }
                else
                {
                    sCharacter.items[i].id = character.Items[i].ID;
                    sCharacter.items[i].appearance = character.Items[i].Appearance;
                }
            }
            using StreamWriter writer = new(path);
            writer.Write(JsonConvert.SerializeObject(sCharacter, Formatting.Indented));
        }
    }
}
