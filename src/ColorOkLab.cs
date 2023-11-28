

using Microsoft.Xna.Framework;

static class OkLab
{

    public struct Lab {public double L; public double a; public double b;};
    public struct RGB {public double r; public double g; public double b;};

    public static Lab ToOkLab(RGB c) 
    {
        double l = 0.4122214708 * c.r + 0.5363325363 * c.g + 0.0514459929 * c.b;
        double m = 0.2119034982 * c.r + 0.6806995451 * c.g + 0.1073969566 * c.b;
        double s = 0.0883024619 * c.r + 0.2817188376 * c.g + 0.6299787005 * c.b;

        double l_ = (double)Math.Cbrt(l);
        double m_ = (double)Math.Cbrt(m);
        double s_ = (double)Math.Cbrt(s);

        return new Lab()
        {
            L = 0.2104542553*l_ + 0.7936177850*m_ - 0.0040720468*s_,
            a = 1.9779984951*l_ - 2.4285922050*m_ + 0.4505937099*s_,
            b = 0.0259040371*l_ + 0.7827717662*m_ - 0.8086757660*s_,
        };
    }
    public static Lab ToOkLab(Color c)
    {
        return ToOkLab(new RGB(){r = ((double)(c.R))/255.0, g = ((double)(c.G))/255.0, b = ((double)(c.B))/255.0});
    }

    public static RGB ToLinearSrgb(Lab c) 
    {
        double l_ = c.L + 0.3963377774 * c.a + 0.2158037573 * c.b;
        double m_ = c.L - 0.1055613458 * c.a - 0.0638541728 * c.b;
        double s_ = c.L - 0.0894841775 * c.a - 1.2914855480 * c.b;

        double l = l_*l_*l_;
        double m = m_*m_*m_;
        double s = s_*s_*s_;

        return new RGB{
            r = +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s,
            g = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s,
            b = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s,
        };
    }
    public static Color ToLinearColor(Lab c)
    {
        RGB rgb = ToLinearSrgb(c);
        return new Color((float)rgb.r, (float)rgb.g, (float)rgb.b, 1.0f);
    }

    public static Lab[] GetColorPallet(int length, double luminance, double maxGamma, double radiance)
    {
        Lab[] result = new Lab[length];
        double a_bias = Math.Asin(radiance);
        double b_bias = Math.Acos(radiance);

        for (int i = 0; i < length; i++)
        {
            double gamma = maxGamma * (i / length);
            result[i] = new Lab(){L = luminance, a = gamma * a_bias, b = gamma * b_bias};
        }

        return result;
    }

}