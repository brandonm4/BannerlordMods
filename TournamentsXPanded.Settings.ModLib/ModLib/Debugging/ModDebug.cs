﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace TXP.ModLib.Debugging
{
    public static class ModDebug
    {
        public static void ShowError(string message, string title = "", Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = "";
            MessageBox.Show($"{message}\n\n{exception?.ToStringFull()}", title);
        }

        public static void ShowMessage(string message, string title = "", bool nonModal = false)
        {
            if (nonModal)
            {
                new Thread(() => MessageBox.Show(message, title)).Start();
            }
            MessageBox.Show(message, title);
        }

        public static void LogError(string error, Exception ex = null)
        {

        }
    }
}
