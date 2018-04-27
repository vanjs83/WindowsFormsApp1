using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using NLog;
using DataAccessLayer;
using BusinessLogicLayer;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Person _person;
      
        public Form1(User user)
        {
            InitializeComponent();
            this._person  = new Person (user);
            RunTimer();
           
        }

        private void get_response(string val)
        {
            
            WebClient wp = new WebClient();
            string url = "http://api.hnb.hr/tecajn?valuta=" + val;
            BusinessLogicLayer.BusinessLayer.LogMessageToFile("");
            var response = wp.DownloadString(url);
             get_data(response);
           
        }


        private void get_data(string response)
        {
            bool resFromRequest = string.IsNullOrEmpty(response);
            if (resFromRequest)
            {
                
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Response has Value " + resFromRequest);
                throw new ArgumentException("response is empty", nameof(response));
               
            }
            BusinessLogicLayer.BusinessLayer.LogMessageToFile("response has Value: " + resFromRequest + " Get Json from Requets");

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

      


    

        //SELECT QUERY
        private void button1_Click(object sender, EventArgs e)
        {
            string ProcedureName=""; 
          
            
            if (this.checkBox1.Checked)
            {
                button4.Enabled = false;
                ProcedureName = "totalSum";
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Execute procedure {totalSum} line{150}");

            }
            else
            {
                button4.Enabled = true;
                ProcedureName = "ShowPayments";
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Execute procedure {ShowPayments} line{157}");

                button4.Enabled = true;
            }
        
                
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("try execute procedure line{172}");

                        //VRATI INFO FOR GRID VIEW                                                                                      item name                     count name                      payname
                   dataGridView1.DataSource = DataLayer.SelectQuery(ProcedureName, this.dateTimeFrom.Value, this.dateTimeTo.Value, this.comboBoxItem.Text.Trim(), this.comboBoxCount.Text.Trim(), this.comboBoxCategory.Text.Trim(), this.comboBoxPay.Text.Trim(),  this._person.Id);
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
                this.chart1.DataSource = DataLayer.SelectQuery(ProcedureName, this.dateTimeFrom.Value, this.dateTimeTo.Value, this.comboBoxItem.Text.Trim(), this.comboBoxCount.Text.Trim(), this.comboBoxCategory.Text.Trim(), this.comboBoxPay.Text.Trim(),  this._person.Id );
                chart1.DataBind();
                 
                // Suma consumption
                if (checkBox1.Checked)
                {
                    double sumPrice = 0;
                    int numItem = 0; //Ako postoji redak racunaj
                    if (dataGridView1.Rows.Count > 1)
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; ++i)
                        {   // nadodavanje izracuna
                            sumPrice += Convert.ToDouble(dataGridView1.Rows[i].Cells["Price"].Value.ToString());
                            numItem += Convert.ToInt32(dataGridView1.Rows[i].Cells["Number"].Value.ToString());
                        }

                
                    this.dataGridView1.Rows[dataGridView1.Rows.Count - 1 ].Cells[4].Value = String.Format("{0:0.00}", sumPrice);
                    this.dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Value = String.Format("{0}", numItem);
                    }
                      
                }

                //List<User> user = new List<WindowsFormsApp1.User>();
                //user = data.Rows.OfType<DataTable>().Select(dr => dr.Field<User>("name")).ToList();

                comboBox1.DisplayMember = "Item";
                comboBox1.DataSource = DataLayer.ShowDataForComboBoxItem(_person.Id, this.comboBoxCount.Text.Trim());
                comboBoxItem.DisplayMember = "Item";
                comboBoxItem.DataSource = DataLayer.ShowDataForComboBoxItem(_person.Id, this.comboBoxCount.Text.Trim());
               

                //SHOW COUNTS CATEGORY PAY
                string CountProcedureName = "ShowNameCounts";
                comboBoxCount.DisplayMember = "Counts";
                comboBoxCount.DataSource = DataLayer.ShowDataForComboBox( CountProcedureName, _person.Id);
                //SHOW CATEGORY 
                string CategoryProcedureName = "ShowNameCategory";
                comboBoxCategory.DisplayMember = "Category";
                comboBoxCategory.DataSource = DataLayer.ShowDataForComboBox(CategoryProcedureName, _person.Id);
                // SHOW TYPE OF PAY
                string PayProcedureName = "ShowNameTypeOfPay";
                comboBoxPay.DisplayMember = "Pay";
                comboBoxPay.DataSource = DataLayer.ShowDataForComboBox(PayProcedureName, _person.Id);

            }
          





        //INSERT DATABASE
        private void button2_Click(object sender, EventArgs e)
        {
            BusinessLogicLayer.BusinessLayer.LogMessageToFile("Validation line 291 ");
            double suma;
            if (string.IsNullOrEmpty(this.textBoxName.Text) || string.IsNullOrEmpty(this.textBoxSuma.Text))
            {
                MessageBox.Show("Please, Entered all data!", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Not entered all data {line 291} ");
                return;
            }
            else if (double.TryParse(this.textBoxSuma.Text, out suma) == false)
            {
                MessageBox.Show("Please, numeric data for Price!", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Not entered numeric data  ");
                textBoxSuma.Clear();
                return;
            }

            try
            {
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Insert Category {line 310} ");
                if (!string.IsNullOrEmpty(this.comboBoxCategory.Text))
                {
                    //poziv funkcije spremi kategoriju
                      if (_person.prava == Prava.User)
                        throw new System.Exception(_person.Name + " You can't insert item, you must be Admin");

                        
                    int numExecute = DataLayer.SaveCategory(_person.Id, comboBoxCategory.Text);
                    if (numExecute == -1)
                    {
                        MessageBox.Show("Category " + comboBoxCategory.Text, "OK", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BusinessLogicLayer.BusinessLayer.LogMessageToFile("Category exists " + comboBoxCategory.Text + "statu: " + numExecute); ;
                    }
                    else if (numExecute != -1)
                    {
                        MessageBox.Show("This Category " + comboBoxCategory.Text + " is now save !", "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BusinessLogicLayer.BusinessLayer.LogMessageToFile("New Category save " + comboBoxCategory.Text + "status: " + numExecute); ;

                    }
                }

                if (!string.IsNullOrEmpty(this.comboBoxPay.Text))
                {
                    BusinessLogicLayer.BusinessLayer.LogMessageToFile("Insert TypeOfPay {line 320} ");

                    int numExecute = DataLayer.SaveTypeOfPay(this.comboBoxPay.Text.Trim());

                    //poziv funkcije spremi način plačanja
                    if (numExecute == -1)
                    {
                        MessageBox.Show("Type of pay " + this.comboBoxPay.Text, "OK", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BusinessLogicLayer.BusinessLayer.LogMessageToFile("Type of pay exists " + comboBoxPay.Text + " " + numExecute); ;

                    }
                    else if (numExecute != -1)
                    {
                        MessageBox.Show("This type of pay " + comboBoxPay.Text + " is now save !", "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BusinessLogicLayer.BusinessLayer.LogMessageToFile("Save Type of pay  " + comboBoxPay.Text + " " + numExecute); ;

                    }
                }

                if (!string.IsNullOrEmpty(this.comboBoxCount.Text))
                {      //Poziv funkcije naziv računa
                    BusinessLogicLayer.BusinessLayer.LogMessageToFile("Insert Counts name {line 334} ");
                    int numExecute = DataLayer.SaveCounts(this.comboBoxCategory.Text.Trim(), this.comboBoxCount.Text.Trim());
                    if (numExecute == -1)
                    {
                        MessageBox.Show("Counts: " + this.comboBoxCount.Text, "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BusinessLogicLayer.BusinessLayer.LogMessageToFile("Caunts exists " + comboBoxCount.Text + " " + numExecute); ;

                    }
                    else if (numExecute != -1)
                    {
                        MessageBox.Show("Counts " + this.comboBoxCount.Text + " is now save !", "ok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BusinessLogicLayer.BusinessLayer.LogMessageToFile("Counts exists " + comboBoxCount.Text + " " + numExecute); ;

                    }
                }

                
                string NamePay = this.comboBoxPay.Text.Trim();
                string NameCount =this.comboBoxCount.Text.Trim();
                string NameItem = this.textBoxName.Text.Trim();
                double Suma = Convert.ToDouble(this.textBoxSuma.Text.Trim());
                string Datum = this.dateTimeInsert.Value.Date.ToString("yyyy-MM-dd HH:mm");
                string Description = this.textBoxDescription.Text.Trim();
          

                
                int NumQuery = DataLayer.InsertItem(NamePay, NameCount, NameItem, Suma, Datum, Description);

                if (NumQuery != -1)
                {
                    MessageBox.Show(this.textBoxName.Text, "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BusinessLogicLayer.BusinessLayer.LogMessageToFile("Save " + textBoxName.Text + "status " + NumQuery); ;
                }
                else
                {
                    BusinessLogicLayer.BusinessLayer.LogMessageToFile("Not save " + textBoxName.Text + "status " + NumQuery); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Error"+ ex.Message);
                 Logger log = NLog.LogManager.GetCurrentClassLogger();
                 log.Error("Execute procedure InsertPayment", ex);
            }
            finally
            {
               
                this.textBoxName.Clear();
                this.textBoxSuma.Clear();
                this.textBoxDescription.Clear();
                
            }
        }




        //EXCEL REPORT
        private void button3_Click(object sender, EventArgs e)
        {
            BusinessLogicLayer.BusinessLayer.LogMessageToFile("Export Excel");
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
            BusinessLogicLayer.BusinessLayer.LogMessageToFile("Export Excel Status: Success");
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

            string NameItem = this.comboBox1.Text.Trim();
            string NameCounts = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            int ItemId =  Convert.ToInt32(this.dataGridView1.CurrentRow.Cells[1].Value);
            string NameCategory = this.dataGridView1.CurrentRow.Cells[2].Value.ToString();
            string TypeOfPay = this.dataGridView1.CurrentRow.Cells[3].Value.ToString();
      
            DataLayer.DeletePayments(NameItem, NameCounts, ItemId, NameCategory, TypeOfPay, _person);
           
        }
   

        private void Print_Click(object sender, EventArgs e)
        {
            BusinessLogicLayer.BusinessLayer.LogMessageToFile("Print pdf line{501}");
            //Open the print dialog
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;
            printDialog.UseEXDialog = true;
            //Get the document
            if (DialogResult.OK == printDialog.ShowDialog())
            {
                printDocument1.DocumentName = "Test Page Print";
                BusinessLogicLayer.BusinessLayer.LogMessageToFile("Print pdf Status Success line{512}");
                printDocument1.Print();
            }
        
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Bitmap btm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            dataGridView1.DrawToBitmap(btm, new System.Drawing.Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(btm, 10, 10);

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
            
          //  listView1.SelectedItems[0].BackColor= Color.Yellow;
            ListViewItem item = listView1.SelectedItems[0];
            if (item.Text == "srednji_tecaj" || item.Text == "kupovni_tecaj" || item.Text == "prodajni_tecaj")
            {    //Dohvati tecaj
               string tecaj = item.SubItems[1].Text;
                if (tecaj != null)
                {
                    string value = textBoxSuma.Text.Trim();
                    this.textBoxSuma.Text = BusinessLayer.ConvertHrk(tecaj, value);
                }
            }
               

        }
    }
}
