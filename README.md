# WoW-Character-Viewer-3D
Unity based app to view character models from wow and dress them up.

In order to reduce the size of the repo models are included in gitignore so they won't be versioned, you can download them from my onedrive:

character: https://1drv.ms/u/s!Av-LW1jhrWyooA2uqMUHiH9oBEhK?e=8N0lAa

creature: https://1drv.ms/u/s!Av-LW1jhrWyooAz2Q21Py12OAgMA?e=wfspL7

To import them you need to follow a 2 step process. First unpack the folders into Resources folder in the Unity Project. Open the procjet in Unity and it will try to import everything but it will fail. You need to use Custom Menu WoW Character Viewer and choose Generate *.bytes files that will generate extra file with data that helps with import. You can also right click on specific folder to generate the files for that folder only, it is recursive. After that simply reimport all the *.m2 and *.bone files and everything should import correctly

You can find latest build here: https://1drv.ms/u/s!Av-LW1jhrWyopWg-dR8ukY6ATaOv?e=tzCQ3w on in Releases tab.

This brand new version is the first step in complete refactoring. Database structure has been changed to match db2 files so it can be updated using automatic tools. Animations are imported(currently only Idle Stand) from model files directly so it no longer relies on WoW Model Viewer exported animations that was kind of a pain recently. Other animation clips can be imported if you change the code but only those that are in the actual model file. Haven't yet touched *.anim files. Texture drawing has been updated to be faster, code has been refactor to be more generalized. Because of all the changes some functionality currently no longer works, it will be be brougth back in future updates. Currently all basic character customziation works afor all races that were in the app up to this point, Dracthyr and Earthen might come later. Druid form and Warlock demon customization doesn't work, it will be the next step. Equipping items doesn't work, saving and opening and importing doesn't work.

To make debugging easier the game app uses files directly from your drive instaed of CASC system while running in Unity Editor. In order for this to work you need to unpack your WoW installation using for example CASCview Software. Currently you need character, models and item folders. You will need listfile that you can download from wago.tools. Path to extracted files and listfile need to be included in config.ini inside the main folder for the project. First line contains your WoW folder instalation, not used for the Editor, second line should contain where you extracted files and third line path to the listfile.
