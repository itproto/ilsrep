
---

# Introduction #

**The goal of this task was to create a solution for creating, editing survey and storing obtained data.**

The project at its final iteration consisted of:

  * Client that could handle both online and offline surveys.
![http://pic.ipicture.ru/uploads/090525/LoCxUBnglu.jpg](http://pic.ipicture.ru/uploads/090525/LoCxUBnglu.jpg)

  * Editor(later merged with client) for editing and creating new surveys.

  * TCP Server, for storing data and providing a backend for first two components.
![http://img505.imageshack.us/img505/8649/server.jpg](http://img505.imageshack.us/img505/8649/server.jpg)


---


# Details #

At first only the offline part of the client was developed, later offline poll editing facility was added and finally the TCP server.

The user registers and logs into the system via Client and answers questions int a Survey, later the data entered is submitted to the Server.

There are two types of sessions:
  * Surveys
  * Tests

The difference is that you can **PASS** or **FAIL** a Test( depending of the minimumum percentage of correct answers required).

In **Survey** mode Sessions may allow user to enter a custom choise, which is also stored on the server later on.

Using a convinient GUI tool for Server managment an administrator may change the number of maximum concurent connections that the database allows, resize connection and memory pools.

The  **Editor** implements an easy and straightforward way for Session creation.
The user is queried for needed input: Questions, available options etc and it usually takes less then 5 minutes to create a rather big Survey or Test.


---

# Technologies Used #

  * **JAXB** for XML marshalling
  * **SQLite** database, though the level of abstracytion we used allows to work with any SQL-compatible database.
  * **Ant** for building the project
  * **jXPilot User configuration GUI**