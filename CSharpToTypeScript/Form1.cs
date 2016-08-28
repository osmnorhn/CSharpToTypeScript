using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CSharpToTypeScript.CSharpToTypeScript;

namespace CSharpToTypeScript
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbDestination.Text =  AppDomain.CurrentDomain.BaseDirectory;
        }

        private void tbSource_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                tbSource.Text = openFileDialog1.FileName;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            string sourcePath = tbSource.Text;
            string destPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + tbDestination.Text;
            try
            {
                Assembly asm = Assembly.UnsafeLoadFrom(sourcePath);

                StringBuilder text = new StringBuilder();
                ModuleWriter mw = new ModuleWriter();
                mw.Write(text, asm);

                string code = text.ToString();

                using (Stream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    using (TextWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(code);
                    }
                }
                MessageBox.Show("Entity was successfully created in the path : " + tbDestination.Text);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Assembly cannot be loaded:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Console.WriteLine(errorMessage);
            }
            catch
            {
                Console.WriteLine("Assembly can not be read!");
            }
        }
    }
}
