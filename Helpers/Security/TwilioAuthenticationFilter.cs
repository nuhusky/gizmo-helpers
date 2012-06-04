using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gizmo.Helpers.Security
{

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class TwilioAuthentication : AuthorizeAttribute
    {
        protected virtual string GetHashFromRequest(HttpRequestBase request)

        {
            return request.Headers.Get("X-Twilio-Signature");
        }
        string GetEncryptionHashFrom(string url)

        {
            //Set your string here:
            string appKey = GetAuthToken();
            HMACSHA1 sha1Encryptor = new HMACSHA1(GetBytes(appKey));

            byte[] urlAsBytes = GetBytes(url);
            byte[] hash = sha1Encryptor.ComputeHash(urlAsBytes);
            return Convert.ToBase64String(hash);

        }
 
        protected virtual string GetAuthToken()
        {
            return ConfigurationManager.AppSettings["AuthToken"];

        }
 
        //Helpers
        private static byte[] GetBytes(string toTransform)

        {
            //create the byte[] array here:
            byte[] byteKey = new byte[toTransform.Length];

            //convert the string to a byte array here:
            Encoding.ASCII.GetBytes(toTransform.ToCharArray(), 0, toTransform.Length, byteKey, 0);

            return byteKey;
        }
        static IEnumerable<KeyValuePair<string, string>> GetOrderedForm(NameValueCollection form)

        {
            Dictionary<string, string> formDictionary = new Dictionary<string, string>();

            for (int i = 0; i < form.Count; i++)
                formDictionary.Add(form.GetKey(i), form.Get(i));
 
            var orderedList = formDictionary.OrderBy(x => x.Key);

 
            return orderedList;
        }
 
        protected override bool AuthorizeCore(HttpContextBase httpContext)

        {
            bool isRequestAuthorized = false;
            if (httpContext.Request.Url != null)

            {
                string url = httpContext.Request.Url.ToString();
                if (httpContext.IsPostNotification)
                {

                    IEnumerable<KeyValuePair<string, string>> orderedList = GetOrderedForm(httpContext.Request.Form);

                    url = orderedList.Aggregate(url, (current, keyValuePair) => current + String.Concat(keyValuePair.Key, keyValuePair.Value));
                }
 
                string calculatedHash = GetEncryptionHashFrom(url);

                string twilioHash = GetHashFromRequest(httpContext.Request);
                //compare to twilio
                if (string.Equals(calculatedHash, twilioHash))

                    isRequestAuthorized = true;
            }
            return isRequestAuthorized;
        }
    }
}