using System.Text;
using System.Text.Json;
using HtmlAgilityPack;
using Org.BouncyCastle.Cms;

namespace MyFirstHttp.Framework.Utilities;

public class SteamMarketService
{
    private readonly int _needItemsCount = 50;
    public async Task<string> GetHeroesHtmlAsync(string heroName)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/141.0.0.0");
        http.Timeout = TimeSpan.FromSeconds(15);
        
        heroName = heroName.ToLower();

        var sb = new StringBuilder();
        sb.AppendLine("<ol>");
        
        int collectedItemsCount = 0;
        
        for (int i = 0; i < _needItemsCount; i+=30)
        {
            try
            {
                var json = await http.GetStringAsync($"https://steamcommunity.com/market/actions?q=Search&qp=[{{\"appid\":570,\"filters\":{{\"Hero\":[\"npc_dota_hero_{heroName}\"]}},\"price\":{{\"eCurrency\":5}},\"accessoryFilters\":{{}},\"start\":{i}}}]");
                var jsonDocument = JsonDocument.Parse(json);
                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("results", out var results) ||
                    results.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine($"[SteamMarketService] Invalid json data about {heroName}");
                    break;
                }



                foreach (var item in results.EnumerateArray())
                {
                    if (collectedItemsCount >= _needItemsCount)
                    {
                        break;
                    }

                    if (item.TryGetProperty("asset_description", out var assetDescription) &&
                        assetDescription.TryGetProperty("name", out var assetName) &&
                        assetDescription.TryGetProperty("type", out var assetType))
                    {
                        sb.AppendLine($"<li>{assetName.GetString()} - {assetType.GetString()}</li>");
                        collectedItemsCount++;
                    }
                }

                if (results.GetArrayLength() < 30)
                {
                    break;
                }

                if (collectedItemsCount < _needItemsCount)
                {
                    await Task.Delay(500);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[SteamMarketService] HttpRequestException or Steam is not available: {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SteamMarketService] Exception: {ex.Message}");
                break;
            }
        }
        sb.AppendLine("</ol>");
        
        return sb.ToString();
    }
}