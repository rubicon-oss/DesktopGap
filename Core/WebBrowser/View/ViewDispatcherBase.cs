﻿// This file is part of DesktopGap (desktopgap.codeplex.com)
// Copyright (c) rubicon IT GmbH, Vienna, and contributors
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Additional permissions are listed in the file DesktopGap_exceptions.txt.
// 
using System;
using DesktopGap.AddIns;
using DesktopGap.AddIns.Events;
using DesktopGap.AddIns.Events.Subscriptions;
using DesktopGap.Utilities;
using DesktopGap.WebBrowser.Arguments;
using DesktopGap.WebBrowser.StartOptions;

namespace DesktopGap.WebBrowser.View
{
  public abstract class ViewDispatcherBase : IDisposable
  {
    private class WindowPreparations
    {
      private const string c_protocolSeparator = "://";
      private const string c_httpProtocolHandler = "http";

      public IExtendedWebBrowser Browser { get; private set; }

      public Uri Url { get; private set; }

      public WindowPreparations (IExtendedWebBrowser browser, string url)
      {
        ArgumentUtility.CheckNotNull ("browser", browser);
        ArgumentUtility.CheckNotNull ("url", url);
        var protocolUrl = !url.Contains (c_protocolSeparator) ? string.Join (c_protocolSeparator, c_httpProtocolHandler, url) : url;
        Browser = browser;
        Uri uri;
        if (Uri.TryCreate (protocolUrl, UriKind.RelativeOrAbsolute, out uri))
          Url = uri;
        else
          throw new ArgumentException ("Invalid URL");
        //Url = new Uri ();
      }
    }

    private class FullWindowPreparations : WindowPreparations
    {
      public BrowserWindowStartMode StartMode { get; private set; }
      public BrowserWindowTarget Target { get; private set; }

      public FullWindowPreparations (IExtendedWebBrowser browser, string url, BrowserWindowStartMode startMode, BrowserWindowTarget target)
          : base (browser, url)
      {
        StartMode = startMode;
        Target = target;
      }
    }

    public event EventHandler<NavigationEventArgs> PrepareNavigation;
    public event EventHandler<NewViewEventArgs> ViewCreated;


    private readonly IWebBrowserFactory _browserFactory;
    private readonly ISubscriptionProvider _subscriptionProvider;

    private WindowPreparations _preparations;


    protected ViewDispatcherBase (IWebBrowserFactory browserFactory, ISubscriptionProvider subscriptionProvider)
    {
      ArgumentUtility.CheckNotNull ("browserFactory", browserFactory);
      ArgumentUtility.CheckNotNull ("subscriptionProvider", subscriptionProvider);

      _browserFactory = browserFactory;
      _subscriptionProvider = subscriptionProvider;
    }

    public void Dispose ()
    {
      PrepareNavigation = null;
    }

    public void OnDocumentRegistered (object sender, DocumentRegisterationEventArgs eventArgs)
    {
      foreach (var subscriber in _subscriptionProvider.GetSubscribers<IWindowEventSubscriber> (eventArgs.DocumentHandle))
      {
        PrepareNavigation += subscriber.OnPrepareNavigation;
        ViewCreated += subscriber.OnViewCreated;
      }
    }

    public void OnBeforeDocumentUnregister (object sender, DocumentRegisterationEventArgs eventArgs)
    {
      foreach (var subscriber in _subscriptionProvider.GetSubscribers<IWindowEventSubscriber> (eventArgs.DocumentHandle))
      {
        PrepareNavigation -= subscriber.OnPrepareNavigation;
        ViewCreated -= subscriber.OnViewCreated;
      }
    }

    public abstract void NewView (BrowserWindowTarget target, string url, BrowserWindowStartMode startMode);


    protected abstract void Dispatch (IExtendedWebBrowser browser, BrowserWindowTarget target, BrowserWindowStartMode startMode, string targetName);

    protected void ViewCreationDone (IWebBrowserView view, BrowserWindowStartMode startMode)
    {
      if (ViewCreated != null)
        ViewCreated (this, new NewViewEventArgs (view, startMode));
    }

    protected IExtendedWebBrowser CreateBrowser ()
    {
      var browser = _browserFactory.CreateBrowser();
      browser.WindowOpen += OnWindowOpen;
      browser.BeforeNavigate += OnBeforeNavigate;
      return browser;
    }

    protected void Prepare (IExtendedWebBrowser webBrowser, string url)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      _preparations = new WindowPreparations (webBrowser, url);
    }

    protected void Prepare (IExtendedWebBrowser webBrowser, string url, BrowserWindowStartMode startMode, BrowserWindowTarget target)
    {
      ArgumentUtility.CheckNotNull ("webBrowser", webBrowser);

      _preparations = new FullWindowPreparations (webBrowser, url, startMode, target);
    }


    private void OnWindowOpen (object sender, WindowOpenEventArgs eventArgs)
    {
      if (eventArgs.BrowserWindowTarget == BrowserWindowTarget.Self)
        return;

      var webBrowser = CreateBrowser();

      eventArgs.TargetView = webBrowser;
      Prepare (webBrowser, eventArgs.Url);
    }

    private void OnBeforeNavigate (object sender, NavigationEventArgs e)
    {
      if (PrepareNavigation != null)
        PrepareNavigation (this, e);

      if (_preparations == null || !_preparations.Url.Equals (e.Url))
        return;
      if (_preparations is FullWindowPreparations)
      {
        var preparations = (FullWindowPreparations) _preparations;
        Dispatch (preparations.Browser, preparations.Target, preparations.StartMode, string.Empty);
      }
      else
        Dispatch (_preparations.Browser, e.BrowserWindowTarget, e.StartMode, e.TargetName);

      _preparations = null;
    }
  }
}