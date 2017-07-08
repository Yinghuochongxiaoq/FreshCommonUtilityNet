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
using System.Threading;

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

    class Animal
    {
        public virtual void Speak()
        {
            Console.WriteLine("Animal.Speak");
        }
    }

    class Cat : Animal
    {
        public override void Speak()
        {
            Console.WriteLine("Cat.Speak");
        }
    }

    class Dog : Animal
    {
        public override void Speak()
        {
            Console.WriteLine("Dog.Speak");
        }
    }

    class Iterator
    {
        public int ForMethod(int[] ints)
        {
            int sum = 0;
            for (int i = 0; i < ints.Length; i++)
            {
                sum += ints[i];
            }
            return sum;
        }

        public int ForeachMethod(int[] ints)
        {
            int sum = 0;
            foreach (int i in ints)
            {
                sum += i;
            }
            return sum;
        }
    }

    class ExceptionHandler
    {
        public static int ConvertToInt32(string str)
        {
            int num = 0;
            try
            {
                num = Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return num;
        }
    }

    public class RandGeneratedEventArgs : EventArgs
    {
        private int _rand;
        public int Rand
        {
            get { return _rand; }
        }

        public RandGeneratedEventArgs(int rand)
        {
            _rand = rand;
        }
    }


    public sealed class Publisher
    {
        private bool isStart = false;
        private Random random = new Random(DateTime.Now.Millisecond);

        public void Start()
        {
            if (!isStart)
            {
                isStart = true;
                GenerateRand();
            }
        }

        public void Stop()
        {
            isStart = false;
        }

        private void GenerateRand()
        {
            while (isStart)
            {
                OnRandGenerated(random.Next(10000));
                Thread.Sleep(1000);
            }
        }

        #region Event

        public event EventHandler<RandGeneratedEventArgs> RandGenerated;

        private void OnRandGenerated(int rand)
        {
            RaiseRandGeneratedEvent(rand);
        }

        private void RaiseRandGeneratedEvent(int rand)
        {
            EventHandler<RandGeneratedEventArgs> temp = RandGenerated;
            if (temp != null)
            {
                RandGeneratedEventArgs arg = new RandGeneratedEventArgs(rand);
                temp(this, arg);
            }
        }

        #endregion      
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
            CallAnddCallvirt();
            MakLoop();
            ExcptionDeal();
            EventMain();
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

        #region [4、Call And Callvirt]

        private delegate void SpeakDelegate(Animal animal);

        /// <summary>
        /// call and call virt
        /// </summary>
        private static void CallAnddCallvirt()
        {
            //定义动态方法，没有返回值，传入参数为Animal，所以在模块选择为Program类所在的模块
            DynamicMethod dynamicSpeakWithCall = new DynamicMethod("DynamicSpeakWithCall", null, new Type[] { typeof(Animal) }, typeof(EmitLearn).Module);
            ILGenerator callIL = dynamicSpeakWithCall.GetILGenerator();
            //加载参数0，即Animal类或其派生类的对象
            callIL.Emit(OpCodes.Ldarg_0);
            //通过Call指令调用Speak方法
            callIL.Emit(OpCodes.Call, typeof(Animal).GetMethod("Speak"));
            callIL.Emit(OpCodes.Ret);

            Console.WriteLine("SpeakWithCall:");
            SpeakDelegate SpeakWithCall = (SpeakDelegate)dynamicSpeakWithCall.CreateDelegate(typeof(SpeakDelegate));
            SpeakWithCall(new Animal());
            SpeakWithCall(new Cat());
            SpeakWithCall(new Dog());

            //定义动态方法，没有返回值，传入参数为Animal，所在的模块选择为Program类所在的模块
            DynamicMethod dynamicSpeakWithCallvirt = new DynamicMethod("DynamicSpeakWithCallvirt", null,
                new Type[] { typeof(Animal) }, typeof(EmitLearn).Module);
            ILGenerator callvirtIL = dynamicSpeakWithCallvirt.GetILGenerator();
            //加载参数0，即Animal类或其派生类的对象
            callvirtIL.Emit(OpCodes.Ldarg_0);
            //通过Callvirt指令调用Speak方法
            callvirtIL.Emit(OpCodes.Callvirt, typeof(Animal).GetMethod("Speak"));
            callvirtIL.Emit(OpCodes.Ret);

            Console.WriteLine("SpeakWithCallvirt:");
            SpeakDelegate SpeakCallvirt =
                (SpeakDelegate)dynamicSpeakWithCallvirt.CreateDelegate(typeof(SpeakDelegate));
            SpeakCallvirt(new Animal());
            SpeakCallvirt(new Cat());
            SpeakCallvirt(new Dog());
        }
        #endregion

        #region [5、MakeForMathod]

        /// <summary>
        /// loop method
        /// </summary>
        private static void MakLoop()
        {
            string name = "EmitExamples.DynamicIterator";
            string fileName = name + ".dll";
            AssemblyName asmName = new AssemblyName(name);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name, fileName);
            TypeBuilder typeBuilder = moduleBuilder.DefineType("EmitExamples.DynamicIterator", TypeAttributes.Public);

            MakeForMethod(typeBuilder);
            MakeForeachMethod(typeBuilder);

            Type type = typeBuilder.CreateType();

            assemblyBuilder.Save(fileName);

            int[] ints = new int[] { 2, 3, 4 };
            Console.WriteLine(type.GetMethod("ForMethod").Invoke(null, new object[] { ints }));
            Console.WriteLine(type.GetMethod("ForeachMethod").Invoke(null, new object[] { ints }));

        }

        /// <summary>
        /// do for loop
        /// </summary>
        /// <param name="typeBuilder"></param>
        private static void MakeForMethod(TypeBuilder typeBuilder)
        {
            //定义一个传入参数为int32[],返回值为int32的方法
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("ForMethod",
                MethodAttributes.Public | MethodAttributes.Static, typeof(Int32), new Type[] { typeof(Int32[]) });
            ILGenerator methodIL = methodBuilder.GetILGenerator();
            //用来保存就和结果的局部变量
            LocalBuilder sum = methodIL.DeclareLocal(typeof(Int32));
            //循环中使用的局部变量
            LocalBuilder i = methodIL.DeclareLocal(typeof(Int32));

            Label compareLabel = methodIL.DefineLabel();
            Label enterLoopLabel = methodIL.DefineLabel();

            //int sum=0
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_0);
            //int i=0
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_1);
            methodIL.Emit(OpCodes.Br, compareLabel);

            //定义一个标签，表示从下面开始进入循环体
            methodIL.MarkLabel(enterLoopLabel);
            //sum+=ints[i]
            //其中Ldelem_I4用来加载一个数组中的int32类型的元素
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldloc_1);
            methodIL.Emit(OpCodes.Ldelem_I4);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Stloc_0);

            //i++
            methodIL.Emit(OpCodes.Ldloc_1);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Stloc_1);

            //定义一个标签，表示从下面开始进入循环的比较
            methodIL.MarkLabel(compareLabel);
            //i<ints.Length
            methodIL.Emit(OpCodes.Ldloc_1);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldlen);
            methodIL.Emit(OpCodes.Clt);
            methodIL.Emit(OpCodes.Brtrue_S, enterLoopLabel);

            //return sum;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// do foreach loop
        /// </summary>
        /// <param name="typeBuilder"></param>
        private static void MakeForeachMethod(TypeBuilder typeBuilder)
        {
            //定义一个传入参数为Int32[]，返回值为Int32的方法
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("ForeachMethod", MethodAttributes.Public | MethodAttributes.Static, typeof(Int32), new Type[] { typeof(Int32[]) });
            ILGenerator methodIL = methodBuilder.GetILGenerator();

            //用来保存求和结果的局部变量
            LocalBuilder sum = methodIL.DeclareLocal(typeof(Int32));
            //foreach中的int i
            LocalBuilder i = methodIL.DeclareLocal(typeof(Int32));
            //用来保存传入的数组
            LocalBuilder ints = methodIL.DeclareLocal(typeof(Int32[]));
            //数组循环用临时变量
            LocalBuilder index = methodIL.DeclareLocal(typeof(Int32));

            Label compareLabel = methodIL.DefineLabel();
            Label enterLoopLabel = methodIL.DefineLabel();

            //int sum=0;
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_0);
            //ints=ints
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Stloc_2);
            //int index=0
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_3);
            methodIL.Emit(OpCodes.Br, compareLabel);

            //定义一个标签，表示从下面开始进入循环体
            methodIL.MarkLabel(enterLoopLabel);
            //其中Ldelem_I4用来加载一个数组中的Int32类型的元素
            //加载i=ints[index]
            methodIL.Emit(OpCodes.Ldloc_2);
            methodIL.Emit(OpCodes.Ldloc_3);
            methodIL.Emit(OpCodes.Ldelem_I4);
            methodIL.Emit(OpCodes.Stloc_1);

            //sum+=i;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ldloc_1);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Stloc_0);

            //index++
            methodIL.Emit(OpCodes.Ldloc_3);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Stloc_3);

            //定义一个标签，表示从下面开始进入循环的比较
            methodIL.MarkLabel(compareLabel);
            //index < ints.Length
            methodIL.Emit(OpCodes.Ldloc_3);
            methodIL.Emit(OpCodes.Ldloc_2);
            methodIL.Emit(OpCodes.Ldlen);
            methodIL.Emit(OpCodes.Conv_I4);
            methodIL.Emit(OpCodes.Clt);
            methodIL.Emit(OpCodes.Brtrue_S, enterLoopLabel);

            //return sum;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ret);

        }
        #endregion

        #region [6、Exception]

        private static void ExcptionDeal()
        {
            #region Init

            string name = "DynamicExceptionHandler";
            string fileName = name + ".dll";
            AssemblyName asmName = new AssemblyName(name);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name, fileName);

            #endregion

            TypeBuilder typeBuilder = moduleBuilder.DefineType("ExceptionHandler", TypeAttributes.Public);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("ConvertToInt32", MethodAttributes.Public | MethodAttributes.Static, typeof(Int32), new Type[] { typeof(string) });

            ILGenerator methodIL = methodBuilder.GetILGenerator();
            LocalBuilder num = methodIL.DeclareLocal(typeof(Int32));

            //int num = 0;
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_0);

            //begin try
            Label tryLabel = methodIL.BeginExceptionBlock();
            //num = Convert.ToInt32(str);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt32", new Type[] { typeof(string) }));
            methodIL.Emit(OpCodes.Stloc_0);
            //end try

            //begin catch 注意，这个时侯堆栈顶为异常信息ex
            methodIL.BeginCatchBlock(typeof(Exception));
            //Console.WriteLine(ex.Message);
            methodIL.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            methodIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            //end catch
            methodIL.EndExceptionBlock();

            //return num;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ret);

            Type type = typeBuilder.CreateType();

            assemblyBuilder.Save(fileName);

            object ob = type.GetMethod("ConvertToInt32").Invoke(null, new object[] { "12" });
            Console.WriteLine(ob);
            ob = type.GetMethod("ConvertToInt32").Invoke(null, new object[] { "s12" });
            Console.WriteLine(ob);
        }
        #endregion

        #region [7、Event]
        private static void EventMain()
        {
            #region Init

            string name = "DynamicPublisher";
            string fileName = name + ".dll";
            AssemblyName asmName = new AssemblyName(name);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name, fileName);

            #endregion

            #region RandGeneratedEventArgs

            TypeBuilder randGeneratedEventArgsTypeBuilder = moduleBuilder.DefineType("RandGeneratedEventArgs", TypeAttributes.Public, typeof(EventArgs));

            #region Field

            FieldBuilder randField = randGeneratedEventArgsTypeBuilder.DefineField("_rand", typeof(Int32), FieldAttributes.Private);

            #endregion

            #region Property

            PropertyBuilder randProperty = randGeneratedEventArgsTypeBuilder.DefineProperty("Rand", PropertyAttributes.HasDefault, typeof(Int32), null);

            #region 定义属性Rand的get方法

            //定义属性Rand的get方法

            MethodBuilder getPropertyRandBuilder = randGeneratedEventArgsTypeBuilder.DefineMethod("get",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(Int32),
                Type.EmptyTypes);

            //生成属性Rand的get方法的IL代码，即返回私有字段_a

            ILGenerator getRandIL = getPropertyRandBuilder.GetILGenerator();

            getRandIL.Emit(OpCodes.Ldarg_0);
            getRandIL.Emit(OpCodes.Ldfld, randField);
            getRandIL.Emit(OpCodes.Ret);

            //设置属性A的get和set方法
            randProperty.SetGetMethod(getPropertyRandBuilder);

            #endregion

            #endregion

            #region Ctor

            ConstructorBuilder constructorBuilder = randGeneratedEventArgsTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(Int32) });

            ILGenerator ctorIL = constructorBuilder.GetILGenerator();

            //加载参数1填充到私有字段_rand
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, randField);
            ctorIL.Emit(OpCodes.Ret);

            #endregion

            Type randGeneratedEventArgsType = randGeneratedEventArgsTypeBuilder.CreateType();

            #endregion

            #region Publisher

            TypeBuilder publisherTypeBuilder = moduleBuilder.DefineType("Publisher", TypeAttributes.Public);

            Type eventType = typeof(EventHandler<>).MakeGenericType(randGeneratedEventArgsType);

            #region Field

            FieldBuilder randomField = publisherTypeBuilder.DefineField("random", typeof(Random), FieldAttributes.Private);
            FieldBuilder isStartField = publisherTypeBuilder.DefineField("isStart", typeof(Boolean), FieldAttributes.Private);
            //通过对自动生成的IL代码的仔细观察，发现系统自动生成了一个与事件同名的私有字段，用来在get set等方法中进行使用
            FieldBuilder randGeneratedField = publisherTypeBuilder.DefineField("RandGenerated", eventType, FieldAttributes.Private);

            #endregion

            #region Ctor

            //注类似private Random random = new Random(DateTime.Now.Millisecond); 的字段初始化，需要在构造函数中进行

            constructorBuilder = publisherTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);

            ctorIL = constructorBuilder.GetILGenerator();

            LocalBuilder dateTime = ctorIL.DeclareLocal(typeof(DateTime));

            //加载参数0/false填充到私有字段isStart
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldc_I4_0);
            ctorIL.Emit(OpCodes.Stfld, isStartField);

            ctorIL.Emit(OpCodes.Ldarg_0);
            //获取DateTime.Now，这里由于DateTime是值类型，所以需要用Ldloca_S方法加载使用
            ctorIL.Emit(OpCodes.Call, typeof(DateTime).GetMethod("get_Now", Type.EmptyTypes));
            ctorIL.Emit(OpCodes.Stloc_0);
            ctorIL.Emit(OpCodes.Ldloca_S, dateTime);
            //获取DateTime.Millisecond，并用此初始化Random对象
            ctorIL.Emit(OpCodes.Call, typeof(DateTime).GetMethod("get_Millisecond", Type.EmptyTypes));
            ctorIL.Emit(OpCodes.Newobj, typeof(Random).GetConstructor(new Type[] { typeof(Int32) }));
            ctorIL.Emit(OpCodes.Stfld, randomField);

            //调用父类的无参构造函数
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, typeof(Object).GetConstructor(Type.EmptyTypes));
            ctorIL.Emit(OpCodes.Ret);

            #endregion

            #region Event

            //定义事件，事件的可访问性由和它相关的getset方法决定
            EventBuilder randGeneratedEvent = publisherTypeBuilder.DefineEvent("RandGenerated", EventAttributes.None, eventType);

            MethodBuilder addEventBuilder = publisherTypeBuilder.DefineMethod("add_RandGenerated", MethodAttributes.Public, null, new Type[] { eventType });
            //注意在我们使用事件时，使用的是+=操作，但在IL代码中并不是如此，应该使用Delegate.Combine方法
            ILGenerator addEventIL = addEventBuilder.GetILGenerator();

            addEventIL.Emit(OpCodes.Ldarg_0);
            addEventIL.Emit(OpCodes.Ldarg_0);
            addEventIL.Emit(OpCodes.Ldfld, randGeneratedField);
            addEventIL.Emit(OpCodes.Ldarg_1);
            addEventIL.Emit(OpCodes.Call, typeof(Delegate).GetMethod("Combine", new Type[] { eventType, eventType }));
            //返回的是Delegate类型，所以需要进行转换
            addEventIL.Emit(OpCodes.Castclass, eventType);
            addEventIL.Emit(OpCodes.Stfld, randGeneratedField);

            randGeneratedEvent.SetAddOnMethod(addEventBuilder);

            MethodBuilder removeEventBuilder = publisherTypeBuilder.DefineMethod("remove_RandGenerated", MethodAttributes.Public, null, new Type[] { eventType });
            //注意在我们使用事件时，使用的是-=操作，但在IL代码中并不是如此，应该使用Delegate.Remove方法
            ILGenerator removeEventIL = removeEventBuilder.GetILGenerator();

            removeEventIL.Emit(OpCodes.Ldarg_0);
            removeEventIL.Emit(OpCodes.Ldarg_0);
            removeEventIL.Emit(OpCodes.Ldfld, randGeneratedField);
            removeEventIL.Emit(OpCodes.Ldarg_1);
            removeEventIL.Emit(OpCodes.Call, typeof(Delegate).GetMethod("Remove", new Type[] { eventType, eventType }));
            //返回的是Delegate类型，所以需要进行转换
            removeEventIL.Emit(OpCodes.Castclass, eventType);
            removeEventIL.Emit(OpCodes.Stfld, randGeneratedField);

            randGeneratedEvent.SetRemoveOnMethod(removeEventBuilder);

            #endregion

            #region Method

            #region Stop

            MethodBuilder stopBuilder = publisherTypeBuilder.DefineMethod("Stop", MethodAttributes.Public, null, Type.EmptyTypes);

            ILGenerator stopIL = stopBuilder.GetILGenerator();

            stopIL.Emit(OpCodes.Ldarg_0);
            stopIL.Emit(OpCodes.Ldc_I4_0);
            stopIL.Emit(OpCodes.Stfld, isStartField);
            stopIL.Emit(OpCodes.Ret);

            #endregion

            #region RaiseRandGeneratedEvent

            MethodBuilder raiseRandGeneratedEventBuilder = publisherTypeBuilder.DefineMethod("RaiseRandGeneratedEvent", MethodAttributes.Private, null, new Type[] { typeof(Int32) });

            ILGenerator raiseRandGeneratedEventIL = raiseRandGeneratedEventBuilder.GetILGenerator();

            LocalBuilder temp = raiseRandGeneratedEventIL.DeclareLocal(eventType);
            LocalBuilder arg = raiseRandGeneratedEventIL.DeclareLocal(randGeneratedEventArgsType);
            Label returnLabel = raiseRandGeneratedEventIL.DefineLabel();

            //EventHandler<RandGeneratedEventArgs> temp = RandGenerated;
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldarg_0);
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldfld, randGeneratedField);
            raiseRandGeneratedEventIL.Emit(OpCodes.Stloc_0);

            //if (temp == null) return
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldloc_0);
            raiseRandGeneratedEventIL.Emit(OpCodes.Brfalse_S, returnLabel);

            //RandGeneratedEventArgs arg = new RandGeneratedEventArgs(rand);
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldarg_1);
            raiseRandGeneratedEventIL.Emit(OpCodes.Newobj, randGeneratedEventArgsType.GetConstructor(new Type[] { typeof(Int32) }));
            raiseRandGeneratedEventIL.Emit(OpCodes.Stloc_1);

            //temp(this, arg);
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldloc_0);
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldarg_0);
            raiseRandGeneratedEventIL.Emit(OpCodes.Ldloc_1);
            raiseRandGeneratedEventIL.Emit(OpCodes.Callvirt, eventType.GetMethod("Invoke"));

            raiseRandGeneratedEventIL.MarkLabel(returnLabel);
            raiseRandGeneratedEventIL.Emit(OpCodes.Ret);

            #endregion

            #region OnRandGenerated

            MethodBuilder onRandGeneratedBuilder = publisherTypeBuilder.DefineMethod("OnRandGenerated", MethodAttributes.Virtual | MethodAttributes.Family | MethodAttributes.NewSlot, null, new Type[] { typeof(Int32) });

            ILGenerator onRandGeneratedIL = onRandGeneratedBuilder.GetILGenerator();

            onRandGeneratedIL.Emit(OpCodes.Ldarg_0);
            onRandGeneratedIL.Emit(OpCodes.Ldarg_1);
            onRandGeneratedIL.Emit(OpCodes.Call, raiseRandGeneratedEventBuilder);
            onRandGeneratedIL.Emit(OpCodes.Ret);

            #endregion

            #region GenerateRand

            MethodBuilder generateRandBuilder = publisherTypeBuilder.DefineMethod("GenerateRand", MethodAttributes.Private, null, Type.EmptyTypes);

            ILGenerator generateRandIL = generateRandBuilder.GetILGenerator();

            Label enterLoopLable = generateRandIL.DefineLabel();
            Label compareLable = generateRandIL.DefineLabel();

            generateRandIL.Emit(OpCodes.Br_S, compareLable);

            generateRandIL.MarkLabel(enterLoopLable);
            //OnRandGenerated(random.Next(10000));
            generateRandIL.Emit(OpCodes.Ldarg_0);
            generateRandIL.Emit(OpCodes.Ldarg_0);
            generateRandIL.Emit(OpCodes.Ldfld, randomField);
            generateRandIL.Emit(OpCodes.Ldc_I4, 10000);
            generateRandIL.Emit(OpCodes.Call, typeof(Random).GetMethod("Next", new Type[] { typeof(Int32) }));
            generateRandIL.Emit(OpCodes.Call, onRandGeneratedBuilder);
            //Thread.Sleep(1000);
            generateRandIL.Emit(OpCodes.Ldc_I4, 1000);
            generateRandIL.Emit(OpCodes.Call, typeof(System.Threading.Thread).GetMethod("Sleep", new Type[] { typeof(Int32) }));

            generateRandIL.MarkLabel(compareLable);
            generateRandIL.Emit(OpCodes.Ldarg_0);
            generateRandIL.Emit(OpCodes.Ldfld, isStartField);
            generateRandIL.Emit(OpCodes.Brtrue_S, enterLoopLable);

            generateRandIL.Emit(OpCodes.Ret);

            #endregion

            #region Start

            MethodBuilder startBuilder = publisherTypeBuilder.DefineMethod("Start", MethodAttributes.Public, null, Type.EmptyTypes);

            ILGenerator startIL = startBuilder.GetILGenerator();

            returnLabel = startIL.DefineLabel();

            //if (isStart) return
            startIL.Emit(OpCodes.Ldarg_0);
            startIL.Emit(OpCodes.Ldfld, isStartField);
            startIL.Emit(OpCodes.Brtrue_S, returnLabel);
            startIL.Emit(OpCodes.Ldarg_0);
            startIL.Emit(OpCodes.Call, generateRandBuilder);

            startIL.MarkLabel(returnLabel);
            startIL.Emit(OpCodes.Ret);

            #endregion

            #endregion

            Type publisherType = publisherTypeBuilder.CreateType();

            #endregion

            //assemblyBuilder.Save(fileName);
            Publisher p = new Publisher();
            p.RandGenerated += new EventHandler<RandGeneratedEventArgs>(p_RandGenerated);
            p.Start();
        }

        static void p_RandGenerated(object sender, RandGeneratedEventArgs e)
        {
            Console.WriteLine(e.Rand);
        }
        #endregion
    }
}
