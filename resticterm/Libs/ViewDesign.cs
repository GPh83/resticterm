using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Libs
{

    /// <summary>
    /// Helper for design view
    /// </summary>
    public static class ViewDesign
    {
        /// <summary>
        /// Design a field with label and text on same ligne
        /// </summary>
        /// <param name="ntop">View</param>
        /// <param name="textField">Object to retrieve value</param>
        /// <param name="caption">Label of field</param>
        /// <param name="text">Initial value</param>
        /// <param name="y">Line</param>
        public static void SetField(Toplevel ntop, TextField textField, String caption, String text, int y)
        {

            var f = new Label(caption) { X = 1, Y = y, TextAlignment = TextAlignment.Right };
            ntop.Add(f);

            var t = new TextField(text.Replace("\r", "")) { X = Pos.Right(f) + 1, Y = y, Width = Dim.Fill() - 1 };
            ntop.Add(t);

        }

        /// <summary>
        /// Design a field with label and text on same ligne
        /// </summary>
        /// <param name="ntop">Window</param>
        /// <param name="textField">Object to retrieve value</param>
        /// <param name="caption">Label of field</param>
        /// <param name="text">Initial value</param>
        /// <param name="xCol">X pos for column</param>
        /// <param name="y">Line</param>
        public static void SetField(Window win, ref TextField textField, String caption, String text, int xCol, int y)
        {

            var f = new Label(caption) { X = 1, Y = y, TextAlignment = TextAlignment.Right, Width = xCol };
            win.Add(f);
            f.ColorScheme = Colors.Base;

            textField = new TextField(text.Replace("\r", "")) { X = xCol + +2, Y = y, Width = Dim.Fill() - 1 };
            win.Add(textField);
            textField.ColorScheme = Colors.Base;
        }

        /// <summary>
        /// Design a field with label and text on same ligne
        /// </summary>
        /// <param name="ntop">View</param>
        /// <param name="textField">Object to retrieve value</param>
        /// <param name="caption">Label of field</param>
        /// <param name="text">Initial value</param>
        /// <param name="xCol">X pos for column</param>
        /// <param name="y">Line</param>
        public static void SetField(Window win, ref TextView textField, String caption, String text, int xCol, int y, int height)
        {

            var f = new Label(caption) { X = 1, Y = y, TextAlignment = TextAlignment.Right, Width = xCol };
            win.Add(f);
            f.ColorScheme = Colors.Base;

            textField = new TextView() { X = xCol + +2, Y = y, Width = Dim.Fill() - 1, Text = text.Replace("\r", ""), Height = height, ColorScheme = Colors.Dialog };
            win.Add(textField);
            //textField.ColorScheme = Colors.Base;
        }

        public static void SetCheck(Window win, ref CheckBox checkBox, String caption, bool value, int xCol, int y)
        {

            var f = new CheckBox(caption, value) { X = xCol + 2, Y = y };
            win.Add(f);
            checkBox = f;
            checkBox.ColorScheme = Colors.Base;
        }

        public static void RefreshView(Toplevel toplevel, View obj)
        {
            toplevel.Redraw(obj.Bounds);
            Application.Refresh();
        }
    }
}
