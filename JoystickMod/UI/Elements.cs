using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoystickMod.UI
{
    public static class Elements
    {
        public static GUIStyle windowStyle = new GUIStyle()
        {
            normal = { background = ModResource.GetTexture("window-background"), textColor = Color.white },
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            border = new RectOffset(4, 4, 30, 4),
            padding = new RectOffset(12, 12, 40, 12),
            contentOffset = new Vector2(0, -33)          // -33 = ((30-16)*0.5)-30-(30-40)
        };

        public static GUIStyle ToggleStyle = new GUIStyle()
        {
            normal = {
                background = ModResource.GetTexture("toggle-normal"),
        },
            onNormal = {
                background = ModResource.GetTexture("toggle-on-normal"),
        },
            hover = {
                background = ModResource.GetTexture("toggle-hover"),
        },
            onHover = {
                background = ModResource.GetTexture("toggle-on-hover"),
        },
            active = {
                background = ModResource.GetTexture("toggle-normal"),
        },
            onActive = {
                background = ModResource.GetTexture("toggle-on-normal"),
        },
            padding = new RectOffset(0, 0, 12, 0),
            border = new RectOffset(0, 0, 6, 0),
            //margin = { right = 10 }
        };

    }
}
