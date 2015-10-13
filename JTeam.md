# Introduction #

JTeam constists of two people:
  * TKOST: Taras Kostiak <taraskostiak@gmail.com>
  * DRC:   Roman Tsiupa <draconyster@gmail.com>

![http://img15.imageshack.us/img15/9881/xd29acb34.jpg](http://img15.imageshack.us/img15/9881/xd29acb34.jpg)

_(presentaion of Poll Application on DE:CODED^09; people from left to right: Taras Kostiak, Roman Tsiupa)_

Our tasks during progress in ILSG can be divided into 2 periods:

  * Poll Application
  * Tab Sender

# Poll Application #

The goal of Poll Application is passing, creating, editing, managing Poll Surveys(Poll Sessions); storing and analyzing results.

Task was introduced with this [XML document](http://code.google.com/p/ilsrep/wiki/FirstXML)

Poll Application can be divided into such subperiods:
  * console client and tcp server ([details](http://code.google.com/p/ilsrep/wiki/ConsolePollCleint))
    * first version was an offline console client that allowed to pick file with Poll Survey and pass it
    * then TCP server was introduced which stored Poll Surveys in files and was using Log4J logging, as well as Poll Editor, that allowed to edit surveys on server
    * server was changed to store Poll Surveys and results in database(SQLite with Apache DB Pool)
    * protocol was changed from plain text to xml-based
  * [GUI client](http://code.google.com/p/ilsrep/wiki/SwingPollCleint)
> > GUI client, that was created using JFC/Swing GUI toolkit, combined features of console client and console editor together in convenient user-friendly GUI interface
  * web interface to Poll Survey features

Poll Application was presented on [DE:CODED^09](http://code.google.com/p/ilsrep/wiki/Decoded).

# TabSender #

TabSender is a plugin to firefox that allows to store your tab sessions in your browser and share / restore on other PC(using TabSender server).

[TabSender project homepage](http://tabsender.appspot.com/home.html)