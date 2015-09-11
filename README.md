# DesktopGap


DesktopGap is a way to deliver HTML-based applications that integrates much better with the user’s desktop environment and gives developers more control over the applications behavior than normal Web browsers. It is, basically, a specialized browser for Web applications rather than Web sites.

Applications are still running on Web servers, there is no need to change an applications architecture or even package it for delivery to the client.

You can also create custom extensions using .NET that use local resources (e.g. launch local applications, communicate with local applications, etc.) and expose them via JavaScript to your Web application.

## Desktop Integration

* Drag and Drop files from and to your application. This includes e-mail attachments in Outlook. (HTML5 Drag and Drop only supports dragging files from Desktop/File Explorer to the browser.)
* Open files from your browser for editing in local desktop applications like Office. Saving those files will send them back to your application for further processing. (Just like using SharePoint, but not limited to Microsoft Office and Windows Integrated Security.)

## Application Behavior

* By installing DesktopGap for your Web application, users choose to trust this application. They will not be bugged by any security warnings.
* The application controls window titles and icons. It gets a separate icon in the taskbar.
* Your application pages will never appear as tabs in the same browser window as external pages, and your tabs will never be distributed among several windows unless you choose so.
* The application has full control over window and tab management. E.g. whether a new page opens as tab or window does not depend on user settings, and popups will not be prevented by security settings.
* Your application control flow will not be challenged by user-controlled navigation (back/forward buttons or refresh), unless you enable those.
* In browsers, users often accidentally close a window with unsaved data, e.g. by closing a browser with many open tabs, entering a URL, picking a favorite, or even clicking a link in another application (depending on browser settings). DesktopGap can prevent this just like a local application.

## Security

* You can limit a DesktopGap instance to certain server URLs, thereby basically eliminating the chance to exploit HTML injection bugs in your application with cross-site scripting attacks (XSS).
* You can also restrict all communication to HTTPS or even a specific certificate and avoid man-in-the-middle attacks. The user cannot be tricked into using an intercepted HTTP connection (using a padlock favicon) or into accepting a forged certificate.
* Dangerous custom extensions can be protected from exploitation via HTML/JavaScript injection. (to be discussed)
* Redirection from browsers to DesktopGap (see below) can prevent cross-site request forgery (XSRF) attacks if you limit redirection to certain resources (blacklist or whitelist).


DesktopGap does not change your default browser, and it is not used to open arbitrary Web pages. It is usually provided with a configuration file for a specific Web application, and installed by system admins or any distribution mechanism that handles MSI packages. If they want though, end users can manually install DesktopGap with or without administrative privileges (e.g. for doing office work on their home PCs).

When DesktopGap is installed, your application can redirect users to it from browsers (e.g. when a user clicks an HTTP-link to your application in an e-mail, which opens in the default browser).

DesktopGap uses Internet Explorer’s Trident rendering engine. This engine is always available on Windows, even if Internet Explorer has been uninstalled. It does not matter what other browsers users have installed, or which one is the default browser. One advantage of using the Trident engine is that it is automatically supplied with security updates via Windows Update, or Windows Server Update Services in enterprises, without needing to update DesktopGap. 


Previously located at: http://desktopgap.codeplex.com/
