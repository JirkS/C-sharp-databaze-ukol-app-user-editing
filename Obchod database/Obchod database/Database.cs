using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.LinkLabel;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Runtime.Remoting.Messaging;

namespace Obchod_database
{
    public class Database
    {

        private static SqlConnection connection = null;
        private List<int> idsZakaznik;
        private List<int> idsVyrobce;
        private List<int> idsObjednavka;
        private List<int> idsVyrobek;
        private bool nacteno;

        /// <summary>
        /// constructor
        /// </summary>
        public Database()
        {
            // setting login form configuration file App.config
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;
            connection = new SqlConnection(consStringBuilder.ConnectionString);

            // the very first attempt of connecting to a database 
            try
            {
                using (connection)
                {
                    connection.Open();
                }
            }
            catch (Exception e)
            { 
                MessageBox.Show("Spatne zadane hodnoty: " +e.Message);
            }

            nacteno = false;
            idsZakaznik = new List<int>();
            idsVyrobce = new List<int>();
            idsObjednavka = new List<int>();
            idsVyrobek = new List<int>();
            nacteniIdsZakaznik();
            nacteniIdsVyrobce();
            nacteniIdsObjednavka();
            nacteniIdsVyrobek();
        }

        /// <summary>
        /// getters
        /// </summary>
        public List<int> IdsZakaznik
        {
            get { return idsZakaznik; }
            set { idsZakaznik = value; }
        }

        public List<int> IdsVyrobce
        {
            get { return idsVyrobce; }
            set { idsVyrobce = value; }
        }

        public List<int> IdsObjednavka
        {
            get { return idsObjednavka; }
            set { idsObjednavka = value; }
        }

        public List<int> IdsVyrobek
        {
            get { return idsVyrobek; }
            set { idsVyrobek = value; }
        }

        /// <summary>
        /// method for loading data from csv file into tables
        /// </summary>
        /// <param name="path"></param>
        public void Nactenidat(string path)
        {
            if (!nacteno)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string zadani = "";
                        string tmp = "";
                        List<string> data = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            tmp = line.Substring(0, line.IndexOf(",")).ToLower();
                            if (!tmp.Equals(""))
                            {
                                zadani = tmp;
                            }
                            else
                            {
                                string query = "INSERT INTO ";
                                switch (zadani)
                                {
                                    case "zakaznik":
                                        data = line.Split(',').ToList();
                                        query += "zakaznik(jmeno, email) values(";
                                        query = fillQuery(data, query);
                                        break;
                                    case "objednavka":
                                        data = line.Split(',').ToList();
                                        query += "objednavka(zak_id, cislo_obj, datum, cena_obj) values(";
                                        query = fillQuery(data, query);
                                        break;
                                    case "vyrobce":
                                        data = line.Split(',').ToList();
                                        query += "vyrobce(nazev, email, overeny) values(";
                                        query = fillQuery(data, query);
                                        break;
                                    case "vyrobek":
                                        data = line.Split(',').ToList();
                                        query += "vyrobek(vyrobce_id, typ, nazev, cena_ks) values(";
                                        query = fillQuery(data, query);
                                        break;
                                    case "polozka":
                                        data = line.Split(',').ToList();
                                        query += "polozka(obj_id, vyrobek_id, pocet_ks, cena_polozky) values(";
                                        query = fillQuery(data, query);
                                        break;
                                    default:
                                        query = "";
                                        break;
                                }
                                data.Clear();
                                if (query.Contains(","))
                                {
                                    query = query.Substring(0, query.Length - 2);
                                    query += ");";
                                }
                                PerformCommand(query);
                                nacteno = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error pri nacitani dat z csv: " + e.Message);
                } 
            }
            else
            {
                MessageBox.Show("Nacitat muzes pouze jednou!");
            }
            
        }

        /// <summary>
        /// filling sql command
        /// </summary>
        /// <param name="data"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public string fillQuery(List<string> data, string query)
        {
            foreach (string item in data)
            {
                if (!item.Equals(""))
                {
                    if (IsDigitsOnly(item))
                    {
                        query += "" + item + ", ";
                    }
                    else
                    {
                        query += "'" + item + "', ";
                    }
                }
            }
            return query;
        }

