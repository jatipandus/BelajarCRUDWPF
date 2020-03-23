using belajarCRUDWPF.Model;
using belajarCRUDWPF.MyContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Configuration;
using System.Data.SqlClient;

namespace belajarCRUDWPF
{
    
    public partial class MainWindow : Window
    {
        myContext connection = new myContext();
        int cb_sup;
        string email;
        public MainWindow(string email_user)
        {
            InitializeComponent();
            tbl_supplier.ItemsSource = connection.Suppliers.ToList();
            tbl_item.ItemsSource = connection.Items.ToList();
            drp_supplier.ItemsSource = connection.Suppliers.ToList();
            
            this.email = email_user;
            Block_Access();
        }

        public MainWindow()
        {

        }

        private void Block_Access()
        {
            var check_access = connection.Users.Where(S => S.Email == email).FirstOrDefault();
            if (check_access.Role == "member")
            {
                var tab = Menu.Items[1] as TabItem;
                tab.IsSelected = true;
                tab = Menu.Items[0] as TabItem;
                tab.IsEnabled = false;
            }
            else
            {
                var tab = Menu.Items[0] as TabItem;
                tab.IsEnabled = true;
                tab = Menu.Items[1] as TabItem;
                tab.IsEnabled = true;
            }
        }

        private void Send_Password(string email, string password)
        {
            MailMessage mm = new MailMessage("tertermyblog@gmail.com", email);
            string today = DateTime.Now.ToString("dd/MM/yyyy");
            mm.Subject = "Password New Account (" + today + ")";
            mm.Body = string.Format("Hii {0},<br /><br />Your password for your account is : <br />{1}<br /><br />Thank You.", email, password);
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
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Clear_Textbox_Supplier()
        {
            txt_id.Clear();
            txt_name.Clear();
            txt_address.Clear();
            txt_email.Clear();
        }

        private void btn_insert_Click(object sender, RoutedEventArgs e)
        {
            var check_exist = connection.Users.Where(S => S.Email == txt_email.Text).FirstOrDefault();
            if (txt_name.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("Please fill supplier Name!");
                txt_name.Focus(); 
                return; 
            }
            else if (txt_address.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("Please fill supplier Address!");
                txt_address.Focus(); 
                return; 
            }
            else if (txt_email.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("Please fill supplier Email!");
                txt_address.Focus(); 
                return; 
            }

            else if (System.Text.RegularExpressions.Regex.IsMatch(txt_email.Text, "[^a-zA-Z0-9@.]"))
            {
                MessageBox.Show("Email format wrong!");
                txt_email.Focus();
                return;
            }
            else if (!txt_email.Text.Contains("@gmail.com") && !txt_email.Text.Contains("@yahoo.com"))
            {
                MessageBox.Show("Email format wrong!");
                txt_email.Focus();
                return;
            }

            else if (check_exist != null)
            {
                MessageBox.Show("Email already used!");
                txt_email.Focus();
                return;
            }

            var input = new Supplier(txt_name.Text, txt_address.Text, txt_email.Text); 
            connection.Suppliers.Add(input); 
            var success = connection.SaveChanges(); 

            var password = Guid.NewGuid().ToString(); 
            var input2 = new User(txt_email.Text, password, "member");
            connection.Users.Add(input2);
            connection.SaveChanges();
            Send_Password(txt_email.Text, password);
            tbl_supplier.ItemsSource = connection.Suppliers.ToList(); 
            MessageBox.Show(success + " Data Successfully Inputted & Password Has Been Sent to Email!");
            Clear_Textbox_Supplier();
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txt_id.Text);
            var myid = connection.Suppliers.Where(S => S.Id == id).FirstOrDefault();
            myid.Name = txt_name.Text;
            myid.Address = txt_address.Text;

            connection.SaveChanges();
            tbl_supplier.ItemsSource = connection.Suppliers.ToList();

            MessageBox.Show("Data Berhasil Diperbarui");
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            
            int deletelist = Convert.ToInt32(txt_id.Text);
            var myid = connection.Suppliers.Where(S => S.Id == deletelist).FirstOrDefault();
            
            MessageBoxResult dr = MessageBox.Show("Are you sure to delete row?", "Confirmation", MessageBoxButton.YesNo);
            if (dr == MessageBoxResult.Yes)
            {

                connection.Suppliers.Remove(myid);
                var delete = connection.SaveChanges();
                MessageBox.Show(delete + "Data Berhasil DiHapus");
                txt_id.Text = string.Empty;
                txt_name.Text = string.Empty;
                txt_address.Text = string.Empty;
                tbl_supplier.ItemsSource = connection.Suppliers.ToList();
            }
            else
            {
                tbl_supplier.ItemsSource = connection.Suppliers.ToList();
            }
        }

        private void txt_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txt_address_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txt_id_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void tbl_supplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = tbl_supplier.SelectedItem;
            if (data == null)
            {
                tbl_supplier.ItemsSource = connection.Suppliers.ToList();
            }
            else
            {
                string Id = (tbl_supplier.SelectedCells[0].Column.GetCellContent(data) as TextBlock).Text;
                txt_id.Text = Id;
                string Name = (tbl_supplier.SelectedCells[1].Column.GetCellContent(data) as TextBlock).Text;
                txt_name.Text = Name;
                string Address = (tbl_supplier.SelectedCells[2].Column.GetCellContent(data) as TextBlock).Text;
                txt_address.Text = Address;
            }
        }
        private void txt_id_Copy_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txt_name_Copy_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txt_price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txt_stock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void drp_supplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cb_sup = Convert.ToInt32(drp_supplier.SelectedValue.ToString());
        }

        private void btn_update_item_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_insert_item_Click(object sender, RoutedEventArgs e)
        {
            var cPrice = Convert.ToInt32(txt_price_item.Text);
            var cStock = Convert.ToInt32(txt_stock_item.Text);
            var cSup = connection.Suppliers.Where(si => si.Id == cb_sup).FirstOrDefault();
            var inputitem = new Item(txt_name_item.Text, cPrice, cStock, cSup);
            var supplierdata = connection.Suppliers.Where(S => S.Id == cb_sup).FirstOrDefault(); 
            var input = new Item(txt_name_item.Text, Convert.ToInt32(txt_price_item.Text), Convert.ToInt32(txt_stock_item.Text), supplierdata); 
            if (txt_name_item.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("Please fill item Name!");
                txt_name_item.Focus(); 
                return; 
            }
            else if (txt_price_item.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("Please fill item Price!");
                txt_price_item.Focus();
                return; 
            }
            else if (txt_stock_item.Text.Trim() == string.Empty) 
            {
                MessageBox.Show("Please fill item Stock!");
                txt_stock_item.Focus(); 
                return; 
            }
            else if (supplierdata == null)
            {
                MessageBox.Show("Please choose Supplier!");
                drp_supplier.Focus(); 
                return; 
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(txt_name_item.Text, "[^a-zA-Z0-9]"))
            {
                MessageBox.Show("Item name format wrong!");
                txt_name_item.Focus();
                return;
            }
            
            connection.Items.Add(inputitem);
            connection.SaveChanges();
            MessageBox.Show("Data Telah Disimpan");
            txt_name_item.Text = "";
            txt_price_item.Text = "";
            txt_stock_item.Text = "";
            tbl_item.ItemsSource = connection.Items.ToList();
        }

        private void txt_email_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
