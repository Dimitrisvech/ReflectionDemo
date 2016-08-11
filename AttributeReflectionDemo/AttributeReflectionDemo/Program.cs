using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AttributeReflectionDemo
{
    class MyTestAttribute : Attribute { }
    class MyTestMethodAttribute : Attribute { }

    [MyTest]
    class MyTestSuite1
    {
        public void HelperPublicMethod()
        {
            Console.WriteLine("Helper public Method here.");
        }

        private void HelperPrivateMethod1()
        {
            Console.WriteLine("PRIVATE Helper Method here. Hello!");
        }

        private void HelperPrivateMethod2()
        {
            Console.WriteLine("Second PRIVATE Helper Method here. Hello again!");
        }

        public static void StaticMethod()
        {
            Console.WriteLine("This is a Static method! o.O");
        }

        [MyTestMethod]
        public void TestMethod1()
        {
            Console.WriteLine("Test method 1.");
        }

        [MyTestMethod]
        public void SomeOtherName()
        {
            Console.WriteLine("Test Method 2.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var testSuites =
                from t in Assembly.GetExecutingAssembly().GetTypes() //Got all classes in the current assembly
                where t.GetCustomAttributes().Any(a => a is MyTestAttribute)//Sort by attribute
                select t;

            foreach (var type in testSuites)
            {
                Console.WriteLine("Running tests in suite: " + type.Name);
                Console.WriteLine();
                object testSuiteInstance = Activator.CreateInstance(type);
                Console.WriteLine("*********************");
                Console.WriteLine("** All methods run **");
                Console.WriteLine("*********************");
                Console.WriteLine();

                IEnumerable<MethodInfo> testMethods = type.GetMethods();//Got (almost) all methods in the class
                //Invoke
                foreach (var mInfo in testMethods)
                {
                    try
                    {
                        mInfo.Invoke(testSuiteInstance, new object[0]);
                    }
                    catch (Exception)
                    {

                        Console.WriteLine("Exception was trown.");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("***************************");
                Console.WriteLine("** Attribute methods run **");
                Console.WriteLine("***************************");
                Console.WriteLine();

                testMethods = from m in type.GetMethods() //Got (almost) all methods in the class
                              where m.GetCustomAttributes().Any(a => a is MyTestMethodAttribute)//Sort by attribute
                              select m;
                //Invoke
                foreach (var mInfo in testMethods)
                {
                    mInfo.Invoke(testSuiteInstance, new object[0]);
                }
                //##################################################################################################
                Console.WriteLine();
                Console.WriteLine("*************************");
                Console.WriteLine("** Private methods run **");
                Console.WriteLine("*************************");
                Console.WriteLine();
                //A call by name:
                var privateMethod = type.GetMethod("HelperPrivateMethod1", BindingFlags.NonPublic | BindingFlags.Instance); //A private, non-static method.
                //Invoke
                privateMethod.Invoke(testSuiteInstance, new object[0]);
                //##################################################################################################
                //Find all private methods:
                testMethods = from m in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance) //Got all methods in the class, Include private and non-static.
                              where m.IsPrivate //Keep private
                              select m;
                //Invoke
                foreach (var mInfo in testMethods)
                {
                    mInfo.Invoke(testSuiteInstance, new object[0]);
                }
                //##################################################################################################
                Console.WriteLine();
                Console.WriteLine("***********************");
                Console.WriteLine("** Static method run **");
                Console.WriteLine("***********************");
                Console.WriteLine();
                //A call by name:
                var staticMethod = type.GetMethod("StaticMethod"); //A static method.
                //Invoke
                staticMethod.Invoke(null, new object[0]);
            }
            Console.ReadKey();
        }
    }
}
