using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class StringToFloatArrayConverter : JsonConverter<float[]>
{
    public override float[] ReadJson(JsonReader reader, Type objectType, float[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            return token.Select(t => float.TryParse(t.ToString(), out float num) ? num : 0).ToArray();
        }
        return new float[0];
    }

    public override void WriteJson(JsonWriter writer, float[] value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

public class StringToIntArrayConverter : JsonConverter<int[]>
{
    public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            return token.Select(t => int.TryParse(t.ToString(), out int num) ? num : 0).ToArray();
        }
        return new int[0];
    }

    public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
