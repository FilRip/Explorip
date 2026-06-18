using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Explorip.Explorer.Helpers;

public class NumericUpDown : Control
{
    private RepeatButton _upButton;
    private RepeatButton _downButton;

    public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumericUpDown));
    public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumericUpDown));
    public readonly static DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown));
    public readonly static DependencyProperty StepProperty = DependencyProperty.Register(nameof(StepValue), typeof(int), typeof(NumericUpDown));

    public NumericUpDown()
    {
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

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _upButton = Template.FindName("Part_UpButton", this) as RepeatButton;
        _downButton = Template.FindName("Part_DownButton", this) as RepeatButton;
        _upButton.Click += UpButton_Click;
        _downButton.Click += DownButton_Click;
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
