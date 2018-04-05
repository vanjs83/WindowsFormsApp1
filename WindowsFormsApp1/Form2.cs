using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using NLog;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
        }

        public string GetConnString()
        {       //connection string on database VSITESTUDNET
           // return "Data Source=VSITESTUDENT;Initial Catalog=Payment;Integrated Security=True";
            return "workstation id=Payments.mssql.somee.com;packet size=4096;user id=tvanjurek_SQLLogin_1;pwd=6ejthpgljo;data source=Payments.mssql.somee.com;persist security info=False;initial catalog=Payments";
        }
            public static void LogMessageToFile(string msg)
        {

            System.IO.StreamWriter sw = System.IO.File.AppendText(@"MyLogFile.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string Hash(string password) //HASH Password
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            
                   //Validacija na textbox
            if (string.IsNullOrEmpty( this.textBox1.Text) || string.IsNullOrEmpty(this.textBox2.Text) || comboRule.SelectedIndex == -1)
            {
                MessageBox.Show("Please, Insert username and password or role","Faild",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
                Form2.LogMessageToFile("Validation not success username or password Failed");
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

            string connString = GetConnString();
            SqlConnection con = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
   
            con.Open();
            cmd.CommandType = CommandType.StoredProcedure;

            if (!checkBox1.Checked)
            {
                // Provjeravam korisnika
                cmd.CommandText = "ShowLogin";
                Form2.LogMessageToFile("Execute procedure ShowLogin line {92}");
            }
            else
            {         //ubacujem novog korisnika
                cmd.CommandText = "InsertLogin";
                Form2.LogMessageToFile("Execute procedure: InsertLogin line {97}");
            }
            cmd.Parameters.AddWithValue("@username", user.Username);
              cmd.Parameters.AddWithValue("@password", Hash(user.Password));
               cmd.Parameters.AddWithValue("@role", user.prava);
              
            if(!checkBox1.Checked)    //Dohvaćam id korisnika
                cmd.Parameters.AddWithValue("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                  cmd.Connection = con;
            if (!checkBox1.Checked)
            {
                try
                {
                    LogMessageToFile("Login User: " + user.Username);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataTable data = new DataTable();
                    adapter.Fill(data);


                    if (data.Rows.Count == 1)
                    {
                        LogMessageToFile("Login User: " + user.Username + " success Login {Line: 114}");
                        int id = int.Parse(cmd.Parameters["@ID"].Value.ToString());// send id user
                        user.Id = id;
                        Form1 form1 = new Form1(user);
                        this.Hide();
                        form1.Show();
                    }
                    else
                    {
                        MessageBox.Show("Check your username and password or rule", "Faild", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        LogMessageToFile("Login User: " + user.Username + " not success Login {Line: 114}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    LogMessageToFile("Login User " + user.Username + " not success, Error: " + ex.Message);//Logging
                    Logger log = NLog.LogManager.GetCurrentClassLogger();
                    log.Error("Login User", ex);
                }
                finally
                {
                    con.Close();

                }

            }
            // otvori formu za unos osobnih podataka
            else if (checkBox1.Checked) {
                int numQuery = cmd.ExecuteNonQuery();
                 if(numQuery != -1)
                    Form2.LogMessageToFile("Execute procedure InserrtLogin Status OK");
                con.Close();
                PersonForm personForm = new PersonForm();
                this.Hide();
                personForm.Show();

            }
          
        }


        //skok u novi red
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                textBox2.Focus();
        }
    }
}
