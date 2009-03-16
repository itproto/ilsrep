import xml.dom.minidom
from google.appengine.api import users
from google.appengine.ext import webapp
from google.appengine.ext.webapp.util import run_wsgi_app
from google.appengine.ext import db

class Categories(db.Model):
  user = db.UserProperty()
  name = db.StringProperty(multiline=False)
  url = db.StringProperty(multiline=False)

class MainPage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/plain'
    action=self.request.get('action')
    output='error'
    if action == "login":
         user = users.get_current_user()
         
         if user:
            output='<?xml version="1.0" encoding="UTF-8"?><response isOk=\'true\'>'
         else:
            output='<?xml version="1.0" encoding="UTF-8"?><response isOk=\'false\'>'
    
    if action == "save":
    	 doc = xml.dom.minidom.parse(self.request.get('urls'))
         user = users.get_current_user()
         if user:
            output='<?xml version="1.0" encoding="UTF-8"?><response isOk=\'true\'>'
            name=doc.getAttribute("name")
            for e in doc.childNodes:
				category= Categories()
				category.user=user
				category.name=name
				category.url=e.getAttribute('link')
				category.put()	

         else:
            output='<?xml version="1.0" encoding="UTF-8"?><response isOk=\'false\'>'   
    if action == "load":
    	 doc = xml.dom.minidom.Document()
    	 user = users.get_current_user()
    	 if user:
            name=self.request.get("name")
            categories_all = db.GqlQuery("SELECT * FROM Categories where name='"+name+"' and user='"+user+"'")
			for cat in categories_all:


         else:
            output='<?xml version="1.0" encoding="UTF-8"?><response isOk=\'false\'>'   
    self.response.out.write(output)

application = webapp.WSGIApplication(
                                     [('/', MainPage)],
                                     debug=True)

def main():
  run_wsgi_app(application)

if __name__ == "__main__":
  main()