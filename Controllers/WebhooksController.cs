using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OAuth;
using ProjectNotes.TwitterOAuth.Queries;

public class WebhooksController : Controller
{
    private readonly ILogger<WebhooksController> logger;
    private readonly IConfiguration config;
    private readonly IMediator mediator;

    public WebhooksController(ILogger<WebhooksController> _logger, IConfiguration config, IMediator mediator)
    {
        logger = _logger;
        this.config = config;
        this.mediator = mediator;
    }

    /// <summary>
    /// Answers twitter CRC token request by signing token with consumer_secret and returning as response_token in JSON format.
    /// </summary>
    /// <param name="crc_token"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Twitter_CRC_Check(string crc_token)
    {
        logger.LogWarning($"Get Request from webhook receivd crc_token {crc_token}");
        var consumer_secret = config.GetValue<string>("TwitterConsumerSecret");
        var sha256_hash_digest = HMACSHA256(consumer_secret, crc_token);

        //construct response data with base64 encoded hash
        var crcResponse = new CRCResponse()
        {
            ResponseToken = "sha256=" + sha256_hash_digest
        };
        var response = JsonConvert.SerializeObject(crcResponse);

        return Ok(response);
    }

    /// <summary>
    /// When registered user receives a message twitter will send messageEvent to this endpoint.
    /// </summary>
    /// <param name="messageEvent"></param>
    /// <returns></returns>
    [HttpPost("/webhooks/twitter")]
    public async Task<IActionResult> EventReceiver([FromBody] JsonElement jsonElement)
    {
        //TODO Add twitter source check
        dynamic messageEvent = JsonConvert.DeserializeObject<dynamic>(jsonElement.GetRawText());
        var text = messageEvent.direct_message_events[0].message_create.message_data.text.ToString();
        var sender_id = messageEvent.direct_message_events[0].message_create.sender_id.ToString();
        var receiver_id = messageEvent.for_user_id.ToString();

        var url = "https://api.twitter.com/1.1/direct_messages/events/new.json";
        var consumer_secret = config.GetValue<string>("TwitterConsumerSecret");
        var consumer_key = config.GetValue<string>("TwitterConsumerKey");
        var accesstokens = await mediator.Send(new GetAccessTokensFromDBQuery(receiver_id));
        var access_token = accesstokens.Split(" ")[0];
        var access_token_secret = accesstokens.Split(" ")[1];

        if (text == "Salam")
        {
            //send message
            var body = "{\"event\": {\"type\": \"message_create\", \"message_create\": {\"target\": {\"recipient_id\": \"" + sender_id + "\"}, \"message_data\": {\"text\": \"Salam!\"}}}}";
            OAuthRequest client = new OAuthRequest
            {
                Method = "POST",
                RequestUrl = url,
                Type = OAuthRequestType.ProtectedResource,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = consumer_key,
                ConsumerSecret = consumer_secret,
                Token = access_token,
                TokenSecret = access_token_secret,
                Version = "1.0a",
                Realm = "twitter.com"
            };
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", client.GetAuthorizationHeader());
            await httpClient.PostAsync(url, new StringContent(body));
            return Ok();
        }

        return Ok();
    }

    public static string HMACSHA256(string key, string dataToSign)
    {
        Byte[] secretBytes = UTF8Encoding.UTF8.GetBytes(key);
        HMACSHA256 hmac = new HMACSHA256(secretBytes);
        Byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(dataToSign);
        Byte[] calcHash = hmac.ComputeHash(dataBytes);
        String calcHashString = Convert.ToBase64String(calcHash);
        return calcHashString;
    }
    public class CRCResponse
    {
        [JsonProperty("response_token")]
        public string ResponseToken { get; set; }
    }


}