using System;


public class RequestTokenResponse
{
    public string oauth_token { get; set; }
    public string oauth_token_secret { get; set; }
    public bool callback_confirmed { get; set; }
}