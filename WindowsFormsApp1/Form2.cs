using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using NLog;
using DataAccessLayer;
using BusinessLogicLayer;


namespace WindowsFormsApp1
{
    
    
        public partial class Form2 : Form
        {
            public Form2()
            {
                InitializeComponent();

            }
            private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        //Start
        private void button1_Click(object sender, EventArgs e)
        {
                      
                   //Validacija na textbox
            if (string.IsNullOrEmpty( this.textBox1.Text) || string.IsNullOrEmpty(this.textBox2.Text) || comboRule.SelectedIndex == -1)
            {
                MessageBox.Show("Please, Insert username and password or role","Faild",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
              
                return;
            }

            string userName = this.textBox1.Text.Trim();
            string password = this.textBox1.Text.Trim();
            User user = new User(userName, password);

            // preko svojstva zadaj prava
            if(comboRule.SelectedItem.ToString() == "Admin")
            {
                user.prava = Prava.Admin;
            }
            else if(comboRule.SelectedItem.ToString() == "User")
            {
                user.prava = Prava.User;
            }

            string NameProcedure = "";

            string Hashpassword = BusinessLayer.Hash(user.Password);
            if (!checkBox1.Checked)
            {
                // Provjeravam korisnika
                NameProcedure = "ShowLogin";
                user.Id = DataLayer.InsertShowUser(NameProcedure, Hashpassword, user);
                Form1 form1 = new Form1(user);
                this.Hide();
                form1.Show();
            }
            else
            {         //ubacujem novog korisnika
                NameProcedure = "InsertLogin";
                DataLayer.InsertShowUser(NameProcedure, Hashpassword, user);
                // otvori formu za unos osobnih podataka
                PersonForm personForm = new PersonForm();
                this.Hide();
                personForm.Show();
            }
            
            }//Završava onclick
          
        }


    }

