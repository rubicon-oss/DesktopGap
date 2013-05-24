using System;
using System.ComponentModel;
using System.Windows;
using DesktopGap.Utilities;

namespace DesktopGap.Clients.Windows.Components
{
  /// <summary>
  /// Interaction logic for CloseableTabHeader.xaml
  /// </summary>
  public partial class CloseableTabHeader : INotifyPropertyChanged
  {
    private string _text;
    private Visibility _visibility = Visibility.Visible;

    public CloseableTabHeader (string headerText, bool isCloseable = false)
    {
      ArgumentUtility.CheckNotNull ("headerText", headerText);
      InitializeComponent();
      IsCloseable = isCloseable;
      Text = headerText;
      DataContext = this;
    }

    public event EventHandler TabClose;
    public event PropertyChangedEventHandler PropertyChanged;


    public String Text
    {
      get { return _text; }
      set
      {
        OnPropertyChanged ("Text");
        _text = value ?? String.Empty;
      }
    }

    public bool IsCloseable
    {
      get { return CloseButtonVisibility == Visibility.Visible; }
      set { CloseButtonVisibility = value ? Visibility.Visible : Visibility.Hidden; }
    }


    public Visibility CloseButtonVisibility
    {
      get { return _visibility; }
      set
      {
        OnPropertyChanged ("CloseButtonVisibility");
        _visibility = value;
      }
    }


    private void OnPropertyChanged (string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler (this, new PropertyChangedEventArgs (propertyName));
    }

    private void CloseButton_Click (object sender, RoutedEventArgs eventArgs)
    {
      if (IsCloseable && TabClose != null)
        TabClose (sender, eventArgs);
    }
  }
}