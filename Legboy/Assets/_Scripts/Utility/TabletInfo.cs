using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _Scripts.Utility
{
    public static class TabletInfo
    {
        private const string FileName = "_Story/Tablets/tabletsData";
        private const string StartSearchString = "[InicioTexto]";
        private const string EndSearchString = "[FimTexto]";

        public static string GetText(int index)
        {
            var lines = TextAssetToList(Resources.Load<TextAsset>(FileName));
            var startFound = false;
            
            List<String> textLines = new List<string>();

            foreach(var line in lines)
            {
                if (!startFound)
                {
                    if (line != null && line.Contains(StartSearchString))
                    {
                        var args = line.Split();
                        if (args[1] == "[" + index + "]") startFound = true;
                    }

                    if (startFound) continue;
                }

                if (startFound)
                {
                    var isEnd = line != null && line.Contains(EndSearchString);
                    if (!isEnd) textLines.Add(line);
                    if (isEnd) break;
                }
            }

            string text = textLines[0];
            for (int i = 1; i < textLines.Count; i++)
            {
                text += "\n" + textLines[i];
            }

            return text;
        }

        public static string GetTitle(int index)
        {
            //using var reader = new StreamReader(FileName);
            
            List<string> lines = TextAssetToList(Resources.Load<TextAsset>(FileName));

            foreach(var line in lines)
            {
                //var line = reader.ReadLine();

                if (line != null && line.Contains(StartSearchString))
                {
                    var args = line.Split();
                    if (args[1] == "[" + index + "]")
                    {
                        var title = args[2];
                        for (int i = 3; i < args.Length; i++)
                        {
                            title += " " + args[i];
                        }

                        return title;
                    }
                }
            }

            return "";
        }

        public static int GetTabletCount()
        {
            List<string> lines = TextAssetToList(Resources.Load<TextAsset>(FileName));
            
            //using var reader = new StreamReader(FileName);
            bool startFound = false;
            int counter = 0;

            foreach(var line in lines)
            {
                //var line = reader.ReadLine();
                if (!startFound && line != null) startFound = line.Contains(StartSearchString);

                if (startFound)
                {
                    if (line != null && line.Contains(EndSearchString)) counter++;
                }
            }

            return counter;
        }

        private static List<string> TextAssetToList(TextAsset ta)
        {
            var arrayString = ta.text.Split('\n');
            return arrayString.ToList();
        }

    }
}
