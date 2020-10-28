using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NFRV_CourseProject
{
    public partial class ImageForm : Form
    {
        public ImageProcessor imageProcessor { get;private set; }
        public EColor C { get; set; } = EColor.R;
        public ImageForm(EDitherCoefficient n)
        {
            InitializeComponent();
            imageProcessor = new ImageProcessor(n);
        }

        private void ApplyImageTo(Bitmap image,PictureBox PB_Image)
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
                    ApplyImageTo(this.imageProcessor.InMap,PB_Image_original);
                }
            PB_Image_dithered.Location = new Point(PB_Image_original.Location.X + PB_Image_original.Width + 30, PB_Image_dithered.Location.Y);
            this.Size = new Size(PB_Image_original.Width * 2 + 100, PB_Image_original.Height + 60);
            PB_Image_original.Visible = true;
        }
        public void DitheringImage()
        {
            Action a=new Action[] { imageProcessor.DitheringProcess_R, imageProcessor.DitheringProcess_G, imageProcessor.DitheringProcess_B, imageProcessor.DitheringProcess_A, }[(int)C];
            a.Invoke();
            ApplyImageTo(imageProcessor.OutMap,PB_Image_dithered);
            PB_Image_dithered.Visible = true;
            
        }

        public bool shouldDispose;
        protected override void Dispose(bool disposing)
        {
            if (!shouldDispose)
            {
                this.SetVisibleCore(false);
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
            int countF = 0;
            int countNF = 0;
            private int n;
            public EDitherCoefficient N {
                get => (EDitherCoefficient)n;
                set { 
                    n = (int)value;
                    DitherMatrix = DitherMaker(n);
                }
            }
            public int[,] DitherMatrix {
                get ;
                private set;
            }
            public bool Reversed { get; set; } = false;
            public ImageProcessor(EDitherCoefficient N)
            {
                this.N = N;
                DitherMatrix = DitherMaker(n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write(DitherMatrix[i, j] + " ");
                    }
                    Console.WriteLine();
                }
            }

            private int[,] DitherMaker(int n)
            {
                if (n == 2)
                {
                    return new int[,] { { 3, 1 },
                                        { 0, 2 } };
                }
                if (n == 3)
                {
                    return new int[,] { { 7, 2, 6},
                                        { 4, 0, 1},
                                        { 3, 8, 5} };
                }
                int[,] matr = new int[n, n];
                int[,] halfMatr = DitherMaker(n / 2);
                //kv1 (0,n/2) - (n/2,n)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i, j + n / 2] = 4 * halfMatr[i, j] + 1;//DithMatr2[0,1] = 1;
                    }
                }
                //kv2 (0,0) - (n/2,n/2)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i, j] = 4 * halfMatr[i, j] + 3;//DithMatr2[0,0] = 3;
                    }
                }
                //kv3 (n/2,0) - (n,n/2)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i + n / 2, j] = 4 * halfMatr[i, j] + 0;//DithMatr2[1,0] = 0;
                    }
                }
                //kv4 (n/2,n/2) - (n,n)
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                    {
                        matr[i + n / 2, j + n / 2] = 4 * halfMatr[i, j] + 2;//DithMatr2[1,1] = 2;
                    }
                }
                return matr;
            }

            private int Approxim(int intensity)
            {
                int n = this.n * this.n;
                for (int i = 0; i < n; i++)
                {
                    if (intensity >= i * (256.0 / n) && intensity < (i + 1) * (256.0 / n))
                    {
                        countF++;
                        //Console.WriteLine("found");
                        if (Reversed)
                        {
                            return i;
                        }
                        return (n - 1) - i;
                    }
                }
                countNF++;
                //Console.WriteLine("not found");
                return n - 1;
            }
            internal void LoadImage(string fileName)
            {
                try
                {
                    if (this.InMap != null) this.InMap.Dispose();
                    this.InMap = new Bitmap(Image.FromFile(fileName));
                }
                catch (Exception error) { MessageBox.Show(error.Message); }
            }
            internal void DitheringProcess_R()
            {
                countNF = 0;
                countF = 0;
                int k, m;
                Debug.Assert(this.InMap != null);
                if (this.OutMap != null) this.OutMap.Dispose();
                this.OutMap = new Bitmap(this.InMap);
                for (int i = 0; i < this.InMap.Width; i++)
                {
                    for (int j = 0; j < this.InMap.Height; j++)
                    {
                        k = i % n;
                        m = j % n;
                        var color = Approxim(this.OutMap.GetPixel(i, j).R) < this.DitherMatrix[k, m] ? Color.White : Color.Black;
                        this.OutMap.SetPixel(i, j, color);
                    }
                }
                Console.WriteLine("Count found=" + countF);
                Console.WriteLine("Count not found=" + countNF);
            }
            internal void DitheringProcess_B()
            {
                countNF = 0;
                countF = 0;
                int k, m;
                Debug.Assert(this.InMap != null);
                if (this.OutMap != null) this.OutMap.Dispose();
                this.OutMap = new Bitmap(this.InMap);
                for (int i = 0; i < this.InMap.Width; i++)
                {
                    for (int j = 0; j < this.InMap.Height; j++)
                    {
                        k = i % n;
                        m = j % n;
                        var color = Approxim(this.OutMap.GetPixel(i, j).B) < this.DitherMatrix[k, m] ? Color.White : Color.Black;
                        this.OutMap.SetPixel(i, j, color);
                    }
                }
                Console.WriteLine("Count found=" + countF);
                Console.WriteLine("Count not found=" + countNF);
            }
            internal void DitheringProcess_G()
            {
                countNF = 0;
                countF = 0;
                int k, m;
                Debug.Assert(this.InMap != null);
                if (this.OutMap != null) this.OutMap.Dispose();
                this.OutMap = new Bitmap(this.InMap);
                for (int i = 0; i < this.InMap.Width; i++)
                {
                    for (int j = 0; j < this.InMap.Height; j++)
                    {
                        k = i % n;
                        m = j % n;
                        var color = Approxim(this.OutMap.GetPixel(i, j).G) < this.DitherMatrix[k, m] ? Color.White : Color.Black;
                        this.OutMap.SetPixel(i, j, color);
                    }
                }
                Console.WriteLine("Count found=" + countF);
                Console.WriteLine("Count not found=" + countNF);
            }
            internal void DitheringProcess_A()
            {
                countNF = 0;
                countF = 0;
                int k, m;
                Debug.Assert(this.InMap != null);
                if (this.OutMap != null) this.OutMap.Dispose();
                this.OutMap = new Bitmap(this.InMap);
                for (int i = 0; i < this.InMap.Width; i++)
                {
                    for (int j = 0; j < this.InMap.Height; j++)
                    {
                        k = i % n;
                        m = j % n;
                        var color = Approxim(this.OutMap.GetPixel(i, j).A) < this.DitherMatrix[k, m] ? Color.White : Color.Black;
                        this.OutMap.SetPixel(i, j, color);
                    }
                }
                Console.WriteLine("Count found=" + countF);
                Console.WriteLine("Count not found=" + countNF);
            }


            
            bool disposed;
            public void Dispose()
            {
                if (disposed) return;
                
                disposed = true;
                this.InMap?.Dispose();
                this.OutMap?.Dispose();
            }
        }
    }
}
