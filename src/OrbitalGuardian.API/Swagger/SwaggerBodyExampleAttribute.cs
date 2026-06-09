namespace OrbitalGuardian.API.Swagger;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SwaggerBodyExampleAttribute : Attribute
{
    public string Json { get; }
    public SwaggerBodyExampleAttribute(string json) => Json = json;
}
