﻿/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2022 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: TextDefinitionExtensions.cs                                     *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using Server.Gumps;
using Server.Network;

namespace Server;

public static class TextDefinitionExtensions
{
    public static void AddHtmlText(
        this TextDefinition def,
        Gump g,
        int x,
        int y,
        int width,
        int height,
        bool back,
        bool scroll,
        int numberColor = -1,
        int stringColor = -1
    )
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            if (numberColor >= 0) // 5 bits per RGB component (15 bit RGB)
            {
                g.AddHtmlLocalized(x, y, width, height, def.Number, numberColor, back, scroll);
            }
            else
            {
                g.AddHtmlLocalized(x, y, width, height, def.Number, back, scroll);
            }
        }
        else if (def.String != null)
        {
            if (stringColor >= 0) // 8 bits per RGB component (24 bit RGB)
            {
                g.AddHtml(
                    x,
                    y,
                    width,
                    height,
                    $"<BASEFONT COLOR=#{stringColor:X6}>{def.String}</BASEFONT>",
                    back,
                    scroll
                );
            }
            else
            {
                g.AddHtml(x, y, width, height, def.String, back, scroll);
            }
        }
    }

    public static void AddTo(this TextDefinition def, IPropertyList list)
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            list.Add(def.Number);
        }
        else if (def.String != null)
        {
            list.Add(def.String);
        }
    }

    public static void AddTo(this TextDefinition def, IPropertyList list, int number)
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            list.Add(number, $"{def.Number:#}");
        }
        else if (def.String != null)
        {
            list.Add(number, $"{def.String}");
        }
    }

    public static void SendMessageTo(this TextDefinition def, Mobile m)
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            m.SendLocalizedMessage(def.Number);
        }
        else if (def.String != null)
        {
            m.SendMessage(def.String);
        }
    }

    public static void SendMessageTo(this TextDefinition def, Mobile m, int hue)
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            m.SendLocalizedMessage(def.Number, "", hue);
        }
        else if (def.String != null)
        {
            m.SendMessage(hue, def.String);
        }
    }

    public static void PublicOverheadMessage(this TextDefinition def, Mobile m, MessageType messageType, int hue)
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            m.PublicOverheadMessage(messageType, hue, def.Number);
        }
        else if (def.String != null)
        {
            m.PublicOverheadMessage(messageType, hue, false, def.String);
        }
    }

    public static void PublicOverheadMessage(this TextDefinition def, Item item, MessageType messageType, int hue)
    {
        if (def == null)
        {
            return;
        }

        if (def.Number > 0)
        {
            item.PublicOverheadMessage(messageType, hue, def.Number);
        }
        else if (def.String != null)
        {
            item.PublicOverheadMessage(messageType, hue, false, def.String);
        }
    }

    public static bool IsNullOrEmpty(this TextDefinition def) => def?.IsEmpty != false;
}
