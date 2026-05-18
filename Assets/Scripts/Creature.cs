using Assets.WoW;
using CASCLib;
using M2Lib;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;
using WoW;
using static WoW.WoWHelper;

public class Creature : ModelRenderer
{
    // Reference to the character
    public Character character;

    // Main creature models
    private Dictionary<int, string> creatureModels;
    // Mapping of Imp models to their colors
    private Dictionary<int, int[]> impStyleColors;
    // Mapping which Imp models have flames option
    private Dictionary<int, int[]> impStyleFlames;
    // Mapping of Felhunter models to their colors
    private Dictionary<int, int[]> felhunterStyleColors;
    // Mapping of Voidwalker models to their colors
    private Dictionary<int, int[]> voidwalkerStyleColors;
    // Mapping of Felguard models to their colors
    private Dictionary<int, int[]> felguardStyleColors;
    // Mapping of Doomguard models to their colors
    private Dictionary<int, int[]> doomguardStyleColors;
    // Mapping of Sayaad models to their colors
    private Dictionary<int, int[]> sayaadStyleColors;
    // Mapping of Infernal models to their colors
    private Dictionary<int, int[]> infernalStyleColors;
    // Mapping of Darkglare models to their colors
    private Dictionary<int, int[]> darkglerStyleColors;
    // Mapping of Tyrant models to their colors
    private Dictionary<int, int[]> tyrantStyleColors;
    // Mapping of Moonkin decorations to their colors
    private Dictionary<int, int[]> decorationColors;
    // Mapping of Moonkin effects to their colors
    private Dictionary<int, int[]> effectsColors;
    // Mapping of Night Elf druid forms
    private Dictionary<int, int[]> nightElfForms;
    // Mapping of Tauren druid forms
    private Dictionary<int, int[]> taurenForms;
    // Mapping of Troll druid forms
    private Dictionary<int, int[]> trollForms;
    // Mapping of Worgen druid forms
    private Dictionary<int, int[]> worgenForms;
    // Mapping of Highmountain druid forms
    private Dictionary<int, int[]> highmountainForms;
    // Mapping of Kul Tiran druid forms
    private Dictionary<int, int[]> kulTiranForms;
    // Mapping of Zandalari druid forms
    private Dictionary<int, int[]> zandalariForms;
    // Mapping of Haranir druid forms
    private Dictionary<int, int[]> haranirForms;
    // Customizations available for various bear forms
    private Dictionary<int, int[]> bearFormCustomizations;
    // Customizations available for various cat forms
    private Dictionary<int, int[]> catFormCustomizations;
    // Customizations available for various aquatic forms
    private Dictionary<int, int[]> aquaticFormCustomizations;
    // Customizations available for various travel forms
    private Dictionary<int, int[]> travelFormCustomizations;
    // Customizations available for various flight forms
    private Dictionary<int, int[]> flightFormCustomizations;
    // Customizations available for various moonkin forms
    private Dictionary<int, int[]> moonkinFormCustomizations;
    // Customizations available for various treant forms
    private Dictionary<int, int[]> treantFormCustomizations;
    // Mapping Batbear customization to specific model
    private Dictionary<int, int[]> batbearCustomizations;
    // index of selected Batbear model
    public int batbear;
    // Geosets that need to be hidden for specific models
    private Dictionary<string, int[]> hiddenGeosets;
    // Particle colors used by the model
    private ParticleColor[] particleColors;
    // model file name
    private string model;
    // Reference to the main camera
    private new Transform camera;

