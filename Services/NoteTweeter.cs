using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OAuth;
using System.Linq;

public class NoteTweeter
{
    private readonly INoteManager noteManager;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;

    public NoteTweeter(INoteManager noteManager,IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
    {
        this.noteManager = noteManager;
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }
    public async Task<bool> TweetNote(int noteId)
    {
        var notes = await noteManager.GetAuthorNotesByNameIdentifier(httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(c=>c.Type=="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
        var noteText=notes.SingleOrDefault(n=>n.Id==noteId).Text;

        var httpClient = new HttpClient();
        var url = $"https://api.twitter.com/1.1/statuses/update.json?status={noteText}";
        
        var consumer_key = configuration.GetValue<string>("TwitterConsumerKey");
        var consumer_secret = configuration.GetValue<string>("TwitterConsumerSecret");

        var access_token = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(c=>c.Type=="accesstoken").Value;
        var access_token_secret = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(c=>c.Type=="accesstokensecret").Value;

        OAuthRequest client = new OAuthRequest
        {
            Method = "POST",
            RequestUrl=url,
            Type = OAuthRequestType.ProtectedResource,
            SignatureMethod = OAuthSignatureMethod.HmacSha1,
            ConsumerKey = consumer_key,
            ConsumerSecret = consumer_secret,
            Token= access_token,
            TokenSecret=access_token_secret,
            Version = "1.0a",
            Realm = "twitter.com"
        };
        httpClient.DefaultRequestHeaders.Add("Authorization",client.GetAuthorizationHeader());
        await httpClient.PostAsync(url,null);
        return true;
    }
}