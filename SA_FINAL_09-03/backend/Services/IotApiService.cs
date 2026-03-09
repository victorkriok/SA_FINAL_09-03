using System.Text.Json;

namespace api.Services
{
    public class IotApiService 
    {
        private readonly HttpClient _httpClient;

        public IotApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // O passe livre pra não travar na tela da Microsoft!
            _httpClient.DefaultRequestHeaders.Add("X-Tunnel-Skip-AntiPhishing-Page", "true");
        }

        public async Task<List<LeituraIotDto>> BuscarLeituras()
        {
            try
            {
                string url = "https://qgsxkcn5-5269.brs.devtunnels.ms/api/sensor/leituras"; 
                
                var resposta = await _httpClient.GetAsync(url);
                
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<LeituraIotDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                return new List<LeituraIotDto>(); 
            }
            catch 
            { 
                return new List<LeituraIotDto>(); // Se der erro, não quebra o sistema
            }
        }
    }
}