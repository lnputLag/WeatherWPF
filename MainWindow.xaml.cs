using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace WeatherWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string API_KEY = "3d9de74844d28377e81415151cbe6a66";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetWeatherBtn_Click(object sender, RoutedEventArgs e)
        {
            string city = UserCityTextBox.Text.Trim();
            if(city.Length < 2)
            {
                MessageBox.Show("Укажите верный город");
                return;
            }

            try 
            {
                string data = await GetWeather(city);
                var json = JObject.Parse(data);
                string temp = json["main"]["temp"].ToString();
                WeatherResults.Content = $"В городе {city} {temp} градусов";
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Укажите корректный город");
                WeatherResults.Content = "";
            }
        }

        private async Task<string> GetWeather(string city)
        {
            HttpClient client = new HttpClient();
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric";
            //string responce = await client.GetStringAsync(url);
            //return responce;
            return await client.GetStringAsync(url);
        }
    }
}
