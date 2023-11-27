using Microsoft.Xna.Framework;

static class ColorConfiguration
{
    //private const float luminance = 0.55f;

    //0.790, 0.000, 0.000
    //0.790, −0.016, −0.109
    //0.790, −0.031, −0.218
    //0.790, −0.047, −0.327
    //0.790, −0.063, −0.436

    private static readonly float luminance = 0.89f;

    private static readonly OkLab.Lab lab_0 = new OkLab.Lab(){L = luminance, a = -0.000f, b = -0.000f};
    private static readonly OkLab.Lab lab_1 = new OkLab.Lab(){L = luminance, a = -0.016f, b = -0.109f};
    private static readonly OkLab.Lab lab_2 = new OkLab.Lab(){L = luminance, a = -0.031f, b = -0.218f};
    private static readonly OkLab.Lab lab_3 = new OkLab.Lab(){L = luminance, a = -0.047f, b = -0.327f};
    private static readonly OkLab.Lab lab_4 = new OkLab.Lab(){L = luminance, a = -0.063f, b = -0.436f};

    public static readonly Color color_0 = OkLab.ToLinearColor(lab_0);
    public static readonly Color color_1 = OkLab.ToLinearColor(lab_1);
    public static readonly Color color_2 = OkLab.ToLinearColor(lab_2);
    public static readonly Color color_3 = OkLab.ToLinearColor(lab_3);
    public static readonly Color color_4 = OkLab.ToLinearColor(lab_4);

}