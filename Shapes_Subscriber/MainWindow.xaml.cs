using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MSMQ.Messaging;
using Newtonsoft.Json;
using Shapes_Publisher.Model;

namespace Shapes_Subscriber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Subcribe();
        }

        public void Subcribe()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!MessageQueue.Exists(@".\private$\MSMQ_MessagingApp")) return;
                MessageQueue queue = new MessageQueue(@".\private$\MSMQ_MessagingApp");
                queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnRecieveCompleted);
                queue.BeginReceive();
            });
        }

        public void OnRecieveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageQueue queue = (MessageQueue)sender;
                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                Message message = queue.EndReceive(e.AsyncResult);
                var shapeData = JsonConvert.DeserializeObject<ShapeModel>((string)message.Body);

                if (shapeData == null) return;
                Shape newShape = CreateShape(shapeData);
                if (newShape == null) return;

                Canvas.SetLeft(newShape, shapeData.X);
                Canvas.SetTop(newShape, shapeData.Y);
                canvas.Children.Add(newShape);

                queue.BeginReceive();
            });
        }

        private Shape CreateShape(ShapeModel shapeData)
        {
            Shape? newShape = Activator.CreateInstance(shapeData.ShapeType) as Shape;
            if (newShape == null) return null;

            newShape.Width = shapeData.Width;
            newShape.Height = shapeData.Height;
            newShape.Stroke = shapeData.Stroke;
            Size size = new Size(shapeData.Width, shapeData.Height);
            newShape.Measure(size);
            newShape.Arrange(new Rect(size));
            newShape.UpdateLayout();
            return newShape;
        }
    }
}