        /// <summary>
        /// method for getting true/false, if string contains only numeral
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// method for getting true/false, if string contains only numeral or point (decimal number)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsDecimal(string str)
        {
            foreach (char c in str)
            {
                if ((c < '0' || c > '9') && c != '.')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// executing SQL command (query) after successful connect to a database
        /// </summary>
        /// <param name="query"></param>
        public void PerformCommand(string query)
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            try
            {
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Spatne zadane hodnoty: " +e.Message);
            }
        }

        /// <summary>
        /// getting ConnectinString from configuration file App.config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "Not Found";
            return result;
        }

        /// <summary>
        /// method for loading id's of table zakaznik for dropdown menu
        /// </summary>
        public void nacteniIdsZakaznik()
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            idsZakaznik.Clear();
            try
            {
                string query = "select id from zakaznik;";
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            idsZakaznik.Add((int)reader.GetValue(0));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        /// <summary>
        /// method for loading id's of table vyrobce for dropdown menu
        /// </summary>
        public void nacteniIdsVyrobce()
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            idsVyrobce.Clear();
            try
            {
                string query = "select id from vyrobce;";
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            idsVyrobce.Add((int)reader.GetValue(0));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        /// <summary>
        /// method for loading id's of table objednavka for dropdown menu
        /// </summary>
        public void nacteniIdsObjednavka()
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            idsObjednavka.Clear();
            try
            {
                string query = "select id from objednavka;";
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            idsObjednavka.Add((int)reader.GetValue(0));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        /// <summary>
        /// method for loading id's of table vyrobek for dropdown menu
        /// </summary>
        public void nacteniIdsVyrobek()
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            idsVyrobek.Clear();
            try
            {
                string query = "select id from vyrobek;";
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            idsVyrobek.Add((int)reader.GetValue(0));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        /// <summary>
        /// method for getting specific id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetId(string query)
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            int end = 0;
            try
            {
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            end = (int)reader.GetValue(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return end;
        }

        /// <summary>
        /// method for write data from specific table from database 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string[] Read(string query, string zadani)
        {
            SqlConnectionStringBuilder consStringBuilder = new SqlConnectionStringBuilder();
            consStringBuilder.UserID = ReadSetting("Name");
            consStringBuilder.Password = ReadSetting("Password");
            consStringBuilder.InitialCatalog = ReadSetting("Database");
            consStringBuilder.DataSource = ReadSetting("DataSource");
            consStringBuilder.ConnectTimeout = 30;

            List<string> lines = new List<string>();
            string tmp = "";

            switch (zadani.ToLower())
            {
                case "zakaznik":
                    tmp = "Zakaznici: ";
                    lines.Add(tmp);
                    tmp = "id, jmeno, e-mail ";
                    lines.Add(tmp);
                    tmp = "";
                    break;
                case "objednavka":
                    tmp = "Objednavky: ";
                    lines.Add(tmp);
                    tmp = "id, zak_id, cislo_obj, datum, cena ";
                    lines.Add(tmp);
                    tmp = "";
                    break;
                case "vyrobce":
                    tmp = "Vyrobci: ";
                    lines.Add(tmp);
                    tmp = "id, nazev, e-mail, overeny ";
                    lines.Add(tmp);
                    tmp = "";
                    break;
                case "vyrobek":
                    tmp = "Vyrobky: ";
                    lines.Add(tmp);
                    tmp = "id, vyrobce_id, typ, nazev, cena_ks ";
                    lines.Add(tmp);
                    tmp = "";
                    break;
                case "polozka":
                    tmp = "Polozky: ";
                    lines.Add(tmp);
                    tmp = "id, obj_id, vyrobek_id, pocet_ks, cena_polozky ";
                    lines.Add(tmp);
                    tmp = "";
                    break;
            }

            try
            {
                using (connection = new SqlConnection(consStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                tmp += reader.GetValue(i).ToString()+ ", ";
                            }
                            lines.Add(tmp);
                            tmp = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error pri vypisu: " + e.Message);
            }
            return lines.ToArray();
        }

    }
}
