using belajarCRUDWPF.Model;
using belajarCRUDWPF.MyContext;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

namespace belajarCRUDWPF
{
    
    public partial class Login : Window
    {
        myContext connection = new myContext();
        public Login()
        {
            InitializeComponent();
        }

        private void txt_email_login_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txt_password_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            var cek_validasi = connection.Users.Where(S => S.Email == txt_email_login.Text).FirstOrDefault(); 
            if (cek_validasi == null)
            {
                MessageBox.Show("Email Yang Anda Masukan Salah!");
                txt_email_login.Focus(); 
                return; 
            }
            else if (cek_validasi.Password != txt_password.Password.ToString())
            {
                MessageBox.Show("Password Yang Anda Masukan Salah!");
                txt_password.Focus(); 
                return;
            }
            MainWindow main = new MainWindow(cek_validasi.Email);
            main.Show();
            this.Close();
        }

        private void btn_forgot_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgot = new ForgotPassword();
            forgot.Show();
            this.Close();
        }
    }
}
