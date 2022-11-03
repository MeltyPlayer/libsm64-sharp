using Newtonsoft.Json.Linq;

namespace Quad64.src.JSON
{
    class OtherTexturesFile
    {
        public static JArray[] LoadOtherTextureFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                JObject o = JObject.Parse(json);
                if(o["Blocks"] != null)
                {
                    List<JArray> blockArrays = new List<JArray>();
                    JArray blocks = (JArray)o["Blocks"];
                    foreach (JToken token in blocks.Children())
                    {
                        if(token.Type == JTokenType.Array)
                        {
                            blockArrays.Add((JArray)token);
                        }
                    }
                    return blockArrays.ToArray();
                }
                return null;
            }
            return null;
        }
    }
}
