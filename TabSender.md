
---

# Introduction #

**TabSender? is a plugin to firefox that allows to store your tab sessions in your browser and share / restore on other PC(using TabSender? server).**


---


# Details #

TabSender consist of two parts:
  * Firefox plugin to manage tab sessions on client side
|![http://img24.imageshack.us/img24/4598/28247620.png](http://img24.imageshack.us/img24/4598/28247620.png)|![http://img91.imageshack.us/img91/513/54986982.jpg](http://img91.imageshack.us/img91/513/54986982.jpg)|
|:--------------------------------------------------------------------------------------------------------|:------------------------------------------------------------------------------------------------------|
> > Plugin was written using XUL and Javascript to created convenient and nice-looking interface on client side. You can load, save, delete your sessions on many PCs - you just need to login with Google account to get access to your data
  * TabSender Server - application to store user's tab sessions [homepage](http://tabsender.appspot.com/home.html)
> > TabSender Server is hosted on Google's AppSpot proving free hosting(limited though) for server applications created in Python 2.5 and GoogleAppEngine. This server provide storing user's sessions, as well as, managing and sharing sessions from user-friendly web interface. You can go to [your account](http://tabsender.appspot.com/tabinfo.html), automatically created if you login with your Google account


---

# Technologies Used #

  * **XUL** as GUI toolkit for Firefox plugin
  * **Javascript** as programming language for Firefox plugin
  * **Python 2.5** as programming language for server part
  * **GoogleAppEngine** - library for creating application hosted on Google's AppSpot and accessing Google's features like: interfacing with Google Accounts, object-oriented database, image hosting