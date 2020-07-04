using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CSharp_Class_17
{
    class ReadonlyDemo
    {
        // Nem működik, mert a const csak fordításidőben kideríthető értékeknél használható
        //private const Random random = new Random();

        // A konstruktor kivételével máshol nem módosítható
        private readonly Random random = new Random();
    }

    class ArrowOperatorDemo
    {
        // Csak getterrel rendelkező tulajdonság, ami mindig 10-el tér vissza
        public int DemoProperty => 10;

        // Egyetlen kifejezésből álló metódus
        public int DemoMethod() => 10 + 25;
    }

    class PropertyInitializerDemo
    {
        public int A { get; set; }

        public string B { get; set; }
    }

    class ReflectionDemo
    {
        public string DemoProperty { get; set; }

        private int demoField;

        public void DemoMethod1()
        {
            Console.WriteLine("Demo Method 1");
        }

        private int DemoMethod1(int a, int b)
        {
            return a + b;
        }
    }

    class ObsoleteDemo
    {
        // Ez a metódus Obsolete
        // Lényegében egy konstruktorhívás, de a C# megengedi, hogy ne írjuk ki a ()-t
        [Obsolete]
        public void ObsoleteMethod()
        {

        }
    }

    // Osztályon és metóduson használható
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DeveloperAttribute : Attribute
    {
        public string Name { get; set; }

        public string Email { get; set; }
    }

    // Konstruktorhívás, de a tulajdonságok speciálisan vannak kezelve attribútumok esetén
    [Developer(Name = "A", Email = "a.a.com")]
    class DeveloperAttributeDemo
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            // A partial class segítségével egy osztályt több fájlban tudunk megvalósítani
            // A fordító egyszerűen fogja és összemásolja a két fájl tartalmát
            // Minden az eddig megismerteknek megfelelően működik
            // Lásd: PartialClass1.cs és PartialClass2.cs

            // Általában ablakozó keretrendszerek (pl. Windows Forms, WPF, UWP) használják a generált és fejlesztő által írt kód szétválasztására

            //--------------------------------------------------

            // Előfordulhat, hogy nem tudunk const adattagot használni, mert a konstans létrehozásához szükséges érték csak futásidőben jön létre
            // Ezekben az esetekben ahhoz hogy megtartsuk a konstans jelleget, de futásidőben is adhassunk értéket a readonly kulcsszót használhatjuk
            // Lásd: ReadonlyDemo osztály

            //--------------------------------------------------

            // A default kulcsszó segítségével típusok alapértelmezett értékét kérdezhetjük le:
            var i = default(int); // 0
            var b = default(bool); // false
            var r = default(Random); // null (a referencia típusok alapértelmezett értéke)

            //--------------------------------------------------

            // A => operátort nem csak lambda kifejezések létrehozásánál használhatjuk
            // Lásd: ArrowOperatorDemo osztály

            //--------------------------------------------------

            // Osztályok példányosításakor, ha az osztálynak van default konstruktora, akkor a publikus, publikus setterrel rendelkező tulajdonságoknak lehetőségünk van értéket adni
            // Ilyenkor a () sem kötelező
            var propertyInitializerDemo1 = new PropertyInitializerDemo { A = 10 };
            var propertyInitializerDemo2 = new PropertyInitializerDemo { B = "Hello" };
            var propertyInitializerDemo3 = new PropertyInitializerDemo { A = 10, B = "Hello" };

            //--------------------------------------------------

            // Bizonyos esetekben előfordulhat, hogy futűsidőben szeretnénk különböző típusokról információkat elérni
            // Ehhez a C#-ban (is) elérhető Reflection használható

            // Fontos: string alapú, így veszélyes, illetve rosszul/túl használva nagyon lassú tud lenni

            // Kétféle módon tudjuk egy típus információit lekérni:
            
            // A typeof kulcsszó segítségével:
            var reflectionDemoType1 = typeof(ReflectionDemo); // Type típusú

            // Egy példányon keresztül az object-ből örökölt GetType() metódus segítségével:
            var reflectionDemoType2 = new ReflectionDemo().GetType(); // Type típusú

            // A Type-on keresztül az adott típus minden információja elérhető

            // Publikus metódusok:
            foreach (var methodInfo in reflectionDemoType2.GetMethods())
            {
                Console.WriteLine(methodInfo.Name); // Metódus neve
                Console.WriteLine(methodInfo.ReturnType); // Metódus visszítérési típusa

                // Paraméterek adatai
                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    Console.WriteLine(parameterInfo.Name); // Paraméter neve
                    Console.WriteLine(parameterInfo.ParameterType); // Paraméter típusa
                }
            }

            // Publikus adattagok:
            foreach (var fieldInfo in reflectionDemoType2.GetFields())
            {
                Console.WriteLine(fieldInfo.Name); // Adattag neve
                Console.WriteLine(fieldInfo.FieldType); // Adattag típusa
            }

            // Stb.

            // Nem csak lekérdezhetünk, hanem interakcióba is léphetünk a lekérdezett dolgokkal:
            // Lekérdezzük a DemoMethod1-et a neve alapján, majd meghívjuk a megadott példányon, a megadott paraméterekkel
            // A megadott példányra azért van szükség, mert a típusinformáció nincs példányhoz kötve, így tudnunk kell pontosan melyik példány metódusát hívjuk
            // static esetén ez null
            // Mivel a metódusnak nincs paramétere, így egy 0 méretű object tömböt adunk át (object, hiszen nem tudjuk pontosan milyen típusúak a paraméterek <- lekérdezhetjük, de hívásnál ez az információ nem áll rendelkezésre)
            reflectionDemoType2.GetMethod("DemoMethod1").Invoke(reflectionDemoType2, new object[0]);
            // Vagy nameof segítségével biztonságosabban lekérhetjük a nevet, így a fordító panaszkodik ha megváltozott:
            reflectionDemoType2.GetMethod(nameof(ReflectionDemo.DemoMethod1)).Invoke(reflectionDemoType2, new object[0]);

            // Tulajdonságokat és adattagok értékét is lekérdezhetünk és módosíthatunk

            // Konstruktort is hívhatunk kétféle módon is (castolhatjuk őket):
            var reflectionDemo1 = typeof(ReflectionDemo).GetConstructors()[0].Invoke(new object[0]); // Legelső konstruktor meghívása paraméterek nélkül
            var reflectionDemo2 = Activator.CreateInstance(typeof(ReflectionDemo)); // Activator használata <- sokkal gyorsabb

            // Lekérdezhetjük az Assemblyt is és így az összes benne levő típust:
            var assembly = typeof(ReflectionDemo).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                Console.WriteLine(type.Name); // Assemblyben levő típus neve
            }

            // Valójában akár privát tagokat is elérhetünk, csak meg kell adnunk hogy direkt ilyen tagot keresünk:
            // A | a bináris vagy jele!!!
            var result = reflectionDemoType2.GetMethod("DemoMethod2", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(reflectionDemoType2, new object[] { 2, 3 }); // Eredmény: 5

            // Mindennek van Reflection megfelelője!

            // A Reflection alapvetően kerülendő, de vannak helyzetek, amikor nagyon kényelmes és hasznos tud lenni, illetve olyan helyzetek is, amikor elkerülhetetlen
            // Példa a következő részben

            //--------------------------------------------------

            // C#-ban (és sok más nyelvben) úgynevezett meta adatokat csatolhatunk az osztályainkhoz, metódusainkhoz, tulajdonságainkhoz stb.
            // Meta adat: adat az adatról

            // Ezeket hívjuk C#-ban attribútumoknak
            // Vannakl beépítettek, de sajátot is készíthetünk

            // Példa beépített:
            // Obsolete: segítségével megjelölhetjük, hogy a kód egy része elavult és ne használja senki (olyan esetekben lehet érdekes, amikor a kódunkat mások használják, emiatt nem törölhetünk csak úgy egy metódust pl.)
            // Lásd: ObsoleteDemo osztály
            // ha hívni akarjuk, akkor jelzi a Visual Studio, hogy elavult:
            var obsoleteDemo = new ObsoleteDemo();
            obsoleteDemo.ObsoleteMethod();

            // Sok más beépített van (pl. Serializable)

            // Példa saját:
            // Szabályok létrehozásnál:
            // 1. publikus
            // 2. Attribute a neve végén (nem kötelező, de erősen ajánlott és a fordító is kezeli, így nem kell kiírnunk használatnál)
            // 3. Attribute-ból származik
            // 4. AttributeUsage attribútum segítségével meg kell mondanunk, hogy hol és hogyan használható

            // Lásd: DeveloperAttribute és DeveloperAttributeDemo

            // Attribútum lekérdezése Reflection segítségével:
            var developerAttribute = (DeveloperAttribute)Attribute.GetCustomAttribute(typeof(DeveloperAttributeDemo), typeof(DeveloperAttribute));
            Console.WriteLine(developerAttribute.Name + " " + developerAttribute.Email);

            //--------------------------------------------------

            // Legelső órán szó volt róla, hogy minden .NET-es nyelv először Intermediate Language-re (IL-re), azaz a Köztes Nyelvre fordul
            // Ez fordul tovább binárisra futás közben

            // Az IL kb. egy magas szintű assembly-szerű nyelv
            // Működése kiértékelő verem alapú:
            // Van egy verem, amibe a műveletek paramétereit tesszük, majd a műveletek innen kiveszik a paramétereket, majd visszarakják az eredményt a veremre

            // IL kódot mi magunk is tudunk C#-ból generálni, így pedig C# kódból lényegében futtatható kódot tudunk generálni

            // Minden ...Info osztályhoz (amiket a Reflection során láttunk) van egy Builder osztály, így a Method-hoz is
            // Készítsünk egy saját statikus összeadó metódust IL kód generálásával!

            // Készítsünk egy saját assembly készítőt:
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);

            // A saját assembly készítővel egy saját modul készítőt az assemblyn belül:
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // A saját modul készítővel egy saját típus készítőt a modulon belül:
            var typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

            // A saját típus készítővel egy saját metódus készítőt a típuson belül:
            var methodBuilder = typeBuilder.DefineMethod("DynamicMethod", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[] { typeof(int), typeof(int) });

            // Kérjük el a metódustól az IL kód generátort:
            var ilGenerator = methodBuilder.GetILGenerator();

            // A metódus futása során először betöltjük az első paraméterét a kiértékelő verembe, a megfelelő IL utasítás segítségével:
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Majd a másodikat:
            ilGenerator.Emit(OpCodes.Ldarg_1);

            // Adjuk össze a két paramétert a kiértékelő verem tetején levő két paraméter kiolvasásával (a veremből kikerülnek a paraméterek, majd bekerül az eredmény):
            ilGenerator.Emit(OpCodes.Add);

            // Térjünk vissza a metódusból úgy, hogy a kiértékelő verem tetején a metódus visszatérési értéke van
            ilGenerator.Emit(OpCodes.Ret);

            // Gyártsuk le a típust:
            var dynamicType = typeBuilder.CreateType();

            // Hívjuk meg Reflection segítségével az összeadó metódust tetszőleges számok átadásával:
            Console.WriteLine(dynamicType.GetMethod("DynamicMethod", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { 1, 2 })); // Eredmény: 3
        }
    }
}
