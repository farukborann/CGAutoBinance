using System.Windows;

namespace AutoBinance.MessageBox
{
    public interface IMessageBoxService
    {
        MessageBoxResult ShowMessage(string text, string caption, MessageBoxButton messageButtons, MessageBoxImage messageIcon);
    }
}
