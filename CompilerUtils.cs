using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicCode;

internal class CompilerUtils
{
    private static readonly Dictionary<string, string> CSharpToClrTypeMap = new()
    {
        {"bool", "Boolean"},
        {"byte", "Byte"},
        {"sbyte", "SByte"},
        {"char", "Char"},
        {"decimal", "Decimal"},
        {"double", "Double"},
        {"float", "Single"},
        {"int", "Int32"},
        {"uint", "UInt32"},
        {"long", "Int64"},
        {"ulong", "UInt64"},
        {"object", "Object"},
        {"short", "Int16"},
        {"ushort", "UInt16"},
        {"string", "String"}
    };

    public static string GetClrTypeName(string csharpType)
    {
        if (CSharpToClrTypeMap.TryGetValue(csharpType, out var clrName))
            return clrName;
        return csharpType;
    }
}
