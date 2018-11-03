﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaterialDesignExtensions.Controls
{
    /// <summary>
    /// A spinner control for integers.
    /// </summary>
    public class OversizedNumberSpinner : Control
    {
        private const string ValueTextBoxName = "ValueTextBox";

        /// <summary>
        /// Internal command used by the XAML template (public to be available in the XAML template). Not intended for external usage.
        /// </summary>
        public static readonly RoutedCommand EditValueCommand = new RoutedCommand();

        /// <summary>
        /// Internal command used by the XAML template (public to be available in the XAML template). Not intended for external usage.
        /// </summary>
        public static readonly RoutedCommand MinusCommand = new RoutedCommand();

        /// <summary>
        /// Internal command used by the XAML template (public to be available in the XAML template). Not intended for external usage.
        /// </summary>
        public static readonly RoutedCommand PlusCommand = new RoutedCommand();

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
            nameof(IsEditing), typeof(bool), typeof(OversizedNumberSpinner), new PropertyMetadata(false));

        private bool IsEditing
        {
            get
            {
                return (bool)GetValue(IsEditingProperty);
            }

            set
            {
                SetValue(IsEditingProperty, value);
            }
        }

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
            nameof(Min), typeof(int), typeof(OversizedNumberSpinner), new PropertyMetadata(0));

        public int Min
        {
            get
            {
                return (int)GetValue(MinProperty);
            }

            set
            {
                SetValue(MinProperty, value);
            }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            nameof(Max), typeof(int), typeof(OversizedNumberSpinner), new PropertyMetadata(5));

        public int Max
        {
            get
            {
                return (int)GetValue(MaxProperty);
            }

            set
            {
                SetValue(MaxProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(int), typeof(OversizedNumberSpinner), new PropertyMetadata(1, ValuePropertyChangedCallback));

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty SeparatorVisibilityProperty = DependencyProperty.Register(
            "SeparatorVisibility", typeof(Visibility), typeof(OversizedNumberSpinner), new PropertyMetadata(Visibility.Visible));

        public Visibility SeparatorVisibility
        {
            get { return (Visibility) GetValue(SeparatorVisibilityProperty); }
            set { SetValue(SeparatorVisibilityProperty, value); }
        }

        private TextBox m_valueTextBox;

        static OversizedNumberSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OversizedNumberSpinner), new FrameworkPropertyMetadata(typeof(OversizedNumberSpinner)));
        }

        public OversizedNumberSpinner()
            : base()
        {
            CommandBindings.Add(new CommandBinding(EditValueCommand, EditValueCommandHandler));
            CommandBindings.Add(new CommandBinding(MinusCommand, MinusCommandHandler));
            CommandBindings.Add(new CommandBinding(PlusCommand, PlusCommandHandler));

            Unloaded += UnloadedHandler;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (m_valueTextBox != null)
            {
                m_valueTextBox.LostFocus -= LostFocusHandler;
                m_valueTextBox.KeyUp -= KeyUpHandler;
            }

            m_valueTextBox = Template.FindName(ValueTextBoxName, this) as TextBox;
            m_valueTextBox.Loaded += (sender, args) =>
            {
                m_valueTextBox.LostFocus += LostFocusHandler;
                m_valueTextBox.KeyUp += KeyUpHandler;
            };
        }
        
        private void UnloadedHandler(object sender, RoutedEventArgs args)
        {
            if (m_valueTextBox == null) return;
            m_valueTextBox.LostFocus -= LostFocusHandler;
            m_valueTextBox.KeyUp -= KeyUpHandler;
        }

        private async void EditValueCommandHandler(object sender, ExecutedRoutedEventArgs args)
        {
            IsEditing = true;
            while (true)
                try
                {
                    m_valueTextBox.Focus();
                    m_valueTextBox.SelectAll();
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(7);
                }
        }

        private void MinusCommandHandler(object sender, ExecutedRoutedEventArgs args)
        {
            Value = Value - 1;
        }

        private void PlusCommandHandler(object sender, ExecutedRoutedEventArgs args)
        {
            Value = Value + 1;
        }

        private static void ValuePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            OversizedNumberSpinner spinner = (OversizedNumberSpinner)dependencyObject;

            if (spinner.Value < spinner.Min)
            {
                spinner.Value = spinner.Min;
            }

            if (spinner.Value > spinner.Max)
            {
                spinner.Value = spinner.Max;
            }
        }

        private void LostFocusHandler(object sender, EventArgs args)
        {
            IsEditing = false;
        }

        private void KeyUpHandler(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                Focus();
            }
        }
    }
}
