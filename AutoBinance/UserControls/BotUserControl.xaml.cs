﻿using System.Windows.Controls;
using System.Windows.Input;

namespace WpfClient.UserControls
{
    /// <summary>
    /// Interaction logic for SymbolUserControl.xaml
    /// </summary>
    public partial class BotUserControl : UserControl
    {
        public BotUserControl()
        {
            InitializeComponent();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool approvedDecimalPoint = false;

            if (e.Text == "." && !((TextBox)sender).Text.Contains('.'))
            {
                approvedDecimalPoint = true;
            }

            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
            {
                e.Handled = true;
            }
        }
    }
}
