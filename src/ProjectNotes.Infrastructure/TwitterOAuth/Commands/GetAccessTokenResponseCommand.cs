using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OAuth;

public class GetAccessTokenResponseCommand: IRequest<AccessTokenResponse>
{
    public string consumer_key { get; set; }
    public string consumer_secret { get; set; }
    public string baseUrl { get; set; }
    public string oauth_token { get; set; }
    public string oauth_verifier { get; set; }
}

public class GetAccessTokenResponseCommandHandler : IRequestHandler<GetAccessTokenResponseCommand, AccessTokenResponse>
{
    public async Task<AccessTokenResponse> Handle(GetAccessTokenResponseCommand request, CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();
        OAuthRequest client = new OAuthRequest
        {
            Method = "POST",
            RequestUrl = request.baseUrl,
            Type = OAuthRequestType.AccessToken,
            SignatureMethod = OAuthSignatureMethod.HmacSha1,
            ConsumerKey = request.consumer_key,
            ConsumerSecret = request.consumer_secret,
            Verifier=request.oauth_verifier,
            Token=request.oauth_token,
            Version = "1.0a",
            Realm = "twitter.com"
        };

        var authorizationHeader = client.GetAuthorizationHeader();
        httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader.Trim());
        StringContent content = new StringContent($"oauth_verifier={request.oauth_verifier}");

        var response = await httpClient.PostAsync(request.baseUrl, content);
        var stringresponse = await response.Content.ReadAsStringAsync();

        var oauth_acess_token = stringresponse.Split("&")[0].Split("=")[1];
        var oauth_acess_token_secret = stringresponse.Split("&")[1].Split("=")[1];
        var userID = stringresponse.Split("&")[2].Split("=")[1];
        var screenName = stringresponse.Split("&")[3].Split("=")[1];
        return new AccessTokenResponse
        {
            oauth_access_token = oauth_acess_token,
            oauth_access_token_secret = oauth_acess_token_secret,
            UserID = userID,
            ScreenName = screenName
        };
    }
}