using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameToolkit.Localization
{
    [HelpURL(Localization.HelpUrl + "/getting-started#3-localization-settings")]
    [CreateAssetMenu(fileName = "LocalizationSettings", menuName = "GameToolkit/Localization/Localization Settings", order = 9999)]
    public sealed class LocalizationSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        //本地化设置的名字，在Resources目录下
        private const string AssetName = "LocalizationSettings";
        private static LocalizationSettings s_Instance = null;

        [SerializeField, HideInInspector]
        private int m_Version = 0;
        
        // Keep it for migration.
        [SerializeField]
        private List<SystemLanguage> m_AvailableLanguages = new List<SystemLanguage>(1)
        {
            SystemLanguage.English
        };
        
        [SerializeField, Tooltip("应用程序启用的语言.")]
        private List<Language> m_AvailableLanguages2 = new List<Language>(1)
        {
            Language.English
        };

        [SerializeField, Tooltip("导入的文本资源位置.")]
        private string m_ImportLocation = "Assets";

        [Tooltip("谷歌云认证文件.")]
        public TextAsset GoogleAuthenticationFile;

        /// <summary>
        /// Settings version related to the Localization system.
        /// </summary>
        internal int Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        /// <summary>
        /// 获取外部本地化文件的导入位置s..
        /// </summary>
        internal string ImportLocation
        {
            get { return m_ImportLocation; }
        }

        /// <summary>
        /// Gets the localization settings instance.
        /// </summary>
        public static LocalizationSettings Instance
        {
            get
            {
                if (!s_Instance)
                {
                    s_Instance = FindByResources();
                }

#if UNITY_EDITOR
                if (!s_Instance)
                {
                    s_Instance = CreateSettingsAndSave();
                }
#endif

                if (!s_Instance)
                {
                    Debug.LogWarning("没有找到该文件 " + AssetName + " ,将使用默认设置.");
                    s_Instance = CreateInstance<LocalizationSettings>();
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// 为应用程序启用的语言
        /// </summary>
        public List<Language> AvailableLanguages
        {
            get { return m_AvailableLanguages2; }
        }

        /// <summary>
        /// 获取包含内置语言和自定义语言的所有语言.
        /// </summary>
        public List<Language> AllLanguages
        {
            get
            {
                var languages = new List<Language>();
                languages.AddRange(Language.BuiltinLanguages);
                languages.AddRange(AvailableLanguages.Where(x => x.Custom));
                return languages;
            }
        }

        private static LocalizationSettings FindByResources()
        {
            return Resources.Load<LocalizationSettings>(AssetName);
        }

#if UNITY_EDITOR
        private static LocalizationSettings CreateSettingsAndSave()
        {
            var localizationSettings = CreateInstance<LocalizationSettings>();

            // Saving during Awake() will crash Unity, delay saving until next editor frame.
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityEditor.EditorApplication.delayCall += () => SaveAsset(localizationSettings);
            }
            else
            {
                SaveAsset(localizationSettings);
            }

            return localizationSettings;
        }

        private static void SaveAsset(LocalizationSettings localizationSettings)
        {
            var assetPath = "Assets/Resources/" + AssetName + ".asset";
            var directoryName = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            var uniqueAssetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);
            UnityEditor.AssetDatabase.CreateAsset(localizationSettings, uniqueAssetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log(AssetName + " has been created: " + assetPath);
        }
#endif
        public void OnBeforeSerialize()
        {
            // Intentionally empty.
        }

        public void OnAfterDeserialize()
        {
            // Migration from SystemLanguage to Language.
            if (m_AvailableLanguages.Any())
            {
                foreach (var availableLanguage in m_AvailableLanguages)
                {
                    m_AvailableLanguages2.Add(availableLanguage);
                }
                
                m_AvailableLanguages.Clear();
            }
        }
    }
}
