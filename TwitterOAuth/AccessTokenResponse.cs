using System;

public class AccessTokenResponse
{
    public string oauth_access_token { get; set; }
    public string oauth_access_token_secret { get; set; }
    public string UserID { get; set; }
    public string ScreenName { get; set; }
}