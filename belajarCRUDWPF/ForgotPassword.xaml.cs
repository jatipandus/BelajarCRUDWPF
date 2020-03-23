using belajarCRUDWPF.MyContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Configuration;
using System.Data.SqlClient;

namespace belajarCRUDWPF
{
    
    public partial class ForgotPassword : Window
    {
        myContext connection = new myContext();
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cek_validasi = connection.Users.Where(S => S.Email == txt_email_forgot.Text).FirstOrDefault(); 
            if (cek_validasi == null)
            {
                MessageBox.Show("Your email is wrong!");
                txt_email_forgot.Focus();
                return;
            }
            var password = Guid.NewGuid().ToString();
            cek_validasi.Password = password;
            connection.SaveChanges();
            MailMessage mm = new MailMessage("tertermyblog@gmail.com", 
                                                cek_validasi.Email);
            string today = DateTime.Now.ToString("dd/MM/yyyy");
            mm.Subject = "Password Recovery (" + today + ")";
            mm.Body = string.Format("Hii {0},<br /><br />This is Your Password Recovery : <br />{1}<br /><br />Thanks!", 
                                    cek_validasi.Email, password);
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;

            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            NetworkCred.UserName = "tertermyblog@gmail.com";
            NetworkCred.Password = "123123";
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            try
            {
                smtp.Send(mm);
            }
            catch
            {

            }
            MessageBox.Show("New password has been sent to your email!");
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
