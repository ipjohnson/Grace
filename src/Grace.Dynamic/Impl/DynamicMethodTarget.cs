namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// dynamic method target that uses array
    /// </summary>
    internal sealed class ArrayDynamicMethodTarget
    {
        public ArrayDynamicMethodTarget(object[] items)
        {
            Items = items;
        }

        /// <summary>
        /// array of constants
        /// </summary>
        public object[] Items;
    }

    /// <summary>
    /// Default dynamic method target
    /// </summary>
    internal sealed class DynamicMethodTarget
    {

    }

    /// <summary>
    /// Dynamic method target with one constant
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    internal sealed class DynamicMethodTarget<T1>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        public DynamicMethodTarget(T1 t1)
        {
            TArg1 = t1;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;
    }

    /// <summary>
    /// dynamic method target with two args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    internal sealed class DynamicMethodTarget<T1,T2>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public DynamicMethodTarget(T1 t1, T2 t2)
        {
            TArg1 = t1;
            TArg2 = t2;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public  T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;
    }


    /// <summary>
    /// dynamic method target with three args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    internal sealed class DynamicMethodTarget<T1, T2,T3>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        public DynamicMethodTarget(T1 t1, T2 t2, T3 t3)
        {
            TArg1 = t1;
            TArg2 = t2;
            TArg3 = t3;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;

        /// <summary>
        /// constnat three
        /// </summary>
        public T3 TArg3;
    }

    /// <summary>
    /// dynamic method target with four args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    internal sealed class DynamicMethodTarget<T1, T2, T3, T4>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        public DynamicMethodTarget(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            TArg1 = t1;
            TArg2 = t2;
            TArg3 = t3;
            TArg4 = t4;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;

        /// <summary>
        /// constnat three
        /// </summary>
        public T3 TArg3;

        /// <summary>
        /// constant four
        /// </summary>
        public T4 TArg4;
    }

    /// <summary>
    /// dynamic method target with five args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    internal sealed class DynamicMethodTarget<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        public DynamicMethodTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            TArg1 = t1;
            TArg2 = t2;
            TArg3 = t3;
            TArg4 = t4;
            TArg5 = t5;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;

        /// <summary>
        /// constnat three
        /// </summary>
        public T3 TArg3;

        /// <summary>
        /// constant four
        /// </summary>
        public T4 TArg4;

        /// <summary>
        /// constant five
        /// </summary>
        public T5 TArg5;
    }

    /// <summary>
    /// dynamic method target with six args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    internal sealed class DynamicMethodTarget<T1, T2, T3, T4, T5, T6>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        public DynamicMethodTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            TArg1 = t1;
            TArg2 = t2;
            TArg3 = t3;
            TArg4 = t4;
            TArg5 = t5;
            TArg6 = t6;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;

        /// <summary>
        /// constnat three
        /// </summary>
        public T3 TArg3;

        /// <summary>
        /// constant four
        /// </summary>
        public T4 TArg4;

        /// <summary>
        /// constant five
        /// </summary>
        public T5 TArg5;

        /// <summary>
        /// constant six
        /// </summary>
        public T6 TArg6;
    }

    /// <summary>
    /// dynamic method target with seven args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    internal sealed class DynamicMethodTarget<T1, T2, T3, T4, T5, T6, T7>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        public DynamicMethodTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            TArg1 = t1;
            TArg2 = t2;
            TArg3 = t3;
            TArg4 = t4;
            TArg5 = t5;
            TArg6 = t6;
            TArg7 = t7;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;

        /// <summary>
        /// constnat three
        /// </summary>
        public T3 TArg3;

        /// <summary>
        /// constant four
        /// </summary>
        public T4 TArg4;

        /// <summary>
        /// constant five
        /// </summary>
        public T5 TArg5;

        /// <summary>
        /// constant six
        /// </summary>
        public T6 TArg6;

        /// <summary>
        /// constant seven
        /// </summary>
        public T7 TArg7;
    }

    /// <summary>
    /// dynamic method target with seven args
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    internal sealed class DynamicMethodTarget<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        public DynamicMethodTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            TArg1 = t1;
            TArg2 = t2;
            TArg3 = t3;
            TArg4 = t4;
            TArg5 = t5;
            TArg6 = t6;
            TArg7 = t7;
            TArg8 = t8;
        }

        /// <summary>
        /// constant one
        /// </summary>
        public T1 TArg1;

        /// <summary>
        /// constant two
        /// </summary>
        public T2 TArg2;

        /// <summary>
        /// constnat three
        /// </summary>
        public T3 TArg3;

        /// <summary>
        /// constant four
        /// </summary>
        public T4 TArg4;

        /// <summary>
        /// constant five
        /// </summary>
        public T5 TArg5;

        /// <summary>
        /// constant six
        /// </summary>
        public T6 TArg6;

        /// <summary>
        /// constant seven
        /// </summary>
        public T7 TArg7;

        /// <summary>
        /// constant 8 
        /// </summary>
        public T8 TArg8;
    }
}
