from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

from HtmlUtils import HtmlUtils

class LoggedInPage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/html'

    htmlUtils = HtmlUtils()

    output = htmlUtils.generateStart("TabSender: logged in")
    output += htmlUtils.generateDefaultMenu()
    output += htmlUtils.generateMenuEnd()

    output += """
<h1>Logged in with username: <span class="colouredText">""" + users.get_current_user().nickname() + """</span>.</h1>
You can close this tab now or go to <a href="/tabinfo.html">your account</a>.
    """

    output += htmlUtils.generateMainPartEnd()
    output += htmlUtils.generateEnd()
    
    self.response.out.write(output)
