using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NFRV_CourseProject
{
    public partial class ImageForm : Form
    {
        public ImageProcessor imageProcessor { get; }
        public EColor C { get; set; } = EColor.R;

        public ImageForm(EDitherCoefficient n)
        {
            InitializeComponent();
            imageProcessor = new ImageProcessor(n);
        }

        private void ApplyImageTo(Bitmap image, PictureBox PB_Image)
        {
            Debug.Assert(image != null);
            Debug.Assert(PB_Image != null);
            PB_Image.Size = new Size(image.Width, image.Height);
            PB_Image.Image = image;
        }

        public void LoadImage()
        {
            PB_Image_dithered.Visible = false;
            PB_Image_original.Visible = false;
            using (var fileDialog = new OpenFileDialog())
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    imageProcessor.LoadImage(fileDialog.FileName);
                    ApplyImageTo(imageProcessor.InMap, PB_Image_original);
                }

            PB_Image_dithered.Location = new Point(
                PB_Image_original.Location.X + PB_Image_original.Width + 30,
                PB_Image_dithered.Location.Y);
            Size = new Size(PB_Image_original.Width * 2 + 100,
                PB_Image_original.Height + 60);
            PB_Image_original.Visible = true;
        }

        public void DitheringImage()
        {
            Action a = new Action[]
            {
                imageProcessor.DitheringProcess_R,
                imageProcessor.DitheringProcess_G,
                imageProcessor.DitheringProcess_B,
                imageProcessor.DitheringProcess_A
            }[(int) C];
            a.Invoke();
            ApplyImageTo(imageProcessor.OutMap, PB_Image_dithered);
            PB_Image_dithered.Visible = true;
        }

        public bool ShouldDispose;

        protected override void Dispose(bool disposing)
        {
            if (!ShouldDispose)
            {
                SetVisibleCore(false);
                return;
            }

            if (disposing)
            {
                imageProcessor.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        public class ImageProcessor : IDisposable
        {
            internal Bitmap InMap { get; private set; }
            internal Bitmap OutMap { get; private set; }
            private int _countF;
            private int _countNf;
            private int _n;

            public EDitherCoefficient N
            {
                get => (EDitherCoefficient) _n;
                set
                {
                    _n = (int) value;
                    DitherMatrix = DitherMaker(_n);
                }
            }

            public int[,] DitherMatrix { get; private set; }
            public bool Reversed { get; set; }

            public ImageProcessor(EDitherCoefficient n)
            {
                N = n;
                DitherMatrix = DitherMaker(_n);
            }

            private int[,] DitherMaker(int n)
            {
                if (n == 2)
                {
                    return new[,]
                    {
                        {3, 1},
                        {0, 2}
                    };
                }

                if (n == 3)
                {
                    return new[,]
                    {
                        {7, 2, 6},
                        {4, 0, 1},
                        {3, 8, 5}
                    };
                }

                int[,] matr = new int[n, n];
                int[,] halfMatr = DitherMaker(n / 2);
                //kv1 (0,n/2) - (n/2,n)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i, j + n / 2] =
                            4 * halfMatr[i, j] + 1; //DithMatr2[0,1] = 1;
                    }
                }

                //kv2 (0,0) - (n/2,n/2)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i, j] =
                            4 * halfMatr[i, j] + 3; //DithMatr2[0,0] = 3;
                    }
                }

                //kv3 (n/2,0) - (n,n/2)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i + n / 2, j] =
                            4 * halfMatr[i, j] + 0; //DithMatr2[1,0] = 0;
                    }
                }

                //kv4 (n/2,n/2) - (n,n)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i + n / 2, j + n / 2] =
                            4 * halfMatr[i, j] + 2; //DithMatr2[1,1] = 2;
                    }
                }

                return matr;
            }

            private int Approxim(int intensity)
            {
                int n = _n * _n;
                for (int i = 0; i < n; i++)
                {
                    if (intensity >= i * (256.0 / n) &&
                        intensity < (i + 1) * (256.0 / n))
                    {
                        _countF++;
                        //Console.WriteLine("found");
                        if (Reversed)
                        {
                            return i;
                        }

                        return (n - 1) - i;
                    }
                }

                _countNf++;
                //Console.WriteLine("not found");
                return n - 1;
            }

            internal void LoadImage(string fileName)
            {
                try
                {
                    if (InMap != null) InMap.Dispose();
                    InMap = new Bitmap(Image.FromFile(fileName));
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }

            internal void DitheringProcess_R()
            {
                _countNf = 0;
                _countF = 0;
                Debug.Assert(InMap != null);
                OutMap?.Dispose();
                OutMap = new Bitmap(InMap);
                for (int i = 0; i < InMap.Width; i++)
                {
                    for (int j = 0; j < InMap.Height; j++)
                    {
                        var k = i % _n;
                        var m = j % _n;
                        var color =
                            Approxim(OutMap.GetPixel(i, j).R) <
                            DitherMatrix[k, m]
                                ? Color.White
                                : Color.Black;
                        OutMap.SetPixel(i, j, color);
                    }
                }

                Console.WriteLine("Count found=" + _countF);
                Console.WriteLine("Count not found=" + _countNf);
            }

            internal void DitheringProcess_B()
            {
                _countNf = 0;
                _countF = 0;
                Debug.Assert(InMap != null);
                OutMap?.Dispose();
                OutMap = new Bitmap(InMap);
                for (int i = 0; i < InMap.Width; i++)
                {
                    for (int j = 0; j < InMap.Height; j++)
                    {
                        var k = i % _n;
                        var m = j % _n;
                        var color =
                            Approxim(OutMap.GetPixel(i, j).B) <
                            DitherMatrix[k, m]
                                ? Color.White
                                : Color.Black;
                        OutMap.SetPixel(i, j, color);
                    }
                }

                Console.WriteLine("Count found=" + _countF);
                Console.WriteLine("Count not found=" + _countNf);
            }

            internal void DitheringProcess_G()
            {
                _countNf = 0;
                _countF = 0;
                Debug.Assert(InMap != null);
                OutMap?.Dispose();
                OutMap = new Bitmap(InMap);
                for (int i = 0; i < InMap.Width; i++)
                {
                    for (int j = 0; j < InMap.Height; j++)
                    {
                        var k = i % _n;
                        var m = j % _n;
                        var color =
                            Approxim(OutMap.GetPixel(i, j).G) <
                            DitherMatrix[k, m]
                                ? Color.White
                                : Color.Black;
                        OutMap.SetPixel(i, j, color);
                    }
                }

                Console.WriteLine("Count found=" + _countF);
                Console.WriteLine("Count not found=" + _countNf);
            }

            internal void DitheringProcess_A()
            {
                _countNf = 0;
                _countF = 0;
                Debug.Assert(InMap != null);
                OutMap?.Dispose();
                OutMap = new Bitmap(InMap);
                for (int i = 0; i < InMap.Width; i++)
                {
                    for (int j = 0; j < InMap.Height; j++)
                    {
                        var k = i % _n;
                        var m = j % _n;
                        var color =
                            Approxim(OutMap.GetPixel(i, j).A) <
                            DitherMatrix[k, m]
                                ? Color.White
                                : Color.Black;
                        OutMap.SetPixel(i, j, color);
                    }
                }

                Console.WriteLine("Count found=" + _countF);
                Console.WriteLine("Count not found=" + _countNf);
            }


            private bool _disposed;

            public void Dispose()
            {
                if (_disposed) return;

                _disposed = true;
                InMap?.Dispose();
                OutMap?.Dispose();
            }
        }
    }
}