// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameToolkit.Localization
{

    [AddComponentMenu(ComponentMenuRoot + "Localized Sprite")]
    public class LocalizedSpriteBehaviour : LocalizedGenericAssetBehaviour<LocalizedSprite, Sprite>
    {

        private void Reset()
        {
            TrySetComponentAndPropertyIfNotSet<Image>("sprite");
        }

        private Image image;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (image == null)
                TryGetComponent(out image);
            image.SetNativeSize();
        }

    }
}
