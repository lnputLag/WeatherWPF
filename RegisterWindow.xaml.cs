﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeatherWPF.Models;

namespace WeatherWPF
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private AppDbContext _db;
        public RegisterWindow()
        {
            InitializeComponent();
           _db = new AppDbContext();
        }

        private void UserRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLoginField.Text.Trim();
            string email = UserEmailField.Text.Trim();
            string password = UserPassField.Password.Trim();
            if(login.Equals("") || !email.Contains("@") || password.Length < 3)
            {
                MessageBox.Show("Вы что-то ввели неверно!");
                return;
            }

            User user = new User(login, email, Hash(password));   
            _db.Users.Add(user);
            _db.SaveChanges();

            UserLoginField.Text = "";
            UserEmailField.Text = "";
            UserPassField.Password = "";
            UserRegister.Content = "Готово";
        }

        private string Hash(string input)
        {
            byte[] temp = Encoding.UTF8.GetBytes(input);
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(temp);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
