using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFRV_CourseProject
{
    public partial class CustomizeForm : Form
    {
        ImageForm imageForm;
        public CustomizeForm()
        {
            InitializeComponent();
            imageForm=new ImageForm(EDitherCoefficient.n16);
            Coefficient.SelectedIndex = 4;
            Coefficient.SelectedIndex = 2;

        }

        private void RedButton_CheckedChanged(object sender, EventArgs e)
        {
            imageForm.C = EColor.R;
        }

        private void GreenButton_CheckedChanged(object sender, EventArgs e)
        {
            imageForm.C = EColor.G;
        }

        private void BlueButton_CheckedChanged(object sender, EventArgs e)
        {
            imageForm.C = EColor.B;
        }

        private void AlphaButton_CheckedChanged(object sender, EventArgs e)
        {
            imageForm.C = EColor.A;
        }

        private void Coefficient_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageForm.imageProcessor.N=(EDitherCoefficient)int.Parse((string)Coefficient.SelectedItem);
            redesignTable((int)imageForm.imageProcessor.N);
        }

        private void redesignTable(int n)
        {
            DitherTable.Height = n * 40;
            DitherTable.Width = n * 40;
            DitherTable.ColumnCount = n;
            DitherTable.RowCount = n;
            

            //for (; DitherTable.Controls.Count > 0;)
            //{
            //    DitherTable.Controls.RemoveAt(0);
            //}
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (DitherTable.GetControlFromPosition(j, i) == null)
                    {
                        Label lb = new Label
                        {
                            Text = imageForm.imageProcessor.DitherMatrix[i, j].ToString(),
                            Width = 60
                        };
                        DitherTable.Controls.Add(lb);
                        DitherTable.SetColumn(lb, j);
                        DitherTable.SetRow(lb, i);
                    }
                    else
                    {
                        DitherTable.GetControlFromPosition(j, i).Text = imageForm.imageProcessor.DitherMatrix[i, j].ToString();
                    }
                }

            }
        }

        private void OnLoadButton(object sender, EventArgs e)
        {
            imageForm.LoadImage();
            imageForm.Show();
        }

        private void OnDitherButton(object sender, EventArgs e)
        {
            imageForm.DitheringImage();
            imageForm.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                imageForm.shouldDispose = true;
                imageForm.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Reverse_CheckedChanged(object sender, EventArgs e)
        {
            imageForm.imageProcessor.Reversed = !imageForm.imageProcessor.Reversed;
        }
    }
}
