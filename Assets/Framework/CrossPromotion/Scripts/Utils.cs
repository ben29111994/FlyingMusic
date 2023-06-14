using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SRS.CrossPromotion
{
    class Utils
    {
        public const string MORE_APPS_FILENAME = "crosspromotion.json";
        public static int MaxScreenshotToLoad = 5;
        //public static int NumScreenshotLoaded = 0;
        public static int MaxIconToLoad = 5;
        //public static int NumIconLoaded = 0;

        private static Dictionary<string, Sprite> loadedSpriteByPath = new Dictionary<string, Sprite>(20);
        private static Queue<Sprite> screenshotQueue = new Queue<Sprite>();
        private static Queue<Sprite> iconQueue = new Queue<Sprite>();

        public static string GetSettingsFilePath()
        {
            return Path.Combine(Application.persistentDataPath, MORE_APPS_FILENAME);
        }

        public static bool ConfigFileExists()
        {
            return File.Exists(GetSettingsFilePath());
        }

        public static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }

        public static Sprite SpriteFromTex2d(Texture2D tex)
        {
            Texture2D old   = tex;
            Texture2D left  = new Texture2D((int)(old.width), old.height, old.format, false);
            Color[] colors  = old.GetPixels(0, 0, (int)(old.width), old.height);
            left.SetPixels(colors);
            left.Apply();

            Sprite sprite   = Sprite.Create(left, new Rect(0, 0, left.width, left.height), new Vector2(0.5f, 0.5f), 40);
            return sprite;
        }

        public static string GetIconPath(int index)
        {
            return Path.Combine(Application.persistentDataPath, "CP_Icon_" + index + ".png");
        }

        public static string GetScreenshotPath (int index) {
            return Path.Combine(Application.persistentDataPath, "CP_Screenshot_" + index + ".png");
        }

		public static Sprite GetIconSprite(int index, bool useCacheLimit = false)
        {
            string path = GetIconPath(index);
			if (useCacheLimit) {
				if (loadedSpriteByPath.ContainsKey (path)) {
					var sprite = loadedSpriteByPath [path];
					if (sprite != null) {
						return sprite;
					} else {
						loadedSpriteByPath.Remove (path);
					}
				}

				//check to see if we need to remove loaded sprite
				if (iconQueue.Count >= MaxIconToLoad) {
					var spriteToKill = iconQueue.Dequeue ();
					GameObject.Destroy (spriteToKill);
				}

				var iconSprite = SpriteFromTex2d (LoadPNG (path));
				iconQueue.Enqueue (iconSprite);
				loadedSpriteByPath.Add (path, iconSprite);
				return iconSprite;
			} else {
				return SpriteFromTex2d(LoadPNG(path));
			}
        }

		public static Sprite GetScreenshotSprite (int index, bool limitRAMUsage = false) {
			
            //return SpriteFromTex2d(LoadPNG(GetScreenshotPath(index)));
            string path = GetScreenshotPath(index);
			if (limitRAMUsage) {
				if (loadedSpriteByPath.ContainsKey (path)) {
					var sprite = loadedSpriteByPath [path];
					if (sprite != null) {
						return sprite;
					} else {
						loadedSpriteByPath.Remove (path);
					}
				}

				//check to see if we need to remove loaded sprite
				if (screenshotQueue.Count >= MaxScreenshotToLoad) {
					var spriteToKill = screenshotQueue.Dequeue ();
					GameObject.Destroy (spriteToKill);
                
				}

				var screenshotSprite = SpriteFromTex2d (LoadPNG (path));
				screenshotQueue.Enqueue (screenshotSprite);
				loadedSpriteByPath.Add (path, screenshotSprite);
				return screenshotSprite;
			} else {
				return SpriteFromTex2d(LoadPNG(path));
			}
        }
    }
}
