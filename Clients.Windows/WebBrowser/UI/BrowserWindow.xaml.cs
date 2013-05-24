using System;
using System.Linq;
using System.Windows;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;
using DesktopGap.WebBrowser.View;

namespace DesktopGap.Clients.Windows.WebBrowser.UI
{
  /// <summary>
  /// Interaction logic for Browser.xaml
  /// </summary>
  public partial class BrowserWindow : IWebBrowserWindow
  {
    private readonly ViewDispatcherBase _viewDispatcher;

    public BrowserWindow (ViewDispatcherBase viewDispatcher)
    {
      ArgumentUtility.CheckNotNull ("viewDispatcher", viewDispatcher);
      InitializeComponent();

      _viewDispatcher = viewDispatcher;

      _viewDispatcher.ViewCreated += OnSubViewCreated;
    }

    public void Dispose ()
    {
      foreach (var disposable in _tabControl.Items.OfType<IDisposable>())
        disposable.Dispose();
    }

    private void btnAddNew_Click_1 (object sender, RoutedEventArgs e)
    {
      NewTab (_urlTextBox.Text, BrowserWindowStartMode.Active);
    }

    public void NewTab (string url, BrowserWindowStartMode mode)
    {
      ArgumentUtility.CheckNotNull ("url", url);

      _viewDispatcher.NewView (BrowserWindowTarget.Tab, url, mode);
    }

    public void NewPopUp (string url, BrowserWindowStartMode mode)
    {
      ArgumentUtility.CheckNotNull ("url", url);

      _viewDispatcher.NewView (BrowserWindowTarget.PopUp, url, mode);
    }

    private void OnSubViewCreated (object sender, NewViewEventArgs args)
    {
      if (args.StartMode == BrowserWindowStartMode.Modal)
        return;

      var tab = args.View as BrowserTab;
      if (tab != null)
      {
        _tabControl.Items.Add (tab);
        args.View.Closing += (s, e) => RemoveTab (tab);
        SetCloseable();
      }
      args.View.Show (args.StartMode);
    }

    private void RemoveTab (BrowserTab browserTab)
    {
      _tabControl.Items.Remove (browserTab);
      SetCloseable();
    }

    private void SetCloseable ()
    {
      if (_tabControl.Items.Count > 0)
        ((BrowserTab) _tabControl.Items[0]).IsCloseable = false;
    }
  }
}