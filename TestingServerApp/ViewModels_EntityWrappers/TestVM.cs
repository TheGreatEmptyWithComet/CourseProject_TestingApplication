using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TestingServerApp
{
    public class TestVM : NotifyPropertyChangeHandler
    {
        public Test Model { get; set; }

        public int Id { get => Model.Id; }

        public string Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }
        public byte[] SetImage
        {
            set
            {
                if (Model.Image != value)
                {
                    Model.Image = value;
                    // Notify that property of type BitmapImage (for reading) is changed
                    NotifyPropertyChanged(nameof(GetImage));
                }
            }
        }
        public BitmapImage? GetImage
        {
            get => Model.Image != null ? ByteArrayToBitmapImage(Model.Image) : null;
        }
        public int QuestionsAmountForTest
        {
            get => Model.QuestionsAmountForTest;
            set
            {
                if (Model.QuestionsAmountForTest != value)
                {
                    Model.QuestionsAmountForTest = value;
                    NotifyPropertyChanged(nameof(QuestionsAmountForTest));
                }
            }
        }
        public int MinutsForTest
        {
            get => Model.MinutsForTest;
            set
            {
                if (Model.MinutsForTest != value)
                {
                    Model.MinutsForTest = value;
                    NotifyPropertyChanged(nameof(MinutsForTest));
                }
            }
        }
        public int MaxTestScores
        {
            get => Model.MaxTestScores;
            set
            {
                if (Model.MaxTestScores != value)
                {
                    Model.MaxTestScores = value;
                    NotifyPropertyChanged(nameof(MaxTestScores));
                }
            }
        }
        public TestCategoryVM TestCategory
        {
            get => new TestCategoryVM(Model.TestCategory);
            set
            {
                if (Model.TestCategory != value.Model)
                {
                    Model.TestCategory = value.Model;
                    NotifyPropertyChanged(nameof(TestCategory));
                }
            }
        }


        public TestVM(Test test)
        {
            Model = test;
        }


        private BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                // Width for best image quality when image is resized
                image.DecodePixelWidth = Properties.Settings.Default.TestImageWidth;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
