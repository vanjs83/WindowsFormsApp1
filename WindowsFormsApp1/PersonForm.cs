using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class PersonForm : Form
    {
        public PersonForm()
        {
            InitializeComponent();
        }




        private void button1_Click(object sender, EventArgs e)
        {

            //Validacjia
            if (string.IsNullOrEmpty(this.textBoxName.Text) || string.IsNullOrEmpty(this.textBoxSurname.Text) ||
                string.IsNullOrEmpty(this.textBoxAddress.Text) || string.IsNullOrEmpty(this.textBoxEmail.Text) ||
                string.IsNullOrEmpty(this.textBoxOIB.Text) || string.IsNullOrEmpty(this.textBoxMob.Text) )
            {
                MessageBox.Show("Please, put all information", "Faild", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;

            }
            else if (!Regex.IsMatch(this.textBoxMob.Text, @"^\d") || !Regex.IsMatch(this.textBoxOIB.Text, @"^\d{11}$") || this.textBoxOIB.TextLength != 11)
            {
                MessageBox.Show("Please, put only digital number  for OIB (11) and Tel", "Faild", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }

            string name = this.textBoxName.Text.Trim();
            string surname = this.textBoxSurname.Text.Trim();
            string address = this.textBoxAddress.Text.Trim();
            string email= this.textBoxEmail.Text.Trim();
            string oib = this.textBoxOIB.Text.Trim();
            string mob = this.textBoxMob.Text.Trim();

            Person person = new Person(name, surname, oib, address, email, mob);

            //get konekciju na bazu
            Form2 f2 = new Form2();
            string connString= f2.GetConnString();

            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertPerson";
                cmd.Parameters.AddWithValue("@name", person.Name);
                cmd.Parameters.AddWithValue("@surname", person.Surname);
                cmd.Parameters.AddWithValue("@oib", person.Oib);
                cmd.Parameters.AddWithValue("@address", person.Address);
                cmd.Parameters.AddWithValue("@email", person.Email);
                cmd.Parameters.AddWithValue("@mob", person.Mob);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
                MessageBox.Show(this.textBoxName.Text, "Saved your Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

               
                conn.Close();
                this.textBoxName.Clear();
                this.textBoxSurname.Clear();
                this.textBoxAddress.Clear();
                this.textBoxEmail.Clear();
                this.textBoxOIB.Clear();
                this.textBoxMob.Clear();
                this.Hide();
                f2.Show();

            }
        }

        

        }
    }
