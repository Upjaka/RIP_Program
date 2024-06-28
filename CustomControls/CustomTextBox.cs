using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.CustomControls
{
    public class CustomTextBox : TextBox
    {
        private void BeginEdit()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => Focus());
            // Здесь можно добавить логику для начала редактирования
            // Например, можно изменить цвет фона или добавить рамку редактирования
        }

        private void CommitEdit()
        {
            // Завершение редактирования
            // Здесь можно добавить логику для сохранения изменений
        }
    }
}
