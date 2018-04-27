using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusinessLogicLayer
{
    public class BusinessLayer
    {
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

        public static string Hash(string password) //HASH Password
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static string ConvertHrk(string tecaj, string sumaValue)
        {
            
            string ResultKuna = "";
            try
            {
                if (sumaValue.Contains('.'))
                    throw new FormatException();
                double Tecaj = Convert.ToDouble(tecaj);
                double Value = Convert.ToDouble(sumaValue);
                ResultKuna = String.Format("{0:0.00}", Value * Tecaj);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.ToString(), "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile(ex.Message); 
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(ex.ToString(), "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile(ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please insert Value for Suma" + " Don't use " + " .", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Please insert Value for Suma" + " Don't use " + " null reference ", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BusinessLogicLayer.BusinessLayer.LogMessageToFile(ex.Message);
            }
            return ResultKuna;
        }
    }
}
