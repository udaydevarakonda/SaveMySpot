using System.Net.Http.Json;
using SaveMySpot.Application.Models;
using SaveMySpot.Application.Models.MasterData;
using SaveMySpot.Application.Models.Requests;
using SaveMySpot.Application.Constants;

namespace SaveMySpot.Services;

public class FoodOrdersClient
{
    private readonly HttpClient _httpClient;

    public Uri? BaseAddress => _httpClient.BaseAddress;

    public FoodOrdersClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FoodOrderDto>> GetMonthlyAsync(int year, int month)
    {
        var url = $"{ApiEndpoints.FoodOrders.GetByMonth}?year={year}&month={month}";

        return await _httpClient.GetFromJsonAsync<List<FoodOrderDto>>(url) ?? new List<FoodOrderDto>();
    }

    public async Task<PagedResult<FoodOrderDto>> GetReportAsync(DateOnly? startDate, DateOnly? endDate, int page, int pageSize)
    {
        var qs = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };
        if (startDate.HasValue)
        {
            qs.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString(FormattingConstants.DateFormats.ApiDate))}");
        }
        if (endDate.HasValue)
        {
            qs.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString(FormattingConstants.DateFormats.ApiDate))}");
        }

        var url = ApiEndpoints.FoodOrders.GetReport + (qs.Count > 0 ? "?" + string.Join("&", qs) : string.Empty);
        return await _httpClient.GetFromJsonAsync<PagedResult<FoodOrderDto>>(url)
               ?? new PagedResult<FoodOrderDto>(Array.Empty<FoodOrderDto>(), page, pageSize, 0);
    }

    public async Task<List<FoodOrderDto>> GetRangeAsync(DateOnly? startDate, DateOnly? endDate)
    {
        var qs = new List<string>();
        if (startDate.HasValue)
        {
            qs.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString(FormattingConstants.DateFormats.ApiDate))}");
        }
        if (endDate.HasValue)
        {
            qs.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString(FormattingConstants.DateFormats.ApiDate))}");
        }

        var url = ApiEndpoints.FoodOrders.GetByRange + (qs.Count > 0 ? "?" + string.Join("&", qs) : string.Empty);
        return await _httpClient.GetFromJsonAsync<List<FoodOrderDto>>(url) ?? new List<FoodOrderDto>();
    }

    public async Task<MasterDataDto?> GetMasterDataAsync()
    {
        return await _httpClient.GetFromJsonAsync<MasterDataDto>(ApiEndpoints.MasterData.Get);
    }

    public async Task<List<FoodOrderDto>> RegisterRangeAsync(CreateFoodOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.FoodOrders.RegisterRange, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOrderDto>>()
               ?? new List<FoodOrderDto>();
    }
}
