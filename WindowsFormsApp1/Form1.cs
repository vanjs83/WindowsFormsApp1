using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;
using Newtonsoft.Json.Linq;
using MongoDB;
using MongoDB.Bson;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
         Person _person;
      
        public Form1(User user)
        {
            InitializeComponent();
            this._person  = new Person (user);
            RunTimer();
            GetPerson();
        }

        private void get_response(string val)
        {
            
            WebClient wp = new WebClient();
            string url = "http://api.hnb.hr/tecajn?valuta=" + val;
            Form2.LogMessageToFile("Get data from WebService url: " + url + " line 35");
            var response = wp.DownloadString(url);
             get_data(response);
           
        }


        private void get_data(string response)
        {
            bool resFromRequest = string.IsNullOrEmpty(response);
            if (resFromRequest)
            {
                
                Form2.LogMessageToFile("Response has Value " + resFromRequest);
                throw new ArgumentException("response is empty", nameof(response));
               
            }
            Form2.LogMessageToFile("response has Value: " + resFromRequest + " Get Json from Requets");
            JArray a = JArray.Parse(response);

            foreach (JObject o in a.Children<JObject>())
            {
                foreach (JProperty p in o.Properties())
                {
                    string name = p.Name;
                    string value = (string)p.Value;
                    ListViewItem item = listView1.Items.Add(name);
                    item.SubItems.Add(value);
                }
            }

            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

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

        private string GetConnString()
        {    //connection string on database VSITESTUDNET
            return "Data Source=VSITESTUDENT;Initial Catalog=Payment;Integrated Security=True";
            // workstation id=Payments.mssql.somee.com;packet size=4096;user id=tvanjurek_SQLLogin_1;pwd=6ejthpgljo;data source=Payments.mssql.somee.com;persist security info=False;initial catalog=Payments
        }


        private void GetPerson() {
            SqlConnection conn = new SqlConnection(GetConnString());
            
            
           
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                conn.Open();
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GetPerson";
                command.Parameters.AddWithValue("@idUser", this._person.Id);
                Form2.LogMessageToFile("Execute procedure {GetPerson}");
                   using (SqlDataReader oReader = command.ExecuteReader())
                   {
                    while (oReader.Read())
                      {
                        _person.Name = oReader["Name"].ToString();
                        _person.Surname = oReader["Surname"].ToString();
                        _person.Oib = oReader["Oib"].ToString();
                        _person.Address = oReader["Address"].ToString();
                        _person.Email = oReader["Email"].ToString();
                        _person.Mob = oReader["Email"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                Form2.LogMessageToFile("Error Execute procedure: GetPerson" + ex.Message);
                Logger log = NLog.LogManager.GetCurrentClassLogger();
                log.Error("Execute procedure GetPerson", ex);
            }
            finally
            {
                conn.Close();
            }

        }


        private void ShowCounts(SqlConnection conn, SqlDataAdapter adapter) {

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ShowNameCounts";
            Form2.LogMessageToFile("Execute procedure {ShowNameCounts}");
            command.Parameters.AddWithValue("@idUser", this._person.Id);
            adapter.SelectCommand = command;
            command.Connection = conn;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            comboBoxCount.DisplayMember = "Counts";
            comboBoxCount.DataSource = dataTable;
            
        }
        private void ShowCategory(SqlConnection conn, SqlDataAdapter adapter)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ShowNameCategory";
            Form2.LogMessageToFile("Execute procedure {ShowNameCategory} line{107}");
            command.Parameters.AddWithValue("@idUser", this._person.Id);
            adapter.SelectCommand = command;
            command.Connection = conn;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);


            //List<string> list = new List<string>();
            //list = dataTable.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Category")).ToList();

            comboBoxCategory.DisplayMember = "Category";
            comboBoxCategory.DataSource = dataTable;
        }

        private void ShowTypeOfPay(SqlConnection conn, SqlDataAdapter adapter)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "ShowNameTypeOfPay";
            Form2.LogMessageToFile("Execute procedure {ShowNameTypeOfPay} line{127}");
            command.Parameters.AddWithValue("@idUser", this._person.Id);
            adapter.SelectCommand = command;
            command.Connection = conn;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            comboBoxPay.DisplayMember = "Pay";
            comboBoxPay.DataSource = dataTable;
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
                Form2.LogMessageToFile("Execute procedure {totalSum} line{150}");

            }
            else
            {
                button4.Enabled = true;
                comm.CommandText = "ShowPayments";
                Form2.LogMessageToFile("Execute procedure {ShowPayments} line{157}");

                button4.Enabled = true;
            }
            // SEND PARAMETARS INTO PROCEDURE
            comm.Parameters.AddWithValue("@from", this.dateTimeFrom.Value);
            comm.Parameters.AddWithValue("@to",this.dateTimeTo.Value);
            comm.Parameters.AddWithValue("@nameItem", this.comboBoxItem.Text.Trim());
            comm.Parameters.AddWithValue("@CountName", this.comboBoxCount.Text.Trim());
            comm.Parameters.AddWithValue("@CategoryName", this.comboBoxCategory.Text.Trim());
            comm.Parameters.AddWithValue("@PayName", this.comboBoxPay.Text.Trim());
            comm.Parameters.AddWithValue("@idUser", this._person.Id);
      
            comm.Connection = conn;
                try
                {
                Form2.LogMessageToFile("try execute procedure line{172}");

                SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = comm;
                    DataTable data = new DataTable();
                    //Add sum
                    adapter.Fill(data);

                   BindingSource bsource = new BindingSource();
                   bsource.DataSource = data;              
                   dataGridView1.DataSource = bsource;
                // Add char Grafikon
                if (!checkBox1.Checked)
                {
                    this.chart1.Series["Name"].XValueMember = "Item";
                    this.chart1.Series["Name"].YValueMembers = "suma";             
                }
                else
                {              
                    this.chart1.Series["Name"].XValueMember = "Item";
                    this.chart1.Series["Name"].YValueMembers = "Price";
                }
                this.chart1.DataSource = bsource;
                chart1.DataBind();
                adapter.Update(data);
                // Suma consumption
                if (checkBox1.Checked)
                {
                    double sumPrice = 0;
                    int numItem = 0; //Ako postoji redak racunaj
                    if (dataGridView1.Rows.Count > 0)
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                        {   // nadodavanje izracuna
                            sumPrice += Convert.ToDouble(dataGridView1.Rows[i].Cells["Price"].Value.ToString());
                            numItem += Convert.ToInt32(dataGridView1.Rows[i].Cells["Number"].Value.ToString());
                        }
                        data.Rows.Add();
                        this.dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[4].Value = String.Format("{0:0.00}", sumPrice);
                        this.dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Value = String.Format("{0}", numItem);
                    }
                }

                //List<User> user = new List<WindowsFormsApp1.User>();
                //user = data.Rows.OfType<DataTable>().Select(dr => dr.Field<User>("name")).ToList();

                //SHOW NAME ITEM 
                SqlCommand com = new SqlCommand();
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "ShowName";
                Form2.LogMessageToFile("Execute procedure {ShowName} line{223}");

                com.Parameters.AddWithValue("@idUser", this._person.Id);
                com.Parameters.AddWithValue("@NameCounts", this.comboBoxCount.Text.Trim());
                adapter.SelectCommand = com;
                com.Connection = conn;
                DataTable table = new DataTable();
                adapter.Fill(table);
              
                comboBox1.DisplayMember = "Item";
                comboBox1.DataSource = table;
                comboBoxItem.DisplayMember = "Item";
                comboBoxItem.DataSource = table;

                //SHOW COUNTS CATEGORY PAY
                ShowCounts(conn, adapter);
                //SHOW CATEGORY 
                ShowCategory(conn, adapter);
                // SHOW TYPE OF PAY
                ShowTypeOfPay(conn, adapter);
    
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Faild",MessageBoxButtons.OKCancel,MessageBoxIcon.Error);
                     Form2.LogMessageToFile("Error Execute procedure line{249}" + ex.Message);
                       Logger log = NLog.LogManager.GetCurrentClassLogger();
                       log.Error("Execute procedure", ex);
            }
            finally
                {
                    conn.Close();
                }

         }
                //poziv funkcije spremi kategoriju
        private int SaveCategory(SqlConnection conn) {

            // spremi kategoriju ako ne postoji
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertCategory";
            cmd.Parameters.AddWithValue("@iduser", this._person.Id);
            cmd.Parameters.AddWithValue("@name", this.comboBoxCategory.Text.Trim());
            return cmd.ExecuteNonQuery();

        }
           // SPREMI NACIN PLACANJA
        private int SaveTypeOfPay(SqlConnection conn) {
            //Spremi način plačanja ako ne postoji
            SqlCommand cm = new SqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "InsertTypeOfPay";
            cm.Parameters.AddWithValue("@namePay", this.comboBoxPay.Text.Trim());
            return cm.ExecuteNonQuery();
        }

        //Spremi Counts ako ne postoji
        private int SaveCounts(SqlConnection conn) {

            SqlCommand cmdCount = new SqlCommand();
            cmdCount.Connection = conn;
            cmdCount.CommandType = CommandType.StoredProcedure;
            cmdCount.CommandText = "InsertCounts";
            cmdCount.Parameters.AddWithValue("@CategoryName", this.comboBoxCategory.Text.Trim());
            cmdCount.Parameters.AddWithValue("@NameCount", this.comboBoxCount.Text.Trim());
            return cmdCount.ExecuteNonQuery();

        }


        //INSERT DATABASE
        private void button2_Click(object sender, EventArgs e)
        {
            Form2.LogMessageToFile("Validation line 291 ");
            double suma;
            if (string.IsNullOrEmpty(this.textBoxName.Text) || string.IsNullOrEmpty(this.textBoxSuma.Text))
            {
                MessageBox.Show("Please, Entered all data!", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Form2.LogMessageToFile("Not entered all data {line 291} ");
                return;
            }
            else if (double.TryParse(this.textBoxSuma.Text, out suma) == false)
            {
                MessageBox.Show("Please, numeric data for Price!", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Form2.LogMessageToFile("Not entered numeric data  ");
                textBoxSuma.Clear();
                return;
            }

         

            SqlConnection conn = new SqlConnection(GetConnString());
            conn.Open();
            try
            {
                Form2.LogMessageToFile("Insert Category {line 310} ");
                if (!string.IsNullOrEmpty(this.comboBoxCategory.Text))
                {
                    //poziv funkcije spremi kategoriju
                      if (_person.prava == Prava.User)
                        throw new System.Exception(_person.Name + " You can't insert item, you must be Admin");

                    int numExecute = SaveCategory(conn);
                    if (numExecute == -1)
                    {
                        MessageBox.Show("Category " + comboBoxCategory.Text, "OK", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Form2.LogMessageToFile("Category exists " + comboBoxCategory.Text + "statu: " + numExecute); ;
                    }
                    else if (numExecute != -1)
                    {
                        MessageBox.Show("This Category " + comboBoxCategory.Text + " is now save !", "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Form2.LogMessageToFile("New Category save " + comboBoxCategory.Text + "status: " + numExecute); ;

                    }
                }
                if (!string.IsNullOrEmpty(this.comboBoxPay.Text))
                {
                    Form2.LogMessageToFile("Insert TypeOfPay {line 320} ");
                    int numExecute = SaveTypeOfPay(conn);
                    //poziv funkcije spremi način plačanja
                    if (numExecute == -1)
                    {
                        MessageBox.Show("Type of pay " + this.comboBoxPay.Text, "OK", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Form2.LogMessageToFile("Type of pay exists " + comboBoxPay.Text + " " + numExecute); ;

                    }
                    else if (numExecute != -1)
                    {
                        MessageBox.Show("This type of pay " + comboBoxPay.Text + " is now save !", "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Form2.LogMessageToFile("Save Type of pay  " + comboBoxPay.Text + " " + numExecute); ;

                    }
                }

                if (!string.IsNullOrEmpty(this.comboBoxCount.Text))
                {      //Poziv funkcije naziv računa
                    Form2.LogMessageToFile("Insert Counts name {line 334} ");
                    int numExecute = SaveCounts(conn);
                    if (numExecute == -1)
                    {
                        MessageBox.Show("Counts: " + this.comboBoxCount.Text, "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Form2.LogMessageToFile("Caunts exists " + comboBoxCount.Text + " " + numExecute); ;

                    }
                    else if (numExecute != -1)
                    {
                        MessageBox.Show("Counts " + this.comboBoxCount.Text + " is now save !", "ok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Form2.LogMessageToFile("Counts exists " + comboBoxCount.Text + " " + numExecute); ;

                    }
                }
                Form2.LogMessageToFile("Insert Payments {line 367}"); ;

                SqlCommand cmm = new SqlCommand();
                cmm.Connection = conn;
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "InsertPayments";
                cmm.Parameters.AddWithValue("@NamePay", this.comboBoxPay.Text.Trim());
                cmm.Parameters.AddWithValue("@NameCount", this.comboBoxCount.Text.Trim());
                cmm.Parameters.AddWithValue("@name", this.textBoxName.Text.Trim());
                cmm.Parameters.AddWithValue("@suma", this.textBoxSuma.Text.Trim());
                cmm.Parameters.AddWithValue("@datum", this.dateTimeInsert.Value.Date.ToString("yyyy-MM-dd HH:mm"));
                cmm.Parameters.AddWithValue("@description", this.textBoxDescription.Text.Trim());
                 int numQuery = cmm.ExecuteNonQuery();
                if (numQuery != -1)
                {
                    MessageBox.Show(this.textBoxName.Text, "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form2.LogMessageToFile("Save " + textBoxName.Text + "status " + numQuery); ;
                }
                else
                {
                    Form2.LogMessageToFile("Not save " + textBoxName.Text + "status " + numQuery); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Form2.LogMessageToFile("Error"+ ex.Message);
                 Logger log = NLog.LogManager.GetCurrentClassLogger();
                 log.Error("Execute procedure InsertPayment", ex);
            }
            finally
            {
                conn.Close();
                this.textBoxName.Clear();
                this.textBoxSuma.Clear();
                this.textBoxDescription.Clear();
                
            }
        }

        //EXCEL REPORT
        private void button3_Click(object sender, EventArgs e)
        {
            Form2.LogMessageToFile("Export Excel");
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
            worksheet.Name = "payments";
            // storing header part in Excel  
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  counts -1
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }
            // save the application  
            //workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            Form2.LogMessageToFile("Export Excel Status: Success");
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
                if (_person.prava == Prava.User)
                    throw new System.Exception(_person.Name + " You can't delete, you must be Admin");
                cmd.CommandText = "DeletePayments";
                Form2.LogMessageToFile("Execute procedure {DeletePayments} line {455}");
                // SEND PARAMETAR INTO PROCEDURE     item for delete
                cmd.Parameters.AddWithValue("@NameItem", this.comboBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@NameCounts", this.dataGridView1.CurrentRow.Cells[0].Value);
                cmd.Parameters.AddWithValue("@id", this.dataGridView1.CurrentRow.Cells[1].Value);
                cmd.Parameters.AddWithValue("@NameCategory", this.dataGridView1.CurrentRow.Cells[2].Value);
                cmd.Parameters.AddWithValue("@TypeOfPay", this.dataGridView1.CurrentRow.Cells[3].Value);
                cmd.Parameters.AddWithValue("@UserId", this._person.Id);
        
                cmd.Connection = conn;
                int numberEffected = cmd.ExecuteNonQuery();

                if (numberEffected != -1)
                {
                    MessageBox.Show(this.comboBox1.Text, "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form2.LogMessageToFile("Delete procedure  " + comboBox1.Text + "Status " + numberEffected); ;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OKCancel,MessageBoxIcon.Error);
                Form2.LogMessageToFile("Error: Execute procedure {DeletePayments} " + ex.Message);
                Logger log = NLog.LogManager.GetCurrentClassLogger();
                log.Error("Execute procedure DeletePayments", ex);
            }

            finally
            {
                conn.Close();              
            }
           
        }
   
        private void Print_Click(object sender, EventArgs e)
        {
            Form2.LogMessageToFile("Print pdf line{501}");
            //Open the print dialog
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;
            printDialog.UseEXDialog = true;
            //Get the document
            if (DialogResult.OK == printDialog.ShowDialog())
            {
                printDocument1.DocumentName = "Test Page Print";
                Form2.LogMessageToFile("Print pdf Status Success line{512}");
                printDocument1.Print();
            }
        
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Bitmap btm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            dataGridView1.DrawToBitmap(btm, new System.Drawing.Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(btm, 10, 10);

        }



        private void ConvertHrk(double tecaj) {
            //     String.Format("{0:00.00}", sumPrice)
            try
            {
                  if (textBoxSuma.Text.Contains('.') )
                    throw new FormatException();
                double val = Convert.ToDouble(textBoxSuma.Text);
                string res = String.Format("{0:0.00}", val * tecaj);
                textBoxSuma.Text = res;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.ToString(), "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form2.LogMessageToFile(ex.Message);
                Logger log = NLog.LogManager.GetCurrentClassLogger();
                log.Error(ex.ToString(), ex);
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(ex.ToString(), "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form2.LogMessageToFile(ex.Message);
                Logger log = NLog.LogManager.GetCurrentClassLogger();
                log.Error(ex.ToString(), ex);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please insert Value for Suma" + " Don't use " + " .", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form2.LogMessageToFile(ex.Message);
                Logger log = NLog.LogManager.GetCurrentClassLogger();
                log.Error(ex.ToString(), ex);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString(), "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form2.LogMessageToFile(ex.Message);
                Logger log = NLog.LogManager.GetCurrentClassLogger();
                log.Error(ex.ToString(), ex);
            }
        }

        private void getCurrency(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string val = comboBoxCurrency.Text;
            get_response(val);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            double val = 0;
          //  listView1.SelectedItems[0].BackColor= Color.Yellow;
            ListViewItem item = listView1.SelectedItems[0];
            if (item.Text == "srednji_tecaj" || item.Text == "kupovni_tecaj" || item.Text == "prodajni_tecaj")
            {    //Dohvati tecaj
               val = Convert.ToDouble(item.SubItems[1].Text);
            }
            if(val > 0)
               ConvertHrk(val);

        }
    }
}
