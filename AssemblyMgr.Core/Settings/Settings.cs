using System.IO;
using System.Xml.Serialization;

namespace AssemblyMgr.Core.Settings
{
    public static class Settings
    {
        public static void Serialize<T>(this T obj, FileInfo filename)
            where T : ISettings
            => obj.Serialize(filename.FullName);
        public static void Serialize<T>(this T obj, string filename)
            where T : ISettings
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, obj);
            writer.Close();
        }

        public static T DeSerialize<T>(string filename)
            where T : ISettings
            => DeSerialize<T>(new FileInfo(filename));
        public static T DeSerialize<T>(FileInfo file)
            where T : ISettings
        {
            try
            {
                if (!file.Exists) return default;

                var mySerializer = new XmlSerializer(typeof(T));
                using (var myFileStream = new FileStream(file.FullName, FileMode.Open))
                {
                    var deserialized = (T)mySerializer.Deserialize(myFileStream);
                    return deserialized;
                };
            }
            catch
            {
                return default;
            }
        }

    }

}
