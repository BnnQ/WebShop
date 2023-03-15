using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;

namespace Homework.Authentication
{
    public class GoogleOAuthEvents : OAuthEvents
    {
        public override async Task CreatingTicket(OAuthCreatingTicketContext context)
        {
            await base.CreatingTicket(context);

            var accessToken = context.AccessToken;
            var credential = GoogleCredential.FromAccessToken(accessToken);
            var service = new PeopleServiceService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "WebShop"
            });

            var request = service.People.Get("people/me");
            request.PersonFields = "birthdays";

            var person = await request.ExecuteAsync();
            var dateOfBirth = person.Birthdays?.FirstOrDefault()?.Date;
            if (dateOfBirth is not null)
            {
                var identity = (context.Principal!.Identity as ClaimsIdentity)!;
                identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, new DateTime(dateOfBirth.Year ?? 0, dateOfBirth.Month ?? 0, dateOfBirth.Day ?? 0).ToString("yyyy-MM-dd"), ClaimValueTypes.Date));
            }   

        }
    }
}