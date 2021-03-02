   public static List<T> GetListFromString<T>(string s)
        {
            try
            {
                return s.Split(',').ToList().Select(l => l.Trim()).Select(x => ((T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(x))).ToList();
            }
            catch (System.Exception ex)
            {
                return new List<T>();
            }
        }
