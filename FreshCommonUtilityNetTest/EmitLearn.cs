#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest
//文件名称：EmitLearn
//创 建 人：FreshMan
//创建日期：2017/6/30 23:30:03
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FreshCommonUtilityNetTest
{
    public class Fibonacci
    {
        public int Calc(int num)
        {
            if (num == 1 || num == 2)
            {
                return 1;
            }
            else
            {
                return Calc(num - 1) + Calc(num - 2);
            }
        }
    }

    public class Add
    {
        private int _a;
        public int A { get { return _a; } set { _a = value; } }

        private int _b;
        public int B { get { return _b; } set { _b = value; } }

        public Add(int a, int b)
        {
            _a = a;
            _b = b;
        }

        public int Calc()
        {
            return _a + _b;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EmitLearn
    {
        public static void LearnInfo()
        {
            Helloworld();
            CreateAssembly();
            AddMethod();
        }

        #region [1、Hello World]

        /// <summary>
        /// 用来调用动态方法的委托
        /// </summary>
        private delegate void HelloWorldDelegate();

        /// <summary>
        /// hello world 方法
        /// </summary>
        private static void Helloworld()
        {
            //定义一个名为HellWorld的动态方法，没有返回值，没有参数
            DynamicMethod helloWorldMethod = new DynamicMethod("HelloWorld", null, null);
            //创建一个MSIL生成器，为动态方法生成代码
            ILGenerator helloWorldIL = helloWorldMethod.GetILGenerator();
            //将要输出的HelloWorld!字符串加载到堆栈上
            helloWorldIL.Emit(OpCodes.Ldstr, "Hello World!");
            //调用Console.WreiteLine(string)方法输出Hello World!
            helloWorldIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            //方法结束，返回
            helloWorldIL.Emit(OpCodes.Ret);

            //完成动态方法的创建，并且获取一个可以执行该动态方法的委托
            HelloWorldDelegate HelloWorld =
                (HelloWorldDelegate)helloWorldMethod.CreateDelegate(typeof(HelloWorldDelegate));

            //执行动态方法，将在屏幕上打印Hello World!
            HelloWorld();
        }
        #endregion

        #region [2、构建程序集]

        private static void CreateAssembly()
        {
            string name = "EmitExamples.DynamicFibonacci";
            string asmFileName = name + ".dll";

            #region Step 1 构建程序集

            //创建程序集名
            AssemblyName asmName = new AssemblyName(name);

            //获取程序集所在的应用程序域
            //你也可以选择用AppDomain.CreateDomain方法创建一个新的应用程序域
            //这里选择当前的应用程序域
            AppDomain domain = AppDomain.CurrentDomain;

            //实例化一个AssemblyBuilder对象来实现动态程序集的构建
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            #endregion

            #region Step 2 定义模块

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name, asmFileName);

            #endregion

            #region Step 3 定义类型

            TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);

            #endregion

            #region Step 4 定义方法

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "Calc",
                MethodAttributes.Public,
                typeof(Int32),
                new Type[] { typeof(Int32) });

            #endregion

            #region Step 5 实现方法

            ILGenerator calcIL = methodBuilder.GetILGenerator();

            //定义标签lbReturn1，用来设置返回值为1
            Label lbReturn1 = calcIL.DefineLabel();
            //定义标签lbReturnResutl，用来返回最终结果
            Label lbReturnResutl = calcIL.DefineLabel();

            //加载参数1，和整数1，相比较，如果相等则设置返回值为1
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_1);
            calcIL.Emit(OpCodes.Beq_S, lbReturn1);

            //加载参数1，和整数2，相比较，如果相等则设置返回值为1
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_2);
            calcIL.Emit(OpCodes.Beq_S, lbReturn1);

            //加载参数0和1，将参数1减去1，递归调用自身
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_1);
            calcIL.Emit(OpCodes.Sub);
            calcIL.Emit(OpCodes.Call, methodBuilder);

            //加载参数0和1，将参数1减去2，递归调用自身
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_2);
            calcIL.Emit(OpCodes.Sub);
            calcIL.Emit(OpCodes.Call, methodBuilder);

            //将递归调用的结果相加，并返回
            calcIL.Emit(OpCodes.Add);
            calcIL.Emit(OpCodes.Br, lbReturnResutl);

            //在这里创建标签lbReturn1
            calcIL.MarkLabel(lbReturn1);
            calcIL.Emit(OpCodes.Ldc_I4_1);

            //在这里创建标签lbReturnResutl
            calcIL.MarkLabel(lbReturnResutl);
            calcIL.Emit(OpCodes.Ret);

            #endregion

            #region Step 6 收获

            Type type = typeBuilder.CreateType();
            assemblyBuilder.Save(asmFileName);
            object ob = Activator.CreateInstance(type);
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine(type.GetMethod("Calc").Invoke(ob, new object[] { i }));
            }

            #endregion
        }
        #endregion

        #region [3、Add method]

        /// <summary>
        /// Add method
        /// </summary>
        private static void AddMethod()
        {
            string name = "EmitExamples.DynamicAdd";
            string asmFileName = name + ".dll";
            #region step1 构建程序集
            //创建程序集名
            AssemblyName asmName = new AssemblyName(name);

            //获取程序集所在的应用程序域
            AppDomain domain = AppDomain.CurrentDomain;

            //实例化一个AssemblyBuilder对象来实现动态程序的构建
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            #endregion

            #region step2 定义模块

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name, asmFileName);

            #endregion

            #region step3 定义类型

            TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);

            #endregion

            #region step4 定义属性
            //定义私有字段_a和_b
            FieldBuilder fieldABuilder = typeBuilder.DefineField("_a", typeof(Int32), FieldAttributes.Private);
            FieldBuilder fieldBBuilder = typeBuilder.DefineField("_b", typeof(Int32), FieldAttributes.Private);
            fieldABuilder.SetConstant(0);
            fieldBBuilder.SetConstant(0);

            //定义共有属性A和B
            PropertyBuilder propertyABuilder = typeBuilder.DefineProperty("A", PropertyAttributes.None, typeof(Int32),
                null);
            PropertyBuilder propertyBBuilder = typeBuilder.DefineProperty("B", PropertyAttributes.None, typeof(Int32),
                null);

            #endregion

            #region step5 定义属性A的get和set方法
            //定义属性A的get方法
            MethodBuilder getPropertyABuilder = typeBuilder.DefineMethod("get",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(Int32),
                Type.EmptyTypes);

            //生成属性A的get方法的IL代码，返回私有字段_a
            ILGenerator getAIL = getPropertyABuilder.GetILGenerator();
            getAIL.Emit(OpCodes.Ldarg_0);
            getAIL.Emit(OpCodes.Ldfld, fieldABuilder);
            getAIL.Emit(OpCodes.Ret);

            //定义属性A的set方法
            MethodBuilder setPropertyABuilder = typeBuilder.DefineMethod("set",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null,
                new Type[] { typeof(Int32) });

            //生成属性A的set方法的IL代码，即设置私有字段_a值为传入的参数1的值
            ILGenerator setAIL = setPropertyABuilder.GetILGenerator();

            setAIL.Emit(OpCodes.Ldarg_0);
            setAIL.Emit(OpCodes.Ldarg_1);
            setAIL.Emit(OpCodes.Stfld, fieldABuilder);
            setAIL.Emit(OpCodes.Ret);

            //设置属性A的get和set方法
            propertyABuilder.SetGetMethod(getPropertyABuilder);
            propertyABuilder.SetGetMethod(setPropertyABuilder);
            #endregion

            #region step6 定义属性B的get和set方法
            //定义属性A的get方法
            MethodBuilder getPropertyBBuilder = typeBuilder.DefineMethod("get",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(Int32),
                Type.EmptyTypes);

            //生成属性B的get方法的IL代码，返回私有字段_b
            ILGenerator getBIL = getPropertyBBuilder.GetILGenerator();
            getBIL.Emit(OpCodes.Ldarg_0);
            getBIL.Emit(OpCodes.Ldfld, fieldBBuilder);
            getBIL.Emit(OpCodes.Ret);

            //定义属性B的set方法
            MethodBuilder setPropertyBBuilder = typeBuilder.DefineMethod("set",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null,
                new Type[] { typeof(Int32) });

            //生成属性B的set方法的IL代码，即设置私有字段_b值为传入的参数1的值
            ILGenerator setBIL = setPropertyBBuilder.GetILGenerator();

            setBIL.Emit(OpCodes.Ldarg_0);
            setBIL.Emit(OpCodes.Ldarg_1);
            setBIL.Emit(OpCodes.Stfld, fieldBBuilder);
            setBIL.Emit(OpCodes.Ret);

            //设置属性B的get和set方法
            propertyBBuilder.SetGetMethod(getPropertyBBuilder);
            propertyBBuilder.SetGetMethod(setPropertyBBuilder);
            #endregion

            #region setp7 定义构造函数

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.HasThis, new Type[] { typeof(Int32), typeof(Int32) });
            ILGenerator ctorIL = constructorBuilder.GetILGenerator();

            //加载参数1填充到私有字段_a
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, fieldABuilder);
            //加载参数2填充到私有字段_b
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_2);
            ctorIL.Emit(OpCodes.Stfld, fieldBBuilder);
            ctorIL.Emit(OpCodes.Ret);

            #endregion

            #region step8 定义方法

            MethodBuilder calcMethodBuilder = typeBuilder.DefineMethod("Calc", MethodAttributes.Public, typeof(Int32),
                Type.EmptyTypes);

            ILGenerator calcIL = calcMethodBuilder.GetILGenerator();

            //加载私有字段_a
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldfld, fieldABuilder);
            //加载私有字段_b
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldfld, fieldBBuilder);
            //相加并返回结果
            calcIL.Emit(OpCodes.Add);
            calcIL.Emit(OpCodes.Ret);
            #endregion

            #region Step9 收获

            Type type = typeBuilder.CreateType();

            int a = 2;
            int b = 3;

            Object ob = Activator.CreateInstance(type, new object[] { a, b });

            Console.WriteLine("The Result of {0} + {1} is {2}.", a, b, ob.GetType().GetMethod("Calc").Invoke(ob, null));

            assemblyBuilder.Save(asmFileName);

            #endregion
        }
        #endregion
    }
}
