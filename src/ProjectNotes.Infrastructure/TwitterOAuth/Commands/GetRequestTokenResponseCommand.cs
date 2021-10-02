using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OAuth;

public class GetRequestTokenResponseCommand : IRequest<RequestTokenResponse>
{
    public string consumer_key { get; set; }
    public string consumer_secret { get; set; }
    public string callback_url { get; set; }
    public string baseUrl { get; set; }
    public GetRequestTokenResponseCommand(string consumer_key,string consumer_secret,string callback_url,string baseUrl)
    {
        this.consumer_key = consumer_key;
        this.consumer_secret = consumer_secret;
        this.callback_url = callback_url;
        this.baseUrl = baseUrl;
    }
}

public class GetRequestTokenResponseCommandHandler : IRequestHandler<GetRequestTokenResponseCommand, RequestTokenResponse>
{
    public async Task<RequestTokenResponse> Handle(GetRequestTokenResponseCommand request, CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();
        
        OAuthRequest client = new OAuthRequest
        {
            Method = "POST",
            Type = OAuthRequestType.RequestToken,
            SignatureMethod = OAuthSignatureMethod.HmacSha1,
            ConsumerKey = request.consumer_key,
            ConsumerSecret = request.consumer_secret,
            RequestUrl = request.baseUrl,
            CallbackUrl = request.callback_url,
            Version = "1.0a",
            Realm = "twitter.com"
        };

        var authorizationHeader = client.GetAuthorizationHeader();
        httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader.Trim());
        var response = await httpClient.PostAsync(request.baseUrl, null);

        var stringresponse = await response.Content.ReadAsStringAsync();
         
        var oauthtoken = stringresponse.Split("&")[0].Split("=")[1];
        var oauthtokensecret = stringresponse.Split("&")[1].Split("=")[1];
        var callbackconfirmed = Convert.ToBoolean(stringresponse.Split("&")[2].Split("=")[1]);

        return new RequestTokenResponse
        {
            oauth_token = oauthtoken,
            oauth_token_secret = oauthtokensecret,
            callback_confirmed = callbackconfirmed
        };
    }
}