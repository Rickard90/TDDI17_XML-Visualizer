using Microsoft.Xna.Framework;
using OkLab;

static class ColorConfiguration
{
    //private const float luminance = 0.55f;

    //0.790, 0.000, 0.000
    //0.790, −0.016, −0.109
    //0.790, −0.031, −0.218
    //0.790, −0.047, −0.327
    //0.790, −0.063, −0.436

    private static readonly float luminance = 0.89f;

    private static readonly Lab lab_0 = new Lab(luminance, -0.000f, -0.000f);
    private static readonly Lab lab_1 = new Lab(luminance, -0.016f, -0.109f);
    private static readonly Lab lab_2 = new Lab(luminance, -0.031f, -0.218f);
    private static readonly Lab lab_3 = new Lab(luminance, -0.047f, -0.327f);
    private static readonly Lab lab_4 = new Lab(luminance, -0.063f, -0.436f);

    public static readonly Color color_0 = lab_0;
    public static readonly Color color_1 = lab_1;
    public static readonly Color color_2 = lab_2;
    public static readonly Color color_3 = lab_3;
    public static readonly Color color_4 = lab_4;

}