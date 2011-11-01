using System;
using System.Collections.Generic;
using System.Text;

namespace CoastalGIS.SpatialDataBase
{
     public class CommonFunc
    {
         static public string NameCheck(string name) //判断名称是否符合规范
         {
             if (name.Contains("-")) //名称中不能包含"-"
             {
                 return "名称中不能包含“-”";
             }
             else if (Char.IsNumber(name,0)) 
             {
                 return "名称第一个字符不能是数字";
             }
             else if (name.Contains("/") || name.Contains(@"\"))
             {
                 return @"名称中不能包含“\”或“/”";
             }
             else if (name.Contains(" "))
             {
                 return "名称中不能包含空格";
             }
             else if (name.Length > 15)
             {
                 return "名称包含字符过多";
             }
             else if (name.Contains("."))
             {
                 return @"名称中不能包含“\”";
             }
             else 
             {
                 return "true";
             }
         }

         static public bool IsChineseLetter(string input)
         {
             int code = 0;
             int chfrom = Convert.ToInt32("4e00", 16);
             int chend = Convert.ToInt32("9fff", 16);
             if (input != "")
             {
                 code = Char.ConvertToUtf32(input, 0);

                 if (code >= chfrom && code <= chend)
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             }
             return false;
         }

         static public bool IsLetter(string str)
         {
             foreach (char c in str)
             {
                 if ((c <= 0x007A && c >= 0x0061) == false && (c <= 0x005A && c >= 0x0041) == false)
                 {
                     return false;
                 }
             }
             return true;
         }

         static public bool IsNumeric(string str)
         {
             foreach (char c in str)
             {
                 if (!Char.IsNumber(c))
                 {
                     return false;
                 }
             }
             return true;
         }


    }
}
