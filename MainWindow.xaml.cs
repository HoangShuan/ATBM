using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AES
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClassAES aes = new ClassAES();
        private string currentKey = "";
        private string currentEncrypt = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpFile(TextBox textBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|Word Document (*.docx)|*.docx",
                Title = "Select a Document File",
                InitialDirectory = @"C:\Users\thanh\Downloads\"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (fileExtension == ".txt")
                {
                    textBox.Text = File.ReadAllText(filePath);
                }
                else if (fileExtension == ".docx")
                {
                    MessageBox.Show("Không thể đọc file '.docx'!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Định dạng tập tin không được hỗ trợ!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string ReadWordFile(string filePath)
        {
            StringBuilder text = new StringBuilder();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                DocumentFormat.OpenXml.Wordprocessing.Body? body = wordDoc.MainDocumentPart.Document.Body;
                text.Append(body.InnerText);
            }

            return text.ToString();
        }

        private void SaveFile(TextBox textBox, bool checkFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Document (*.txt)|*.txt|Word Document (*.docx)|*.docx";
            saveFileDialog.Title = "Save the content as a Word or Text document";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string fileName = saveFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(fileName);
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                string directory = System.IO.Path.GetDirectoryName(fileName);

                if (extension.ToLower() == ".docx")
                {
                    SaveTextToWordDocument(fileName, textBox.Text);
                    if(checkFile == true)
                    {
                        SaveTextToWordDocument(System.IO.Path.Combine(directory, fileNameWithoutExtension + "_key" + extension), txtKeyMa.Text);
                    }
       
                    MessageBox.Show("Lưu file thành công.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (extension.ToLower() == ".txt")
                {
                    File.WriteAllText(fileName, textBox.Text);
                    if (checkFile == true)
                    {
                        File.WriteAllText(System.IO.Path.Combine(directory, fileNameWithoutExtension + "_key" + extension), txtKeyMa.Text);
                    }
                        
                    MessageBox.Show("Lưu file thành công.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Định dạng tập tin không được hỗ trợ!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveTextToWordDocument(string filePath, string text)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();
                Paragraph paragraph = new Paragraph();
                Run run = new Run();
                run.Append(new Text(text));
                paragraph.Append(run);
                body.Append(paragraph);
                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }

        private void btnMaHoa_Click(object sender, RoutedEventArgs e)
        {
            if (txtBanRo.Text.Length > 0)
            {
                if (txtKeyMa.Text.Length == 24)
                {
                    txtMaHoa.Text = aes.MaHoa(txtBanRo.Text, txtKeyMa.Text);
                    currentKey = txtKeyMa.Text;
                    currentEncrypt = txtMaHoa.Text;
                    MessageBox.Show("Mã hóa thành công.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Khóa dùng mã hóa phải có độ dài 24 ký tự!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Bản rõ không được bỏ trống!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGiaiMa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtBanMa.Text.Length > 0)
                {
                    if (txtKeyGiai.Text.Length == 24)
                    {
                        txtGiaiMa.Text = aes.GiaiMa(txtBanMa.Text, txtKeyGiai.Text);
                        MessageBox.Show("Giải mã thành công.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        MessageBox.Show("Khoá dùng giải mã phải có độ dài 24 ký tự!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bản mã hóa không được bỏ trống!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception ex)
            {
                if(txtBanMa.Text != currentEncrypt)
                {
                    MessageBox.Show("Bản mã hóa đã bị thay đổi!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (txtKeyGiai.Text != currentKey)
                {
                    MessageBox.Show("Khóa dùng giải mã đã bị thay đổi!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            txtBanMa.Text = "";
            txtBanRo.Text = "";
            txtKeyGiai.Text = "";
            txtKeyMa.Text = "";
            txtGiaiMa.Text = "";
            txtMaHoa.Text = "";
        }

        private void txtKeyMa_TextChanged(object sender, TextChangedEventArgs e)
        {
            lenghtKeyMa.Content = txtKeyMa.Text.Length;
        }

        private void txtKeyGiai_TextChanged(object sender, TextChangedEventArgs e)
        {
            lenghtKeyGiai.Content = txtKeyGiai.Text.Length;
        }

        private void btnUpFile_Click(object sender, RoutedEventArgs e)
        {
            Confirm dialog = new Confirm("Chọn loại file:", "Bản rõ", "Bản mã hóa");
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string selectedOption = dialog.SelectedOption;
                if(selectedOption == "Bản rõ")
                {
                    UpFile(txtBanRo);
                }
                else if(selectedOption == "Bản mã hóa")
                {
                    UpFile(txtBanMa);
                }
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            Confirm dialog = new Confirm("Chọn file cần lưu:", "Bản mã hóa", "Bản giải mã");
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string selectedOption = dialog.SelectedOption;
                if (selectedOption == "Bản mã hóa")
                {
                    if(txtMaHoa.Text.Length > 0)
                    {
                        SaveFile(txtMaHoa, true);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi! Bản mã hóa đang trống.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (selectedOption == "Bản giải mã")
                {
                    if (txtGiaiMa.Text.Length > 0)
                    {
                        SaveFile(txtGiaiMa, false);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi! Bản giải mã đang trống.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnUpKey_Click(object sender, RoutedEventArgs e)
        {
            Confirm dialog = new Confirm("Chọn File khoá cho:", "Mã hóa", "Giải mã");
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string selectedOption = dialog.SelectedOption;
                if (selectedOption == "Mã hóa")
                {
                    UpFile(txtKeyMa);
                }
                else if (selectedOption == "Giải mã")
                {
                    UpFile(txtKeyGiai);
                }
            }
        }

        private void btnCreateKey_Click(object sender, RoutedEventArgs e)
        {
            txtKeyMa.Text = GenerateRandomString(24);
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }
    }
}