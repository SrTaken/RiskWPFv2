using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class User: INotifyPropertyChanged
    {
        private string username;
        private string password;
        private int id;
        private string nombre;

        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonProperty("login")]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [JsonProperty("password")]
        public string Password
        {
            get { return password; }
            set { password = cifrate(value); }
        }

        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty("nom")]
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }


        public User(string username, string password)
        {
            this.username = username;
            this.password = cifrate(password);
        }
        public User(string username)
        {
            this.username = username;
        }
        public User()
        {
        }

        private string cifrate(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
