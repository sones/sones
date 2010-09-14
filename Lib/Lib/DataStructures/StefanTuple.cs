///* GraphFS - Tupel
// * (c) Stefan Licht, 2009
// * 
// * This class provides you the logic of creating a typed tupel.
// * This could be used for a Method returning parameter or 
// * ParametrizedThread parameter.
// * 
// * Lead programmer:
// *      Stefan Licht
// * 
// * */
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace sones.Lib.DataStructures
//{

/// <summary>
/// Create a Tupel of 2 typed elements.
/// </summary>
/// <typeparam name="T1">The typed element 1.</typeparam>
/// <typeparam name="T2">The typed element 2.</typeparam>
public class StefanTuple<T1, T2>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }

    public StefanTuple(T1 myTupelElement1, T2 myTupelElement2)
    {
        Item1 = myTupelElement1;
        Item2 = myTupelElement2;
    }
}

/// <summary>
/// Create a Tupel of 3 typed elements.
/// </summary>
/// <typeparam name="T1">The typed element 1.</typeparam>
/// <typeparam name="T2">The typed element 2.</typeparam>
/// <typeparam name="T3">The typed element 3.</typeparam>
public class StefanTuple<T1, T2, T3>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
    public T3 Item3 { get; set; }

    public StefanTuple(T1 myTupelElement1, T2 myTupelElement2, T3 myTupelElement3)
    {
        Item1 = myTupelElement1;
        Item2 = myTupelElement2;
        Item3 = myTupelElement3;
    }
}

//    /// <summary>
//    /// Create a Tupel of 4 typed elements.
//    /// </summary>
//    /// <typeparam name="T1">The typed element 1.</typeparam>
//    /// <typeparam name="T2">The typed element 2.</typeparam>
//    /// <typeparam name="T3">The typed element 3.</typeparam>
//    /// <typeparam name="T4">The typed element 4.</typeparam>
//    public class Tuple<T1, T2, T3, T4>
//    {
//        public T1 TupelElement1 { get; set; }
//        public T2 TupelElement2 { get; set; }
//        public T3 TupelElement3 { get; set; }
//        public T4 TupelElement4 { get; set; }

//        public Tuple(T1 myTupelElement1, T2 myTupelElement2, T3 myTupelElement3, T4 myTupelElement4)
//        {
//            TupelElement1 = myTupelElement1;
//            TupelElement2 = myTupelElement2;
//            TupelElement3 = myTupelElement3;
//            TupelElement4 = myTupelElement4;
//        }
//    }

//    /// <summary>
//    /// Create a Tupel of 5 typed elements.
//    /// </summary>
//    /// <typeparam name="T1">The typed element 1.</typeparam>
//    /// <typeparam name="T2">The typed element 2.</typeparam>
//    /// <typeparam name="T3">The typed element 3.</typeparam>
//    /// <typeparam name="T4">The typed element 4.</typeparam>
//    /// <typeparam name="T5">The typed element 5.</typeparam>
//    public class Tuple<T1, T2, T3, T4, T5>
//    {
//        public T1 TupelElement1 { get; set; }
//        public T2 TupelElement2 { get; set; }
//        public T3 TupelElement3 { get; set; }
//        public T4 TupelElement4 { get; set; }
//        public T5 TupelElement5 { get; set; }

//        public Tuple(T1 myTupelElement1, T2 myTupelElement2, T3 myTupelElement3, T4 myTupelElement4, T5 myTupelElement5)
//        {
//            TupelElement1 = myTupelElement1;
//            TupelElement2 = myTupelElement2;
//            TupelElement3 = myTupelElement3;
//            TupelElement4 = myTupelElement4;
//            TupelElement5 = myTupelElement5;
//        }
//    }
//}
