﻿#pragma checksum "..\..\..\..\UserControls\BotUserControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "46D1C569B43891FA92541FB52CD2A511C8FAFCFA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Bu kod araç tarafından oluşturuldu.
//     Çalışma Zamanı Sürümü:4.0.30319.42000
//
//     Bu dosyada yapılacak değişiklikler yanlış davranışa neden olabilir ve
//     kod yeniden oluşturulursa kaybolur.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WpfClient.UserControls {
    
    
    /// <summary>
    /// BotUserControl
    /// </summary>
    public partial class BotUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WpfClient.UserControls.BotUserControl SymbolControl;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FirstOrderSizeTextBox;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox UpperSizeTextBox;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox UpperStopPriceTextBox;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LowerSizeTextBox;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LowerStopPriceTextBox;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\..\..\UserControls\BotUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox logsListView;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.2.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AutoBinance;V1.0.0.0;component/usercontrols/botusercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControls\BotUserControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.2.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SymbolControl = ((WpfClient.UserControls.BotUserControl)(target));
            return;
            case 2:
            this.FirstOrderSizeTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 34 "..\..\..\..\UserControls\BotUserControl.xaml"
            this.FirstOrderSizeTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 3:
            this.UpperSizeTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 61 "..\..\..\..\UserControls\BotUserControl.xaml"
            this.UpperSizeTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 4:
            this.UpperStopPriceTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 77 "..\..\..\..\UserControls\BotUserControl.xaml"
            this.UpperStopPriceTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 5:
            this.LowerSizeTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 102 "..\..\..\..\UserControls\BotUserControl.xaml"
            this.LowerSizeTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 6:
            this.LowerStopPriceTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 118 "..\..\..\..\UserControls\BotUserControl.xaml"
            this.LowerStopPriceTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 7:
            this.logsListView = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

