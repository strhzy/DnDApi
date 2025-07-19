using RazorLight;
using RazorLight.Extensions;
namespace DnDAPI.Services;

public class RazorTemplateService
{
    private readonly RazorLightEngine _engine;

    public RazorTemplateService()
    {
        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject("Assets")
            .Build();
    }

    public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
    {
        var templatePath = $"{templateName}.cshtml";
        return await _engine.CompileRenderAsync(templatePath, model);
    }
}
