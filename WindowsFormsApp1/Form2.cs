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

        private string Hash(string password) //HASH Password
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "" || this.textBox2.Text == "")
            {
                MessageBox.Show("Please, Insert username and password","Faild",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
                return;
            }
               //string connString = "Data Source=VSITESTUDENT;Initial Catalog=Payment;Integrated Security=True";
            string connString = "workstation id=payments.mssql.somee.com;packet size=4096;user id=tvanjurek_SQLLogin_1;pwd=6ejthpgljo;data source=payments.mssql.somee.com;persist security info=False;initial catalog=payments";

            SqlConnection con = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
   
            con.Open();
            cmd.CommandType = CommandType.StoredProcedure;

            if (!checkBox1.Checked)
            {
                cmd.CommandText = "ShowLogin";       
            }


            else
            {
                cmd.CommandText = "InsertLogin";
                
            }

            cmd.Parameters.AddWithValue("@username", this.textBox1.Text.Trim());
            cmd.Parameters.AddWithValue("@password", Hash(this.textBox2.Text.Trim()));
            if(!checkBox1.Checked)
                cmd.Parameters.AddWithValue("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;


            cmd.Connection = con;
            if (!checkBox1.Checked)
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataTable data = new DataTable();
                    adapter.Fill(data);
                    if (data.Rows.Count == 1)
                    {
                        int id = int.Parse(cmd.Parameters["@ID"].Value.ToString());// send id user
                        Form1 form1 = new Form1(id);
                        this.Hide();
                        form1.Show();
                    }
                    else
                    {
                        MessageBox.Show("Check your username and password", "Faild", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
                
            }
            else
            {
                cmd.ExecuteNonQuery();

            }
                
            con.Close();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                textBox2.Focus();
        }
    }
}
