using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Dosya_işlemleri
{
    public partial class Form1 : Form
    {

        private FileInfo[] dosyalar;  // dosyalar dizisi
        private string logFilePath = "dosya_islemleri_log.txt";  // Log dosyasının yolu

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add(".txt");
            comboBox1.Items.Add(".xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
                    dosyalar = di.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                         .Concat(di.GetFiles("*.xml", SearchOption.TopDirectoryOnly))
                         .ToArray();

                    listBox1.Items.Clear();

                    foreach (FileInfo file in dosyalar)
                    {
                        listBox1.Items.Add(file.Name);
                    }

                    Log("Klasör tarandı: " + fbd.SelectedPath);
                }
                catch (Exception ex)
                {
                    Log("Klasör tarama hatası: " + ex.Message);
                }
            }
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1) // Bir öğe seçildiyse devam et
            {
                // Seçilen öğenin adını al
                string selectedFileName = listBox1.SelectedItem.ToString();

                // dosyalar dizisinden seçilen dosyanın bilgilerini al
                FileInfo selectedFile = dosyalar.FirstOrDefault(file => file.Name == selectedFileName);

                if (selectedFile != null)
                {
                    // Dosya detaylarını göstermek için MessageBox kullanıldı
                    MessageBox.Show("DOSYA ÖZELLİKLERİ:\n" +
                                    $"Dosya Adı: {selectedFile.Name}\n" +
                                    $"Dosya Boyutu: {selectedFile.Length} bytes\n" +
                                    $"Oluşturulma Tarihi: {selectedFile.CreationTime}\n" +
                                    $"Değiştirilme Tarihi: {selectedFile.LastWriteTime}");
                }
            }
        }

        private void btnDosyaEkle_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Metin Dosyaları (*.txt)|*.txt|XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen dosyaların bilgilerini alabıp listeye ekleme.
                foreach (string fileName in openFileDialog.FileNames)
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    dosyalar = dosyalar.Concat(new[] { fileInfo }).ToArray();
                    listBox1.Items.Add(fileInfo.Name);

                    Log("Dosya eklendi: " + fileInfo.FullName);
                }
            }
        }

        private void btnDosyaSil_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedFileName = listBox1.SelectedItem.ToString();
                FileInfo selectedFile = dosyalar.FirstOrDefault(file => file.Name == selectedFileName);

                if (selectedFile != null)
                {
                    // Dosyayı silmeden önce kullanıcıdan onay alma işlemi.
                    DialogResult result = MessageBox.Show($"'{selectedFile.Name}' adlı dosyayı silmek istiyor musunuz?", "Dosya Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            // Dosyayı listbox'tan ve dosyalar listesinden kaldırır.
                            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                            dosyalar = dosyalar.Where(file => file.Name != selectedFileName).ToArray();

                            // Dosyayı fiziksel olarak siler.
                            File.Delete(selectedFile.FullName);

                            Log("Dosya silindi: " + selectedFile.FullName);
                            MessageBox.Show($"'{selectedFile.Name}' adlı dosya başarıyla silindi.", "Dosya Silme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            Log("Dosya silme hatası: " + ex.Message);
                            MessageBox.Show($"Dosya silme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnDosyaKopyala_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedFileName = listBox1.SelectedItem.ToString();
                FileInfo selectedFile = dosyalar.FirstOrDefault(file => file.Name == selectedFileName);

                if (selectedFile != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Tüm Dosyalar (*.*)|*.*";
                    saveFileDialog.FileName = selectedFile.Name;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string destinationPath = saveFileDialog.FileName;

                        try
                        {
                            // Dosya kopyalama işlemi.
                            File.Copy(selectedFile.FullName, destinationPath);

                            Log($"Dosya kopyalandı: {selectedFile.FullName} -> {destinationPath}");
                            MessageBox.Show($"Dosya başarıyla kopyalandı: {destinationPath}", "Dosya Kopyalama", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            Log($"Dosya kopyalama hatası: {ex.Message}");
                            MessageBox.Show($"Dosya kopyalama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnDosyaTasi_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedFileName = listBox1.SelectedItem.ToString();
                FileInfo selectedFile = dosyalar.FirstOrDefault(file => file.Name == selectedFileName);

                if (selectedFile != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Tüm Dosyalar (*.*)|*.*";
                    saveFileDialog.FileName = selectedFile.Name;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string destinationPath = saveFileDialog.FileName;

                        try
                        {
                            // Dosya taşıma işlemi.
                            File.Move(selectedFile.FullName, destinationPath);

                            // Taşıma işlemi başarılıysa, dosyanın listbox'tan ve dosyalar listesinden kaldırılması için.
                            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                            dosyalar = dosyalar.Where(file => file.Name != selectedFileName).ToArray();

                            Log($"Dosya taşındı: {selectedFile.FullName} -> {destinationPath}");
                            MessageBox.Show($"Dosya başarıyla taşındı: {destinationPath}", "Dosya Taşıma", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            Log($"Dosya taşıma hatası: {ex.Message}");
                            MessageBox.Show($"Dosya taşıma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void txtHizliArama_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = txtHizliArama.Text.ToLower();

            listBox1.Items.Clear();

            if (dosyalar != null)
            {
                foreach (FileInfo file in dosyalar)
                {
                    if (file.Name.ToLower().Contains(aramaMetni))
                    {
                        listBox1.Items.Add(file.Name);
                    }
                }

                Log($"Hızlı arama yapıldı: '{aramaMetni}'");
            }
            else
            {
                MessageBox.Show("Dosyalar yüklenmemiş veya null durumunda.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedFileExtension = comboBox1.SelectedItem.ToString().ToLower();

                if (dosyalar != null)
                {
                    // Seçilen dosya uzantısına göre filtreleme.
                    var filteredFiles = dosyalar.Where(file => file.Extension.ToLower() == selectedFileExtension);

                    // Filtrelenmiş dosyaları ListBox'a ekleme.
                    listBox1.Items.Clear();
                    foreach (FileInfo file in filteredFiles)
                    {
                        listBox1.Items.Add(file.Name);
                    }

                    Log($"Dosya uzantısına göre filtreleme yapıldı: '{selectedFileExtension}'");
                }
                else
                {
                    MessageBox.Show("Dosyalar yüklenmemiş veya null durumunda.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Log(string logMessage)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine($"{DateTime.Now} - {logMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Log yazma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

