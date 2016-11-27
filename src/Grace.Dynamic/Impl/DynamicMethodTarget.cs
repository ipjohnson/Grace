using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Dynamic.Impl
{
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
    /// dynamic method target with two args
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
}
