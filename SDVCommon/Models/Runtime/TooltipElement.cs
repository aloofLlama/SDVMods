using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDVCommon.Helpers;
using SDVCommon.Icons;


namespace SDVCommon.Tooltip;

public class TooltipElement
{
    // Text
    public string? Text { get; set; }
    public Color TextColor { get; set; } = TooltipColors.Normal;
    public bool Bold { get; set; }
    public bool Underline { get; set; }


    // Icon (optional)
    public Texture2D? IconTexture { get; set; }   // dynamic item icons
    public Icon? Icon { get; set; }         // static UI icons


    // Layout
    public bool IsLineBreak { get; set; }
    public int PaddingTop { get; set; } = 2;
    public int PaddingBottom { get; set; } = 2;
    public int PaddingRight { get; set; } = 4;


    public List<InlineSegment>? InlineSegments { get; set; }
    public bool IsSeparator { get; set; } = false;
}

// Supports putting multiple items with different formatting on the same line.
public class InlineSegment
{
    public Icon? Icon { get; set; }
    public string Text { get; set; } = "";
    public Color TextColor { get; set; } = TooltipColors.Normal;
    public bool Bold { get; set; }
    public bool Underline { get; set; }
    public bool IsLineBreak { get; set; }
}





