using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Uri _redirectUri = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Clear the outputs
            //
            txtRequest.Text = string.Empty;
            txtResponse.Text = string.Empty;

            // Generate the request URI
            //
            Uri requestUri = GetAuthorizeRequestUri();

            // Make the request via the webview
            //
            webView.Width = this.ActualWidth;
            webView.Height = this.ActualHeight;
            webView.Visibility = Visibility.Visible;
            webView.Navigate(requestUri);

            // Prepare the outputs
            //
            txtRequest.Text = requestUri.ToString();
            lblRequest.Visibility = Visibility.Visible;
            lblResponse.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event that captures when the webview navigates to unsupported uri schemes.
        /// In this case, this would intercept redirects to the "urn:ietf:wg:oauth:2.0:oob" mobile app URI.
        /// </summary>
        private void webView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            string uri = args.Uri.ToString();

            if (uri.StartsWith("urn"))
            {
                // Response is in the query string.
                //
                txtResponse.Text = uri;

                // Close the webview
                //
                webView.Visibility = Visibility.Collapsed;

                // This will prevent the webview from trying to actually load the page,
                // which will result in windows asking what program to open "urn" with
                //
                args.Handled = true;

                _redirectUri = null;
            }
        }

        private void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.Host == _redirectUri.Host)
            {
                txtResponse.Text = args.Uri.ToString();
                webView.Visibility = Visibility.Collapsed;

                _redirectUri = null;
            }
        }

        private Uri GetAuthorizeRequestUri()
        {
            const string RequestTemplate = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={0}&response_type={1}&redirect_uri={2}&scope={3}&state={4}&nonce={5}{6}";

            string appId = string.IsNullOrWhiteSpace(txtAppId.Text)
                ? "c724ef5a-c459-4b53-be63-7de6e7e3316a"
                : txtAppId.Text;

            string responseType = string.IsNullOrWhiteSpace(txtResponseType.Text)
                ? "id_token"
                : txtResponseType.Text;

            string redirectUri = string.IsNullOrWhiteSpace(txtRedirectUri.Text)
                ? "urn:ietf:wg:oauth:2.0:oob"
                : txtRedirectUri.Text;

            // Hack -- record the redirect URI in a member variable so we the NavigationStarted
            // event handler knows which event is the final redirect with the IDP's response.
            //
            _redirectUri = new Uri(redirectUri);

            string scopes = string.IsNullOrWhiteSpace(txtScopes.Text)
                ? "openid+profile+email"
                : txtScopes.Text;

            string state = string.IsNullOrWhiteSpace(txtState.Text)
                ? "12345"
                : txtState.Text;

            string nonce = string.IsNullOrWhiteSpace(txtNonce.Text)
                ? "67890"
                : txtNonce.Text;

            string additionalParams = string.IsNullOrWhiteSpace(txtAdditionalParams.Text)
                ? string.Empty
                : "&" + txtAdditionalParams.Text.TrimStart('&');

            string requestUri = string.Format(
                RequestTemplate,
                appId,
                responseType,
                redirectUri,
                scopes,
                state,
                nonce,
                additionalParams);

            return new Uri(requestUri);
        }

        /// <summary>
        /// Helper event handler that copies text from request/response
        /// text blocks to the clipboard when they are clicked.
        /// </summary>
        private void text_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            DataPackage data = new DataPackage();
            data.SetText(textBlock.Text);
            Clipboard.SetContent(data);
        }

        /// <summary>
        /// Helper event handler that resizes the webview to match the app's window
        /// when the app window is resized.
        /// </summary>
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (webView.Visibility == Visibility.Visible)
            {
                webView.Width = this.ActualWidth;
                webView.Height = this.ActualHeight;
            }
        }
    }
}
