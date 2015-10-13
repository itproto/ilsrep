
---

# Introduction #

**The goal of this task was to create an GUI client solution for creating, editing, passing survey and storing results on created before TCP server**


---


# Details #

GUI client combined features of console client and console editor together in convenient user-friendly GUI interface

We had choose of next GUI toolkits:
  * AWT
  * Swing
  * SWT
  * QT/Jambi

We were going to create cross-plathform application, so SWT and QT/Jambi wasn't applicable since they require native libraries(though availile for most popular plathfroms). AWT was standart way to create cross-plathform apps in Java, but rather outdated, so we choosed Swing - pure Java, full-featured GUI toolkit, with ability of applying "Look and Feel" - a way to introduce nice looking GUI theme changes

GUI client have next "Look and Feel" applied as shown on screenshot

![http://pic.ipicture.ru/uploads/090525/U3fLFW1uYz.jpg](http://pic.ipicture.ru/uploads/090525/U3fLFW1uYz.jpg)

Features include:
  * retrieving list of Poll Surveys stored on choosen server during login
  * passing a Poll Survey and sending results to server
![http://pic.ipicture.ru/uploads/090525/VLmLlVsC21.jpg](http://pic.ipicture.ru/uploads/090525/VLmLlVsC21.jpg)
  * editing / creating / deleting Poll Surveys on server
![http://pic.ipicture.ru/uploads/090525/V66TpQLeO4.jpg](http://pic.ipicture.ru/uploads/090525/V66TpQLeO4.jpg)


---

# Technologies Used #

  * **JFC/Swing** as GUI toolkit
  * **Log4J** for logging