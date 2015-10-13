# Introduction #

SharpTeam members:
  * vladykx(Vladyslav Petrovytch)
  * kostjerry(Yaroslaw Kost)

During ILS process we created two projects:
  * PollApplication
    1. PollConsole\_SharpTeam
    1. PollClientGUI\_SharpTeam
    1. PollWeb\_SharpTeam
  * Kiosk\_SharpTeam

# PollApplication #

PollApplication has for his goal to work with tests and surveys.

**PollConsole**

It's fully console based application. It works with SQLite database and data transfering is based on XML. Project provide survey creation, updating, removing and passing. All results of surveys passing are saved in database. User login is provided.

**PollClientGUI**

Similar functions to PollConsole application, but this one is based on GUI.

**PollWeb**

Web based survey serving. Innovations:
  * Possibility of watching statistics
  * Possibility of PollWidget creating

# Kiosk #

It's web based application for kiosks. It provide user login and give user possibility to view internet pages. The goal is to provide access to Internet. Also user can run some program from programs list.