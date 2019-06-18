using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.AboutSocket
{
    public class FormShow
    {
        public static void RichTextBoxColorChange(RichTextBox richTextBoxForm, string message)
        {
            RichTextBoxHasTheMessage(richTextBoxForm, message);
            //char[] msg = message.ToCharArray();

            //int len = richTextBoxForm.Find(msg,0,msg.Length);

            //for (int i = 0; i < list.Count; i++)
            //{
            //    int index = (int)list[i];
            //    richTextBoxForm.Select(index, "str1".Length);
            //    richTextBoxForm.SelectionColor = Color.Black;
            //    richTextBoxForm.SelectionBackColor = Color.DeepSkyBlue;
            //}
        }

        public static void RichTextBoxHasTheMessage(RichTextBox richTextBoxForm, String findStr)
        {
            int M_int_start;
            int M_int_index = 0, M_int_end=0;
            while (M_int_index != -1)
            {
                M_int_start = 0;
                M_int_end = richTextBoxForm.Text.Length;
                //find 不到文本就返回-1
                M_int_index = richTextBoxForm.Find(findStr, M_int_start, M_int_end, RichTextBoxFinds.None);
                if (M_int_index == -1)
                {
                    MessageBox.Show(
                        "没得啊", "？？？？",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                else
                {
                    richTextBoxForm.SelectionBackColor = Color.SeaGreen;
                    M_int_index += findStr.Length;
                }
            }
        }
    }
}
