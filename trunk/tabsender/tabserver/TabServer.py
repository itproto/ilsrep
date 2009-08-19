from google.appengine.ext import webapp
from google.appengine.ext.webapp.util import run_wsgi_app

from Categories import Categories
from MainPage import MainPage
from TabInfoPage import TabInfoPage
from LoggedInPage import LoggedInPage
from ProjectHomePage import ProjectHomePage
from SharePage import SharePage

application = webapp.WSGIApplication(
    [('/', MainPage), ('/tabinfo.html', TabInfoPage),
     ('/loggedin.html', LoggedInPage), ('/home.html', ProjectHomePage),
     ('/share.html', SharePage)],
     debug=True)

def main():
  run_wsgi_app(application)

if __name__ == "__main__":
  main()
