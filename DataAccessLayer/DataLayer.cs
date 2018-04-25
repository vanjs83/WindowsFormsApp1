using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace DataAccessLayer
{
    public class DataLayer
    {

        public static string GetConnString()
        {    //connection string on database VSITESTUDNET
            return "Data Source=VSITESTUDENT;Initial Catalog=Payment;Integrated Security=True";
            //return "workstation id=Payments.mssql.somee.com;packet size=4096;user id=tvanjurek_SQLLogin_1;pwd=6ejthpgljo;data source=Payments.mssql.somee.com;persist security info=False;initial catalog=Payments";
        }


        public static void DeletePayments(string NameItem, string NameCounts, int ItemId, string NameCategory, string TypeOfPay, Person person)
        {

            SqlConnection conn = new SqlConnection(GetConnString());
            try
            {
                SqlCommand cmd = new SqlCommand();
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                if (person.prava == Prava.User)
                {
                    throw new System.Exception(person.Name + " You can't delete, you must be Admin");
                }

                cmd.CommandText = "DeletePayments";
                // SEND PARAMETAR INTO PROCEDURE     item for delete
                cmd.Parameters.AddWithValue("@NameItem", NameItem);
                cmd.Parameters.AddWithValue("@NameCounts", NameCounts);
                cmd.Parameters.AddWithValue("@id", ItemId);
                cmd.Parameters.AddWithValue("@NameCategory", NameCategory);
                cmd.Parameters.AddWithValue("@TypeOfPay", TypeOfPay);
                cmd.Parameters.AddWithValue("@UserId", person.Id);

                cmd.Connection = conn;
                int numberEffected = cmd.ExecuteNonQuery();

                if (numberEffected != -1)
                {
                    MessageBox.Show(NameItem, "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch 
            {

                MessageBox.Show(NameItem, "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            finally
            {
                conn.Close();
            }

        }




        //poziv funkcije spremi kategoriju
        public static int SaveCategory(int UserId, string CategoryName)
        {
       
           
                // spremi kategoriju ako ne postoji
                SqlConnection conn = new SqlConnection(GetConnString());
                conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertCategory";
                cmd.Parameters.AddWithValue("@iduser", UserId);
                cmd.Parameters.AddWithValue("@name", CategoryName);
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();

            }

        }
        // SPREMI NACIN PLACANJA
        public static int SaveTypeOfPay(string TypeOfPay)
        {
            //Spremi način plačanja ako ne postoji
            SqlConnection conn = new SqlConnection(GetConnString());
            conn.Open();
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "InsertTypeOfPay";
                cm.Parameters.AddWithValue("@namePay", TypeOfPay);
                return cm.ExecuteNonQuery();
            }
           
            finally
            {
                conn.Close();

            }
        }

        //Spremi Counts ako ne postoji
        public static int SaveCounts(string CategoryName, string CountName)
        {
            SqlConnection conn = new SqlConnection(GetConnString());
            conn.Open();
            try
            {
                SqlCommand cmdCount = new SqlCommand();
                cmdCount.Connection = conn;
                cmdCount.CommandType = CommandType.StoredProcedure;
                cmdCount.CommandText = "InsertCounts";
                cmdCount.Parameters.AddWithValue("@CategoryName", CategoryName);
                cmdCount.Parameters.AddWithValue("@NameCount", CountName);
                return cmdCount.ExecuteNonQuery();
            }

            finally
            {
                conn.Close();
            }
        }






        public static int InsertItem(string NamePay,string NameCount,string NameItem,double Suma,string Datum,string Description )
        { 

            SqlConnection conn = new SqlConnection(DataLayer.GetConnString());
            conn.Open();
            try
            {
                SqlCommand cmm = new SqlCommand();
                cmm.Connection = conn;
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "InsertPayments";
                cmm.Parameters.AddWithValue("@NamePay", NamePay);
                cmm.Parameters.AddWithValue("@NameCount", NameCount);
                cmm.Parameters.AddWithValue("@name", NameItem);
                cmm.Parameters.AddWithValue("@suma", Suma);
                cmm.Parameters.AddWithValue("@datum", Datum);
                cmm.Parameters.AddWithValue("@description", Description);
                int numQuery = cmm.ExecuteNonQuery();
                return numQuery;
            }


            finally {
                conn.Close();
            }
        }


        public static DataTable ShowDataForComboBox(string ProcedureName, int UserId)
        {

            SqlConnection conn = new SqlConnection(GetConnString());
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand();
                SqlDataAdapter adapter = new SqlDataAdapter();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = ProcedureName;
                command.Parameters.AddWithValue("@idUser", UserId);
                adapter.SelectCommand = command;
                command.Connection = conn;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable;
            }
            finally
            {
                conn.Close();
            }
        }




        public static DataTable ShowDataForComboBoxItem(int UserId, string CountName)
        {

            //SHOW NAME ITEM 
            SqlConnection conn = new SqlConnection(GetConnString());
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand();
                SqlDataAdapter adapter = new SqlDataAdapter();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ShowName";
                command.Parameters.AddWithValue("@idUser", UserId);
                command.Parameters.AddWithValue("@NameCounts", CountName);
                adapter.SelectCommand = command;
                command.Connection = conn;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
            finally
            {

                conn.Close();
            }
    
        }




        public static BindingSource SelectQuery(string ProcedureName, DateTime From, DateTime To, string ItemName, string CountName, string CategoryName, string PayName, int UsedId)
        {
           
                SqlConnection conn = new SqlConnection(DataLayer.GetConnString());
          
                conn.Open();
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = ProcedureName;
                comm.Connection = conn;
                // SEND PARAMETARS INTO PROCEDURE
                comm.Parameters.AddWithValue("@from", From);
                comm.Parameters.AddWithValue("@to", To);
                comm.Parameters.AddWithValue("@nameItem", ItemName);
                comm.Parameters.AddWithValue("@CountName", CountName);
                comm.Parameters.AddWithValue("@CategoryName", CategoryName);
                comm.Parameters.AddWithValue("@PayName", PayName);
                comm.Parameters.AddWithValue("@idUser", UsedId);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = comm;
                DataTable data = new DataTable();
                //Add sum
                adapter.Fill(data);


                BindingSource bsource = new BindingSource();
                bsource.DataSource = data;
                adapter.Update(data);

                if (ProcedureName == "totalSum")
                    data.Rows.Add();

                return bsource;

            }
            finally
            {
                conn.Close();
            }
        }

        public static int InsertShowUser(string ProcedureName, string Hash, User user)
        {


            SqlConnection conn = new SqlConnection(GetConnString());
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = ProcedureName;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", Hash);
                cmd.Parameters.AddWithValue("@role", user.prava);
                if(ProcedureName == "ShowLogin")
                cmd.Parameters.AddWithValue("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
            
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable data = new DataTable();
                adapter.Fill(data);

                if (data.Rows.Count == 1)
                  return int.Parse(cmd.Parameters["@ID"].Value.ToString());

                return cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }

            
        }

        public static int InsertPerson(Person person)
        {

            SqlConnection conn = new SqlConnection(GetConnString());
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
                return cmd.ExecuteNonQuery();
            }

            finally
            {
                conn.Close();

            }
        }

    }

 }

