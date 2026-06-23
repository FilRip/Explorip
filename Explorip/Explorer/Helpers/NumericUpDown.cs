using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Explorip.Explorer.Helpers;

public class NumericUpDown : Control
{
    private TextBox _textBox;

    public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumericUpDown));
    public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumericUpDown));
    public readonly static DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
    public readonly static DependencyProperty StepProperty = DependencyProperty.Register(nameof(StepValue), typeof(int), typeof(NumericUpDown));

    public NumericUpDown()
    {
        Style = (Style)Application.Current.FindResource("NumericUpDownStyle");
    }

    #region DependencyPropertyAccessor

    public int Maximum
    {
        get { return (int)GetValue(MaximumProperty); }
        set { SetValue(MaximumProperty, value); }
    }

    public int Minimum
    {
        get { return (int)GetValue(MinimumProperty); }
        set { SetValue(MinimumProperty, value); }
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (NumericUpDown)d;
        control.UpdateTextBox();
    }

    public int Value
    {
        get { return (int)GetValue(ValueProperty); }
        set { SetCurrentValue(ValueProperty, value); }
    }

    public int StepValue
    {
        get { return (int)GetValue(StepProperty); }
        set { SetValue(StepProperty, value); }
    }

    #endregion

    private void UpdateTextBox()
    {
        _textBox?.Text = Value.ToString();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RepeatButton upButton = Template.FindName("Part_UpButton", this) as RepeatButton;
        RepeatButton downButton = Template.FindName("Part_DownButton", this) as RepeatButton;
        _textBox = Template.FindName("Part_TextBox", this) as TextBox;
        upButton.Click += UpButton_Click;
        downButton.Click += DownButton_Click;
        _textBox.TextChanged += TextBox_TextChanged;
        _textBox.Text = Value.ToString();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(_textBox.Text, out int v))
            Value = v;
    }

    private void DownButton_Click(object sender, RoutedEventArgs e)
    {
        if (Value > Minimum)
        {
            Value -= StepValue;
            if (Value < Minimum)
                Value = Minimum;
        }
    }

    private void UpButton_Click(object sender, RoutedEventArgs e)
    {
        if (Value < Maximum)
        {
            Value += StepValue;
            if (Value > Maximum)
                Value = Maximum;
        }
    }
}
