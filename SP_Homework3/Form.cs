using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SP_Homework3
{
    public partial class Form : System.Windows.Forms.Form
    {
        private Thread thread;
        public Form()
        {
            InitializeComponent();
            progressBar.Visible = false;
        }

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            filePathTextBox.Text = openFileDialog.FileName;
        }

        private void Encrypt()
        {
            byte password;
            Byte.TryParse(passwordTextBox.Text, out password);
            try
            {
                Crypt(password);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Decrypt()
        {
            byte password;
            Byte.TryParse(passwordTextBox.Text, out password);
            try
            {
                Crypt(password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Crypt(byte password)
        {
            using (var encryptFile = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.ReadWrite))
            {
                while (encryptFile.Position < encryptFile.Length)
                {
                    byte[] array = new byte[encryptFile.Length];
                    encryptFile.Read(array, 0, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] ^= password;
                        encryptFile.Seek(0, SeekOrigin.Begin);
                        encryptFile.Write(array, 0, array.Length);
                        progressBar.Invoke(new Action(() => progressBar.Increment((int)encryptFile.Length)));
                    }

                    progressBar.Invoke(new Action(() => progressBar.Value = 0));
                }
            }
        }
        
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (passwordTextBox == null)
            {
                MessageBox.Show("Не введен пароль для шифрования!");
            }
            if (filePathTextBox.Text != null)
            {
                progressBar.Visible = true;
                if (encryptRadioButton.Checked)
                {
                    try
                    {
                        thread = new Thread(Encrypt);
                        thread.Start();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Выбран неверный путь файла или отказано в доступе!\n"
                            +ex.Message);
                    }
                    MessageBox.Show("Файл успешно зашифрован!");
                }
                else if (decryptRadioButton.Checked)
                {
                    try
                    {
                        thread = new Thread(Decrypt);
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Выбран неверный путь файла или отказано в доступе!\n"
                            + ex.Message);
                    }
                    MessageBox.Show("Файл успешно расшифрован!");
                }
                else
                {
                    MessageBox.Show("Не выбрано действие над файлом!");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран!");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }
}
