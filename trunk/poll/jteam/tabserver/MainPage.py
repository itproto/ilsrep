from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

import xml.dom.minidom

from Categories import Categories

class MainPage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/plain'
    action=self.request.get('action')
    output='error'
    if action == "login":
         user = users.get_current_user()
         
         if user:
            output=self.createResponseXML('true')
         else:
            output=self.createResponseXML('false') 
    if action == "regtemp":
         self.redirect(users.create_login_url("http://tabsender.appspot.com/html/logedin.html"))
    if action =="remove":
         user = users.get_current_user()
         if user:
            id=self.request.get('id')
            toDelete=db.GqlQuery("select * FROM Categories where id="+id)
            for cat in toDelete:
                  cat.delete();
            output=self.createResponseXML('true')
         else:
            output=self.createResponseXML('false')   
    if action == "save":
         doc = xml.dom.minidom.parseString(self.request.get('urls'))
         doc = doc.childNodes[0]
         user = users.get_current_user()
         if user:
            output=self.createResponseXML('true')
            name=doc.getAttribute("name")
            categories_all = db.GqlQuery("SELECT * FROM Categories order by id desc limit 1")
            curid=0
            for cat in categories_all:
               curid=cat.id+1;
            for e in doc.childNodes:
                category= Categories()
                category.user=user.nickname()
                category.name=name
                category.id=curid
                category.url=e.getAttribute('link')
                category.put()  

         else:
            output=self.createResponseXML('false')   
    if action == "load":
         doc = xml.dom.minidom.Document()
         urlsChild = doc.createElement("urls")
         doc.appendChild(urlsChild)
         urlsChild=doc.childNodes[0]
         user = users.get_current_user()
         if user:
            id=self.request.get("id")
            categories_all = db.GqlQuery("SELECT * FROM Categories where id="+id)
            for cat in categories_all:
               urlsChild.setAttribute('name',cat.name)
               linkChild=doc.createElement("url")
               linkChild.setAttribute('link',cat.url)
               urlsChild.appendChild(linkChild)
            output=doc.toxml()
    
         else:
            output=self.createResponseXML('false')
    if action == "list":
         doc = xml.dom.minidom.Document()
         urlsChild = doc.createElement("categories")
         doc.appendChild(urlsChild)
         urlsChild=doc.childNodes[0]
         user = users.get_current_user()
         if user:
            categories_all = db.GqlQuery("SELECT * FROM Categories where user='"+user.nickname()+"'")
            unique_results = []
            for cat in categories_all:
               if cat.id not in unique_results:
                  linkChild=doc.createElement("cat")
                  linkChild.setAttribute('name',cat.name)
                  linkChild.setAttribute('id',str(cat.id))
                  urlsChild.appendChild(linkChild)
                  unique_results.append(cat.id)
            output=doc.toxml()
    
         else:
            output=self.createResponseXML('false') 

    self.response.out.write(output)
  def createResponseXML(self, responseText):
      return '<?xml version="1.0" encoding="UTF-8"?><response isOk=\''+responseText+'\' />'
