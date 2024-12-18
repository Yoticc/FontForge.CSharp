namespace FontForge;

[AttributeUsage(AttributeTargets.Field)]
public class UseCustomSerializerAttribute<T, T2> : Attribute where T : ICustomSerializer<T2> { }