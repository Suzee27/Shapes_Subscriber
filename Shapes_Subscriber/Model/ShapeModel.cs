using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Shapes_Publisher.Model
{
    internal class ShapeModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Type? ShapeType { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Brush? Stroke { get; set; }
    }
}
