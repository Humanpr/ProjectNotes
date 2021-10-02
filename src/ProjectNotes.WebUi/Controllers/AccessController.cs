using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectNotes.Infrastructure.Persistance;
using ProjectNotes.Infrastructure.Users.Queries;
using ProjectNotes.Core.Entities;
using ProjectNotes.Infrastructure.Users.Commands;

namespace ProjectNotes.WebUi.Controllers
{
    //TODO HTTPClient
    public class AccessController : Controller
    {
        private readonly ILogger<AccessController> _logger;
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;
        private readonly NotesDbContext dbContext;

        public AccessController(ILogger<AccessController> logger, IMediator mediator, IConfiguration Configuration, NotesDbContext dbContext)
        {
            this.mediator = mediator;
            configuration = Configuration;
            this.dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        #region Login Tweeter
        /// <summary>
        /// Getting request token then Redirecting user to twitter login page to authenticate and authorize applicaiton.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoginTwitter()
        {
            var httpClient = new HttpClient();

            var consumer_key = configuration.GetValue<string>("TwitterConsumerKey");
            var consumer_secret = configuration.GetValue<string>("TwitterConsumerSecret");

            var callbackUrl = configuration.GetValue<string>("AppUrl") + "/signin-twitter";

#if DEBUG
            callbackUrl = "http://localhost:5000/signin-twitter";
#endif

            var baseUrl = "https://api.twitter.com/oauth/request_token";
            var requestTokenResponse = await mediator.Send(new GetRequestTokenResponseCommand(consumer_key, consumer_secret, callbackUrl, baseUrl));
            if (requestTokenResponse.callback_confirmed)
                return Redirect($"https://api.twitter.com/oauth/authenticate?oauth_token={requestTokenResponse.oauth_token}");
            return new JsonResult("CallbackNotConfirmed");
        }

        /// <summary>
        /// After Successful authentication and authorization twitter sends access tokens to this endpoint.
        /// </summary>
        /// <param name="oauth_token"></param>
        /// <param name="oauth_verifier"></param>
        /// <returns></returns>
        [HttpGet("/signin-twitter")]
        public async Task<IActionResult> Callback([FromQuery] string oauth_token, [FromQuery] string oauth_verifier)
        {
            //TODO make sure oauth_token is the same with the one acquired in step 1
            var httpClient = new HttpClient();
            var url = "https://api.twitter.com/oauth/access_token";

            var consumer_key = configuration.GetValue<string>("TwitterConsumerKey");
            var consumer_secret = configuration.GetValue<string>("TwitterConsumerSecret");

            var accessTokenResponse = await mediator.Send(new GetAccessTokenResponseCommand
            {
                oauth_token = oauth_token,
                oauth_verifier = oauth_verifier,
                consumer_key = consumer_key,
                consumer_secret = consumer_secret,
                baseUrl = url
            });

            // if user record exists already then update access tokens
            if (await mediator.Send(new CheckUserExistanceCommand(accessTokenResponse.UserID)))
            {
                var author = await mediator.Send(new GetUserByNameIdentifierQuery(accessTokenResponse.UserID));
                author.accesstoken = accessTokenResponse.oauth_access_token;
                author.accesstokensecret = accessTokenResponse.oauth_access_token_secret;
                dbContext.Update(author);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                //Add user records to DB
                var newAuthor = new Author { NameIdentifier = accessTokenResponse.UserID, AuthorName = accessTokenResponse.ScreenName, accesstoken = accessTokenResponse.oauth_access_token, accesstokensecret = accessTokenResponse.oauth_access_token_secret, RegisterMethod = "Twitter" };
                await mediator.Send(new AddUserCommand { NewUser = newAuthor });
            }
            // signin user
            Claim name = new Claim("name", accessTokenResponse.ScreenName);
            Claim nameid = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", accessTokenResponse.UserID);
            Claim token = new Claim("accesstoken", accessTokenResponse.oauth_access_token);
            Claim tokensecret = new Claim("accesstokensecret", accessTokenResponse.oauth_access_token_secret);

            List<Claim> claims = new() { name, nameid, token, tokensecret };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(principal);
            return Redirect("/");
        }
        #endregion

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult AskUserLogoutMainAccount()
        {
            return View();
        }

    }
}
