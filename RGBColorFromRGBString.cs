   public static Color ColorFromRGBString(string colorRGB)
        {
            List<double> rgb = Prompts.GetListFromString<double>(colorRGB);
            return Color.FromRgb((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
        }
