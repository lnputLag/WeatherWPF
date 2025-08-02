using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.IO;
using System.Xml.Serialization;
using WeatherWPF.Models;
using System;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Media;

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

            StackPanel[] panels = { MainScreenPanel, CabinetScreenPanel, NotesScreenPanel };
            foreach (var panel in panels)
                panel.Visibility = Visibility.Hidden;
           
            switch(objName)
            {
                case "MainScreen": MainScreenPanel.Visibility = Visibility.Visible; break;
                case "CabinetScreen": CabinetScreenPanel.Visibility = Visibility.Visible; break;
                case "NotesScreen": NotesScreenPanel.Visibility = Visibility.Visible; break;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("user.xml");
            ShowAuthWindow();
        }

        private void UserChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLogin.Text.Trim();
            string email = UserEmail.Text.Trim();
            if (login.Equals("") || !email.Contains("@") )
            {
                MessageBox.Show("Вы что-то ввели неверно!");
                return;
            }

            AppDbContext db = new AppDbContext();
            int countUsers = Convert.ToInt32(db.Users.Count(el => el.Login == login));
            if(countUsers !=0 && !login.Equals(UserNameLabel.Content))
            {
                MessageBox.Show("Такой логин уже занят");
                return;
            }

            User user = db.Users.FirstOrDefault(el => el.Login == UserNameLabel.Content.ToString());
            user.Email = email;
            user.Login = login;
            db.SaveChanges();
            UserNameLabel.Content = login;
            UserChangeBtn.Content = "Готово";

            AuthUser auth = new AuthUser(login, email);
            XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
            using (FileStream file = new FileStream("user.xml", FileMode.Create))
            {
                xml.Serialize(file, auth);
            }
        }

        private void MenuOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
        }

        private void MenuSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
        }

        private void TimesNewRomanSetText_Click(object sender, RoutedEventArgs e)
        {
            UserNotesTextBox.FontFamily = new FontFamily("Times New Roman");
            VerdanaSetText.IsChecked = false;
        }

        private void VerdanaSetText_Click(object sender, RoutedEventArgs e)
        {
            UserNotesTextBox.FontFamily = new FontFamily("Verdana");
            TimesNewRomanSetText.IsChecked = false;
        }
    }
}
