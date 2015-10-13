# Introduction #

The task was to create a web interface for our Poll solution.

This task was designed in a few steps:
  1. JSP pages that work with sql database directly.
  1. Apache Tapestry framework version
  1. JSP/Javascript Editor
  1. Web Service as backend
  1. Servlet for graph generation
  1. Widget for creating site surveys


# Details #

### JSP pages ###
The first iteration was created using simple JSP scripts and client classes from Client version.
During the session the user is similarly to the same process in Client asked series of questions, and the results are stored in the database.

### Tapestry version ###
Apache Tapestry version was designed to get a grip of this frameworks' technology. The functionality was the same as in the JSP version. This version was later discontinued.

### JSP/Javascript Editor ###
This part of the site allowed creation and edition of poll sessions that were stored. For better usability it was largely based on Jvascript, **XML for `<`SCRIPT`>`** library and JQuery framework. The interface created provided a quick and straightforward way for quick manipulation of data.

### Web Service ###
The necessity of a Web Service in our solution was obvious: we wanted to make our sever as open to any clients as possible. We used **JAX-WS** technology which was the most popular and easiest to deploy. The methods created were so close to database methods that it made client-server communication much easier to operate, so all previous tasks (except for **Tapestry** version) were changed to use this technology.

### Servlet ###
To create dynamic images for our Web Analyzer we used a servlet that would generate these images.

### Widget ###
The widget was not directly an expansion of the Poll solution, it was rather a spin-off.
Using our JSP/Javascript Editor the user could create a widget for his page and receive a code to be inserted into his page. This was largely more of a design task, so again we used Javascript and Ajax technology.

# Technologies and libraries used #
  * XML for `<`Script`>`
  * Jquery
  * JAX-WS
  * Ajax
  * Tapestry
  * JFreeChart