using RazorLight;

namespace AuthenticationService.Feature.Email;

/// <summary>
/// The <see cref="EmailTemplateRenderer"/> class
/// setups an <see cref="RazorLightEngine"/> to fill string and email templates. 
/// </summary>
public class EmailTemplateRenderer
{
    /// <summary>
    /// The engine pointing towards the template directory.
    /// </summary>
    private readonly RazorLightEngine _razorEngine = new RazorLightEngineBuilder()
        .UseFileSystemProject(Directory.GetCurrentDirectory() + "/Templates") 
        .UseMemoryCachingProvider()
        .Build();

    /// <summary>
    /// Fills a target template with the passed model values. 
    /// </summary>
    /// <param name="templateName">The template name.</param>
    /// <param name="model">The model, a class or struct holding the values to fill the template with.</param>
    /// <typeparam name="T">The generic.</typeparam>
    /// <returns>A <see cref="Task"/> with the filled template.</returns>
    public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
    {
        var templatePath = $"{templateName}.cshtml"; 
        return await _razorEngine.CompileRenderAsync(templatePath, model);
    }
}
