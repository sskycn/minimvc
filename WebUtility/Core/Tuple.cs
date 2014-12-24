using System;

namespace Cvv.WebUtility.Core
{
    public class Tuple<T1, T2>
    {
        public Tuple()
        {
        }

        public Tuple(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public T1 Value1;
        public T2 Value2;
    }

    public class Tuple<T1, T2, T3>
    {
        public Tuple()
        {
        }

        public Tuple(T1 value1, T2 value2, T3 value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }

        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
    }

}