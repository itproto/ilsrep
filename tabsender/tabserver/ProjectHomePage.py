from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

from HtmlUtils import HtmlUtils

class ProjectHomePage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/html'

    htmlUtils = HtmlUtils()

    output = htmlUtils.generateStart("TabSender: logged in")
    output += htmlUtils.generateDefaultMenu()
    output += htmlUtils.generateMenuEnd()

    output += """
<h1>TabSender project</h1>
TabSender is an plugin to firefox that allows to
store your tab sessions in your browser and share / restore
on other PC.
<br />
<br />
At the moment project is in active development.
<br />
<br />
You can download recent version of plugin <a href="/html/sender.xpi">here</a>.
    """

    output += htmlUtils.generateMainPartEnd()
    output += htmlUtils.generateEnd()
    
    self.response.out.write(output)
