from google.appengine.ext import db

class Categories(db.Model):
  user = db.StringProperty()
  name = db.StringProperty(multiline=False)
  url = db.StringProperty(multiline=False)
  id = db.IntegerProperty()

def deleteSession(id):
    toDelete=db.GqlQuery("select * FROM Categories where id=" + id)
    for cat in toDelete:
        cat.delete();

def deleteURL(id, url):
    # TODO: This method should be fixed.
    toDelete=db.GqlQuery("select * FROM Categories where id=" + id + " and url=\"" + url + "\"")
    for cat in toDelete:
        cat.delete();
