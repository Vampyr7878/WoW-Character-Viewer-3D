using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;

public class Creature : ModelRenderer
{
    // Reference to the character
    public Character character;

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
    // Particle colors used by the model
    private ParticleColor[] particleColors;
    // model file name
    private string model;
    // Reference to the main camera
    private new Transform camera;
    // List of geosets that are enabled for loading
    private List<int> activeGeosets;

    private void Start()
    {
        // Initiazlie
        modelsPath = @"creature\";
        camera = Camera.main.transform;
        activeGeosets = new List<int>();
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
                    4319, 4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342, } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4344, 4345, 4346, 4347, 4348, 4376, 4377, 4378, 4379, 4380,
                    4381, 4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4267, 4268, 4269, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4223, 4224, 4225, 4226, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4159 } }
            };
        taurenForms = new()
            {
                { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4297, 4298, 4299, 4300, 4301, 4319,
                    4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4339, 4342 } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4354, 4355, 4356, 4357, 4358, 4377, 4378, 4379, 4380, 4381,
                    4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4402 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4270, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4231, 4233, 4238, 4240, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4160 } }
            };
        trollForms = new()
            {
                { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4302, 4303, 4304, 4305, 4306, 4319,
                    4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4340, 4342 } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4359, 4360, 4361, 4362, 4363, 4377, 4378, 4379, 4380, 4381,
                    4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4401 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4270, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4232, 4236, 4239, 4242, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4161 } }
            };
        worgenForms = new()
            {
                { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4292, 4293, 4294, 4295, 4296, 4319,
                    4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4341, 4342 } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4349, 4350, 4351, 4352, 4353, 4377, 4378, 4379, 4380, 4381,
                    4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400, 4403 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4267, 4268, 4269, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4227, 4228, 4229, 4230, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4162 } }
            };
        highmountainForms = new()
            {
                { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4307, 4308, 4309, 4310, 4319,
                    4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4364, 4365, 4366, 4367, 4377, 4378, 4379, 4380, 4381,
                    4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4252, 4253, 4254, 4255 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4275, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4231, 4234, 4238, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4163 } }
            };
        kulTiranForms = new()
            {
                { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4311, 4312, 4313, 4314, 4319,
                    4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4372, 4373, 4374, 4375, 4377, 4378, 4379, 4380, 4381,
                    4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4253, 4254, 4255, 4260, 4261, 4262, 4263 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4271, 4272, 4273, 4274, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4230, 4231, 4235, 4241, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4158 } }
            };
        zandalariForms = new()
            {
                { (int)WoWHelper.CreatureForm.BearForm, new int[] { 4170, 4171, 4172, 4173, 4174, 4175, 4289, 4290, 4291, 4315, 4316, 4317, 4318, 4319,
                    4320, 4321, 4322, 4323, 4324, 4325, 4326, 4327, 4328, 4329, 4330, 4331, 4332, 4333, 4334, 4335, 4336, 4337, 4338, 4342 } },
                { (int)WoWHelper.CreatureForm.CatForm, new int[] { 4176, 4177, 4178, 4368, 4369, 4370, 4371, 4377, 4378, 4379, 4380, 4381,
                    4382, 4383, 4384, 4385, 4386, 4387, 4388, 4389, 4390, 4391, 4392, 4393, 4394, 4395, 4396, 4397, 4398, 4399, 4400 } },
                { (int)WoWHelper.CreatureForm.AquaticForm, new int[] { 4183, 4253, 4254, 4255, 4256, 4257, 4258, 4259 } },
                { (int)WoWHelper.CreatureForm.TravelForm, new int[] { 4180, 4181, 4182, 4188, 4189, 4190, 4191, 4192, 4268, 4269, 4276, 4277, 4278 } },
                { (int)WoWHelper.CreatureForm.FlightForm, new int[] { 4184, 4186, 4187, 4232, 4237, 4239, 4243, 4244, 4245, 4246, 4247, 4248, 4249 } },
                { (int)WoWHelper.CreatureForm.MoonkinForm, new int[] { 141, 4164 } }
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
                        if (Model.Skin.Textures[i].Layer == 0)
                        {
                            SetMaterial(renderer, i);
                        }
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
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x - 90, camera.eulerAngles.y - 90, camera.eulerAngles.z);
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
                case (int)WoWHelper.CreatureForm.MoonkinForm:
                    character.helper.ChangeRelatedTextureOptions("Decorations", "Decoration Color", decorationColors);
                    character.helper.ChangeRelatedTextureOptions("Effects", "Effects Color", effectsColors);
                    break;
            }
        }
    }

    // Show only options available for selected race
    public void ChangeRacialOptions()
    {
        if (character.Class == WoWHelper.Class.Druid)
        {
            switch (character.Race)
            {
                case WoWHelper.Race.NightElf:
                    character.helper.ChangeRacialOptions("Bear Form", nightElfForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", nightElfForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", nightElfForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", nightElfForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", nightElfForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", nightElfForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    break;
                case WoWHelper.Race.Tauren:
                    character.helper.ChangeRacialOptions("Bear Form", taurenForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", taurenForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", taurenForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", taurenForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", taurenForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", taurenForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    break;
                case WoWHelper.Race.Troll:
                    character.helper.ChangeRacialOptions("Bear Form", trollForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", trollForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", trollForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", trollForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", trollForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", trollForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    break;
                case WoWHelper.Race.Worgen:
                    character.helper.ChangeRacialOptions("Bear Form", worgenForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", worgenForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", worgenForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", worgenForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", worgenForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", worgenForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    break;
                case WoWHelper.Race.Highmountain:
                    character.helper.ChangeRacialOptions("Bear Form", highmountainForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", highmountainForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", highmountainForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", highmountainForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", highmountainForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", highmountainForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    break;
                case WoWHelper.Race.Zandalari:
                    character.helper.ChangeRacialOptions("Bear Form", zandalariForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", zandalariForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", zandalariForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", zandalariForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", zandalariForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", zandalariForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
                    break;
                case WoWHelper.Race.KulTiran:
                    character.helper.ChangeRacialOptions("Bear Form", kulTiranForms[(int)WoWHelper.CreatureForm.BearForm]);
                    character.helper.ChangeRacialOptions("Cat Form", kulTiranForms[(int)WoWHelper.CreatureForm.CatForm]);
                    character.helper.ChangeRacialOptions("Aquatic Form", kulTiranForms[(int)WoWHelper.CreatureForm.AquaticForm]);
                    character.helper.ChangeRacialOptions("Travel Form", kulTiranForms[(int)WoWHelper.CreatureForm.TravelForm]);
                    character.helper.ChangeRacialOptions("Flight Form", kulTiranForms[(int)WoWHelper.CreatureForm.FlightForm]);
                    character.helper.ChangeRacialOptions("Full Transformation", kulTiranForms[(int)WoWHelper.CreatureForm.MoonkinForm]);
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
                    (int)WoWHelper.CreatureForm.MoonkinForm => Array.FindIndex(character.Options, o => o.Name == "Full Transformation" && o.Model == character.ModelID),
                    _ => 0,
                };
                path = character.Options[index].Choices[character.Customization[index]].Creatures.Length > 0 ?
                    character.Options[index].Choices[character.Customization[index]].Creatures[0].Model : @"tindralmoonkin\tindralmoonkin";
                break;
            default:
                index = -1;
                path = null;
                break;
        }
        activeGeosets.Clear();
        activeGeosets.Add(0);
        if (character.Options[index].Choices[character.Customization[index]].Creatures.Length > 0)
        {
            LoadGeosets(character.Options[index].Choices[character.Customization[index]].Creatures[0].Geosets);
            particleColors = character.Options[index].Choices[character.Customization[index]].Creatures[0].ParticleColors;
        }
        else
        {
            character.helper.ChangeGeosetOption(activeGeosets, "Horns");
            character.helper.ChangeGeosetOption(activeGeosets, "Beak");
            character.helper.ChangeGeosetOption(activeGeosets, "Eyebrows");
            character.helper.ChangeGeosetOption(activeGeosets, "Whiskers");
            character.helper.ChangeGeosetOption(activeGeosets, "Feathers");
            character.helper.ChangeGeosetOption(activeGeosets, "Decorations");
            character.helper.ChangeGeosetOption(activeGeosets, "Effects");
        }
        if (!string.IsNullOrEmpty(path) && model != path)
        {
#if UNITY_EDITOR
            LoadModel(path, listFile, dataPath);
#else
            LoadModel(path, casc);
#endif
        }
    }

    // Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (activeGeosets.Count == 0 || activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id) && !IsDeathModel(i))
        {
            Material material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
            if (material == null)
            {
                Debug.LogError(Model.Skin.Textures[i].Shader);
            }
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(material.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = material.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(material);
            Debug.Log($"Shader: {Model.Skin.Textures[i].Shader}");
            SetTexture(renderer.materials[Model.Skin.Textures[i].Id], i);
        }
        else
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
        }
    }

    // Check if there is a seperate death model to be hidden
    private bool IsDeathModel(int i)
    {
        if (model.Contains("illidan"))
        {
            if (i == 3 || i == 4)
            {
                return true;
            }
            return false;
        }
        if (model.Contains("abyssal"))
        {
            if (i == 0)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    // Setup particle effects for rendering
    private void ParticleEffects()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        particles = particles.OrderBy(x => x.name).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            if (Model.Particles[i].ColorIndex != 0)
            {
                ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles[i].colorOverLifetime;
                ParticleColor particleColor = particleColors.Length > 0 ? particleColors[Model.Particles[i].ColorIndex - 11] : null;
                if (particleColor != null)
                {
                    Gradient gradient = new();
                    GradientColorKey[] colorKeys = new GradientColorKey[3];
                    colorKeys[0] = new GradientColorKey(particleColor.Start, 0f);
                    colorKeys[1] = new GradientColorKey(particleColor.Mid, 0.5f);
                    colorKeys[2] = new GradientColorKey(particleColor.End, 1f);
                    GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
                    alphaKeys[0] = new GradientAlphaKey(particleColor.Start.a, 0f);
                    alphaKeys[1] = new GradientAlphaKey(particleColor.Mid.a, 0.5f);
                    alphaKeys[2] = new GradientAlphaKey(particleColor.End.a, 1f);
                    gradient.SetKeys(colorKeys, alphaKeys);
                    colorOverLifetime.color = gradient;
                }
            }
            ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = particles[i].limitVelocityOverLifetime;
            limitVelocityOverLifetime.enabled = true;
            limitVelocityOverLifetime.drag = Model.Particles[i].Drag;
            limitVelocityOverLifetime.multiplyDragByParticleSize = false;
            limitVelocityOverLifetime.multiplyDragByParticleVelocity = true;
            particles[i].transform.localScale = transform.lossyScale;
            ParticleSystemRenderer renderer = particles[i].GetComponent<ParticleSystemRenderer>();
            Material material = ParticleMaterial(Model.Particles[i].Blend);
            renderer.material.shader = material.shader;
            renderer.material.CopyPropertiesFromMaterial(material);
            Texture2D temp = textures[Model.Particles[i].Textures[0]];
            Texture2D texture = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
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
        material.SetInt("_SrcColorBlend", (int)SrcColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstColorBlend", (int)DstColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_SrcAlphaBlend", (int)SrcAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstAlphaBlend", (int)DstAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend < 2 ? 0.5f : 0f);
        Color color = Color.white;
        if (Model.Skin.Textures[i].Color != -1)
        {
            color = colors[Model.Skin.Textures[i].Color];
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Back;
        material.SetInt("_Cull", (int)cull);
        float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
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
                    (int)WoWHelper.CreatureForm.MoonkinForm => Array.FindIndex(character.Options, o => o.Name == "Full Transformation" && o.Model == character.ModelID),
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
                    file = character.Options[index].Choices[character.Customization[index]].Creatures[0].Texture0;
                    break;
                case 12:
                    file = character.Options[index].Choices[character.Customization[index]].Creatures[0].Texture1;
                    break;
                case 13:
                    file = character.Options[index].Choices[character.Customization[index]].Creatures[0].Texture2;
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
                    index = Array.FindIndex(character.Options, o => o.Name == "Feather Color" && o.Model == character.ModelID);
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                    break;
                case 12:
                    index = Array.FindIndex(character.Options, o => o.Name == "Horn & Beak Color" && o.Model == character.ModelID);
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                    break;
                case 13:
                    index = Array.FindIndex(character.Options, o => o.Name == "Decoration Color" && o.Model == character.ModelID);
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                    break;
            }
        }
        return file;
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
        converter = new System.Drawing.ImageConverter();
        GameObject prefab = Resources.Load<GameObject>($"{modelsPath}{modelfile}_prefab");
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        M2Model m2 = GetComponentInChildren<M2Model>();
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
            Transform[] bones = GetComponentInChildren<SkinnedMeshRenderer>().bones;
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
