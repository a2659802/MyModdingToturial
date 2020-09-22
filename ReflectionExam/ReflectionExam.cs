using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Modding;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Reflection.Emit;
using JetBrains.Annotations;
using Vasi;
namespace ReflectionExam
{
    public class ReflectionExam : Mod//,ITogglableMod
    {
        public override void Initialize()
        {

            //ModdingReflectionHelperReadTest();
            //ModdingReflectionHelperWriteTest();
            //VasiMirrorReadTest();
            //VasiMirrorWriteTest();
            InvokeTest();
            //ActivatorTest();
        }
        public void Unload()
        {

        }

        public override string GetVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            string ver = asm.GetName().Version.ToString();

            using SHA1 sha1 = SHA1.Create();
            using FileStream stream = File.OpenRead(asm.Location);

            byte[] hashBytes = sha1.ComputeHash(stream);

            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            return $"{ver}-{hash.Substring(0, 6)}";
        }

        private void ModdingReflectionHelperReadTest()
        {
            Log("=========Reflection Read Start==========");
            InstanceClass instance = new InstanceClass();

            // get  StaticClass._static_field
            FieldInfo fi = ReflectionHelper.GetField(typeof(StaticClass), "_static_field", false);
            Log($"StaticClass._static_field is {fi.GetValue(null)}");

            // get  InstanceClass._static_field
                //first way
            fi = ReflectionHelper.GetField(typeof(InstanceClass), "_static_field",false);
            Log($"(First way)InstanceClass._static_field is {fi.GetValue(null)}");
                //second way
            /*string val = ReflectionHelper.GetAttr<InstanceClass, string>("_static_field");
            Log($"(second way)InstanceClass._static_field is {val}");*/

            // get InstanceClass._non_static_field
                //first way
            fi = ReflectionHelper.GetField(typeof(InstanceClass), "_non_static_field");
            Log($"(First way)InstanceClass._non_static_field is {fi.GetValue(instance)}");
                //second way
            int nonstatic = ReflectionHelper.GetAttr<InstanceClass, int>(instance,"_non_static_field");
            Log($"(second way)InstanceClass._non_static_field is {nonstatic}");

            Log("=========Reflection End==========");
        }
        
        private void ModdingReflectionHelperWriteTest()
        {
            Log("=========Reflection Write Start==========");
            InstanceClass instance = new InstanceClass();

            // set  StaticClass._static_field
            FieldInfo fi = ReflectionHelper.GetField(typeof(StaticClass), "_static_field", false);
            fi.SetValue(null, "modify static field in static");
            Log($"StaticClass._static_field is {fi.GetValue(null)}"); // verify change

            // set  InstanceClass._static_field
            fi = ReflectionHelper.GetField(typeof(InstanceClass), "_static_field",false);
            fi.SetValue(null, "modify static field in instance");
            Log($"(First way)InstanceClass._static_field is {fi.GetValue(null)}"); // verify

            // set InstanceClass._non_static_field
            //first way
            fi = ReflectionHelper.GetField(typeof(InstanceClass), "_non_static_field");
            fi.SetValue(instance, 1);
            Log($"(First way)InstanceClass._non_static_field is {fi.GetValue(instance)}");
            //second way
            ReflectionHelper.SetAttr<InstanceClass, int>(instance, "_non_static_field", 2);
            int nonstatic = ReflectionHelper.GetAttr<InstanceClass, int>(instance,"_non_static_field");
            Log($"(second way)InstanceClass._non_static_field is {nonstatic}");

            Log("=========Reflection End==========");
        }
        private void VasiMirrorReadTest()
        {
            Log("=========Mirror Read Start==========");
            InstanceClass instance = new InstanceClass();

            string f1 = Mirror.GetField<InstanceClass, string>("_static_field");
            Log($"InstanceClass._static_field is {f1}");

            int f2 = Mirror.GetField<InstanceClass, int>(instance,"_non_static_field");
            Log($"InstanceClass._non_static_field is {f2}");

            Log("=========Mirror End==========");
        }
        private void VasiMirrorWriteTest()
        {
            Log("=========Mirror Write Start==========");
            InstanceClass instance = new InstanceClass();

            //first way : use SetField
            Mirror.SetField<InstanceClass, string>("_static_field", "modify static in instance");
            string f1 = Mirror.GetField<InstanceClass, string>("_static_field");
            Log($"InstanceClass._static_field is {f1}");

            //second way : use ref
            ref string rf1 = ref Mirror.GetFieldRef<InstanceClass, string>("_static_field");
            rf1 = "(way2) modify static in instance";
            f1 = Mirror.GetField<InstanceClass, string>("_static_field");
            Log($"InstanceClass._static_field is {f1}");

            //first way : use SetField
            Mirror.SetField<InstanceClass, int>(instance, "_non_static_field", 1);
            int f2 = Mirror.GetField<InstanceClass, int>(instance, "_non_static_field");
            Log($"InstanceClass._non_static_field is {f2}");

            //second way : use ref
            ref int rf2 = ref Mirror.GetFieldRef<InstanceClass, int>(instance, "_non_static_field");
            rf2 = 2;
            f2 = Mirror.GetField<InstanceClass, int>(instance, "_non_static_field");
            Log($"InstanceClass._non_static_field is {f2}");

            Log("=========Mirror End==========");
        }
        private void ActivatorTest()
        {
            // this will get an array like this {typeof(RectShape),typeof(CicrleShape)}
            Type[] subclasses = typeof(Shapes).GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);

            foreach(Type t in subclasses)
            {
                object shape = Activator.CreateInstance(t); //CreateInstance(Type) like new Type()
            }
        }
        
        private void InvokeTest()
        {
            BindingFlags private_static_flags = BindingFlags.NonPublic | BindingFlags.Static;
            BindingFlags private_instance_flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo m;
            string strReturn;
            int intReturn;
            InstanceClass instance = new InstanceClass();

            // Invoke a static method in a static class (without arguments)
            m = typeof(StaticClass).GetMethod("_static_method", private_static_flags);
            intReturn = (int) m.Invoke(null, new object[] { });
            Log("StaticClass._static_method() return:" + intReturn);

            // Invoke a static method in a static class (with arguments)
            m = typeof(StaticClass).GetMethod("_static_method_with_arg", private_static_flags);
            strReturn = (string) m.Invoke(null, new object[] {"sawyer" });
            Log("StaticClass._static_method_with_arg(\"sawyer\") return:" + strReturn);

            // Invoke a static method in a instance class (without arguments)
            m = typeof(InstanceClass).GetMethod("_static_method", private_static_flags);
            intReturn = (int) m.Invoke(null, new object[] { });
            Log("InstanceClass._static_method() return:" + intReturn);

            // Invoke a instance method in a instance class (with arguments)
            m = typeof(InstanceClass).GetMethod("_non_static_method_with_arg", private_instance_flags);
            strReturn = (string) m.Invoke(instance, new object[] { "hollowknight" });
            Log("instance._non_static_method_with_arg(\"hollowknight\") return:" + strReturn);
        }

    }
    public static class StaticClass
    {
        private static string _static_field = "a static field in a static class";

        private static int _static_method()
        {
            return 26;
        }
        private static string _static_method_with_arg(string name)
        {
            return $"Hello, {name}";
        }
    }
    public class InstanceClass
    {
        private static string _static_field = "a static field in an instance class";
        private int _non_static_field = 59;
        private static int _static_method()
        {
            return 80;
        }
        private string _non_static_method_with_arg(string name)
        {
            return $"Hello, {name}";
        }
    }


}
