using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoPoints();
            //DoShapes();
            //DoIntArrays();
            //DoShapeArrays();
            //DoList();
            //DoSortedDictionary();
            //DoDictionary();
            //DoHashSet();
            //DoLinkedList();
            //DoCalculator();
            //DoConcurrentQueue();
        }

        static void DoConcurrentQueue()
        {
            var ShapeQueue = new System.Collections.Concurrent.ConcurrentQueue<int>();

            for (int i = 0; i < 1000; i++)
            {
                ShapeQueue.Enqueue(i);
            }

            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    //lock ((ShapeQueue as ICollection).SyncRoot)
                    {
                        int outint;
                        while (ShapeQueue.TryDequeue(out outint))
                        {
                            Console.WriteLine(System.Threading.Interlocked.Increment(ref count));
                            Console.WriteLine(outint);
                        }
                    }
                });
            }
        }

        static void DoCalculator()
        {
            Console.WriteLine("Enter expression expression with integers and operations +, -, *, / or ^ (enter blank link to quit):");
            do
            {
                var Expression = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(Expression))
                    break;
                try
                {
                    ProcessExpression(Expression);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
            while (true);


        }

        static void ProcessExpression(string Expression)
        {
            foreach (var c in Expression.ToCharArray())
            {
                if (c >= '0' && c <= '9')
                    ProcessDigit((int)c - (int)'0');
                else if (c == '.')
                    ProcessDecimalPoint();
                else if (c == '(' || c == ')')
                    ProcessParenthesis(c);
                else
                {
                    OpType Op = CharToOp(c);
                    if (Op != OpType.None)
                        ProcessOp(Op);
                    else
                        throw new ArgumentException("Not a valid expression character");
                }
            }

            if (_ResetValue)
                throw new ArgumentException();

            while (_CurrentOp != OpType.None)
            {
                _CurrentValue = PerformOp(_LastValue, _CurrentOp, _CurrentValue);

                if (_OpStack.Count() != 0)
                {
                    var StackedOp = _OpStack.Pop();
                    _LastValue = StackedOp.LastValue;
                    _CurrentOp = StackedOp.Op;
                }
                else
                    _CurrentOp = OpType.None;
            }
            _ResetValue = true;
            Console.WriteLine(_CurrentValue);
        }

        enum OpType
        {
            None,
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Power,
        }

        struct StackedOp
        {
            public OpType Op;
            public double LastValue;
        }

        static double _LastValue;
        static OpType _CurrentOp = OpType.None;
        static double _CurrentValue;
        static bool _ResetValue = true;
        static int _DecimalPlace = 1;

        static Stack<StackedOp> _OpStack = new Stack<StackedOp>();

        static void ProcessDigit(int digit)
        {
            if (_ResetValue == true)
            {
                _CurrentValue = digit;
                _DecimalPlace = 0;
            }
            else
            {
                if (_DecimalPlace == 0)
                {
                    _CurrentValue *= 10;
                    _CurrentValue += digit;
                }
                else
                {
                    _CurrentValue += Math.Pow(10, -_DecimalPlace) * digit;
                    _DecimalPlace++;
                }
            }
            _ResetValue = false;
        }

        static void ProcessOp(OpType NewOp)
        {
            if (_CurrentOp != OpType.None)
            {
                int CompareResult = CompareOpPrecedence(NewOp, _CurrentOp);
                if (CompareResult > 0)
                {
                    // Push Op
                    _OpStack.Push(new StackedOp() { Op = _CurrentOp, LastValue = _LastValue });
                }
                else if (CompareResult <= 0)
                {
                    _CurrentValue = PerformOp(_LastValue, _CurrentOp, _CurrentValue);
                }
            }

            _LastValue = _CurrentValue;
            _CurrentOp = NewOp;
            _ResetValue = true;
        }

        static void ProcessDecimalPoint()
        {
            if (_DecimalPlace != 0)
                throw new InvalidOperationException("Can't have two decimal points");
            _DecimalPlace = 1;
        }

        static void ProcessParenthesis(char parenthesis)
        {
            if (parenthesis == '(')
            {
                if (_ResetValue == false)
                    throw new InvalidOperationException("Can only open parentheses where number would go");

                _OpStack.Push(new StackedOp() { Op = _CurrentOp, LastValue = _LastValue });
                _CurrentOp = OpType.None;
            }
            else
            {
                if (_ResetValue == true)
                    throw new InvalidOperationException("Can't only close parentheses at end of number");

                if (_OpStack.Count() == 0)
                    throw new InvalidOperationException("Can't close parentheses when there are no more open");


                var StackedOp = _OpStack.Pop();

                if (_CurrentOp != OpType.None)
                    _CurrentValue = PerformOp(_LastValue, _CurrentOp, _CurrentValue);

                _LastValue = StackedOp.LastValue;
                _CurrentOp = StackedOp.Op;
            }
        }

        static OpType CharToOp(char op)
        {
            switch (op)
            {
                case '+':
                    return OpType.Addition;
                case '-':
                    return OpType.Subtraction;
                case '*':
                    return OpType.Multiplication;
                case '/':
                    return OpType.Division;
                case '^':
                    return OpType.Power;
                default:
                    return OpType.None;
            }
        }

        static double PerformOp(double Left, OpType Op, double Right)
        {
            switch (Op)
            {
                case OpType.Addition:
                    return Left + Right;
                case OpType.Subtraction:
                    return Left - Right;
                case OpType.Multiplication:
                    return Left * Right;
                case OpType.Division:
                    return Left / Right;
                case OpType.Power:
                    return Math.Pow(Left, Right);
                default:
                    throw new InvalidOperationException("Don't know how to perform operation type");
            }
        }

        static int CompareOpPrecedence(OpType New, OpType Old)
        {
            int NewLevel = GetOpPrecedenceLevel(New);
            int OldLevel = GetOpPrecedenceLevel(Old);

            if (NewLevel > OldLevel)
                return 1;
            else if (NewLevel < OldLevel)
                return -1;
            else
                return 0;
        }

        static int GetOpPrecedenceLevel(OpType Op)
        {
            switch (Op)
            {
                case OpType.Addition:
                case OpType.Subtraction:
                    return 0;
                case OpType.Multiplication:
                case OpType.Division:
                    return 1;
                case OpType.Power:
                    return 2;
            }
            throw new ArgumentException("Can't find precendence level for operation type");
        }

        static void DoLinkedList()
        {
            double[] ints = { 1, 2, 3 };
            var LL = new LinkedList<double>(ints);

            var Node = LL.First.Next;
            LL.AddAfter(Node, 2.5);


            foreach (var N in LL)
                Console.WriteLine(N);

        }

        static void DoSortedDictionary()
        {
            var DefaultShapes = new SortedDictionary<string, Shape>();
            DefaultShapes.Add("triangle", new Triangle(2, 3));
            DefaultShapes.Add("square", new Square(3));

            foreach (var S in DefaultShapes)
                Console.WriteLine(S.Key);

        }

        static void DoHashSet()
        {
            int[] a = { 1, 2, 3, 4 };
            var SetA = new HashSet<int>(a);
            var SetB = new HashSet<int> { 4, 5, 6, 7 };

            if (SetA.Overlaps(SetB))
                Console.WriteLine("Overlaps");
            else
                Console.WriteLine("No overlap");

            SetA.ExceptWith(SetB);
            foreach (var m in SetA)
                Console.WriteLine(m);
        }

        static void DoDictionary()
        {
            var DefaultShapes = new Dictionary<string, Shape>();
            DefaultShapes.Add("triangle", new Triangle(2, 3));
            DefaultShapes.Add("square", new Square(3));
            //var RNG = new Random();
            //var DefaultShapes = new Dictionary<string, Func<Shape>>();
            //DefaultShapes.Add("square", () => new Square(RNG.Next(2) + 1));
            //DefaultShapes.Add("triangle", () => new Triangle(RNG.Next(2) + 1, RNG.Next(2) + 1));

            do
            {
                var ShapeName = Console.ReadLine();
                if (ShapeName.ToLower().Equals("exit")) break;

                if (DefaultShapes.ContainsKey(ShapeName))
                {
                    var Shape = DefaultShapes[ShapeName];
                    //var Shape = DefaultShapes[ShapeName]();

                    Console.WriteLine(Shape.Area());
                }
            }
            while (true);


        }

        static void DoList()
        {
            var Squares = RandomSquareGenerator.GeneratorSquares();
        }

        class RandomSquareGenerator
        {
            public static ICollection<Shape> GeneratorSquares()
            {
                var L = new List<Shape>();
                var RNG = new Random();
                var Num = RNG.Next(9) + 1;

                for (int i = 0; i < Num; i++)
                    L.Add(new Square(RNG.Next(2) + 1));

                return new ShapeCollection(L);
            }
        }

        class ShapeCollection : ReadOnlyCollection<Shape>
        {
            //public ShapeCollection()
            //    : base()
            //{
            //}

            public ShapeCollection(IList<Shape> List)
                : base(List)
            {
            }

        }

        class Shape
        {
            public virtual string Name
            {
                get
                {
                    return "Unknown";
                }
            }
            public virtual int NumSides { get { throw new InvalidOperationException(); } }

            private double _Area = 0;
            public virtual double Area()
            {
                return _Area;
            }

            public static Shape operator +(Shape a, Shape b)
            {
                var NewShape = new Shape();
                NewShape._Area = a.Area() + b.Area();
                return NewShape;
            }

            public override string ToString()
            {
                return Area().ToString();
            }
        }

        static void DoShapes()
        {
            var Shape = new Shape();

            Shape Triangle = new Triangle(2, 2);
            var Square = new Square(2);
            var NewShape = Triangle + Square;

            Console.WriteLine(NewShape);
        }

        class Square : Shape
        {
            double _Size;

            public Square(double Size)
            {
                _Size = Size;
            }

            public override string Name
            {
                get
                {
                    return "Square";
                }
            }

            public override int NumSides
            {
                get { return 4; }
            }

            public override double Area()
            {
                return _Size * _Size;
            }
        }

        class Rectangle : Shape
        {
            double _Width;
            double _Height;

            public Rectangle(double Width, double Height)
            {
                _Width = Width;
                _Height = Height;
            }

            public override string Name
            {
                get
                {
                    return "Rectangle";
                }
            }

            public override int NumSides
            {
                get { return 4; }
            }

            public override double Area()
            {
                return _Width * _Height;
            }
        }

        class Triangle : Shape
        {
            double _Base;
            double _Height;

            public Triangle(double Base, double Height)
            {
                _Base = Base;
                _Height = Height;
            }

            public override string Name
            {
                get
                {
                    return "Triangle";
                }
            }

            public override int NumSides
            {
                get { return 3; }
            }

            public override double Area()
            {
                return _Base * _Height / 2;
            }
        }

        struct Point
        {
            public double X;
            public double Y;

            public Point(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }

            public static Point operator +(Point a, Point b)
            {
                return new Point(a.X + b.X, a.Y + b.Y);
            }

            public double DistanceFrom(Point that)
            {
                return Math.Sqrt(Math.Pow(this.X - that.X, 2) + Math.Pow(this.Y - that.Y, 2));
            }


            public override string ToString()
            {
                return String.Format("{0},{1}", this.X, this.Y);
            }
        }

        static void DoPoints()
        {
            Point a = new Point(1, 2);
            var b = new Point(4, 6);
            Console.WriteLine(a.DistanceFrom(b));
        }

        static void DoIntArrays()
        {
            var IDs1 = new int[] { 1, 2, 3, 4, 5 };
            var IDs2 = new int[5] { 1, 2, 3, 4, 5 };

            int[] IDs3 = { 1, 2, 3, 4, 5 };
            int[] IDs4 = new int[] { 1, 2, 3, 4, 5 };
            int[] IDs5 = new int[5] { 1, 2, 3, 4, 5 };

            var BlankInts = new int[5];

            int[,] IDGrid = { { 1, 2 }, { 3, 4 } };
            var SingleID = IDGrid[0, 0];

            for (int x = 0; x < IDGrid.GetLength(0); x++)
            {
                for (int y = 0; y < IDGrid.GetLength(1); y++)
                    Console.WriteLine(IDGrid[x, y]);
            }

            int[][] IDRows = { new int[] { 1, 2, 3 }, new int[] { 4, 5 } };
            var LengthRow1 = IDRows[0].Length;
            IDRows[0][1] = 12;


            Console.WriteLine(IDs1.Rank); // No of dimensions that array has-1
            Console.WriteLine(IDs1.Length); // 5
            Console.WriteLine(IDs1.GetLength(0)); // if more than two dimensions
            Console.WriteLine();

            for (int i = 0; i < IDs1.Length; i++)
                Console.WriteLine(IDs1[i]);
            Console.WriteLine();

            foreach (var ID in IDs1)
                Console.WriteLine(ID);
            // Systerm.Array - All arrays derived from
            //In case of resizing the array size
            //System.Array.Resize<int>(ref IDs1,100) - It copies the old array with a new array
        }

        static void DoShapeArrays()
        {
            Shape[] Shapes = { new Square(3), new Rectangle(2, 3) };


            for (int i = 0; i < Shapes.Length; i++)
                Console.WriteLine(Shapes[i].Name);

            Shape[] Shapes2 = { new Square(5) };

            foreach (var S in Shapes)
                Console.WriteLine(S);

            Array.Resize(ref Shapes, 10);

            Shape[,] ShapeGrid = { { new Square(3), new Square(4) }, { new Square(2), new Square(5) } };
            foreach (var S in ShapeGrid)
                Console.WriteLine(S);

            Shape[][] ShapeLines = { new Shape[] { new Square(3) }, new Shape[] { new Square(2), new Square(5) } };
            foreach (var S in ShapeLines)
                Console.WriteLine(S);

        }
    }
}
