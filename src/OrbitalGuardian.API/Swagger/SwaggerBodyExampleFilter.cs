using System.Reflection;
using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OrbitalGuardian.API.Swagger;

public sealed class SwaggerBodyExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attr = context.MethodInfo.GetCustomAttribute<SwaggerBodyExampleAttribute>(inherit: false);
        if (attr is null || operation.RequestBody is null)
            return;

        if (!operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
            return;

        using var doc = JsonDocument.Parse(attr.Json);
        mediaType.Example = ToOpenApiAny(doc.RootElement);
    }

    private static IOpenApiAny ToOpenApiAny(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new OpenApiObject();
                foreach (var prop in element.EnumerateObject())
                    obj[prop.Name] = ToOpenApiAny(prop.Value);
                return obj;
            case JsonValueKind.Array:
                var arr = new OpenApiArray();
                foreach (var item in element.EnumerateArray())
                    arr.Add(ToOpenApiAny(item));
                return arr;
            case JsonValueKind.String:
                return new OpenApiString(element.GetString()!);
            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intVal))
                    return new OpenApiInteger(intVal);
                return new OpenApiDouble(element.GetDouble());
            case JsonValueKind.True:
                return new OpenApiBoolean(true);
            case JsonValueKind.False:
                return new OpenApiBoolean(false);
            default:
                return new OpenApiNull();
        }
    }
}
