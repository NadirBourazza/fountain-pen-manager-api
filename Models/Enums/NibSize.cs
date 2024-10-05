using System.ComponentModel;
// ReSharper disable InconsistentNaming

namespace Models.Enums;

public enum NibSize
{
    [Description("Extra Fine")] EF, // Extra Fine

    [Description("Fine")] F, // Fine

    [Description("Medium Fine")] MF, // Medium Fine

    [Description("Medium")] M, // Medium

    [Description("Broad")] B, // Broad

    [Description("Double Broad")] BB, // Double Broad

    [Description("Stub")] Stub, // Stub nibs provide a wider line and a specific writing style

    [Description("Architect")] Architect, // Typically has a specialized tip for creating line variation

    [Description("Oblique")] Oblique, // Designed for a specific writing angle

    [Description("Flexible")] Flexible, // Flexible nibs that provide varying line widths depending on pressure

    [Description("Music")] Music, // Wider nib for calligraphy and musical notation

    [Description("Broad Italic")] BroadItalic // An italicized version of the broad nib for added flair
}