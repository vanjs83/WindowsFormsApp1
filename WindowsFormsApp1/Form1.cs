﻿using System;
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
    public partial class Form1 : Form
    {
        public int idUser;

        public Form1(int _idUser)
        {
            InitializeComponent();
            this.idUser = _idUser;
            RunTimer();
        }

        private void RunTimer()
        {
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(ShowTime);
            timer1.Start();
        }

        private void ShowTime(object sender, EventArgs e) {
            labelaTime.Text = DateTime.Now.ToString("dd/MM/yyyy  HH:mm:ss");
        }

        private string GetConnString() {//connection string on database VSITESTUDNET
            return "Data Source=VSITESTUDENT;Initial Catalog=Payment;Integrated Security=True";
        }

        //SELECT QUERY
        private void button1_Click(object sender, EventArgs e)
        {
            
            SqlConnection conn = new SqlConnection(GetConnString());
            SqlCommand comm = new SqlCommand();
            conn.Open();
            comm.CommandType = CommandType.StoredProcedure;
            
            if (this.checkBox1.Checked)
            {
                button4.Enabled = false;
                comm.CommandText = "totalSum";
            }
            else
            {
                button4.Enabled = true;
                comm.CommandText = "ShowPayments";
                button4.Enabled = true;
            }
            // SEND PARAMETARS INTO PROCEDURE
            comm.Parameters.AddWithValue("@from", this.dateTimeFrom.Value);
            comm.Parameters.AddWithValue("@to",this.dateTimeTo.Value);
            comm.Parameters.AddWithValue("@name", this.comboBox2.Text.Trim());
            comm.Parameters.AddWithValue("@CountName", this.comboBox3.Text.Trim());
            comm.Parameters.AddWithValue("@idUser", this.idUser);
      
            comm.Connection = conn;
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = comm;
                    DataTable data = new DataTable();
                    //Add sum
                    adapter.Fill(data);

                   BindingSource bsource = new BindingSource();
                   bsource.DataSource = data;
                   dataGridView1.DataSource = bsource;
                   adapter.Update(data);

                if (checkBox1.Checked)
                {
                    double sum = 0;
                    for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                    {
                        sum += Convert.ToDouble(dataGridView1.Rows[i].Cells["price"].Value.ToString());
                    }
                    data.Rows.Add();
                    this.dataGridView1.Rows[dataGridView1.Rows.Count-1].Cells[2].Value = String.Format("{0:0.00}", sum);

                }
               
                //SHOW NAME 
                SqlCommand com = new SqlCommand();
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "ShowName";
                com.Parameters.AddWithValue("@idUser", this.idUser);
                adapter.SelectCommand = com;
                com.Connection = conn;
                DataTable table = new DataTable();
                adapter.Fill(table);

                comboBox1.DisplayMember = "name";
                comboBox1.DataSource = table;
            
                comboBox2.DisplayMember = "name";
                comboBox2.DataSource = table;

                //SHOW COUNTS
                SqlCommand command = new SqlCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ShowNameCounts";
                command.Parameters.AddWithValue("@idUser", this.idUser);
                adapter.SelectCommand = command;
                command.Connection = conn;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                comboBox3.DisplayMember = "Counts";
                comboBox3.DataSource = dataTable;
                
            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Faild",MessageBoxButtons.OKCancel,MessageBoxIcon.Error);

                }
                finally
                {
                    conn.Close();
                }

         }
       
        //INSERT DATABASE
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (this.textBoxName.Text == ""  || this.textBoxSuma.Text == "")
            {
                MessageBox.Show("Please, Entered all data!", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // double suma = Convert.ToDouble(this.textBoxSuma.Text);
            //  string query = "INSERT INTO Payment.dbo.Payments(name, suma, datum, description) VALUES('" + this.textBoxName.Text + "', '" +this.textBoxSuma.Text+ "' , '" + this.dateTimeInsert.Value.Date.ToString("yyyy-MM-dd HH:mm") + "', '" +this.textBoxDescription.Text +"') ;";
            
                SqlConnection conn = new SqlConnection(GetConnString());
            try
            { 
                conn.Open();
                SqlCommand cmd= new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertPayments";
                cmd.Parameters.AddWithValue("@idUser", this.idUser);
                cmd.Parameters.AddWithValue("@NameCount", this.comboBox3.Text.Trim());
                cmd.Parameters.AddWithValue("@name", this.textBoxName.Text.Trim());
                cmd.Parameters.AddWithValue("@suma", this.textBoxSuma.Text.Trim());
                cmd.Parameters.AddWithValue("@datum", this.dateTimeInsert.Value.Date.ToString("yyyy-MM-dd HH:mm"));
                cmd.Parameters.AddWithValue("@description", this.textBoxDescription.Text.Trim());
                cmd.ExecuteNonQuery();

             

                // while (dataRider.Read()) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
                MessageBox.Show(this.textBoxName.Text,"Saved",MessageBoxButtons.OK,MessageBoxIcon.Information);

                this.textBoxName.Clear();
                this.textBoxSuma.Clear();
                this.textBoxDescription.Clear();
            }
        }

        //EXCEL REPORT
        private void button3_Click(object sender, EventArgs e)
        {
            //printDialog1.Document = printDocument1;
            //if(printDialog1.ShowDialog() == DialogResult.OK)
            //printDocument1.Print();
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program  
            app.Visible = true;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            worksheet.Name = "Payments";
            // storing header part in Excel  
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }
            // save the application  
            workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            app.Quit();


        }

        //DELETE PROCEDURE
        private void button4_Click(object sender, EventArgs e) 
        {
            if (checkBox1.Checked)
            {
                MessageBox.Show("Yon can't delete item now", "Faild",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            SqlConnection conn = new SqlConnection(GetConnString());
            try
            {

            SqlCommand cmd = new SqlCommand();
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeletePayments";
                // SEND PARAMETAR INTO PROCEDURE 
                cmd.Parameters.AddWithValue("@name", this.comboBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@NameCounts", comboBox3.Text.Trim());
                cmd.Parameters.AddWithValue("@id", this.dataGridView1.CurrentRow.Cells[1].Value);
                cmd.Connection = conn;
                int numberEffected= cmd.ExecuteNonQuery();

                if (numberEffected == 0)
                {
                    MessageBox.Show("Try again!", "Faild!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OKCancel,MessageBoxIcon.Error);
            }

            finally
            {
                conn.Close();              
            }
            MessageBox.Show(this.comboBox1.Text, "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
   

        private void Print_Click(object sender, EventArgs e)
        {
            //Open the print dialog
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;
            printDialog.UseEXDialog = true;
            //Get the document
            if (DialogResult.OK == printDialog.ShowDialog())
            {
                printDocument1.DocumentName = "Test Page Print";
                printDocument1.Print();
            }
           
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap btm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            dataGridView1.DrawToBitmap(btm,new Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(btm, 10, 10);
        }    
    }
}