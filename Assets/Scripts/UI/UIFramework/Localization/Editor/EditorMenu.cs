// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;

namespace GameToolkit.Localization.Editor
{
    /// <summary>
    /// Unity Editor menu for changing localization under "Tools/Localization".
    /// </summary>
    public static class EditorMenu
    {
        public const string RootMenu = "MyTools/本地化/";
        
        private const string SetLocaleRootMenu = RootMenu + "区域设置/";

        [MenuItem(RootMenu + "导入 .csv", false, 1)]
        private static void Import()
        {
            EditorSerialization.Import();
        }
        
        [MenuItem(RootMenu + "导出 .csv", false, 2)]
        private static void Export()
        {
            EditorSerialization.Export();
        }
        
        [MenuItem(RootMenu + "帮助", false, 3)]
        private static void OpenHelpUrl()
        {
            Application.OpenURL(Localization.HelpUrl);
        }

        [MenuItem(SetLocaleRootMenu + "南非荷兰语")]
        private static void ChangeToAfrikaans()
        {
            SetLanguage(Language.Afrikaans);
        }

        [MenuItem(SetLocaleRootMenu + "阿拉伯语")]
        private static void ChangeToArabic()
        {
            SetLanguage(Language.Arabic);
        }

        [MenuItem(SetLocaleRootMenu + "巴斯克语")]
        private static void ChangeToBasque()
        {
            SetLanguage(Language.Basque);
        }

        [MenuItem(SetLocaleRootMenu + "白俄罗斯语")]
        private static void ChangeToBelarusian()
        {
            SetLanguage(Language.Belarusian);
        }

        [MenuItem(SetLocaleRootMenu + "保加利亚语")]
        private static void ChangeToBulgarian()
        {
            SetLanguage(Language.Bulgarian);
        }

        [MenuItem(SetLocaleRootMenu + "加泰罗尼亚语")]
        private static void ChangeToCatalan()
        {
            SetLanguage(Language.Catalan);
        }

        [MenuItem(SetLocaleRootMenu + "中文")]
        private static void ChangeToChinese()
        {
            SetLanguage(Language.Chinese);
        }

        [MenuItem(SetLocaleRootMenu + "捷克语")]
        private static void ChangeToCzech()
        {
            SetLanguage(Language.Czech);
        }

        [MenuItem(SetLocaleRootMenu + "丹麦语")]
        private static void ChangeToDanish()
        {
            SetLanguage(Language.Danish);
        }

        [MenuItem(SetLocaleRootMenu + "荷兰语")]
        private static void ChangeToDutch()
        {
            SetLanguage(Language.Dutch);
        }

        [MenuItem(SetLocaleRootMenu + "英语")]
        private static void ChangeToEnglish()
        {
            SetLanguage(Language.English);
        }

        [MenuItem(SetLocaleRootMenu + "爱沙尼亚语")]
        private static void ChangeToEstonian()
        {
            SetLanguage(Language.Estonian);
        }

        [MenuItem(SetLocaleRootMenu + "法罗语")]
        private static void ChangeToFaroese()
        {
            SetLanguage(Language.Faroese);
        }

        [MenuItem(SetLocaleRootMenu + "芬兰语")]
        private static void ChangeToFinnish()
        {
            SetLanguage(Language.Finnish);
        }

        [MenuItem(SetLocaleRootMenu + "法语")]
        private static void ChangeToFrench()
        {
            SetLanguage(Language.French);
        }

        [MenuItem(SetLocaleRootMenu + "德语")]
        private static void ChangeToGerman()
        {
            SetLanguage(Language.German);
        }

        [MenuItem(SetLocaleRootMenu + "希腊语")]
        private static void ChangeToGreek()
        {
            SetLanguage(Language.Greek);
        }

        [MenuItem(SetLocaleRootMenu + "希伯来语")]
        private static void ChangeToHebrew()
        {
            SetLanguage(Language.Hebrew);
        }


        [MenuItem(SetLocaleRootMenu + "匈牙利语")]
        private static void ChangeToHugarian()
        {
            SetLanguage(Language.Hungarian);
        }

        [MenuItem(SetLocaleRootMenu + "冰岛语")]
        private static void ChangeToIcelandic()
        {
            SetLanguage(Language.Icelandic);
        }

        [MenuItem(SetLocaleRootMenu + "印度尼西亚语")]
        private static void ChangeToIndonesian()
        {
            SetLanguage(Language.Indonesian);
        }

