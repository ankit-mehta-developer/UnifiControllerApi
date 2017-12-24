using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using UnifiControllerCommunicator.Helpers;

namespace UnifiControllerCommunicator
{
    public class UnifiControllerApi : IUnifiControllerApi
    {
        protected string Server { get; private set; }
        protected string UserName { get; private set; }
        protected SecureString Password { get; private set; }
        protected string SiteName { get; set; }

        /// <summary>
        /// Cookie container for all operations.
        /// </summary>
        protected CookieContainer Cookies { get; private set; }

        /// <summary>
        /// Logged in variable.
        /// </summary>
        public bool IsLoggedIn { get; set; }


        public UnifiControllerApi(string server, string username, SecureString pwd, string siteName = "default")
        {
            Server = server;
            UserName = username;
            Password = pwd;
            SiteName = siteName;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //Set our override class for checking certificate policies on SSL.
            ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
        }

        ~UnifiControllerApi()
        {
            if (IsLoggedIn)
            {
                LogOut().Wait();
            }
        }

        public async Task<bool> Login()
        {
            bool loggedIn = false;
            try
            {

                if (IsLoggedIn)
                {
                    return loggedIn;
                }

                Cookies = new CookieContainer();

                var jsonSer = new JavaScriptSerializer();

                var jsonParams = new Dictionary<string, object>();
                jsonParams.Add("username", UserName);
                jsonParams.Add("password", SecureStringHelper.ToInsecureString(Password));


                var currentCommand = jsonSer.Serialize(jsonParams);

                jsonParams.Clear();

                // Create the request and setup for the post.
                var strRequest = string.Format("{0}api/login", Server);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequest);

                httpWebRequest.ContentType = "application/json;charset=UTF-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = WebRequestMethods.Http.Post;
                httpWebRequest.ServicePoint.Expect100Continue = false;


                httpWebRequest.CookieContainer = Cookies;


                // Set the callback to handle the post of data.
                var postStream = await httpWebRequest.GetRequestStreamAsync();
                var byteArray = Encoding.UTF8.GetBytes(currentCommand);

                // Write to the request stream.
                await postStream.WriteAsync(byteArray, 0, byteArray.Length);
                await postStream.FlushAsync();
                postStream.Close();

                // Now read the reponse and process the data.
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseText = await streamReader.ReadToEndAsync();

                    jsonParams = jsonSer.Deserialize<Dictionary<string, object>>(responseText);

                    if (!jsonParams.ContainsKey("data"))
                    {
                        loggedIn = false;
                    }
                    else
                    {
                        IsLoggedIn = true;
                        loggedIn = true;
                    }

                }

            }
            catch (Exception ex)
            {
                if(System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Trace.WriteLine(ex);
                }
                throw;
            }

            return loggedIn;
        }

        public async Task LogOut()
        {
            try
            {

                if (!IsLoggedIn)
                {
                    return;
                }
                else
                {
                    string strRequest = string.Format("{0}logout", Server);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequest);
                    httpWebRequest.ContentType = "application/json;charset=UTF-8";
                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    httpWebRequest.AllowAutoRedirect = false;
                    httpWebRequest.CookieContainer = Cookies;

                    var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();

                    Cookies = null;
                    IsLoggedIn = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IDictionary<string, object>[]> GetClients()
        {
            var clients = new List<IDictionary<string, object>>();
            try
            {

                if (!IsLoggedIn)
                {
                    await Login();
                }

                var jsonSer = new JavaScriptSerializer();

                var jsonParams = new Dictionary<string, object>();

                var currentCommand = jsonSer.Serialize(jsonParams);

                jsonParams.Clear();

                // Create the request and setup for the post.
                string strRequest = string.Format("{0}api/s/{1}/stat/sta", Server, SiteName);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequest);
                httpWebRequest.ContentType = "application/json;charset=UTF-8";
                httpWebRequest.Method = WebRequestMethods.Http.Post;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.CookieContainer = Cookies;

                // Set the callback to handle the post of data.
                var postStream = await httpWebRequest.GetRequestStreamAsync();
                byte[] byteArray = Encoding.UTF8.GetBytes(currentCommand);

                // Write to the request stream.
                await postStream.WriteAsync(byteArray, 0, byteArray.Length);
                await postStream.FlushAsync();
                postStream.Close();


                // Now read the reponse and process the data.
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseText = await streamReader.ReadToEndAsync();

                    jsonParams = jsonSer.Deserialize<Dictionary<string, object>>(responseText);

                    if (!jsonParams.ContainsKey("data"))
                    {
                        throw new Exception("Error getting clients");
                    }

                    var list = (ArrayList)jsonParams["data"];

                    foreach (Dictionary<string, object> data in list)
                    {
                        clients.Add(data);
                    };
                }
            }
            catch (Exception)
            {
                await LogOut();
                throw;
            }

            return clients.ToArray();
        }
    }
}
