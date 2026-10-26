using System.Net.Http.Json;

namespace VotePreferentiel;

public class VoteProvider(HttpClient httpClient)
{
    public async Task<VoteModel> GetVotesAsync(string url, CancellationToken cancellationToken = default)
    {
        var voteModel = await httpClient.GetFromJsonAsync<VoteModel>(url, cancellationToken);

        return voteModel ?? throw new InvalidOperationException($"Le fichier de votes {url} est vide.");
    }
}
