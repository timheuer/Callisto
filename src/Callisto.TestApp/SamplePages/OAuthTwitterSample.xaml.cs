using Callisto.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OAuthTwitterSample : Page
    {
        // REST API v1.1 https://dev.twitter.com/docs/api/1.1
        string _getHomeTimelineUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        string _getMentionsUrl = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";
        string _getRetweetsOfMeUrl = "http://api.twitter.com/1.1/statuses/retweets_of_me.json";

        
        string _twitterRequestTokenUrl = "http://api.twitter.com/oauth/request_token";
        string _twitterAccessTokenUrl = "http://api.twitter.com/oauth/access_token";
        string _twitterAuthorizeUrl = "http://api.twitter.com/oauth/authorize";


        public OAuthTwitterSample()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {           
        }

        private async Task<string> sendHttpRequest(HttpRequestMessage requestMessage)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = int.MaxValue;
                httpClient.DefaultRequestHeaders.ExpectContinue = false;

                var response = await httpClient.SendAsync(requestMessage);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                setOutputText(e.Message);
                return string.Empty;
            }
        }        

        private static TokenPair getTokenPair(string responseText)
        {
            var tokenPair = new TokenPair();

            string[] keyValPairs = responseText.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        tokenPair.Token = splits[1];
                        break;
                    case "oauth_token_secret":
                        tokenPair.TokenSecret = splits[1];
                        break;
                }
            }

            return tokenPair;
        }

        private static HttpRequestMessage getTokenHttpRequestMessage(string url, string postHeader)
        {
            var httpRequestContent = new StringContent(postHeader);
            httpRequestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var httpRequestMsg = new HttpRequestMessage
            {
                Content = httpRequestContent,
                Method = HttpMethod.Post,
                RequestUri = new Uri(url, UriKind.Absolute),
            };
            return httpRequestMsg;
        }

        private string getOAuthSignature(string url, WebMethod webMethod, WebParameterCollection parameters, string tokenSecretKey = null)
        {            
            string stringToSign = OAuthTools.ConcatenateRequestElements(webMethod, url, parameters);

            string oAuthSignature = OAuthTools.GetSignature(OAuthSignatureMethod.HmacSha1, 
                OAuthSignatureTreatment.Escaped,
                stringToSign,
                consumerSecretKey.Text,
                tokenSecretKey);
            
            return oAuthSignature;   
        }

        private async void getRequestToken_Click_1(object sender, RoutedEventArgs e)
        {
            string url = _twitterRequestTokenUrl;

            string nonce = OAuthTools.GetNonce();
            string timeStamp = OAuthTools.GetTimestamp();

            var parameters = new WebParameterCollection(new[]{
                new WebPair("oauth_consumer_key", consumerKey.Text),
                new WebPair("oauth_nonce", nonce),
                new WebPair("oauth_signature_method", "HMAC-SHA1"),
                new WebPair("oauth_timestamp", timeStamp),
                new WebPair("oauth_version", "1.0"),                
            });            

            // prepare OAuth Header
            string oAuthSignature = getOAuthSignature(url, WebMethod.Post, parameters);
            string stringParams = OAuthTools.NormalizeRequestParameters(parameters);
            string httpAuthHeader = stringParams + "&oauth_signature=" + oAuthSignature;

            // prepare HttpRequestMessage
            var httpRequestMsg = getTokenHttpRequestMessage(url, httpAuthHeader);
            string responseText = await sendHttpRequest(httpRequestMsg);            

            if (!string.IsNullOrEmpty(responseText))
            {
                var tokenPair = getTokenPair(responseText);
                requestToken.Text = tokenPair.Token;
                requestTokenSecretKey.Text = tokenPair.TokenSecret;
                oAuthAuthorizeLink.Content = Uri.UnescapeDataString(_twitterAuthorizeUrl + "?oauth_token=" + tokenPair.Token);
            }
        }
       
        private async void getAccessToken_Click_1(object sender, RoutedEventArgs e)
        {
            string tokenSecretKey = requestTokenSecretKey.Text;
            string token = requestToken.Text;
            string url = _twitterAccessTokenUrl;

            string nonce = OAuthTools.GetNonce();
            string timeStamp = OAuthTools.GetTimestamp();

            var parameters = new WebParameterCollection(new[]{
                new WebPair("oauth_consumer_key", consumerKey.Text),
                new WebPair("oauth_nonce", nonce),
                new WebPair("oauth_signature_method", "HMAC-SHA1"),
                new WebPair("oauth_timestamp", timeStamp),                     
                new WebPair("oauth_token", token),
                new WebPair("oauth_verifier", oAuthVerifier.Text),
                new WebPair("oauth_version", "1.0"),           
            });

            // prepare OAuth Header
            string oAuthSignature = getOAuthSignature(url, WebMethod.Post, parameters, tokenSecretKey);
            string stringParams = OAuthTools.NormalizeRequestParameters(parameters);
            string httpAuthHeader = stringParams + "&oauth_signature=" + oAuthSignature;            

            // prepare HttpRequestMessage
            var httpRequestMsg = getTokenHttpRequestMessage(url, httpAuthHeader);
            string responseText = await sendHttpRequest(httpRequestMsg);        
            
            if (!string.IsNullOrEmpty(responseText))
            {
                var tokenPair = getTokenPair(responseText);
                accessToken.Text = tokenPair.Token;
                accessTokenSecretKey.Text = tokenPair.TokenSecret;                
            }
        }

        private async void requestTwitterApi(string url)
        {
            // Now we use Access Token Secret Key
            string tokenSecretKey = accessTokenSecretKey.Text;
            string token = accessToken.Text;

            string urlWithParams = url + "?" + QueryParams.Text;

            string nonce = OAuthTools.GetNonce();
            string timeStamp = OAuthTools.GetTimestamp();

            var parameters = new WebParameterCollection(new[]{
                new WebPair("oauth_consumer_key", consumerKey.Text),
                new WebPair("oauth_nonce", nonce),
                new WebPair("oauth_signature_method", "HMAC-SHA1"),
                new WebPair("oauth_timestamp", timeStamp),                            
                new WebPair("oauth_token", token),
                new WebPair("oauth_verifier", oAuthVerifier.Text),
                new WebPair("oauth_version", "1.0"),    
            });
            
            if (!string.IsNullOrEmpty(QueryParams.Text))
            {
                // e.g., from count=5&screen_name=user to IDictionary<string, string>
                var queryParams = QueryParams.Text
                    .Split('&')
                    .ToDictionary(p => p.Substring(0, p.IndexOf('=')), p => p.Substring(p.IndexOf('=') + 1));

                parameters.AddCollection(queryParams);

                // We need this line as OAuthHelper.SortParametersExcludingSignature does not return sorted parameters right now
                parameters = new WebParameterCollection(parameters.OrderBy(wp => wp.Name).ThenBy(wp => wp.Value));
            }

            // https://dev.twitter.com/docs/auth/creating-signature
            string oAuthSignature = getOAuthSignature(url, WebMethod.Get, parameters, tokenSecretKey);
            string stringQueryParams = OAuthTools.NormalizeRequestParameters(parameters);
            string httpAuthHeader = stringQueryParams + "&oauth_signature=" + oAuthSignature;


            string oAuthHeaderValue = "oauth_consumer_key=\"" + consumerKey.Text +
                  "\", oauth_nonce=\"" + nonce +
                  "\", oauth_signature=\"" + oAuthSignature +
                  "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timeStamp +
                  "\", oauth_token=\"" + accessToken.Text +
                  "\", oauth_verifier=\"" + oAuthVerifier.Text +
                  "\", oauth_version=\"1.0\"";

            // prepare HttpRequestMessage
            var httpRequestMsg = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(urlWithParams),
            };

            httpRequestMsg.Headers.Authorization = new AuthenticationHeaderValue("OAuth", oAuthHeaderValue);


            string responseText = await sendHttpRequest(httpRequestMsg);
            setOutputText(responseText);
        }

        private void setOutputText(string responseText)
        {
            webViewHost.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            logTextBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
            logTextBox.Text = responseText;
        }

        private void oAuthAuthorizeLink_Click_1(object sender, RoutedEventArgs e)
        {
            webViewHost.Visibility = Windows.UI.Xaml.Visibility.Visible;
            webViewHost.Navigate(new Uri(oAuthAuthorizeLink.Content.ToString()));
        }

        private void getHomeTimeline_Click_1(object sender, RoutedEventArgs e)
        {
            requestTwitterApi(_getHomeTimelineUrl);
        }        

        private void getMentions_Click_1(object sender, RoutedEventArgs e)
        {
            requestTwitterApi(_getMentionsUrl);
        }

        private void getRetweetsOfMe_Click_1(object sender, RoutedEventArgs e)
        {
            requestTwitterApi(_getRetweetsOfMeUrl);
        }
        
        class TokenPair
        {
            public string Token { get; set; }
            public string TokenSecret { get; set; }
        }

    }
}
