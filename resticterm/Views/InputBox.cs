using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    public class InputBox
    {
        public static String ShowModal(String title, String caption, String initialvalue = "", String help="")
        {
            String ret = null;

            bool okpressed = false;
            var ok = new Button("Ok");
            ok.Clicked += () => { Application.RequestStop(); okpressed = true; };
            var cancel = new Button("Cancel");
            cancel.Clicked += () => { Application.RequestStop(); };

            var dialog = new Dialog(title, ok, cancel)
            {
                Width = Dim.Percent(50),
                Height = Dim.Percent(40)
            };
            var label = new Label(caption)
            {
                X = 1,
                Y = 0,
                Width = Dim.Fill(),
                Height = 1
            };
            dialog.Add(label);
            var txt = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = 1
            };
            dialog.Add(txt);
            var hlp = new Label(help)
            {
                X = 1,
                Y = 3,
                Width = Dim.Fill(),
                Height = 3,
            };
            dialog.Add(hlp);

            txt.FocusFirst();
            Application.Run(dialog);

            ok.Clicked -= () => { Application.RequestStop(); okpressed = true; };
            cancel.Clicked -= () => { Application.RequestStop(); };
            if (okpressed)
            {
                ret = txt.Text.ToString();
            }
            return ret;
        }
    }
}
