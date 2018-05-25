using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirstFloor.ModernUI.App.Content
{
    /// <summary>
    /// Interaction logic for ControlsStylesSampleForm.xaml
    /// </summary>
    public partial class ControlsStylesLogin : UserControl
    {
        Window parentWindow;

        public ControlsStylesLogin()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // select first control on the form
            Keyboard.Focus(this.Username);
            parentWindow = Window.GetWindow(this);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Content.BBCode = string.Empty;
            if (string.IsNullOrEmpty(this.Username.Text) && string.IsNullOrEmpty(this.Password.Text))
                return;

            parentWindow.ProgressStatus.Visibility = Visibility.Visible;
            
            Content.BBCode = await GetContent();
            ProgressStatus.Visibility = Visibility.Hidden;
        }

        private async Task<string> GetContent()
        {
            string formUrl = "https://vozforums.com/login.php?do=login"; // NOTE: This is the URL the form POSTs to, not the URL of the form (you can find this in the "action" attribute of the HTML's form tag
            string postData = "vb_login_username=otarachi&vb_login_password=&s=&securitytoken=guest&do=login&vb_login_md5password=7c07f76663afad7ef45477c103368260&vb_login_md5password_utf=7c07f76663afad7ef45477c103368260";
            string pageSource = string.Empty;
            string cookieHeader;

            await Task.Factory.StartNew(() =>
            {
                WebRequest req = WebRequest.Create(formUrl);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(postData);
                req.ContentLength = bytes.Length;
                using (Stream os = req.GetRequestStream())
                {
                    os.Write(bytes, 0, bytes.Length);
                }
                WebResponse resp = req.GetResponse();
                cookieHeader = resp.Headers["Set-cookie"];

                string getUrl = "https://vozforums.com/search.php?do=getnew";
                WebRequest getRequest = WebRequest.Create(getUrl);
                getRequest.Headers.Add("Cookie", cookieHeader);
                WebResponse getResponse = getRequest.GetResponse();
                using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                {
                    pageSource = sr.ReadToEnd();
                }
            });

            return pageSource;


            //string applicationEncoded = ;
            //var response = Task.Run(async () =>
            //{
            //    responseMessage = await client.PostAsync(loginUrl,
            //        new StringContent(postData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
            //    HttpResponseMessage resultPlaylist = await client.GetAsync(loginUrl);
            //    return resultPlaylist.Content.ReadAsStringAsync();
            //});
            //Content.BBCode = response.Result.Result.ToString();
        }

        public static HttpClient client = new HttpClient();
        public static HttpResponseMessage responseMessage = new HttpResponseMessage();
        private static string loginUrl = "https://vozforums.com/login.php?do=login";
    }
}
