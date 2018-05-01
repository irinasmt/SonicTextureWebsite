using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Repos
{
    public class BusinessRepo
    {
        public bool InsertSubmitCertification(string businessName, string businessEmail, string intention, string messageExample)
        {
            using (ToDoListEntities db = new ToDoListEntities())
            {   
                var businessesCertification = new BusinessesCertification()
                {
                    BusinessEmail = businessEmail,
                    BusinessName = businessName,
                    MessagesIntetnion = intention,
                    MessagesExamples = messageExample
                };

                db.BusinessesCertifications.Add(businessesCertification);

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
    }
}