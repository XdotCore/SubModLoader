using SubmachineModLib;
using SubModLoader.GameData.Extensions;
using SubModLoader.Mods;
using SubModLoader.Mods.Attributes;
using SubModLoader.Storage.Widget;
using SubModLoader.Storage.Widget.Item;
using SubModLoader.Utils;
using System;
using System.Runtime.InteropServices;

[assembly: SubModInfo<TestMod.Mod>("Test Mod", "0.0.0", "X.Core")]
[assembly: SubModColor(0xFF, 0x45, 0x00)]

namespace TestMod {
    public sealed class Mod : SubMod {
        public override void OnLoad() {
            SettingsCategory category = Settings.GetCategory("Gene\"[]\\#=ral", true, "Gene ral", "Test tooltip");

            SettingsBool.Get(category, "hel\"[]\\#=lo", true, true, "hel lo", "Another test tooltip");

            SettingsCategory subcategory = category.GetCategory("Subcategory1", true, "Subcategory 1", "Yet another test tooltip");

            SettingsBool.Get(subcategory, "Subsetting1", false, true, "Subsetting 1");

            SettingsBool.Get(category, "upper again", true, true, "upper again");
        }

        public static byte Byte(byte b) => b;

        public static sbyte SByte(sbyte sb) => sb;

        public static ushort UShort(ushort us) => us;

        public static short Short(short s) => s;

        public static uint UInt(uint u) => u;

        public static int Int(int i) => i;

        public static long Long(long l) => l;

        public static Half Half(Half h) => h;

        public static float Float(float f) => f;

        public static double Double(double d) => d;

        public static bool Bool(bool b) => b;

        public static string String(string s) => s;

        public static char Char(char c) => c;

        public static IntPtr IntPtr(IntPtr ptr) => ptr;

        public static int[] IntArray(int[] ints) => ints;

        public override void ApplyMod(GameMakerData gameData) {
            Logger.WriteLine("Applying mod...");

            AddOnGMLInitialize(gameData.AddCodeAndFunction("test_func", $$"""
                {{Logger.WriteLineFromGML(CallFromGML(Byte, $"{byte.MaxValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(SByte, $"{sbyte.MinValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(UShort, $"{ushort.MaxValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(Short, $"{short.MinValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(UInt, $"{uint.MaxValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(Int, $"{int.MinValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(Long, $"{long.MinValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(Half, $"{System.Half.MaxValue}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(Float, $"{float.MaxValue:F}"))}}
                {{Logger.WriteLineFromGML(CallFromGML(Double, $"{double.MaxValue:F}"))}}
                var b = {{CallFromGML(Bool, "true")}}
                if (b)
                    {{Logger.WriteLineFromGML("true")}}
                else
                    {{Logger.WriteLineFromGML("false")}}
                {{Logger.WriteLineFromGML(CallFromGML(String, "\"yo\""))}}
                {{Logger.WriteLineFromGML(CallFromGML(Char, "\"❤️\""))}}
                {{Logger.WriteLineFromGML(CallFromGML(IntPtr, $"{new Func<IntPtr>(() => { IntPtr result = Marshal.AllocHGlobal(1); Marshal.FreeHGlobal(result); return result; })()}"))}}
                {{Logger.WriteLineFromGML("\"hello\"", Color.RebeccaPurple.ToGML(), Color.IndianRed.ToGML())}}
                var a = array_create(4)
                array_set(a, 0, 1)
                array_set(a, 1, 3)
                array_set(a, 2, 2)
                array_set(a, 3, 5)
                {{Logger.WriteLineFromGML(CallFromGML(IntArray, "a"))}}
                """).function);

            Logger.WriteLine("█▇▆▅▄▃▂▁ 🤯");
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBlack);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleRed);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleGreen);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleYellow);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBlue);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleMagenta);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleCyan);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleWhite);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldBlack);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldRed);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldGreen);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldYellow);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldBlue);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldMagenta);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldCyan);
            Logger.WriteLine("  ", backgroundColor: Color.ConsoleBoldWhite);

            Logger.Write("hi", Color.Aqua, Color.DarkGoldenRod);
            Logger.Write("!", Color.Orange, Color.Chartreuse);
            Logger.EndWrite();

            Logger.Write("hi");
            Logger.Write("hi", default, default, Color.ConsoleModifier.Bold);
            Logger.EndWrite();

            Logger.Write("hi");
            Logger.Write("hi", default, default, Color.ConsoleModifier.Italic);
            Logger.EndWrite();

            Logger.Write("hi");
            Logger.Write("hi", default, default, Color.ConsoleModifier.Bold, Color.ConsoleModifier.Italic);
            Logger.EndWrite();

            Logger.WriteLine("hello", Color.Red, Color.Cyan, Color.ConsoleModifier.Bold, Color.ConsoleModifier.Italic, Color.ConsoleModifier.Underline, Color.ConsoleModifier.Negative, Color.ConsoleModifier.Strike);
        }

        public override void OnUnload() {
            Logger.WriteLine("goodbye world");
        }
    }
}