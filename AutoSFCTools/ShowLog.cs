using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoSFCTools
{
    public class ShowLog
    {
        public static RichTextBox richTextBox;
        public static void ShowErrorLog(string text)
        {
            if (!richTextBox.IsDisposed)
            {
                if (richTextBox.InvokeRequired)
                {
                    MethodInvoker d = () =>
                    {
                        if (richTextBox.Lines.Length > 100)
                        {
                            string[] sLines = richTextBox.Lines;
                            string[] sNewLines = new string[sLines.Length - 1];
                            Array.Copy(sLines, 1, sNewLines, 0, sNewLines.Length);
                            richTextBox.Lines = sNewLines;
                        }
                        richTextBox.SelectionColor = Color.Red;
                        richTextBox.AppendText(string.Format("{0} {1}", text, System.Environment.NewLine));
                        richTextBox.SelectionStart = richTextBox.Text.Length;
                        richTextBox.SelectionLength = 0;
                        richTextBox.ScrollToCaret();
                       
                       
                    };
                    richTextBox.Invoke(d);
                }
                else
                {
                    richTextBox.SelectionColor = Color.Red;
                    richTextBox.AppendText(string.Format("{0} {1}",text, System.Environment.NewLine));
                    richTextBox.SelectionStart = richTextBox.Text.Length;
                    richTextBox.SelectionLength = 0;
                    richTextBox.ScrollToCaret();
                }
            }
        }
        public static void ShowTestLog(string text)
        {
            if (!richTextBox.IsDisposed)
            {
                if (richTextBox.InvokeRequired)
                {
                    MethodInvoker d = () =>
                    {                      
                        richTextBox.SelectionColor = Color.Black;
                       // if(text.Contains("UUT")
                        richTextBox.AppendText(string.Format("{0} {1}",text, System.Environment.NewLine));
                        richTextBox.SelectionStart = richTextBox.Text.Length;
                        richTextBox.SelectionLength = 0;
                        richTextBox.ScrollToCaret();
                        if (richTextBox.Lines.Length > 100)
                        {
                            string[] sLines = richTextBox.Lines;
                            string[] sNewLines = new string[sLines.Length - 1];
                            Array.Copy(sLines, 1, sNewLines, 0, sNewLines.Length);
                            richTextBox.Lines = sNewLines;
                        }
                    };
                    richTextBox.Invoke(d);
                }
                else
                {
                    richTextBox.SelectionColor = Color.Black;
                    richTextBox.AppendText(string.Format("{0} {1}", text, System.Environment.NewLine));
                    richTextBox.SelectionStart = richTextBox.Text.Length;
                    richTextBox.SelectionLength = 0;
                    richTextBox.ScrollToCaret();
                }
            }
        }


        public static void ShowTestLog0(string text)
        {
            if (!richTextBox.IsDisposed)
            {
                if (richTextBox.InvokeRequired)
                {
                    MethodInvoker d = () =>
                    {

                        if (richTextBox.Lines.Length > 100)
                        {
                            string[] sLines = richTextBox.Lines;
                            string[] sNewLines = new string[sLines.Length - 1];
                            Array.Copy(sLines, 1, sNewLines, 0, sNewLines.Length);
                            richTextBox.Lines = sNewLines;
                        }
                        richTextBox.SelectionColor = Color.Black;
                        richTextBox.AppendText(string.Format("{0} {1} {2}", DateTime.Now, text, System.Environment.NewLine));
                        richTextBox.SelectionStart = richTextBox.Text.Length;
                        richTextBox.SelectionLength = 0;
                        richTextBox.ScrollToCaret();
                    };
                    richTextBox.Invoke(d);
                }
                else
                {
                    richTextBox.SelectionColor = Color.Black;
                    richTextBox.AppendText(string.Format("{0} {1} {2}", DateTime.Now, text, System.Environment.NewLine));
                    richTextBox.SelectionStart = richTextBox.Text.Length;
                    richTextBox.SelectionLength = 0;
                    richTextBox.ScrollToCaret();
                }
            }
        }
    }
}