        [MenuItem(SetLocaleRootMenu + "意大利语")]
        private static void ChangeToItalian()
        {
            SetLanguage(Language.Italian);
        }

        [MenuItem(SetLocaleRootMenu + "日语")]
        private static void ChangeToJapanese()
        {
            SetLanguage(Language.Japanese);
        }

        [MenuItem(SetLocaleRootMenu + "韩语")]
        private static void ChangeToKorean()
        {
            SetLanguage(Language.Korean);
        }

        [MenuItem(SetLocaleRootMenu + "拉脱维亚语")]
        private static void ChangeToLatvian()
        {
            SetLanguage(Language.Latvian);
        }

        [MenuItem(SetLocaleRootMenu + "立陶宛语")]
        private static void ChangeToLithuanian()
        {
            SetLanguage(Language.Lithuanian);
        }

        [MenuItem(SetLocaleRootMenu + "挪威语")]
        private static void ChangeToNorwegian()
        {
            SetLanguage(Language.Norwegian);
        }

        [MenuItem(SetLocaleRootMenu + "波兰语")]
        private static void ChangeToPolish()
        {
            SetLanguage(Language.Polish);
        }

        [MenuItem(SetLocaleRootMenu + "葡萄牙语")]
        private static void ChangeToPortuguese()
        {
            SetLanguage(Language.Portuguese);
        }

        [MenuItem(SetLocaleRootMenu + "罗马尼亚语")]
        private static void ChangeToRomanian()
        {
            SetLanguage(Language.Romanian);
        }

        [MenuItem(SetLocaleRootMenu + "俄语")]
        private static void ChangeToRussian()
        {
            SetLanguage(Language.Russian);
        }

        [MenuItem(SetLocaleRootMenu + "塞尔维亚语")]
        private static void ChangeToSerboCroatian()
        {
            SetLanguage(Language.SerboCroatian);
        }

        [MenuItem(SetLocaleRootMenu + "斯洛伐克语")]
        private static void ChangeToSlovak()
        {
            SetLanguage(Language.Slovak);
        }

        [MenuItem(SetLocaleRootMenu + "斯洛文尼亚语")]
        private static void ChangeToSlovenian()
        {
            SetLanguage(Language.Slovenian);
        }

        [MenuItem(SetLocaleRootMenu + "西班牙语")]
        private static void ChangeToSpanish()
        {
            SetLanguage(Language.Spanish);
        }

        [MenuItem(SetLocaleRootMenu + "瑞典语")]
        private static void ChangeToSwedish()
        {
            SetLanguage(Language.Swedish);
        }

        [MenuItem(SetLocaleRootMenu + "泰语")]
        private static void ChangeToThai()
        {
            SetLanguage(Language.Thai);
        }

        [MenuItem(SetLocaleRootMenu + "土耳其语")]
        private static void ChangeToTurkish()
        {
            SetLanguage(Language.Turkish);
        }

        [MenuItem(SetLocaleRootMenu + "乌克兰语")]
        private static void ChangeToUkrainian()
        {
            SetLanguage(Language.Ukrainian);
        }

        [MenuItem(SetLocaleRootMenu + "越南语")]
        private static void ChangeToVietnamese()
        {
            SetLanguage(Language.Vietnamese);
        }

        [MenuItem(SetLocaleRootMenu + "简体中文")]
        private static void ChangeToChineseSimplified()
        {
            SetLanguage(Language.ChineseSimplified);
        }

        [MenuItem(SetLocaleRootMenu + "繁体中文")]
        private static void ChangeToChineseTraditional()
        {
            SetLanguage(Language.ChineseTraditional);
        }

        [MenuItem(SetLocaleRootMenu + "Unknown")]
        private static void ChangeToUnknown()
        {
            SetLanguage(Language.Unknown);
        }

        private static void SetLanguage(Language currentLanguage)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("设置语言仅在应用程序正在播放时可用.");
                return;
            }
            
            var previousLanguage = Localization.Instance.CurrentLanguage;
            Localization.Instance.CurrentLanguage = currentLanguage;
            
            Menu.SetChecked(GetMenuName(previousLanguage), false);
            Menu.SetChecked(GetMenuName(currentLanguage), true);
        }

        private static string GetMenuName(Language language)
        {
            return SetLocaleRootMenu + language;
        }
    }
}
