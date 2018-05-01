using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace WebApplication2.Repos
{
    public class UsersRepo
    {
        public bool HasTheUserSignedUpForLists(string email)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                var user = db.UsersEmails.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    if (!user.HasGivenAccess)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public string GetUserIdByEmail(string email)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                var user = db.UsersEmails.FirstOrDefault(u => u.Email == email);

                return user.UserId;

            }
        }

        public bool HasTheBusinessAlreadySentAMessageForUser(string userEmail, string businessEmail)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                var user = db.UserMessages.FirstOrDefault(u => u.BusinessEmail == businessEmail 
                                                               && System.Data.Entity.DbFunctions.TruncateTime(u.AddedDate) == DateTime.Today.Date &&
                                                               u.CustomerEmail == userEmail);

                return user != null;
            }

        }

        public bool HasTheBusinessAccessToTheUserEmail(string userEmail, string businessId)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                var user = db.BusinessesTestEmails.FirstOrDefault(u => u.BusinessUserId == businessId
                                                               && u.UserEmail == userEmail);

                return user != null;
            }

        }

        public bool InsertMessage(string userId, string userEmail, string businessEmail, string message)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                var userMessage = new UserMessage
                {
                    UserId = userId,
                    CustomerEmail = userEmail,
                    BusinessEmail = businessEmail,
                    Message = message,
                    AddedDate = DateTime.Today.Date,
                    WasSent = false
                };

                db.UserMessages.Add(userMessage);
               
                try
                {
                    db.SaveChanges();
                    db.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public bool InsertTestEmails(string userId, List<string> emails)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                foreach (var email in emails)
                {
                    var businessesTestSecondEmail = new BusinessesTestEmail
                    {
                        BusinessUserId = userId,
                        UserEmail = email,
                    };

                    db.BusinessesTestEmails.Add(businessesTestSecondEmail);
                }
              
                try
                {
                    db.SaveChanges();
                    db.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        public bool IsTheUserInTestRole(string userEmail)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {
                var user = db.AspNetUsers.FirstOrDefault(u => u.UserName == userEmail);
                var testRoleId = db.AspNetRoles.FirstOrDefault(r => r.Name == "Test")?.Id;
                //return db.AspNetUserRoles.FirstOrDefault(r => r.Name == "Test")?.Id;
                return user != null;
            }

        }
    }
}