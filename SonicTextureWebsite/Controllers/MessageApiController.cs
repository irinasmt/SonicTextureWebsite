using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using WebApplication2.Models.MessageApi;
using WebApplication2.Repos;
using WebApplication2.Filters;

namespace WebApplication2.Controllers
{
    public class MessageApiController : ApiController
    {
       
        [HttpGet]
        [Authorize]
        [Route("test")]
        public IHttpActionResult Test()
        {
            GetBusinessEmail();

            return Ok();
        }

        private string GetBusinessEmail()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var idClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var userEmail = idClaim?.Value;
            return userEmail;
        }

        [HttpPost, Route("api/message")]
        [Authorize]
        public dynamic Index(List<BusinessMessagesRequest> messagesRequest)
        {
            var response = new List<MessageResponse>();
          
            // get the access token from LWA
            HttpResponseMessage responseApi = GetAccessTokenFromLWA();
            SkillMessagingApiResponse accessTokenResult = responseApi.Content.ReadAsAsync<SkillMessagingApiResponse>().Result;
            if(messagesRequest==null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            if (!responseApi.IsSuccessStatusCode)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var businessEmail = GetBusinessEmail();
            var userRepo = new UsersRepo();
            var businessId = userRepo.GetUserIdByEmail(businessEmail);
            var roles = Roles.GetRolesForUser(businessId);
            var isUserInTestMode = roles.FirstOrDefault(r => r.Contains("test")) != null;

            foreach (var message in messagesRequest)
            {
                if (!EmailAddress.IsValidEmail(message.Email))
                {
                    MessageHasNotBeenSent(response, message, "invalid email");
                    continue;
                }

                if (message.Message.Length > 256)
                {
                    MessageHasNotBeenSent(response, message, "message is too long, message length must not be longer than 256 characters");
                    continue;
                }

                if (!userRepo.HasTheUserSignedUpForLists(message.Email))
                {
                    MessageHasNotBeenSent(response, message, "email address does not exit in our system");
                    continue;
                }

                if (isUserInTestMode)
                {
                    if (!userRepo.HasTheBusinessAccessToTheUserEmail(message.Email, businessId))
                    {
                        MessageHasNotBeenSent(response, message, "You don't have permission to test with this email address.");
                        return response;
                    }

                }
                else
                {
                    if (userRepo.HasTheBusinessAlreadySentAMessageForUser(message.Email, businessEmail))
                    {
                        MessageHasNotBeenSent(response, message, "You have already sent a message to this user's email today");
                        continue;
                    }
                }
                // check if has access to those emaill addresses
                // if user is in production role
                // carry on

                var userId = userRepo.GetUserIdByEmail(message.Email);

                
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessTokenResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var getTokenRequest = new GetTokenRequest.MessageAttribute { Data = new object() };
                var response1 = client.PostAsJsonAsync("https://api.eu.amazonalexa.com/v1/skillmessages/users/" + userId,
                    getTokenRequest).Result;

                if (response1.IsSuccessStatusCode)
                {
                    if (userRepo.InsertMessage(userId, message.Email, businessEmail, message.Message))
                    {
                        response.Add(new MessageResponse
                        {
                            Email = message.Email,
                            HasMessageBeenSent = true
                        });
                    }
                    else
                    {
                        MessageHasNotBeenSent(response, message, "internal server error, you can try again");
                    }
                }
                else
                {
                    MessageHasNotBeenSent(response, message, "internal server error, you can try again");

                }
            }

            return response;

        }



        [HttpPost, Route("testmessageapi")]
        [Authorize]
        public dynamic SendMessageForTestApi(BusinessMessagesRequest message)
        {
            if (message == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var response = new List<MessageResponse>();

            // get the access token from LWA
            HttpResponseMessage responseApi = GetAccessTokenFromLWA();
            SkillMessagingApiResponse accessTokenResult = responseApi.Content.ReadAsAsync<SkillMessagingApiResponse>().Result;
            if (!responseApi.IsSuccessStatusCode)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var businessEmail = GetBusinessEmail();
            var userRepo = new UsersRepo();

          
                if (!EmailAddress.IsValidEmail(message.Email))
                {
                    MessageHasNotBeenSent(response, message, "invalid email");
                    return response;
                }

                if (message.Message.Length > 256)
                {
                    MessageHasNotBeenSent(response, message, "message is too long, message length must not be longer than 256 characters");
                    return response;
            }

                if (!userRepo.HasTheUserSignedUpForLists(message.Email))
                {
                    MessageHasNotBeenSent(response, message, "email address does not exit in our system");
                    return response; 
                }
               
                if (!userRepo.HasTheBusinessAccessToTheUserEmail(message.Email, User.Identity.GetUserId()))
                {
                    MessageHasNotBeenSent(response, message, "You don't have permission to test with this email address.");
                    return response;
                }
               

                var userId = userRepo.GetUserIdByEmail(message.Email);


                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessTokenResult.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var getTokenRequest = new GetTokenRequest.MessageAttribute { Data = new object() };
                var response1 = client.PostAsJsonAsync("https://api.eu.amazonalexa.com/v1/skillmessages/users/" + userId,
                    getTokenRequest).Result;

                if (response1.IsSuccessStatusCode)
                {
                    if (userRepo.InsertMessage(userId, message.Email, businessEmail, message.Message))
                    {
                        response.Add(new MessageResponse
                        {
                            Email = message.Email,
                            HasMessageBeenSent = true
                        });
                    }
                    else
                    {
                        MessageHasNotBeenSent(response, message, "internal server error, you can try again");
                    }
                }
                else
                {
                    MessageHasNotBeenSent(response, message, "internal server error, you can try again");

                }
            

            return response;

        }
        
      

        private static void MessageHasNotBeenSent(List<MessageResponse> response, BusinessMessagesRequest message, string failureReason)
        {
            response.Add(new MessageResponse
            {
                Email = message.Email,
                HasMessageBeenSent = false,
                FailureReason = failureReason
            });
            return;
        }

        private static HttpResponseMessage GetAccessTokenFromLWA()
        {
            HttpClient clientGetAccessToken = new HttpClient();
            clientGetAccessToken.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var skillMessaging = new SkillMessagingApiRequest();
            var responseApi = clientGetAccessToken.PostAsJsonAsync("https://api.amazon.com/auth/O2/token", skillMessaging).Result;
            return responseApi;
        }
    }

    public static class EmailAddress
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
