using System.Reflection;

namespace FontForge;
public class Serializable<T>
{
    public static List<FieldInfo> SerializableFields;

    static Serializable()
    {
        var type = typeof(T);
        var fields = type.GetFields();

        SerializableFields = [];
        foreach (var field in fields)
        {
            var notSerializableAttributes = field.GetCustomAttributes(typeof(NotSerializableAttribute)).ToArray();
            if (notSerializableAttributes.Length != 0)
                continue;

            SerializableFields.Add(field);
        }
    }        
}