    private void Start()
    {
        // Initiazlie
        modelsPath = @"creature\";
        camera = Camera.main.transform;
        activeGeosets = new();
        var connection = new SqliteConnection($"URI=file:{Application.streamingAssetsPath}/database.sqlite");
        connection.Open();
        creatureModels = DB.GetCreatureModels(connection);
        connection.Close();
        impStyleColors = new()
        {
            { 4084, new int[] { 32186, 32183, 32184, 32185 } },
            { 4085, new int[] { 32187, 32193, 32192, 32191, 32190, 32189, 32188 } },
            { 4086, new int[] { 50555, 50556, 50557, 50558 } }
        };
        impStyleFlames = new()
        {
            { 365, new int[] { 18705, 18706, 50548 } },
            { 3723, new int[] { 18706 } }
        };
        felhunterStyleColors = new()
        {
            { 4087, new int[] { 54412, 54410, 54411 } },
            { 4088, new int[] { 54206, 54207 } }
        };
        voidwalkerStyleColors = new()
        {
            { 4090, new int[] { 54639, 54640 } },
            { 4091, new int[] { 54654, 54657 } },
            { 4106, new int[] { 54633, 54634, 54635, 54636, 54637 } }
        };
        felguardStyleColors = new()
        {
            { 4094, new int[] { 54680, 54681, 54682, 54683, 54684 } },
            { 4095, new int[] { 54689, 54724, 54725 } }
        };
        doomguardStyleColors = new()
        {
            { 4096, new int[] { 54692, 54722, 54723 } }
        };
        sayaadStyleColors = new()
        {
            { 4098, new int[] { 54698, 54704, 54705 } },
            { 4099, new int[] { 54699 } },
            { 4100, new int[] { 54702, 54713, 54714, 54715 } }
        };
        infernalStyleColors = new()
        {
            { 4101, new int[] { 54708, 54716, 54717 } },
            { 4102, new int[] { 54710, 54718, 54726 } }
        };
        darkglerStyleColors = new()
        {
            { 4407, new int[] { 55677, 55771, 55671, 55674, 55672, 55675 } },
            { 4408, new int[] { 55683, 55682, 55681, 55680, 55679 } },
            { 4409, new int[] { 55684 } }
        };
        tyrantStyleColors = new()
        {
            { 4410, new int[] { 55693, 55690, 55688, 55691, 55689 } }
        };
        decorationColors = new()
        {
            { 4168, new int[] { 55619 } },
            { 4169, new int[] { 55620, 55621, 55622, 55623, 55624 } },
            { 4281, new int[] { 55620, 55621, 55622, 55623, 55624 } }
        };
        effectsColors = new()
        {
            { 4166, new int[] { 55625 } },
            { 4167, new int[] { 55626, 55627, 55628, 55629 } },
            { 4280, new int[] { 55626, 55627, 55628, 55629 } }
        };
        nightElfForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4283, 4284, 4285, 4286, 4287, 4288, 4289, 4290, 4291,
                4319, 4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4344, 4345, 4346, 4347, 4348, 4376, 4377, 4378, 4379, 4380,
                4381, 4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4267, 4268, 4269, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4223, 4224, 4225, 4226, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4159, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        taurenForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4297, 4298, 4299, 4300, 4301, 4319,
                4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4339, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4354, 4355, 4356, 4357, 4358, 4377, 4378, 4379, 4380, 4381,
                4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4402 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4270, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4231, 4233, 4238, 4240, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4160, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        trollForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4302, 4303, 4304, 4305, 4306, 4319,
                4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4340, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4359, 4360, 4361, 4362, 4363, 4377, 4378, 4379, 4380, 4381,
                4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4401 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4270, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4232, 4236, 4239, 4242, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4161, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        worgenForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4292, 4293, 4294, 4295, 4296, 4319,
                4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4341, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4349, 4350, 4351, 4352, 4353, 4377, 4378, 4379, 4380, 4381,
                4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4403 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4267, 4268, 4269, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4227, 4228, 4229, 4230, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4162, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        highmountainForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4307, 4308, 4309, 4310, 4319,
                4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4364, 4365, 4366, 4367, 4377, 4378, 4379, 4380, 4381,
                4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4275, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4231, 4234, 4238, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4163, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        kulTiranForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4311, 4312, 4313, 4314, 4319,
                4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4372, 4373, 4374, 4375, 4377, 4378, 4379, 4380, 4381,
                4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4253, 4254, 4255, 4260, 4261, 4262, 4263 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4271, 4272, 4273, 4274, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4230, 4231, 4235, 4241, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4158, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        zandalariForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4315, 4316, 4317, 4318, 4319,
                4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4368, 4369, 4370, 4371, 4377, 4378, 4379, 4380, 4381,
                4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4253, 4254, 4255, 4256, 4257, 4258, 4259 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4276, 4277, 4278 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4232, 4237, 4239, 4243, 4244, 4245, 4246, 4247, 4248, 4249, 4649 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4164, 4279 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141 } }
        };
        haranirForms = new()
        {
            { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4288, 4289, 4290, 4291, 4319, 4320, 4321,
                4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342, 4566 } },
            { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4376, 4377, 4378, 4379, 4380, 4381, 4382, 4383,
                4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4566 } },
            { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 141, 4183, 4253, 4254, 4255, 4566 } },
            { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4277, 4278, 4566 } },
            { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4224, 4226, 4244, 4245, 4246, 4247, 4248, 4249, 4566, 4649, 4890 } },
            { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4566, 4657 } },
            { (int)WoWHelper.CreatureForm.TreantForm, new int[] { 141, 4566 } }
        };
        bearFormCustomizations = new()
        {
            { 4566, new int[] { 6513 } }
        };
        catFormCustomizations = new()
        {
            { 4566, new int[] { 7150 } }
        };
        aquaticFormCustomizations = new()
        {
            { 141, new int[] { 7206, 7207, 7210, 7211, 7212, 15485, 15495, 15496, 15504, 15505, 15506, 15511, 15512, 55655 } },
            { 4566, new int[] { 7206 } }
        };
        travelFormCustomizations = new()
        {
            { 4566, new int[] { 7227 } }
        };
        flightFormCustomizations = new()
        {
            { 4566, new int[] { 7243 } },
            { 4890, new int[] { 7243 } }
        };
        moonkinFormCustomizations = new()
        {
            { 141, new int[] { 9987 } },
            { 4279, new int[] { 9987 } },
            { 4657, new int[] { 62148 } }
        };
        treantFormCustomizations = new()
        {
            { 141, new int[] { 62154, 62153 } },
            { 4566, new int[] { 62153 } }
        };
        batbearCustomizations = new()
        {
            { 62155, new int[] { 62197, 62070, 62080 } },
            { 62167, new int[] { 62197, 62071, 62080 } },
            { 62168, new int[] { 62197, 62072, 62080 } },
            { 62169, new int[] { 62197, 62072, 62081 } },
            { 62170, new int[] { 62197, 62071, 62081 } },
            { 62171, new int[] { 62197, 62070, 62081 } },
            { 62172, new int[] { 62198, 62072, 62081 } },
            { 62173, new int[] { 62198, 62071, 62081 } },
            { 62175, new int[] { 62198, 62070, 62081 } },
            { 62176, new int[] { 62198, 62072, 62080 } },
            { 62177, new int[] { 62198, 62071, 62080 } },
            { 62178, new int[] { 62198, 62070, 62080 } },
            { 62179, new int[] { 62199, 62070, 62080 } },
            { 62180, new int[] { 62199, 62071, 62080 } },
            { 62181, new int[] { 62199, 62072, 62080 } },
            { 62182, new int[] { 62199, 62070, 62081 } },
            { 62183, new int[] { 62199, 62071, 62081 } },
            { 62184, new int[] { 62199, 62072, 62081 } },
            { 62185, new int[] { 62200, 62072, 62081 } },
            { 62186, new int[] { 62200, 62071, 62081 } },
            { 62187, new int[] { 62200, 62070, 62081 } },
            { 62188, new int[] { 62200, 62072, 62080 } },
            { 62189, new int[] { 62200, 62071, 62080 } },
            { 62190, new int[] { 62200, 62070, 62080 } },
            { 62191, new int[] { 62201, 62070, 62080 } },
            { 62192, new int[] { 62201, 62071, 62080 } },
            { 62193, new int[] { 62201, 62072, 62080 } },
            { 62194, new int[] { 62201, 62070, 62081 } },
            { 62195, new int[] { 62201, 62071, 62081 } },
            { 62196, new int[] { 62201, 62072, 62081 } },
        };
        hiddenGeosets = new()
        {
            { @"owlbear\owlbear", new int[] { 0 } },
            { @"imp3\imp3", new int[] { 0 } },
            { @"druidflightzandalaritroll\druidflightzandalaritroll", new int[] { 0 } },
            { @"druidflightzandalaritroll_noarmor\druidflightzandalaritroll_noarmor", new int[] { 0 } },
            { @"voidwalker2\voidwalker2", new int[] { 3, 4 } },
            { @"voidlord2\voidlord2", new int[] { 4, 5 } },
            { @"infernal2_low\infernal2_low", new int[] { 6, 7, 8 } },
            { @"voidwalker_outland\voidwalker_outland", new int[] { 0, 1, 2, 3 } },
            { @"fel_abyssal_outland\fel_abyssal_outland", new int[] { 0 } },
            { @"abyssal_illidan\abyssal_illidan", new int[] { 0, 3, 4 } }
        };
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            // Steup and render model
            if (Change)
            {
                try
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    LoadTextures();
                    ParticleEffects();
                    for (int i = 0; i < Model.Skin.Textures.Length; i++)
                    {
                        SetMaterial(renderer, i);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                }
                Change = false;
            }
            // Rotate billboards
            for (int i = 0; i < Model.Skeleton.Bones.Length; i++)
            {
                if ((Model.Skeleton.Bones[i].Flags & 0x8) != 0)
                {
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x + 90, camera.eulerAngles.y + 90, camera.eulerAngles.z);
                }
            }
            // Animate textures
            if (character.Loaded)
            {
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    AnimateTextures(renderer, i);
                    AnimateColors(renderer, i);
                }
                for (int i = 0; i < textureTime.Length; i++)
                {
                    textureTime[i] += Time.deltaTime;
                }
                for (int i = 0; i < colorTime.Length; i++)
                {
                    colorTime[i] += Time.deltaTime;
                }
                for (int i = 0; i < transparencyTime.Length; i++)
                {
                    transparencyTime[i] += Time.deltaTime;
                }
            }
        }
    }

    // Activate optiopns available for selected form
    public void ActivateRelatedOptions()
    {
        if (character.Class == WoWHelper.Class.Warlock)
        {
            switch (character.ModelID)
            {
                case (int)WoWHelper.CreatureForm.Imp:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", impStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Felhunter:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", felhunterStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Voidwalker:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", voidwalkerStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Felguard:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", felguardStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Doomguard:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", doomguardStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Sayaad:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", sayaadStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Infernal:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", infernalStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Darkglare:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", darkglerStyleColors);
                    break;
                case (int)WoWHelper.CreatureForm.Tyrant:
                    character.helper.ActivateRelatedTextureOptions("Style", "Color", tyrantStyleColors);
                    break;
            }
        }
    }

    // Show and hide otpions based on selected form and customization
    public void ChangeRelatedOptions()
    {
        if (character.Class == WoWHelper.Class.Warlock)
        {
            switch (character.ModelID)
            {
                case (int)WoWHelper.CreatureForm.Imp:
                    character.helper.ChangeRelatedTextureOptions("Style", "Flames", impStyleFlames);
                    break;
            }
        }
        else if (character.Class == WoWHelper.Class.Druid)
        {
            switch (character.ModelID)
            {
                case (int)WoWHelper.CreatureForm.BearForm:
                    character.helper.ChangeRelatedTextureOptions("Bear Form", "Ears", bearFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Bear Form", "Nose", bearFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Bear Form", "Color", bearFormCustomizations);
                    break;
                case (int)WoWHelper.CreatureForm.CatForm:
                    character.helper.ChangeRelatedTextureOptions("Cat Form", "Ears", catFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Cat Form", "Nose", catFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Cat Form", "Color", catFormCustomizations);
                    break;
                case (int)WoWHelper.CreatureForm.AquaticForm:
                    character.helper.ChangeRelatedTextureOptions("Aquatic Form", "Gills", aquaticFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Aquatic Form", "Tusks", aquaticFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Aquatic Form", "Brows", aquaticFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Aquatic Form", "Scale Color", aquaticFormCustomizations);
                    break;
                case (int)WoWHelper.CreatureForm.TravelForm:
                    character.helper.ChangeRelatedTextureOptions("Travel Form", "Ears", travelFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Travel Form", "Nose", travelFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Travel Form", "Color", travelFormCustomizations);
                    break;
                case (int)WoWHelper.CreatureForm.FlightForm:
                    character.helper.ChangeRelatedTextureOptions("Flight Form", "Ears", flightFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Flight Form", "Nose", flightFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Flight Form", "Color", flightFormCustomizations);
                    break;
                case (int)WoWHelper.CreatureForm.MoonkinForm:
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Feather Color", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Horns", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Beak", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Horn & Beak Color", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Eyebrows", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Whiskers", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Feathers", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Decorations", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Decorations", "Decoration Color", decorationColors);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Effects", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Effects", "Effects Color", effectsColors);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Color", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Haranir Ears", moonkinFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Moonkin Form", "Haranir Brows", moonkinFormCustomizations);
                    break;
                case (int)WoWHelper.CreatureForm.TreantForm:
                    character.helper.ChangeRelatedTextureOptions("Treant Form", "Foliage", treantFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Treant Form", "Brow Leaves", treantFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Treant Form", "Tusks", treantFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Treant Form", "Sprout", treantFormCustomizations);
                    character.helper.ChangeRelatedTextureOptions("Treant Form", "Vines", treantFormCustomizations);
                    break;
            }
            int categories = character.Options.Where(o => o.Model == character.ModelID && o.Choices.Count > 1)
                .Select(o => o.Category).Distinct().Count();
            foreach (var category in character.CustomizationCategories)
            {
                category.gameObject.SetActive(categories > 1);
            }
        }
    }

    // Show only options available for selected race
    public void ChangeRacialOptions()
    {
        if (!character.Loaded)
        {
            return;
        }   
        if (character.Class == WoWHelper.Class.Druid)
        {
            switch (character.Race)
            {
                case WoWHelper.Race.NightElf:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, nightElfForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, nightElfForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, nightElfForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, nightElfForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, nightElfForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, nightElfForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, nightElfForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.Tauren:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, taurenForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, taurenForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, taurenForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, taurenForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, taurenForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, taurenForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, taurenForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.Troll:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, trollForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, trollForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, trollForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, trollForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, trollForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, trollForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, trollForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.Worgen:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, worgenForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, worgenForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, worgenForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, worgenForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, worgenForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, worgenForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, worgenForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.Highmountain:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, highmountainForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, highmountainForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, highmountainForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, highmountainForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, highmountainForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, highmountainForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, highmountainForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.Zandalari:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, zandalariForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, zandalariForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, zandalariForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, zandalariForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, zandalariForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, zandalariForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, zandalariForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.KulTiran:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, kulTiranForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, kulTiranForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, kulTiranForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, kulTiranForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, kulTiranForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, kulTiranForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, kulTiranForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
                case WoWHelper.Race.Haranir:
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.BearForm, haranirForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.CatForm, haranirForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.AquaticForm, haranirForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TravelForm, haranirForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.FlightForm, haranirForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.MoonkinForm, haranirForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    character.helper.ChangeRacialOptions((int)WoWHelper.CreatureForm.TreantForm, haranirForms[(int)WoWHelper.CreatureForm.TreantForm]);
                    break;
            }
        }
    }

    // Change currentl model based on selected customization and form
#if UNITY_EDITOR
    public void ChangeModel(Dictionary<int, string> listFile, string dataPath)
#else
    public void ChangeModel(CASCHandler casc)
#endif
    {
        string path;
        int index;
        switch (character.Class)
        {
            case WoWHelper.Class.Warlock:
                index = Array.FindIndex(character.Options, o => o.Name == "Color" && o.Model == character.ModelID);
                switch (character.ModelID)
                {
                    case (int)WoWHelper.CreatureForm.Imp:
                        int index2 = Array.FindIndex(character.Options, o => o.Name == "Flames" && o.Model == character.ModelID);
                        path = character.Options[index].Choices[character.Customization[index]].Creatures.
                            FirstOrDefault(c => c.Related == character.Options[index2].Choices[character.Customization[index2]].ID)?.Model;
                        if (string.IsNullOrEmpty(path))
                        {
                            path = character.Options[index].Choices[character.Customization[index]].Creatures[0].Model;
                        }
                        break;
                    default:
                        path = character.Options[index].Choices[character.Customization[index]].Creatures[0].Model;
                        break;
                }
                break;
            case WoWHelper.Class.Druid:
                index = character.ModelID switch
                {
                    (int)WoWHelper.CreatureForm.BearForm => Array.FindIndex(character.Options, o => o.Name == "Bear Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.CatForm => Array.FindIndex(character.Options, o => o.Name == "Cat Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.AquaticForm => Array.FindIndex(character.Options, o => o.Name == "Aquatic Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.TravelForm => Array.FindIndex(character.Options, o => o.Name == "Travel Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.FlightForm => Array.FindIndex(character.Options, o => o.Name == "Flight Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.MoonkinForm => Array.FindIndex(character.Options, o => o.Name == "Moonkin Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.TreantForm => Array.FindIndex(character.Options, o => o.Name == "Treant Form" && o.Model == character.ModelID),
                    _ => 0,
                };
                batbear = GetBatBear();
                path = character.Options[index].Choices[character.Customization[index]].Creatures.Length > 0 ?
                    character.Options[index].Choices[character.Customization[index]].Creatures
                    .First(c => c.Related == batbear).Model : creatureModels[character.ModelID];
                break;
            default:
                index = -1;
                path = null;
                break;
        }
        activeGeosets.Clear();
        activeGeosets.Add(0);
        if (!string.IsNullOrEmpty(path) && model != path)
        {
#if UNITY_EDITOR
            LoadModel(path, listFile, dataPath);
#else
            LoadModel(path, casc);
#endif
        }
        if (character.Options[index].Choices[character.Customization[index]].Creatures.Length > 0)
        {
            LoadGeosets(character.Options[index].Choices[character.Customization[index]].Creatures
                .First(c => c.Related == batbear).Geosets);
            particleColors = character.Options[index].Choices[character.Customization[index]].Creatures
                .First(c => c.Related == batbear).ParticleColors;
        }
        else
        {
            switch (character.ModelID)
            {
                case (int)WoWHelper.CreatureForm.BearForm:
                    ChangeBearGeosets();
                    break;
                case (int)WoWHelper.CreatureForm.CatForm:
                    ChangeCatGeosets();
                    break;
                case (int)WoWHelper.CreatureForm.AquaticForm:
                    ChangeAquaticGeosets();
                    break;
                case (int)WoWHelper.CreatureForm.TravelForm:
                    ChangeTravelGeosets();
                    break;
                case (int)WoWHelper.CreatureForm.FlightForm:
                    ChangeFlightGeosets();
                    break;
                case (int)WoWHelper.CreatureForm.MoonkinForm:
                    ChangeMoonkinGeosets();
                    break;
                case (int)WoWHelper.CreatureForm.TreantForm:
                    ChangeTreantGeosets();
                    break;
            }
        }
    }

    private int GetBatBear()
    {
        int index1 = Array.FindIndex(character.Options, o => o.Name == "Color" && o.Model == (int)WoWHelper.CreatureForm.MoonkinForm);
        int index2 = Array.FindIndex(character.Options, o => o.Name == "Haranir Ears" && o.Model == (int)WoWHelper.CreatureForm.MoonkinForm);
        int index3 = Array.FindIndex(character.Options, o => o.Name == "Haranir Brows" && o.Model == (int)WoWHelper.CreatureForm.MoonkinForm);
        if (character.Options[index1].Choices.Count > 0)
        {
            return batbearCustomizations.First(b => b.Value.Contains(character.Options[index1].Choices[character.Customization[index1]].ID)
                && b.Value.Contains(character.Options[index2].Choices[character.Customization[index2]].ID)
                && b.Value.Contains(character.Options[index3].Choices[character.Customization[index3]].ID)).Key;
        }
        return 0;
    }

    // Change Bear geosets
    private void ChangeBearGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Ears");
        character.helper.ChangeGeosetOption(activeGeosets, "Nose");
    }

    // Change Cat geosets
    private void ChangeCatGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Ears");
        character.helper.ChangeGeosetOption(activeGeosets, "Nose");
    }

    // Change Aquatic geosets
    private void ChangeAquaticGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Gills");
        character.helper.ChangeGeosetOption(activeGeosets, "Tusks");
        character.helper.ChangeGeosetOption(activeGeosets, "Brows");
    }

    // Change Travel geosets
    private void ChangeTravelGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Ears");
        character.helper.ChangeGeosetOption(activeGeosets, "Nose");
    }

    // Change Flight geosets
    private void ChangeFlightGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Ears");
        character.helper.ChangeGeosetOption(activeGeosets, "Nose");
    }

    // Change Moonkin geosets
    private void ChangeMoonkinGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Horns");
        character.helper.ChangeGeosetOption(activeGeosets, "Beak");
        character.helper.ChangeGeosetOption(activeGeosets, "Eyebrows");
        character.helper.ChangeGeosetOption(activeGeosets, "Whiskers");
        character.helper.ChangeGeosetOption(activeGeosets, "Feathers");
        character.helper.ChangeGeosetOption(activeGeosets, "Decorations");
        character.helper.ChangeGeosetOption(activeGeosets, "Effects");
    }

    // Change Treant geosets
    private void ChangeTreantGeosets()
    {
        character.helper.ChangeGeosetOption(activeGeosets, "Foliage");
        character.helper.ChangeGeosetOption(activeGeosets, "Brow Leaves");
        character.helper.ChangeGeosetOption(activeGeosets, "Tusks");
        character.helper.ChangeGeosetOption(activeGeosets, "Sprout");
        character.helper.ChangeGeosetOption(activeGeosets, "Vines");
    }

    // Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (hiddenGeosets.ContainsKey(model) && hiddenGeosets[model].Contains(i))
        {
            renderer.materials[i] = new(hiddenMaterial.shader);
            renderer.materials[i].shader = hiddenMaterial.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(hiddenMaterial);
        }
        else if (activeGeosets.Count == 0 || activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            string materialName = $@"Materials\{Model.Skin.Textures[i].Shader}\{Model.Skin.Textures[i].Shader}" +
                $"{IsUnlit(Model.Materials[Model.Skin.Textures[i].Material].Flags)}{IsTwoSided(Model.Materials[Model.Skin.Textures[i].Material].Flags)}" +
                $"{IsDepth(Model.Materials[Model.Skin.Textures[i].Material].Flags)}{BlendMode(Model.Materials[Model.Skin.Textures[i].Material].Blend)}";
            var material = Resources.Load<Material>(materialName);
            if (material == null)
            {
                Debug.LogError($"Shader:{Model.Skin.Textures[i].Shader}\tFlags:{Model.Materials[Model.Skin.Textures[i].Material].Flags}" +
                    $"\tBlending:{Model.Materials[Model.Skin.Textures[i].Material].Blend}");
            }
            renderer.materials[i] = new Material(material.shader);
            renderer.materials[i].shader = material.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(material);
            SetTexture(renderer.materials[i], i);
            Debug.Log($"Shader:{Model.Skin.Textures[i].Shader}\tFlags:{Model.Materials[Model.Skin.Textures[i].Material].Flags}" +
                    $"\tBlending:{Model.Materials[Model.Skin.Textures[i].Material].Blend}");
        }
        else
        {
            renderer.materials[i] = new(hiddenMaterial.shader);
            renderer.materials[i].shader = hiddenMaterial.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(hiddenMaterial);
        }
    }

    // Setup particle effects for rendering
    private void ParticleEffects()
    {
        var particles = GetComponentsInChildren<ParticleSystem>();
        particles = particles.OrderBy(x => x.name).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            if (Model.Particles[i].ColorIndex != 0)
            {
                var colorOverLifetime = particles[i].colorOverLifetime;
                var particleColor = particleColors.Length > 0 ? particleColors[Model.Particles[i].ColorIndex - 11] : null;
                if (particleColor != null && particleColor.Start != Color.black)
                {
                    Gradient gradient = new();
                    var colorKeys = new GradientColorKey[3];
                    colorKeys[0] = new GradientColorKey(particleColor.Start, 0f);
                    colorKeys[1] = new GradientColorKey(particleColor.Mid, 0.5f);
                    colorKeys[2] = new GradientColorKey(particleColor.End, 1f);
                    var alphaKeys = new GradientAlphaKey[3];
                    alphaKeys[0] = new GradientAlphaKey(particleColor.Start.a / 255f, 0f);
                    alphaKeys[1] = new GradientAlphaKey(particleColor.Mid.a / 255f, 0.5f);
                    alphaKeys[2] = new GradientAlphaKey(particleColor.End.a / 255f, 1f);
                    gradient.SetKeys(colorKeys, alphaKeys);
                    colorOverLifetime.color = gradient;
                }
            }
            var limitVelocityOverLifetime = particles[i].limitVelocityOverLifetime;
            limitVelocityOverLifetime.enabled = true;
            limitVelocityOverLifetime.drag = Model.Particles[i].Drag;
            limitVelocityOverLifetime.multiplyDragByParticleSize = false;
            limitVelocityOverLifetime.multiplyDragByParticleVelocity = true;
            particles[i].transform.localScale = transform.lossyScale;
            var renderer = particles[i].GetComponent<ParticleSystemRenderer>();
            var material = ParticleMaterial(Model.Particles[i].Blend);
            renderer.material.shader = material.shader;
            renderer.material.CopyPropertiesFromMaterial(material);
            var temp = textures[Model.Particles[i].Textures[0]];
            Texture2D texture = new(temp.width, temp.height, TextureFormat.ARGB32, false);
            texture.SetPixels32(temp.GetPixels32());
            texture.Apply();
            renderer.material.SetTexture("_MainTex", texture);
            renderer.material.renderQueue = 3100;
        }
    }

    // Setup all the material properties
    protected override void SetTexture(Material material, int i)
    {
        material.SetTexture("_Texture1", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        if (Model.Skin.Textures[i].TextureCount > 1)
        {
            material.SetTexture("_Texture2", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]]);
        }
        if (Model.Skin.Textures[i].TextureCount > 2)
        {
            material.SetTexture("_Emission", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 2]]);
        }
        var color = Color.white;
        if (Model.Skin.Textures[i].Color != -1)
        {
            color = colors[Model.Skin.Textures[i].Color];
        }
        if (Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]].Transparency.Values[0].Length > 0)
        {
            color.a *= Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]].Transparency.Values[0][0];
        }
        material.SetColor("_Color", color);
    }

    // Enable proper geosets
    public void LoadGeosets(CustomizationGeoset[] geosets)
    {
        if (geosets.Length > 0)
        {
            foreach (var geoset in geosets)
            {
                activeGeosets.Add(geoset.Type * 100 + geoset.ID);
            }
        }
        if (model == @"owlbear\owlbear")
        {
            activeGeosets.Add(102);
        }
    }

    // Load specific texture
    private int LoadTexture(M2Texture texture, int i)
    {
        int index = 0;
        switch (character.Class)
        {
            case WoWHelper.Class.Warlock:
                index = Array.FindIndex(character.Options, o => o.Name == "Color" && o.Model == character.ModelID);
                break;
            case WoWHelper.Class.Druid:
                index = character.ModelID switch
                {
                    (int)WoWHelper.CreatureForm.BearForm => Array.FindIndex(character.Options, o => o.Name == "Bear Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.CatForm => Array.FindIndex(character.Options, o => o.Name == "Cat Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.AquaticForm => Array.FindIndex(character.Options, o => o.Name == "Aquatic Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.TravelForm => Array.FindIndex(character.Options, o => o.Name == "Travel Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.FlightForm => Array.FindIndex(character.Options, o => o.Name == "Flight Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.MoonkinForm => Array.FindIndex(character.Options, o => o.Name == "Moonkin Form" && o.Model == character.ModelID),
                    (int)WoWHelper.CreatureForm.TreantForm => Array.FindIndex(character.Options, o => o.Name == "Treant Form" && o.Model == character.ModelID),
                    _ => 0,
                };
                break;
        }
        int file = -1;
        if (character.Options[index].Choices[character.Customization[index]].Creatures.Length > 0)
        {
            switch (texture.Type)
            {
                case 0:
                    file = Model.TextureIDs[i];
                    break;
                case 11:
                    file = character.Options[index].Choices[character.Customization[index]].Creatures.First(c => c.Related == batbear).Texture0;
                    break;
                case 12:
                    file = character.Options[index].Choices[character.Customization[index]].Creatures.First(c => c.Related == batbear).Texture1;
                    break;
                case 13:
                    file = character.Options[index].Choices[character.Customization[index]].Creatures.First(c => c.Related == batbear).Texture2;
                    break;
            }
        }
        else
        {
            switch (texture.Type)
            {
                case 0:
                    file = Model.TextureIDs[i];
                    break;
                case 5:
                    index = Array.FindIndex(character.Options, o => o.Name == "Effects Color" && o.Model == character.ModelID);
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                    break;
                case 11:
                    index = GetBody1ColorIndex();
                    file = character.Options[index].Choices[character.Customization[index]].Textures.FirstOrDefault(t => t.Target == GetBody1Target()).ID;
                    break;
                case 12:
                    index = GetBody2ColorIndex();
                    file = character.Options[index].Choices[character.Customization[index]].Textures.FirstOrDefault(t => t.Target == GetBody2Target()).ID;
                    break;
                case 13:
                    index = Array.FindIndex(character.Options, o => o.Name == "Decoration Color" && o.Model == character.ModelID);
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                    break;
            }
        }
        return file;
    }

    // Get id of Body Color option
    private int GetBody1ColorIndex()
    {
        int index = character.ModelID switch
        {
            (int)WoWHelper.CreatureForm.AquaticForm =>
                Array.FindIndex(character.Options, o => o.Name == "Scale Color" && o.Model == character.ModelID),
            (int)WoWHelper.CreatureForm.MoonkinForm => 
                Array.FindIndex(character.Options, o => o.Name == "Feather Color" && o.Model == character.ModelID),
            _ => Array.FindIndex(character.Options, o => o.Name == "Color" && o.Model == character.ModelID)
        };
        return index;
    }

    // Get target id for Ornaments for specific race
    private int GetBody1Target()
    {
        int target = character.ModelID switch
        {
            (int)WoWHelper.CreatureForm.AquaticForm => 1,
            (int)WoWHelper.CreatureForm.MoonkinForm => 1,
            _ => 2
        };
        return target;
    }

    // Get id of Body Color option
    private int GetBody2ColorIndex()
    {
        int index = character.ModelID switch
        {
            (int)WoWHelper.CreatureForm.AquaticForm =>
                Array.FindIndex(character.Options, o => o.Name == "Scale Color" && o.Model == character.ModelID),
            (int)WoWHelper.CreatureForm.MoonkinForm =>
                Array.FindIndex(character.Options, o => o.Name == "Horn & Beak Color" && o.Model == character.ModelID),
            _ => Array.FindIndex(character.Options, o => o.Name == "Color" && o.Model == character.ModelID)
        };
        return index;
    }

    // Get target id for Ornaments for specific race
    private int GetBody2Target()
    {
        int target = character.ModelID switch
        {
            (int)WoWHelper.CreatureForm.MoonkinForm => 10,
            _ => 25
        };
        return target;
    }

    // Load and prepare all model textures
    private void LoadTextures()
    {
        Texture2D texture;
        for (int i = 0; i < textures.Length; i++)
        {
            int file = LoadTexture(Model.Textures[i], i);
            if (file == -1)
            {
                textures[i] = new Texture2D(200, 200);
            }
            else
            {
                texture = TextureFromBLP(file);
                textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                textures[i].SetPixels32(texture.GetPixels32());
                textures[i].wrapModeU = (Model.Textures[i].Flags & 1) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].wrapModeV = (Model.Textures[i].Flags & 2) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].Apply();
            }
        }
    }

    // Load and setup model prefab
    public IEnumerator LoadPrefab(string modelfile)
    {
        DestroyImmediate(mesh);
        if (string.IsNullOrEmpty(modelsPath))
        {
            modelsPath = @"creature\";
        }
        bool done = false;
        var prefab = Resources.Load<GameObject>($"{modelsPath}{modelfile}_prefab");
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        var m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel == null ? null : m2.skel.bytes;
        loadBinaries = new Thread(() => { Model = m2.LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        while (loadBinaries.IsAlive)
        {
            yield return null;
        }
        if (done)
        {
            LoadColors();
            textures = new Texture2D[Model.Textures.Length];
            var bones = GetComponentInChildren<SkinnedMeshRenderer>().bones;
            yield return null;
            if (Model.Particles.Length > 0)
            {
                GameObject[] particles = new GameObject[Model.Particles.Length];
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i] = WoWHelper.ParticleEffect(Model.Particles[i]);
                    particles[i].transform.parent = bones[Model.Particles[i].Bone];
                    particles[i].transform.localPosition = Vector3.zero;
                    particles[i].name = $"Particle{i}";
                    Debug.Log($"Particle{i}: Flags-{Model.Particles[i].Flags}\tBlend-{Model.Particles[i].Blend}");
                    yield return null;
                }
            }
            textureTime = new float[Model.TextureAnimations.Length];
            textureFrame = new int[Model.TextureAnimations.Length];
            colorTime = new float[Model.TextureAnimations.Length];
            colorFrame = new int[Model.TextureAnimations.Length];
            transparencyTime = new float[Model.TextureAnimations.Length];
            transparencyFrame = new int[Model.TextureAnimations.Length];
            for (int i = 0; i < textureTime.Length; i++)
            {
                textureTime[i] = 0f;
                textureFrame[i] = 0;
                yield return null;
            }
            for (int i = 0; i < colorTime.Length; i++)
            {
                colorTime[i] = 0f;
                colorFrame[i] = 0;
                yield return null;
            }
            for (int i = 0; i < transparencyTime.Length; i++)
            {
                transparencyTime[i] = 0f;
                transparencyFrame[i] = 0;
                yield return null;
            }
            renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            Change = true;
            Loaded = !loadBinaries.IsAlive;
        }
    }

    // Load creature model
#if UNITY_EDITOR
    public void LoadModel(string modelfile, Dictionary<int, string> listFile, string dataPath)
#else
    public void LoadModel(string modelfile, CASCHandler casc)
#endif
    {
        Loaded = false;
        model = modelfile;
#if UNITY_EDITOR
        this.listFile = listFile;
        this.dataPath = dataPath;
#else
        this.casc = casc;
#endif
        loadBinaries?.Abort();
        StartCoroutine(LoadPrefab(modelfile));
    }
}
