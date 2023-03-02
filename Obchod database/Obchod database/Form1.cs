using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Obchod_database
{
    public partial class Form1 : Form
    {
        private static Database db;
        private bool upravaActive;
        private static string path;
        private BindingSource bzSource;
        private BindingSource bveSource;
        private BindingSource boSource;
        private BindingSource bvkSource;

        /// <summary>
        /// constructor - creating form
        /// </summary>
        public Form1()
        {
            db = new Database();
            InitializeComponent();
            upravaActive = false;
            path = "nacteniDat.csv";
            bzSource = new BindingSource();
            bzSource.DataSource = db.IdsZakaznik;
            comboBox1.DataSource = bzSource;

            bveSource = new BindingSource();
            bveSource.DataSource = db.IdsVyrobce;
            comboBox2.DataSource = bveSource;

            boSource = new BindingSource();
            boSource.DataSource = db.IdsObjednavka;
            comboBox3.DataSource = boSource;

            bvkSource = new BindingSource();
            bvkSource.DataSource = db.IdsVyrobek;
            comboBox4.DataSource = bvkSource;
        }

        /// <summary>
        /// ADD - adding object to a database, selecting which object is for adding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
                    {
                        db.PerformCommand(this.AddZakaznik());
                        db.IdsZakaznik.Add(db.GetId("select id from zakaznik where email = '" + textBox2.Text + "';"));
                        bzSource.DataSource = new List<int>();
                        bzSource.DataSource = db.IdsZakaznik;
                        comboBox1.DataSource = bzSource;
                        textBox2.Text = "";
                    } else { 
                        MessageBox.Show("Nektere z poli je prazdne, nebo spatne zadane!");
                    }
                    break;
                case 1:
                    if ((int)numericUpDown3.Value > 0 && (int)numericUpDown2.Value > 0) {
                        db.PerformCommand(this.AddObjednavka());
                        db.IdsObjednavka.Add(db.GetId("select id from objednavka where cislo_obj = " + numericUpDown2.Value + ";"));
                        boSource.DataSource = new List<int>();
                        boSource.DataSource = db.IdsObjednavka;
                        comboBox3.DataSource = boSource;
                        numericUpDown2.Value = 0;
                    } 
                    else 
                    {
                        MessageBox.Show("Nektere z poli je prazdne, nebo spatne zadane!");
                    }
                    break;
                case 2:
                    if (textBox4.Text.Length > 0 && textBox3.Text.Length > 0 && ((int)numericUpDown4.Value == 0 || (int)numericUpDown4.Value == 1)) {
                        db.PerformCommand(this.AddVyrobce());
                        db.IdsVyrobce.Add(db.GetId("select id from vyrobce where email = '" + textBox3.Text + "';"));
                        bveSource.DataSource = new List<int>();
                        bveSource.DataSource = db.IdsVyrobce;
                        comboBox2.DataSource = bveSource;
                        textBox3.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Nektere z poli je prazdne, nebo spatne zadane!");
                    }
                    break;
                case 3:
                    if ((textBox6.Text.Equals("obchodni") || textBox6.Text.Equals("reklamni") || textBox6.Text.Equals("provozni")) && textBox5.Text.Length > 0 && (int)numericUpDown6.Value > 0) {
                        db.PerformCommand(this.AddVyrobek());
                        db.IdsVyrobek.Add(db.GetId("select id from vyrobek where nazev = '" + textBox5.Text + "';"));
                        bvkSource.DataSource = new List<int>();
                        bvkSource.DataSource = db.IdsVyrobek;
                        comboBox4.DataSource = bvkSource;
                        textBox5.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Nektere z poli je prazdne, nebo spatne zadane!");
                    }
                    break;
                case 4:
                    if ((int)numericUpDown9.Value > 0 && db.IsDecimal(textBox9.Text)) {
                        db.PerformCommand(this.AddPolozka());
                    }
                    else
                    {
                        MessageBox.Show("Nektere z poli je prazdne, nebo spatne zadane!");
                    }
                    break;
                default:
                    MessageBox.Show("Error - spatny tab!");
                    break;
            }
        }

        /// <summary>
        /// adding methods for every single one type of object
        /// </summary>
        /// <returns></returns>
        private string AddZakaznik()
        {
            string jmeno = textBox1.Text;
            string email = textBox2.Text;

            string query = "INSERT INTO zakaznik(jmeno, email) values('" + jmeno + "', '" + email + "');";

            textBox1.Text = "";

            return query;
        }

        private string AddObjednavka()
        {
            int zakaznik_id = (int)comboBox1.SelectedValue;
            int cislo_objednavky = (int)numericUpDown2.Value;
            DateTime datum = dateTimePicker1.Value;
            int cena_objednavky = (int)numericUpDown3.Value;

            string datumF = datum.ToString("yyyy-MM-dd");
            string query = "INSERT INTO objednavka(zak_id, cislo_obj, datum, cena_obj) values(" + zakaznik_id + ", " + cislo_objednavky + ", '" + datumF + "', " + cena_objednavky + ");";

            dateTimePicker1.Value = DateTime.Now;
            numericUpDown3.Value = 0;

            return query;
        }

        private string AddVyrobce()
        {
            string nazev = textBox4.Text;
            string email = textBox3.Text;
            int overeny = (int)numericUpDown4.Value;

            string query = "INSERT INTO vyrobce(nazev, email, overeny) values('" + nazev + "', '" + email + "', " + overeny + ");";

            textBox4.Text = "";
            numericUpDown4.Value = 0;

            return query;
        }

        private string AddVyrobek()
        {
            int vyrobce_id = (int)comboBox2.SelectedValue;
            string typ = textBox6.Text;
            string nazev = textBox5.Text;
            int cena_za_kus = (int)numericUpDown6.Value;

            string query = "INSERT INTO vyrobek(vyrobce_id, typ, nazev, cena_ks) values(" + vyrobce_id + ", '" + typ + "', '" + nazev + "', " + cena_za_kus + ");";

            textBox6.Text = "";
            numericUpDown6.Value = 0;

            return query;
        }

        private string AddPolozka()
        {
            int objednavka_id = (int)comboBox3.SelectedValue;
            int vyrobek_id = (int)comboBox4.SelectedValue;
            int pocek_kusu = (int)numericUpDown9.Value;
            string cena_polozky = textBox9.Text;

            string query = "INSERT INTO polozka(obj_id, vyrobek_id, pocet_ks, cena_polozky) values(" + objednavka_id + ", " + vyrobek_id + "," + pocek_kusu + ", " + cena_polozky + ");";

            numericUpDown9.Value = 0;
            textBox9.Text = "";

            return query;
        }

        /// <summary>
        /// DELETE - deleting object from a database, selecting which object is for deleting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox7.Text.Length > 0)
            {
                string zadani = textBox7.Text;
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        db.PerformCommand(this.RemoveZakaznik(zadani));
                        db.nacteniIdsZakaznik();
                        bzSource.DataSource = new List<int>();
                        bzSource.DataSource = db.IdsZakaznik;
                        comboBox1.DataSource = bzSource;
                        break;
                    case 1:
                        db.PerformCommand(this.RemoveObjednavka(zadani));
                        db.nacteniIdsObjednavka();
                        boSource.DataSource = new List<int>();
                        boSource.DataSource = db.IdsObjednavka;
                        comboBox3.DataSource = boSource;
                        break;
                    case 2:
                        db.PerformCommand(this.RemoveVyrobce(zadani));
                        db.nacteniIdsVyrobce();
                        bveSource.DataSource = new List<int>();
                        bveSource.DataSource = db.IdsVyrobce;
                        comboBox2.DataSource = bveSource;
                        break;
                    case 3:
                        db.PerformCommand(this.RemoveVyrobek(zadani));
                        db.nacteniIdsVyrobek();
                        bvkSource.DataSource = new List<int>();
                        bvkSource.DataSource = db.IdsVyrobek;
                        comboBox4.DataSource = bvkSource;
                        break;
                    case 4:
                        db.PerformCommand(this.RemovePolozka(zadani));
                        break;
                    default:
                        MessageBox.Show("Error - spatny tab!");
                        break;
                }
                textBox7.Text = "";
            }
            else
            {
                MessageBox.Show("pole bylo prazdne!");
            }      
        }

        /// <summary>
        /// deleting methods for every single one type of object
        /// </summary>
        /// <param name="zadani"></param>
        /// <returns></returns>
        private string RemoveZakaznik(string zadani)
        {
            return "DELETE FROM zakaznik WHERE jmeno = '" + zadani + "';";
        }

        private string RemoveObjednavka(string zadani)
        {
            return "DELETE FROM objednavka WHERE cislo_obj = " + Int32.Parse(zadani) + ";";
        }

        private string RemoveVyrobce(string zadani)
        {
            return "DELETE FROM vyrobce WHERE nazev = '" + zadani + "';";
        }

        private string RemoveVyrobek(string zadani)
        {
            return "DELETE FROM vyrobek WHERE nazev = '" + zadani + "';";
        }

        private string RemovePolozka(string zadani)
        {
            return "DELETE FROM polozka WHERE obj_id = " + Int32.Parse(zadani) + ";";
        }

        /// <summary>
        /// EDIT - changing object properties which exists in a database, selecting which object is for editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if(upravaActive)
            {
                string zadani = textBox7.Text;
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        db.PerformCommand(this.EditZakaznik(zadani));
                        break;
                    case 1:
                        db.PerformCommand(this.EditObjednavka(zadani));
                        break;
                    case 2:
                        db.PerformCommand(this.EditVyrobce(zadani));
                        break;
                    case 3:
                        db.PerformCommand(this.EditVyrobek(zadani));
                        break;
                    case 4:
                        db.PerformCommand(this.EditPolozka(zadani));
                        break;
                    default:
                        MessageBox.Show("Error!");
                        break;
                }
                upravaActive = false;
                textBox7.Text = "";
            }
            if (textBox7.Text.Length > 0)
            {
                upravaActive = true;
            }
        }

        /// <summary>
        /// editing methods for every single one type of object
        /// </summary>
        /// <param name="zadani"></param>
        /// <returns></returns>
        private string EditZakaznik(string zadani)
        {
            string query = "UPDATE zakaznik set ";
            if(textBox1.Text.Length > 0)
            {
                string jmeno = textBox1.Text;
                query += "jmeno = '" + jmeno + "', ";
                zadani = jmeno;
                textBox1.Text = "";
            }
            if(textBox2.Text.Length > 0)
            {
                string email = textBox2.Text;
                query += "email = '" + email + "', ";
                textBox2.Text = "";
            }

            if (!query.Contains(","))
            {
                return "";
            } else {
                query = query.Substring(0, query.Length - 2);
                query += " where jmeno = '" + zadani + "';";
                return query;
            }
        }

        private string EditObjednavka(string zadani)
        {
            string query = "UPDATE objednavka set ";
            int zadaniN = Int32.Parse(zadani);

            int zakaznik_id = (int)comboBox1.SelectedValue;
            query += "zak_id = " + zakaznik_id + ", ";

            if ((int)numericUpDown2.Value != 0)
            {
                int cislo_objednavky = (int)numericUpDown2.Value;
                query += "cislo_obj = " + cislo_objednavky + ", ";
                zadaniN = cislo_objednavky;
                numericUpDown2.Value = 0;
            }
            if(dateTimePicker1.Value != DateTime.Now)
            {
                DateTime datum = dateTimePicker1.Value;
                string datumF = datum.ToString("yyyy-MM-dd");
                query += "datum = '" + datumF + "', ";
                dateTimePicker1.Value = DateTime.Now;
            }
            if ((int)numericUpDown3.Value != 0)
            {
                int cena_objednavky = (int)numericUpDown3.Value;
                query += "cena_obj = " + cena_objednavky + ", ";
                numericUpDown3.Value = 0;
            }

            if (!query.Contains(","))
            {
                return "";
            } else {
                query = query.Substring(0, query.Length - 2);
                query += " where cislo_obj = " + zadaniN + ";";
                return query;
            }
        }

        private string EditVyrobce(string zadani)
        {
            string query = "UPDATE vyrobce set ";
            if (textBox4.Text.Length > 0)
            {
                string nazev = textBox4.Text;
                query += "nazev = '" + nazev + "', ";
                zadani = nazev;
                textBox4.Text = "";
            }
            if (textBox3.Text.Length > 0)
            {
                string email = textBox3.Text;
                query += "email = '" + email + "', ";
                textBox3.Text = "";
            }
            if(numericUpDown4.Value == 0 || numericUpDown4.Value == 1)
            {
                int overeny = (int)numericUpDown4.Value;
                query += "overeny = '" + overeny + "', ";
                numericUpDown4.Value = 0;
            }

            if (!query.Contains(","))
            {
                return "";
            } else {
                query = query.Substring(0, query.Length - 2);
                query += " where nazev = '" + zadani + "';";
                return query;
            }
        }

        private string EditVyrobek(string zadani)
        {
            string query = "UPDATE vyrobek set ";

            int vyrobce_id = (int)comboBox2.SelectedValue;
            query += "vyrobce_id = " + vyrobce_id + ", ";

            if (textBox6.Text.Equals("obchodni") || textBox6.Text.Equals("reklamni") || textBox6.Text.Equals("provozni"))
            {
                string typ = textBox6.Text;
                query += "typ = '" + typ + "', ";
                textBox6.Text = "";
            }
            if (textBox5.Text.Length > 0)
            {
                string nazev = textBox5.Text;
                query += "nazev = '" + nazev + "', ";
                zadani = nazev;
                textBox5.Text = "";
            }
            if ((int)numericUpDown6.Value != 0)
            {
                int cena_za_kus = (int)numericUpDown6.Value;
                query += "cena_ks = " + cena_za_kus + ", ";
                numericUpDown6.Value = 0;
            }

            if (!query.Contains(","))
            {
                return "";
            } else {
                query = query.Substring(0, query.Length - 2);
                query += " where nazev = '" + zadani + "';";
                return query;
            }
        }

        private string EditPolozka(string zadani)
        {
            string query = "UPDATE polozka set ";
            int zadaniN = Int32.Parse(zadani);

            int objednavka_id = (int)comboBox3.SelectedValue;
            query += "obj_id = " + objednavka_id + ", ";
            zadaniN = objednavka_id;


            int vyrobek_id = (int)comboBox4.SelectedValue;
            query += "vyrobek_id = " + vyrobek_id + ", ";

            if ((int)numericUpDown9.Value != 0)
            {
                int pocek_kusu = (int)numericUpDown9.Value;
                query += "pocet_ks = " + pocek_kusu + ", ";
                numericUpDown9.Value = 0;
            }
            if (textBox9.Text.Length > 0 && db.IsDecimal(textBox9.Text))
            {
                string cena_polozky = textBox9.Text;
                query += "cena_polozky = " + cena_polozky + ", ";
                textBox9.Text = "";
            }

            if (!query.Contains(","))
            {
                return "";
            } else {
                query = query.Substring(0, query.Length - 2);
                query += " where obj_id = " + zadaniN + ";";
                return query;
            }
        }

        /// <summary>
        /// LOAD DATA - load data from csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            db.Nactenidat(path);
        }

        /// <summary>
        /// WRITE - write data from database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            string zadani = textBox7.Text;
            if (zadani.Length > 0)
            {
                textBox8.Lines = db.Read("select * from " + zadani + ";", zadani);
            }
            else
            {
                MessageBox.Show("Policko je prazdne!");
            }
            textBox7.Text = "";
        }
    }
}
