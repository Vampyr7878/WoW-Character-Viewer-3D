using BLPLib;
using CASCLib;
using Crosstales.FB;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serializable;
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

//Class handling main UI input in the scene
public class ScreenInput : MonoBehaviour
{
    //Reference to the camera
    public Camera mainCamera;
    //Reference to prefeb for element on list of items to equip
    public GameObject scrollItem;
    //Reference to prefab that is used to create customization UI
    public GameObject customizationDropdown;
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
    //Reference to the Gilenan form for Worgen characters
    public Gilnean gilnean;
    //Reference for object that handles rendering of druid forms
    public Druid druid;
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

    //List of all customization option dropdowns
    private List<GameObject> customizationOptions;
    //Reference to the database connection object
    private SqliteConnection connection;
    //Dictionary mapping id to race name
    private Dictionary<int, string> races;
    //Dictionary mapping id to class name
    private Dictionary<int, string> classes;
    //Dictionary mapping id to druid form model
    private Dictionary<int, string> druidModels;
    //Referenec to the opened CASC storage
    private CASCHandler casc;
    //List of all items for opened slot
    private List<Item> items;
    //List of all items displayed on the equipment panel, max 100
    private List<GameObject> scrollItems;
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
        string path;
        using (StreamReader reader = new StreamReader("config.ini"))
        {
            path = reader.ReadLine();
        }
        casc = CASCHandler.OpenLocalStorage(path, "wow");
        casc.Root.SetFlags(LocaleFlags.enGB, true, false);
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
        dbPath = $"URI=file:{Application.streamingAssetsPath}/database.sqlite";
        connection = new SqliteConnection(dbPath);
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Races;";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                races.Add(reader.GetInt32(0), reader.GetString(1));
            }
        }
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Classes;";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                classes.Add(reader.GetInt32(0), reader.GetString(1));
            }
        }
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM DruidModels;";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                druidModels.Add(reader.GetInt32(0), reader.GetString(1));
            }
        }
        connection.Close();
        items = new List<Item>();
        scrollItems = new List<GameObject>();
        customize = false;
        GenderButton(gender);
        character.Class = 1;
    }

    private void Update()
    {
        //Displayt "Loading..," text when characrter is being loaded
        if (character.Race > 0)
        {
            loading.gameObject.SetActive(!character.Loaded);
        }
        customizeButton.gameObject.SetActive(!gear);
        //Exit when autoscreenshotting is done
        if (screenshot && ((!allied && r >= coreRaceNames.Count) || r >= coreRaceNames.Count + alliedRaceNames.Count))
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
        //Autoscreenshot if enabled
        if (c < coreRaceNames.Count && screenshot)
        {
            string g = character.Gender ? "Male" : "Female";
            if (r >= coreRaceNames.Count && allied && r < coreRaceNames.Count + alliedRaceNames.Count)
            {
                StartCoroutine(TakeScreenshot(@$"Screenshots\{g}{alliedRaceNames[r - coreRaceNames.Count]}{classes[character.Class].Replace(" ", "")}.png"));
            }
            else if (r < coreRaceNames.Count)
            {
                StartCoroutine(TakeScreenshot(@$"Screenshots\{g}{coreRaceNames[r]}{classes[character.Class].Replace(" ", "")}.png"));
            }
        }
    }

    //Get API access token
    string GetToken()
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "https://us.battle.net/oauth/token"))
            {
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
        zzTransparencyCapture.captureScreenshot(@$"Screenshots\{(character.Gender ? "Male" : "Female")}{races[character.Race].Replace(" ", "").Replace("'", "")}{classes[character.Class].Replace(" ", "")}.png");
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
        string folder = character.Gender ? "Male" : "Female";
        if (r < coreRaceNames.Count)
        {
            if (c < classNames.Count && File.Exists(@$"Save\{folder}\{itemSet}\{coreRaceNames[r]}\{coreRaceNames[r]}{classNames[c]}.chr"))
            {
                Open(@$"Save\{folder}\{itemSet}\{coreRaceNames[r]}\{coreRaceNames[r]}{classNames[c]}.chr");
            }
        }
        else if (allied && r < coreRaceNames.Count + alliedRaceNames.Count)
        {
            if (c < classNames.Count && File.Exists(@$"Save\{folder}\{itemSet}\{alliedRaceNames[r - coreRaceNames.Count]}\{alliedRaceNames[r - coreRaceNames.Count]}{classNames[c]}.chr"))
            {
                Open(@$"Save\{folder}\{itemSet}\{alliedRaceNames[r - coreRaceNames.Count]}\{alliedRaceNames[r - coreRaceNames.Count]}{classNames[c]}.chr");
            }
        }
        yield return new WaitForSeconds(4);
        screenshot = true;
    }

    //Translate camera
    private void TranslateCamera()
    {
        if (translate && Input.GetMouseButton(1))
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
        if (rotate && Input.GetMouseButton(0))
        {
            character.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10f, 0f);
            gilnean.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10f, 0f);
            druid.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10f, 0f);
        }
    }

    //Zoom camare
    private void ZoomCamera()
    {
        if (zoom)
        {
            mainCamera.transform.Translate(0f, 0f, Input.GetAxis("Mouse ScrollWheel"));
            Vector3 value = mainCamera.transform.position;
            value.z = Mathf.Clamp(value.z, -3f, -0.25f);
            mainCamera.transform.position = value;
        }
    }

    //Get all customization choices from database
    private void GetCustomizationOptions()
    {
        character.Choices = new CustomizationChoice[character.Options.Length][];
        connection.Open();
        for (int i = 0; i < character.Options.Length; i++)
        {
            List<CustomizationChoice> choices = new List<CustomizationChoice>();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT * FROM Customization WHERE Option = {character.Options[i].ID};";
                SqliteDataReader reader = command.ExecuteReader();
                int j = 1;
                while (reader.Read())
                {
                    int index = reader.GetInt32(0);
                    string name = reader.IsDBNull(2) ? j.ToString() : $"{j}: {reader.GetString(2)}";
                    int color;
                    Color color1, color2;
                    if (reader.IsDBNull(3))
                    {
                        color = 0;
                        color1 = Color.clear;
                    }
                    else
                    {
                        color = reader.GetInt32(3);
                        ColorUtility.TryParseHtmlString($"#{color.ToString("X8").Substring(2)}", out color1);
                    }
                    if (reader.IsDBNull(4))
                    {
                        color = 0;
                        color2 = Color.clear;
                    }
                    else
                    {
                        color = reader.GetInt32(4);
                        ColorUtility.TryParseHtmlString($"#{color.ToString("X8").Substring(2)}", out color2);
                    }
                    if (color1 != Color.clear)
                    {
                        name = $"{name}: ";
                    }
                    int model = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
                    int bone = reader.IsDBNull(6) ? -1 : reader.GetInt32(6);
                    int c = reader.IsDBNull(7) ? -1 : reader.GetInt32(7);
                    int id = reader.IsDBNull(8) ? -1 : reader.GetInt32(8);
                    choices.Add(new CustomizationChoice(index, name, color1, color2, model, bone, c, id));
                    if ((choices[choices.Count - 1].Class & (1 << (character.Class - 1))) != 0)
                    {
                        j++;
                    }
                }
            }
            foreach (CustomizationChoice choice in choices)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationGeosets> geosets = new List<CustomizationGeosets>();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT * FROM CustomizationGeosets WHERE Customization = {choice.Index};";
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int geoset1 = reader.IsDBNull(2) ? -1 : reader.GetInt32(2);
                        int geoset2 = reader.IsDBNull(3) ? -1 : reader.GetInt32(3);
                        int geoset3 = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                        geosets.Add(new CustomizationGeosets(geoset1, geoset2, geoset3));
                    }
                    choice.Geosets = geosets.ToArray();
                }
                using (SqliteCommand command = connection.CreateCommand())
                {
                    List<CustomizationTextures> textures = new List<CustomizationTextures>();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT * FROM CustomizationTextures WHERE Customization = {choice.Index};";
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int texture1 = reader.IsDBNull(2) ? -1 : reader.GetInt32(2);
                        int texture2 = reader.IsDBNull(3) ? -1 : reader.GetInt32(3);
                        int texture3 = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                        int texture4 = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
                        textures.Add(new CustomizationTextures(texture1, texture2, texture3, texture4));
                    }
                    choice.Textures = textures.ToArray();
                }
            }
            character.Choices[i] = choices.ToArray();
            SetCustomizationNamesAndColors(i);
        }
        connection.Close();
        SetupCategories();
        Category(0);
    }

    //Setup UI for current customization
    public void SetCustomizationNamesAndColors(int i)
    {
        Sprite sprite = Resources.LoadAll<Sprite>("Icons/charactercreate").Single(s => s.name == "color");
        Dropdown dropdown = customizationOptions[i].GetComponentInChildren<Dropdown>(true);
        dropdown.options.Clear();
        for (int j = 0; j < character.Choices[i].Length; j++)
        {
            if ((character.Choices[i][j].Class & (1 << (character.Class - 1))) == 0)
            {
                continue;
            }
            Rect rect = sprite.textureRect;
            Texture2D texture = new Texture2D((int)rect.width + 20, (int)rect.height + 10);
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
            for (int x = 0; x < texture.width - 20; x++)
            {
                for (int y = 10; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, sprite.texture.GetPixel((int)rect.x + x, (int)rect.y + y - 10) * character.Choices[i][j].Color1);
                }
            }
            for (int x = 20; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height - 10; y++)
                {
                    Color c1 = texture.GetPixel(x, y);
                    Color c2 = sprite.texture.GetPixel((int)rect.x + x - 20, (int)rect.y + y) * character.Choices[i][j].Color2;
                    texture.SetPixel(x, y, Color.Lerp(c1, c2, c2.a));
                }
            }
            texture.Apply();
            Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            CustomOptionData data = new CustomOptionData(character.Choices[i][j].Name, s, j);
            dropdown.options.Add(data);
            ((CustomDropdown)dropdown).RefreshShownValue();
        }
    }

    //Hide unused categories for customization
    private void SetupCategories()
    {
        Button[] categories = customizationPanel.GetComponentInChildren<HorizontalLayoutGroup>().GetComponentsInChildren<Button>(true);
        foreach (Button category in categories)
        {
            category.gameObject.SetActive(false);
        }
        for (int i = 0; i < character.Options.Length; i++)
        {
            if (customizationOptions[i].GetComponentInChildren<Dropdown>().options.Count > 1 && character.Options[i].Form == character.Form)
            {
                categories[character.Options[i].Category - 1].gameObject.SetActive(true);
            }
        }
    }

    //Load customization options and prepare UI for them
    private void SetupCustomizationPanel()
    {
        foreach (GameObject customizaionOption in customizationOptions)
        {
            Destroy(customizaionOption);
        }
        customizationOptions.Clear();
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            List<CustomizationOption> options = new List<CustomizationOption>();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM CustomizationOptions WHERE Race = {character.Race} AND Gender = {character.Gender};";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int category = reader.GetInt32(3);
                string name = reader.GetString(4);
                int form = reader.GetInt32(5);
                int blizzard = reader.GetInt32(6);
                options.Add(new CustomizationOption(id, category, name, form, blizzard));
            }
            character.Options = options.ToArray();
            character.Customization = new int[options.Count];
            for (int i = 0; i < character.Customization.Length; i++)
            {
                character.Customization[i] = 0;
            }
        }
        connection.Close();
        GameObject option;
        Button[] buttons;
        Dropdown dropdown;
        Button[] categories = customizationPanel.GetComponentInChildren<HorizontalLayoutGroup>().GetComponentsInChildren<Button>(true);
        for (int i = 0; i < character.Options.Length; i++)
        {
            option = Instantiate(customizationDropdown, customizationPanel.GetComponentInChildren<VerticalLayoutGroup>().transform);
            buttons = option.GetComponentsInChildren<Button>();
            int customizationValue = i;
            foreach (Button button in buttons)
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
            dropdown = option.GetComponentInChildren<Dropdown>();
            dropdown.onValueChanged.AddListener(delegate { Dropdown(customizationValue); });
            EventTrigger trigger = option.GetComponent<EventTrigger>();
            trigger.triggers[0].callback.AddListener(delegate { PointerEnter(); });
            trigger.triggers[1].callback.AddListener(delegate { PointerExit(); });
            option.GetComponentInChildren<Text>().text = character.Options[i].Name;
            customizationOptions.Add(option);
        }
    }

    //Load all the items matching the opened equipment slot
    private void FillItems(int slot, GameObject panel)
    {
        panel.GetComponentInChildren<InputField>().text = "";
        connection.Open();
        string condition;
        switch (slot)
        {
            case 5:
                condition = "Slot = 5 OR Slot = 20";
                break;
            case 21:
                condition = "Slot = 21 OR Slot = 13 OR Slot = 15 OR Slot = 17 OR Slot = 26";
                break;
            case 22:
                condition = "Slot = 22 OR Slot = 13 OR Slot = 14 OR Slot = 17 OR Slot = 23";
                break;
            default:
                condition = "Slot = " + slot;
                break;
        }
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT ID, Version, Name, [Display ID], Icon, Quality, Slot FROM Items WHERE (" + condition + ");";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Item(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6)));
            }
        }
        connection.Close();
        Toggle none = panel.GetComponentInChildren<Toggle>();
        for (int i = 0; i < 100; i++)
        {
            scrollItems.Add(Instantiate(scrollItem, panel.GetComponentInChildren<VerticalLayoutGroup>().transform));
            scrollItems[i].name = i.ToString();
            scrollItems[i].GetComponent<Toggle>().group = panel.GetComponentInChildren<ToggleGroup>();
            scrollItems[i].GetComponentInChildren<Text>().text = items[i].Name;
            scrollItems[i].GetComponentInChildren<Text>().color = WoWHelper.QualityColor(items[i].Quality);
            scrollItems[i].GetComponentInChildren<Image>().sprite = IconFromBLP(items[i].Icon);
            scrollItems[i].GetComponentsInChildren<Image>()[1].color = WoWHelper.QualityColor(items[i].Quality);
            Image tooltip = Array.Find(scrollItems[i].GetComponentsInChildren<Image>(true), x => x.CompareTag("tooltip"));
            tooltip.gameObject.SetActive(true);
            tooltip.GetComponentInChildren<Text>().text = items[i].Name;
            tooltip.GetComponentInChildren<Text>().color = WoWHelper.QualityColor(items[i].Quality);
            tooltip.gameObject.SetActive(false);
        }
        none.isOn = true;
    }

    //Create icon from BLP file
    private Sprite IconFromBLP(int file)
    {
        try
        {
            BLP blp = new BLP(casc.OpenFile(file));
            System.Drawing.Bitmap image = blp.GetImage();
            Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, true);
            texture.LoadImage((byte[])converter.ConvertTo(image, typeof(byte[])));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        }
        catch
        {
            Debug.LogError(file);
            return null;
        }
    }

    //Clear all the equipped items
    private void ClearItems()
    {
        foreach (Button button in gearPanel.GetComponentsInChildren<Button>())
        {
            if (button.CompareTag("equippable"))
            {
                button.GetComponent<Image>().sprite = Resources.Load<Sprite>(@"Icons\ui-paperdoll-slot-" + button.name);
            }
        }
        character.ClearItems();
    }

    //Change colors for ui buttons for hero classes
    private void ChangeButtonColors()
    {
        Color color;
        switch (character.Class)
        {
            case 6:
                color = new Color(0f, 0.5f, 1f);
                break;
            case 12:
                color = new Color(0.25f, 0.5f, 0f);
                break;
            default:
                color = new Color(0.75f, 0f, 0f);
                break;
        }
        exitButton.GetComponent<Image>().color = color;
        customizeButton.GetComponent<Image>().color = color;
        gearButton.GetComponent<Image>().color = color;
        importButton.GetComponent<Image>().color = color;
        openButton.GetComponent<Image>().color = color;
        foreach (Button button in leftPanel.GetComponentsInChildren<Button>())
        {
            button.GetComponent<Image>().color = color;
        }
        foreach (Button button in rightPanel.GetComponentsInChildren<Button>())
        {
            button.GetComponent<Image>().color = color;
        }
    }

    //Setup shadpeshift forms panel
    private void SetupFormPanel()
    {
        Mask[] buttons = formPanel.GetComponentsInChildren<Mask>(true);
        foreach (Mask button in buttons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }
        if (character.Race == 22)
        {
            buttons[7].transform.parent.gameObject.SetActive(true);
            buttons[6].transform.parent.gameObject.SetActive(true);
        }
        ChangeBorder(buttons[7].GetComponentInChildren<Button>().gameObject);
        if (character.Class == 11)
        {
            buttons[7].transform.parent.gameObject.SetActive(true);
            buttons[5].transform.parent.gameObject.SetActive(Array.Exists(character.Options, o => o.Name == "Bear Form"));
            buttons[4].transform.parent.gameObject.SetActive(Array.Exists(character.Options, o => o.Name == "Cat Form"));
            buttons[3].transform.parent.gameObject.SetActive(Array.Exists(character.Options, o => o.Name == "Moonkin Form"));
            buttons[2].transform.parent.gameObject.SetActive(Array.Exists(character.Options, o => o.Name == "Flight Form"));
            buttons[1].transform.parent.gameObject.SetActive(Array.Exists(character.Options, o => o.Name == "Aquatic Form"));
            buttons[0].transform.parent.gameObject.SetActive(Array.Exists(character.Options, o => o.Name == "Travel Form"));
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
            foreach (Image image in images)
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

    //Reset round buttons borders
    private void ResetBorder()
    {
        Image[] images = classPanel.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
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
            foreach (Image image in images)
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
            foreach (Image image in images)
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
        foreach (Button button in buttons)
        {
            string icon = button.image.sprite.name.Replace("female", "").Replace("male", "");
            icon += gender ? "male" : "female";
            Sprite sprite = sprites.Single(s => s.name == icon);
            button.image.sprite = sprite;
        }
        buttons = hordePanel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            string icon = button.image.sprite.name.Replace("female", "").Replace("male", "");
            icon += gender ? "male" : "female";
            Sprite sprite = sprites.Single(s => s.name == icon);
            button.image.sprite = sprite;
        }
        formPanel.SetActive(true);
        buttons = formPanel.GetComponentsInChildren<Button>(true);
        for (int i = 6; i < 8; i++)
        {
            string icon = buttons[i].image.sprite.name.Replace("female", "").Replace("male", "");
            icon += gender ? "male" : "female";
            Sprite sprite = sprites.Single(s => s.name == icon);
            buttons[i].image.sprite = sprite;
        }
        formPanel.SetActive(false);
    }

    //Link customization dropdowns to customization options stored in Character object
    private void LinkDropdowns()
    {
        character.CustomizationDropdowns.Clear();
        foreach (GameObject option in customizationOptions)
        {
            character.CustomizationDropdowns.Add(option.GetComponentInChildren<CustomDropdown>(true));
        }
    }

    //Load Gilenan model
    private void LoadGilnean()
    {
        string model;
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = 23 AND Gender = {character.Gender};";
            SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            model = reader.GetString(3);
            gilnean.RacePath = reader.GetString(4);
        }
        connection.Close();
        ClearItems();
        gilnean.Gender = character.Gender;
        gilnean.LoadModel(model, casc);
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
        Category(0);
        string model;
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = {character.Race} AND Gender = {gender};";
            SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            model = reader.GetString(3);
            character.RacePath = reader.GetString(4);
            character.DemonHunterFile = reader.IsDBNull(5) ? null : reader.GetString(5);
            character.RacialCollection = reader.IsDBNull(6) ? null : reader.GetString(6);
        }
        connection.Close();
        ClearItems();
        int c = character.Class;
        character.Class = 1;
        character.Form = 0;
        SetupCustomizationPanel();
        GetCustomizationOptions();
        LinkDropdowns();
        ChangeButtonColors();
        ClassButton(c);
        gilnean.gameObject.SetActive(true);
        character.LoadModel(model, casc);
        if (character.Race == 22)
        {
            LoadGilnean();
        }
        else
        {
            gilnean.UnloadModel();
        }
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
        formPanel.gameObject.SetActive(true);
        mainFormButton.image.sprite = current.image.sprite;
        formPanel.gameObject.SetActive(false);
        ChangeRaceBorder(EventSystem.current.currentSelectedGameObject);
        if (character.Race == race)
        {
            return;
        }
        Category(0);
        bool value;
        string model;
        connection.Open();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceClassCombos WHERE Race = {race};";
            SqliteDataReader reader = command.ExecuteReader();
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
            SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            model = reader.GetString(3);
            character.RacePath = reader.GetString(4);
            character.DemonHunterFile = reader.IsDBNull(5) ? null : reader.GetString(5);
            character.RacialCollection = reader.IsDBNull(6) ? null : reader.GetString(6);
        }
        connection.Close();
        ClearItems();
        character.Race = race;
        character.Class = 1;
        character.Form = 0;
        SetupCustomizationPanel();
        GetCustomizationOptions();
        ResetBorder();
        LinkDropdowns();
        ChangeButtonColors();
        character.LoadModel(model, casc);
        gilnean.gameObject.SetActive(true);
        if (race == 22)
        {
            LoadGilnean();
        }
        else
        {
            gilnean.UnloadModel();
        }
        druid.gameObject.SetActive(false);
    }

    //Toggle different shapeshift form
    public void FormButton(int form)
    {
        if (character.Form == form)
        {
            return;
        }
        ChangeBorder(EventSystem.current.currentSelectedGameObject);
        character.Form = form;
        SetupCategories();
        switch (form)
        {
            case 0:
                gilnean.gameObject.SetActive(false);
                druid.gameObject.SetActive(false);
                character.gameObject.SetActive(true);
                Category(0);
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                gilnean.gameObject.SetActive(false);
                druid.gameObject.SetActive(true);
                character.gameObject.SetActive(false);
                Category(3);
                break;
            case 7:
                gilnean.gameObject.SetActive(true);
                druid.gameObject.SetActive(false);
                character.gameObject.SetActive(false);
                Category(0);
                break;
        }
        Dropdown(Array.FindIndex(character.Options, o => o.Form == character.Form));
    }

    //Set class
    public void ClassButton(int id)
    {
        if (id == character.Class)
        {
            return;
        }
        ChangeBorder(EventSystem.current.currentSelectedGameObject);
        Category(0);
        character.Class = id;
        int[] customization = character.Customization;
        SetupCustomizationPanel();
        GetCustomizationOptions();
        LinkDropdowns();
        for (int i = 0; i < customization.Length; i++)
        {
            EventSystem.current.SetSelectedGameObject(customizationOptions[i].GetComponentInChildren<Dropdown>(true).gameObject);
            customization[i] = customization[i] >= character.Choices[i].Length ? character.Choices[i].Length - 1 : customization[i];
            customizationOptions[i].GetComponentInChildren<CustomDropdown>(true).SetValue(customization[i]);
        }
        ChangeButtonColors();
        character.InitializeHelper();
        character.Change = true;
    }

    //Select customization category
    public void Category(int index)
    {
        string icon;
        Sprite sprite;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Icons/charactercreate");
        Button[] buttons = customizationPanel.GetComponentInChildren<HorizontalLayoutGroup>().GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            icon = buttons[i].image.sprite.name.Replace("on", "off");
            sprite = sprites.Single(s => s.name == icon);
            buttons[i].image.sprite = sprite;
        }
        icon = buttons[index].image.sprite.name.Replace("off", "on");
        sprite = sprites.Single(s => s.name == icon);
        buttons[index].image.sprite = sprite;
        for (int i = 0; i < customizationOptions.Count; i++)
        {
            if (character.Options[i].Category == index + 1 && customizationOptions[i].GetComponentInChildren<Dropdown>().options.Count > 1 && character.Form == character.Options[i].Form)
            {
                customizationOptions[i].SetActive(true);
            }
            else
            {
                customizationOptions[i].SetActive(false);
            }
        }
    }

    //Setup display for newly selected dropdown value and load new druid form when applicable
    public void Dropdown(int index)
    {
        CustomDropdown dropdown = customizationOptions[index].GetComponentInChildren<CustomDropdown>();
        character.Customization[index] = dropdown.GetValue();
        string str = dropdown.captionText.text;
        str = str.Substring(str.IndexOf(':') + 1);
        dropdown.captionText.text = str;
        Text text = dropdown.GetComponentInChildren<Text>();
        TextGenerationSettings settings = new TextGenerationSettings();
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
        if (druid.gameObject.activeSelf)
        {
            druid.LoadModel(druidModels[character.Choices[index][character.Customization[index]].Model], casc);
            ParticleColor[] particleColors = new ParticleColor[3];
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                int start, mid, end;
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT * FROM ParticleColors WHERE ID = {character.Choices[index][character.Customization[index]].Textures[0].Texture4};";
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    start = reader.GetInt32(1);
                    mid = reader.GetInt32(4);
                    end = reader.GetInt32(7);
                    particleColors[0] = new ParticleColor(start, mid, end);
                    start = reader.GetInt32(2);
                    mid = reader.GetInt32(5);
                    end = reader.GetInt32(8);
                    particleColors[1] = new ParticleColor(start, mid, end);
                    start = reader.GetInt32(3);
                    mid = reader.GetInt32(6);
                    end = reader.GetInt32(9);
                    particleColors[2] = new ParticleColor(start, mid, end);
                }
            }
            connection.Close();
            druid.ParticleColors = particleColors;
        }
        character.Change = true;
        gilnean.Change = true;
        druid.Change = true;
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

    //Equipment button on the left toggles equipement panel on the left side
    public void LeftButton(int slot)
    {
        rightPanel.gameObject.SetActive(false);
        items.Clear();
        foreach (GameObject item in scrollItems)
        {
            Destroy(item);
        }
        scrollItems.Clear();
        slotName = WoWHelper.Slot(slot);
        leftPanel.gameObject.SetActive(true);
        leftPanel.GetComponentInChildren<InputField>().text = "";
        FillItems(slot, leftPanel);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    //Equipment button on the right toggles equipement panel on the righ side
    public void RightButton(int slot)
    {
        leftPanel.gameObject.SetActive(false);
        items.Clear();
        foreach (GameObject item in scrollItems)
        {
            Destroy(item);
        }
        scrollItems.Clear();
        slotName = WoWHelper.Slot(slot);
        rightPanel.gameObject.SetActive(true);
        rightPanel.GetComponentInChildren<InputField>().text = "";
        FillItems(slot, rightPanel);
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    //Equip selected item when pressed OK on equipment panel
    public void OkButton()
    {
        rotate = false;
        translate = false;
        bool none = true;
        GameObject panel = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;
        for (int i = 0; i < scrollItems.Count; i++)
        {
            if (scrollItems[i].GetComponent<Toggle>().isOn)
            {
                none = false;
                GameObject.Find(slotName).GetComponent<Image>().sprite = scrollItems[i].GetComponentInChildren<Image>().sprite;
                int j = int.Parse(scrollItems[i].name);
                character.Items[WoWHelper.Slot(slotName)] = new ItemModel(items[j].ID, items[j].Version, items[j].Slot, items[j].Display, character.Race, character.Gender);
                if (character.Items[7] != null && (character.Items[7].Slot == 15))
                {
                    character.Items[12] = null;
                    GameObject.Find("offhand").GetComponent<Image>().sprite = Resources.Load<Sprite>(@"Icons\ui-paperdoll-slot-offhand");
                }
                break;
            }
        }
        if (none)
        {
            character.Items[WoWHelper.Slot(slotName)] = null;
            GameObject.Find(slotName).GetComponent<Image>().sprite = Resources.Load<Sprite>(@"Icons\ui-paperdoll-slot-" + slotName);
        }
        panel.GetComponentInChildren<InputField>().text = "";
        panel.SetActive(false);
        items.Clear();
        if (scrollItems.Count > 0)
        {
            foreach (GameObject item in scrollItems)
            {
                DestroyImmediate(item);
            }
            scrollItems.Clear();
        }
        character.Change = true;
        gilnean.Change = true;
        PointerExit();
    }

    //Close equipment panel without equiping anything with Cancel button
    public void CancelButton()
    {
        rotate = false;
        translate = false;
        GameObject panel = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject;
        panel.GetComponentInChildren<InputField>().text = "";
        EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
        items.Clear();
        if (scrollItems.Count > 0)
        {
            foreach (GameObject item in scrollItems)
            {
                DestroyImmediate(item);
            }
            scrollItems.Clear();
        }
        PointerExit();
    }

    //Filter displayed items using filter box
    public void Filter(string text)
    {
        Transform parent = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.GetComponentInChildren<VerticalLayoutGroup>().transform;
        if (scrollItems.Count > 0)
        {
            foreach (GameObject item in scrollItems)
            {
                DestroyImmediate(item);
            }
            scrollItems.Clear();
        }
        Toggle none = parent.parent.GetComponentInChildren<Toggle>();
        if (text == "")
        {
            for (int i = 0; i < 100; i++)
            {
                scrollItems.Add(Instantiate(scrollItem, parent));
                scrollItems[i].name = i.ToString();
                scrollItems[i].GetComponent<Toggle>().group = parent.parent.GetComponentInChildren<ToggleGroup>();
                scrollItems[i].GetComponentInChildren<Text>().text = items[i].Name;
                scrollItems[i].GetComponentInChildren<Text>().color = WoWHelper.QualityColor(items[i].Quality);
                scrollItems[i].GetComponentInChildren<Image>().sprite = IconFromBLP(items[i].Icon);
                scrollItems[i].GetComponentsInChildren<Image>()[1].color = WoWHelper.QualityColor(items[i].Quality);
                Image tooltip = Array.Find(scrollItems[i].GetComponentsInChildren<Image>(true), x => x.CompareTag("tooltip"));
                tooltip.gameObject.SetActive(true);
                tooltip.GetComponentInChildren<Text>().text = items[i].Name;
                tooltip.GetComponentInChildren<Text>().color = WoWHelper.QualityColor(items[i].Quality);
                tooltip.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0, j = 0; i < 100; j++)
            {
                if (j >= items.Count)
                {
                    break;
                }
                if (items[j].Name.ToLower().Contains(text.ToLower()))
                {
                    scrollItems.Add(Instantiate(scrollItem, parent));
                    scrollItems[i].name = j.ToString();
                    scrollItems[i].GetComponent<Toggle>().group = parent.parent.GetComponentInChildren<ToggleGroup>();
                    scrollItems[i].GetComponentInChildren<Text>().text = items[j].Name;
                    scrollItems[i].GetComponentInChildren<Text>().color = WoWHelper.QualityColor(items[j].Quality);
                    scrollItems[i].GetComponentInChildren<Image>().sprite = IconFromBLP(items[j].Icon);
                    scrollItems[i].GetComponentsInChildren<Image>()[1].color = WoWHelper.QualityColor(items[j].Quality);
                    Image tooltip = Array.Find(scrollItems[i].GetComponentsInChildren<Image>(true), x => x.CompareTag("tooltip"));
                    tooltip.gameObject.SetActive(true);
                    tooltip.GetComponentInChildren<Text>().text = items[j].Name;
                    tooltip.GetComponentInChildren<Text>().color = WoWHelper.QualityColor(items[j].Quality);
                    tooltip.gameObject.SetActive(false);
                    i++;
                }
            }
        }
        none.isOn = true;
    }

    //If in customize mode go back, else exit the application
    public void Exit()
    {
        if (!customize)
        {
            casc.Clear();
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
            genderPanel.gameObject.SetActive(!customize);
            alliancePanel.gameObject.SetActive(!customize);
            hordePanel.gameObject.SetActive(!customize);
            classPanel.gameObject.SetActive(!customize);
            customizationPanel.gameObject.SetActive(customize);
            gearButton.gameObject.SetActive(customize);
            formPanel.gameObject.SetActive(false);
            gearPanel.gameObject.SetActive(gear);
            character.Form = 0;
            gilnean.gameObject.SetActive(false);
            druid.gameObject.SetActive(false);
            character.gameObject.SetActive(true);
            SetupCategories();
            Category(0);
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
                genderPanel.gameObject.SetActive(!customize);
                alliancePanel.gameObject.SetActive(!customize);
                hordePanel.gameObject.SetActive(!customize);
                classPanel.gameObject.SetActive(!customize);
                customizationPanel.gameObject.SetActive(customize);
                gearButton.gameObject.SetActive(customize);
                formPanel.gameObject.SetActive(true);
                SetupFormPanel();
            }
            else
            {
                Save();
            }
        }
        customizeButton.GetComponentInChildren<Text>().text = customize ? "Save" : "Customize";
        exitButton.GetComponentInChildren<Text>().text = customize ? "Back" : "Exit";
    }

    //Toggle gear panel
    public void Gear()
    {
        gear = !gear;
        if (character.Race == 22 || character.Race == 23)
        {
            formPanel.gameObject.SetActive(!gear);
        }
        customizationPanel.gameObject.SetActive(!gear);
        gearPanel.gameObject.SetActive(gear);
        if (!gear)
        {
            leftPanel.gameObject.SetActive(false);
            rightPanel.gameObject.SetActive(false);
        }
    }

    //Open saved character json file
    public void Open(string file)
    {
        if (!Directory.Exists("Save"))
        {
            Directory.CreateDirectory("Save");
        }
        string path = file == "" ? FileBrowser.OpenSingleFile("Open character", "Save", "chr") : file;
        if (path != "")
        {
            SerializableCharacter sCharacter;
            using (StreamReader reader = new StreamReader(path))
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
                ((CustomDropdown)customizationOptions[i].GetComponentInChildren<Dropdown>(true)).SetValue(character.Customization[i]);
            }
            connection.Open();
            gearPanel.gameObject.SetActive(true);
            int icon;
            for (int i = 0; i < sCharacter.items.Length; i++)
            {
                if (sCharacter.items[i].id != 0)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT [Display ID], Icon, Slot FROM Items WHERE ID = " + sCharacter.items[i].id + " AND Version = " + sCharacter.items[i].version + ";";
                        SqliteDataReader reader = command.ExecuteReader();
                        reader.Read();
                        character.Items[i] = new ItemModel(sCharacter.items[i].id, sCharacter.items[i].version, reader.GetInt32(2), reader.GetInt32(0), character.Race, character.Gender);
                        icon = reader.GetInt32(1);
                    }
                    GameObject.Find(WoWHelper.SlotName(i)).GetComponent<Image>().sprite = IconFromBLP(icon);
                }
                else
                {
                    character.Items[i] = null;
                    GameObject.Find(WoWHelper.SlotName(i)).GetComponent<Image>().sprite = Resources.Load<Sprite>(@"Icons\ui-paperdoll-slot-" + WoWHelper.SlotName(i));
                }
            }
            gearPanel.gameObject.SetActive(false);
            connection.Close();
            character.Change = true;
        }
    }

    //Show import panel
    public void Import()
    {
        importPanel.SetActive(true);
    }

    //Import character from armory when pressed OK button
    public void OKImportButton()
    {
        using (HttpClient client = new HttpClient())
        {
            InputField[] boxes = importPanel.GetComponentsInChildren<InputField>();
            string name = boxes[0].text.ToLower();
            string realm = boxes[1].text.ToLower().Replace(' ', '-');
            Dropdown dropdown = importPanel.GetComponentInChildren<Dropdown>();
            string region = "";
            switch (dropdown.value)
            {
                case 0:
                    region = "us";
                    break;
                case 1:
                    region = "eu";
                    break;
                case 2:
                    region = "kr";
                    break;
                case 3:
                    region = "tw";
                    break;
                case 4:
                    region = "cn";
                    break;
            }
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), "https://" + region + ".api.blizzard.com/profile/wow/character/" + realm + "/" + name + "/appearance?namespace=profile-" + region + "&locale=en_us&access_token=" + token))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                HttpResponseMessage response = client.SendAsync(request).Result;
                string resp = response.Content.ReadAsStringAsync().Result;
                Debug.Log(response.StatusCode);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (gear)
                    {
                        Gear();
                    }
                    if (customize)
                    {
                        Exit();
                    }
                    JObject character = JObject.Parse(resp);
                    int race = character["playable_race"].Value<int>("id");
                    bool gender = character["gender"].Value<string>("name") == "Male";
                    int id = character["playable_class"].Value<int>("id");
                    if (race == 25 || race == 26)
                    {
                        race = 24;
                    }
                    EventSystem.current.SetSelectedGameObject(GameObject.Find(races[race].ToLower()).GetComponentInChildren<Button>().gameObject);
                    RaceButton(race);
                    EventSystem.current.SetSelectedGameObject(GameObject.Find(gender ? "male" : "female").GetComponentInChildren<Button>().gameObject);
                    GenderButton(gender);
                    EventSystem.current.SetSelectedGameObject(GameObject.Find(classes[id].ToLower()).GetComponentInChildren<Button>().gameObject);
                    ClassButton(id);
                    List<JToken> list = character["customizations"].Children().ToList();
                    foreach (JToken custom in list)
                    {
                        int index = Array.FindIndex(this.character.Options, o => o.Blizzard == custom["option"].Value<int>("id"));
                        int value;
                        if (index != -1)
                        {
                            value = Array.FindIndex(this.character.Choices[index], o => o.ID == custom["choice"].Value<int>("id"));
                            if (value == -1)
                            {
                                value = Array.FindIndex(this.character.Choices[index], o => o.Model == custom["choice"].Value<int>("id"));
                            }
                            if (value == -1)
                            {
                                value = Array.FindIndex(this.character.Choices[index], o => o.Bone == custom["choice"].Value<int>("id"));
                            }
                            if (value == -1)
                            {
                                value = 0;
                            }
                            this.character.CustomizationDropdowns[index].SetValue(value);
                        }
                    }
                    list = character["items"].Children().ToList();
                    connection.Open();
                    gearPanel.gameObject.SetActive(true);
                    foreach (JToken item in list)
                    {
                        EquipItem(item);
                    }
                    gearPanel.gameObject.SetActive(false);
                    connection.Close();
                    this.character.Change = true;
                }
            }
        }
        importPanel.SetActive(false);
        PointerExit();
    }

    //Equip item for imported character
    private void EquipItem(JToken item)
    {
        int icon;
        int slot = WoWHelper.Slot(item["slot"].Value<string>("type").Replace("_", ""));
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT [Display ID], Icon, Slot FROM Items WHERE ID = " + item.Value<int>("id") + " AND Version = " + item.Value<int>("item_appearance_modifier_id") + "; ";
            SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            character.Items[slot] = new ItemModel(item.Value<int>("id"), item.Value<int>("item_appearance_modifier_id"), reader.GetInt32(2), reader.GetInt32(0), character.Race, character.Gender);
            icon = reader.GetInt32(1);
        }
        GameObject.Find(WoWHelper.SlotName(slot)).GetComponent<Image>().sprite = IconFromBLP(icon);
    }

    //Close import panel without importing with Cancel button
    public void CancelImportButton()
    {
        importPanel.SetActive(false);
        PointerExit();
    }

    //Save character as json file
    public void Save()
    {
        if (!Directory.Exists("Save"))
        {
            Directory.CreateDirectory("Save");
        }
        string path = FileBrowser.SaveFile("Save character", "Save", races[character.Race] + " " + classes[character.Class], "chr");
        if (path != "")
        {
            SerializableCharacter sCharacter = new SerializableCharacter();
            sCharacter.raceid = character.Race;
            sCharacter.gender = character.Gender;
            sCharacter.classid = character.Class;
            sCharacter.customization = new int[character.Customization.Length];
            for (int i = 0; i < sCharacter.customization.Length; i++)
            {
                sCharacter.customization[i] = character.Customization[i];
            }
            sCharacter.items = new SerializableItems[character.Items.Length];
            for (int i = 0; i < sCharacter.items.Length; i++)
            {
                sCharacter.items[i] = new SerializableItems();
                if (character.Items[i] == null)
                {
                    sCharacter.items[i].id = 0;
                    sCharacter.items[i].version = 0;
                }
                else
                {
                    sCharacter.items[i].id = character.Items[i].ID;
                    sCharacter.items[i].version = character.Items[i].Version;
                }
            }
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(JsonConvert.SerializeObject(sCharacter, Formatting.Indented));
            }
        }
    }
}
