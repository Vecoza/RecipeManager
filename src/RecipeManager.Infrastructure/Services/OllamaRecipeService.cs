using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.Infrastructure.Services;

public class OllamaRecipeService : IAiRecipeService
{
    private readonly HttpClient _http;
    private readonly string _ollamaUrl;
    private readonly string _model;

    public OllamaRecipeService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromMinutes(10);
        _ollamaUrl = config["Ollama:BaseUrl"] ?? "http://localhost:11434";
        _model = config["Ollama:Model"] ?? "llama3";
    }

    public async Task<GeneratedRecipeDto> GenerateRecipeAsync(IEnumerable<PantryItemDto> pantryItems, GenerateRecipeRequestDto preferences)
    {
        var ingredientList = string.Join(", ", pantryItems.Select(p => $"{p.Quantity} {p.Unit} {p.Name}".Trim()));

        var prompt = $$"""
            You are a chef. Respond with ONLY a JSON object, no explanation, no markdown.
            Recipe type: {{preferences.RecipeType}}. Meal: {{preferences.MealType}}. Cooking time: {{preferences.CookingTime}}.
            Given ingredients: {{ingredientList}}
            JSON format:
            {"title":"...","description":"...","prepTimeMinutes":10,"cookTimeMinutes":20,"servings":4,"ingredients":[{"name":"...","quantity":1.0,"unit":"..."}],"steps":[{"instruction":"..."}]}
            """;

        var requestBody = JsonSerializer.Serialize(new
        {
            model = _model,
            prompt,
            stream = false
        });

        var response = await _http.PostAsync(
            $"{_ollamaUrl}/api/generate",
            new StringContent(requestBody, Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var ollamaResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);
        var generatedText = ollamaResponse.GetProperty("response").GetString()
            ?? throw new InvalidOperationException("Empty response from Ollama.");

        var cleaned = CleanJson(generatedText);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<GeneratedRecipeDto>(cleaned, options)
            ?? throw new InvalidOperationException("Failed to parse generated recipe.");
    }

    private static string CleanJson(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start == -1 || end == -1)
            throw new InvalidOperationException("No valid JSON found in AI response.");
        return text[start..(end + 1)];
    }
}
