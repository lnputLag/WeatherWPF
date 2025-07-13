using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.IO;
using System.Xml.Serialization;
using WeatherWPF.Models;
using System;

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
            MainScreen.IsChecked = true;

            if(!File.Exists("user.xml"))
            {
                ShowAuthWindow();
            }

            //Десириализация
            XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
            using (FileStream file = new FileStream("user.xml", FileMode.Open))
            {
                AuthUser auth = (AuthUser)xml.Deserialize(file);
                UserNameLabel.Content = auth.Login;
            }
        }

        private void ShowAuthWindow()
        {
            Hide();
            AuthWindow window = new AuthWindow();
            window.Show();
            Close();
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string objName = ((RadioButton)sender).Name;
            //MessageBox.Show(objName);

            StackPanel[] panels = { MainScreenPanel };
            foreach (var panel in panels)
                panel.Visibility = Visibility.Hidden;
           
            switch(objName)
            {
                case "MainScreen": MainScreenPanel.Visibility = Visibility.Visible; break;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("user.xml");
            ShowAuthWindow();
        }
    }
}
