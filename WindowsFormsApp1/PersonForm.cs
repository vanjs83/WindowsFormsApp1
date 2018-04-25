using DataAccessLayer;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BusinessLogicLayer;

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

            BusinessLayer.LogMessageToFile("Insert Personal Information: Validation");
            //Validacjia
            if (string.IsNullOrEmpty(this.textBoxName.Text) || string.IsNullOrEmpty(this.textBoxSurname.Text) ||
                string.IsNullOrEmpty(this.textBoxAddress.Text) || string.IsNullOrEmpty(this.textBoxEmail.Text) ||
                string.IsNullOrEmpty(this.textBoxOIB.Text) || string.IsNullOrEmpty(this.textBoxMob.Text) )
            {
                MessageBox.Show("Please, put all information", "Faild", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                BusinessLayer.LogMessageToFile("Validation not success " +"not all field populate "+ " Line 30");
                return;

            }
            else if (!Regex.IsMatch(this.textBoxMob.Text, @"^\d") || !Regex.IsMatch(this.textBoxOIB.Text, @"^\d{11}$") || this.textBoxOIB.TextLength != 11)
            {
                MessageBox.Show("Please, put only digital number  for OIB (11) and Tel", "Faild", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                BusinessLayer.LogMessageToFile("Validation not success" + " not numerc nuber insert"+ " Line 39");
                return;
            }



            BusinessLayer.LogMessageToFile("Validation success");
            string name = this.textBoxName.Text.Trim();
            string surname = this.textBoxSurname.Text.Trim();
            string address = this.textBoxAddress.Text.Trim();
            string email= this.textBoxEmail.Text.Trim();
            string oib = this.textBoxOIB.Text.Trim();
            string mob = this.textBoxMob.Text.Trim();

            Person person = new Person(name, surname, oib, address, email, mob);
            int InsertPersonNumberRow = DataLayer.InsertPerson(person);

            if (InsertPersonNumberRow == 1)
            {
                BusinessLayer.LogMessageToFile("Insert Person is success");
                MessageBox.Show("Insert Person " + person.Name + " Success", "Insert Person" ,MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            }
                this.textBoxName.Clear();
                this.textBoxSurname.Clear();
                this.textBoxAddress.Clear();
                this.textBoxEmail.Clear();
                this.textBoxOIB.Clear();
                this.textBoxMob.Clear();
                Form2 f2 = new Form2();
                this.Hide();
                f2.Show();

        }
            
          
     }

 }
  
